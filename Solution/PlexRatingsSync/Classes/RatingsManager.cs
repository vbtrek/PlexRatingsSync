using DS.Library.MessageHandling;
using DS.PlexRatingsSync.Classes;
using DS.PlexRatingsSync.Classes.PlexApi;
using DS.PlexRatingsSync.Classes.PlexTvApi;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace DS.PlexRatingsSync
{
  public static class RatingsManager
  {
    public static void SyncRatings(SyncArgs args)
    {
      if (args.Worker.CancellationPending) return;

      if (!Settings.SyncRatings) return;

      // Get all the files to sync ratings for
      args.ReportProgress("Reading Track Data From Plex...");

      // Login and get auth token
      // *************************************************************************************************************
      // USER CONFIGURABLE SETTINGS
      // *************************************************************************************************************
      string username = Settings.PlexUsername;
      string password = Settings.PlexPassword;

      // TODO_DS1 We can call http://10.1.14.114:32400/library/sections/ and pick out the items where type="artist",
      // I think there could be multiple Locations, then we are mapping Location.Path to a user defined path
      Dictionary<string, string> localMusicRoots = new Dictionary<string, string>
      {
        { "/data/music", @"\\homeNAS\Music" }
      };
      // *************************************************************************************************************


      //var plainTextBytes = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
      //var auth = Convert.ToBase64String(plainTextBytes);
      var loginResult = RestClient.Create(new Uri("https://plex.tv/users/sign_in.json"), RestClient.httpMethod.Post, string.Empty)
        .AddHeader("X-Plex-Client-Identifier", "Plexapi")
        .AddHeader("X-Plex-Product", "PlexRatingsSync")
        .AddHeader("X-Plex-Version", Application.ProductVersion)
        .AddHeader("X-Plex-Device", Environment.MachineName)
        .AddHeader("X-Plex-Platform", "Desktop")
        .AcceptHeader(RestClient.httpContentType.Json)
        .AuthorizationBasic(username, password)
        .SendRequestWithExceptionResponse();

      var plexUser = JsonConvert.DeserializeObject<PlexTvRoot>(loginResult);

      // Get all tracks
      // X-Plex-Token=xcxGLUCG-MABLz-pxnkk
      // http://10.1.14.114:32400/library/all?type=10
      var result = RestClient.Create(
        new Uri("http://10.1.14.114:32400/library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      MediaContainer mediaContainer = new MediaContainer();

      try
      {
        using (XmlReader reader = XmlReader.Create(new StringReader(result)))
          mediaContainer.ReadFromXml(reader);

        /*
        XmlSerializer serializer = new XmlSerializer(typeof(MediaContainer));

        using (StringReader reader = new StringReader(result))
          mediaContainer = (MediaContainer)serializer.Deserialize(reader);
        */
      }
      catch
      { }

      args.ReportProgress(mediaContainer.Size);
      
      args.ReportProgress("Syncing Ratings...");

      args.PlexUser = plexUser;

      args.MusicFolderMappings = localMusicRoots;

      // Process all the files
      foreach (var track in mediaContainer.Tracks)
      {
        if (args.Worker.CancellationPending) return;

        args.CurrentTrack = track;

        args.ReportProgress($"Syncing \"{track.GrandparentTitle} - {track.ParentTitle} - {track.Title}\"...");

        SyncRating(args);

        args.ReportProgress(SyncArgs.ProgressType.IncrementProgressBar);
      }
    }

    private static void SyncRating(SyncArgs args)
    {
      try
      {
        if (args.CurrentTrack != null && File.Exists(args.CurrentLocalFile.FullName))
        {
          try
          {
            int? currentNormalisedSourceRating = CurrentNormalisedSourceRating(args);

            if (currentNormalisedSourceRating != args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex))
            {
              switch (DetermineClashHandling(args))
              {
                case RatingsClashResult.UpdatePlex:
                  UpdatePlexRating(args);
                  
                  break;

                case RatingsClashResult.UpdateFile:
                  if (Settings.SyncSource == SyncSources.FileProperties)
                    UpdateFileRating(args);

                  break;
              }
            }
          }
          catch (Exception ex)
          {
            SentrySdk.CaptureException(ex);

            MessageManager.Instance.ExceptionWrite(new object(), ex);
          }
        }
      }
      catch (Exception ex)
      {
        SentrySdk.CaptureException(ex);

        MessageManager.Instance.ExceptionWrite(new object(), ex);
      }
    }

    private static RatingsClashResult DetermineClashHandling(SyncArgs args)
    {
      var result = RatingsClashResult.Cancel;

      switch (Settings.SyncHandling)
      {
        case SyncModes.FileToPlex:
          result = RatingsClashResult.UpdatePlex;
          break;

        case SyncModes.PlexToFile:
          result = RatingsClashResult.UpdateFile;
          break;

        case SyncModes.TwoWay:
          int? currentNormalisedSourceRating = CurrentNormalisedSourceRating(args);

          int? normalisedPlexRating = args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex);

          if (Settings.ClashHandling != ClashWinner.AlwaysPrompt)
          {
            if ((args.CurrentPlexRating ?? 0) == 0 && currentNormalisedSourceRating > 0)
              result = RatingsClashResult.UpdatePlex;

            if ((currentNormalisedSourceRating ?? 0) == 0 && args.CurrentPlexRating > 0)
              result = RatingsClashResult.UpdateFile;
          }

          if (result == RatingsClashResult.Cancel)
          {
            switch (Settings.ClashHandling)
            {
              case ClashWinner.File:
                result = RatingsClashResult.UpdatePlex;
                break;

              case ClashWinner.Plex:
                result = RatingsClashResult.UpdateFile;
                break;
            }

            if (result == RatingsClashResult.Cancel && Settings.ClashHandling != ClashWinner.Skip)
            {
              FileInfo fi = args.CurrentLocalFile;

              TaskDialogs dlg = new TaskDialogs();
              
              result = dlg.RatingsClash($"{args.PlexTitle}{Environment.NewLine}{fi.Name}", normalisedPlexRating, currentNormalisedSourceRating);
            }
          }

          /*
          if (result == RatingsClashResult.Cancel)
          {
            Program.DoDebug("SKIPPED: Auto sync of '" + so.Name + "'. iTunes rating: " + iRating.ToString() + ", file rating: " + fRating.ToString());
          }
          */
          break;
      }

      return result;
    }

    private static void UpdatePlexRating(SyncArgs args)
    {
      int? currentRatingInPlexFormat = CurrentRatingInPlexFormat(args);

      int? plexDbRating = args.CurrentPlexRating;

      string message = string.Format("Updating Plex rating for file \"{0}\" from {1} to {2}",
          args.CurrentLocalFile.FullName,
          plexDbRating == null ? 0 : plexDbRating,
          currentRatingInPlexFormat == null ? 0 : currentRatingInPlexFormat);

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
          message);

      args.ReportProgress(SyncArgs.ProgressType.IncrementUpdatedCount);

      // Update a rating entry using the rest API
      var result = RestClient.Create(new Uri("http://10.1.14.114:32400/:/rate"), RestClient.httpMethod.Put, string.Empty)
        .AddHeader("X-Plex-Token", args.PlexUser.user.authToken)
        .AddParameter("key", args.CurrentTrack.CleanKey)
        .AddParameter("identifier", "com.plexapp.plugins.library")
        .AddParameter("rating", currentRatingInPlexFormat.ToString())
        .SendRequestWithExceptionResponse();

      Debug.Print(message);
    }

    private static void UpdateFileRating(SyncArgs args)
    {
      int? currentFileRating = args.CurrentFileRating;

      uint? newFileRating = Convert.ToUInt32(args.ConvertRating(args.CurrentPlexRating, RatingConvert.Plex, RatingConvert.File));

      if (newFileRating == 0) newFileRating = null;

      string message = string.Format("Updating file rating for file \"{0}\", from {1} to {2}",
          args.CurrentLocalFile.FullName,
          currentFileRating == null ? 0 : currentFileRating,
          newFileRating == null ? 0 : newFileRating);

      ShellFile so = ShellFile.FromFilePath(args.CurrentLocalFile.FullName);

#if DEBUG
      Debug.Print(message);
#else
      so.Properties.System.Rating.Value = newFileRating;
#endif

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information, message);
    }

    private static int? CurrentRatingInPlexFormat(SyncArgs args)
    {
      switch (Settings.SyncSource)
      {
        case SyncSources.FileProperties:
          return args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex);
      }

      return null;
    }

    private static int? CurrentNormalisedSourceRating(SyncArgs args)
    {
      switch (Settings.SyncSource)
      {
        case SyncSources.FileProperties:
          return args.NormalisedRating(args.CurrentFileRating, RatingConvert.File);
      }

      return null;
    }
  }
}
