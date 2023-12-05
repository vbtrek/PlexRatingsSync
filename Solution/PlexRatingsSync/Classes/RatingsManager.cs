using DS.Library.MessageHandling;
using DS.PlexRatingsSync.Classes;
using DS.PlexRatingsSync.Classes.PlexApi;
using DS.PlexRatingsSync.Classes.PlexTvApi;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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
        new Uri($"{Settings.PlexUri}library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      MediaContainer mediaContainer = new MediaContainer();

      try
      {
        using (XmlReader reader = XmlReader.Create(new StringReader(result)))
          mediaContainer.ReadFromXml(reader);
      }
      catch
      { }

      args.ReportProgress(mediaContainer.Size);

      args.ReportProgress(SyncArgs.ProgressType.UpdateSubLabel, $"{mediaContainer.Size:#,###,##0} Tracks");

      args.ReportProgress("Syncing Ratings...");

      args.PlexUser = plexUser;

      args.MusicFolderMappings = localMusicRoots;

      int trackCount = 1;

      // Process all the files
      foreach (var track in mediaContainer.Tracks)
      {
        if (args.Worker.CancellationPending) return;

        args.CurrentTrack = track;

        args.ReportProgress(SyncArgs.ProgressType.UpdateSubLabel, $"{trackCount:#,###,##0} / {mediaContainer.Size:#,###,##0} Tracks");

        args.ReportProgress($"Syncing \"{track.GrandparentTitle} - {track.ParentTitle} - {track.Title}\"...");

        SyncRating(args);

        args.ReportProgress(SyncArgs.ProgressType.IncrementProgressBar);

        trackCount++;
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

              // TODO_DS1 Need to call an api to get this
              // https://www.plexopedia.com/plex-media-server/api/server/identity/
              string server = "edffd15c22217aafa277ecc18b741967c2a4874c";

              string artistUri = $"{Settings.PlexUri}web/index.html#!/server/{server}/details?key=%2Flibrary%2Fmetadata%2F{args.CurrentTrack.CleanGrandparentKey}";
              string albumUri = $"{Settings.PlexUri}web/index.html#!/server/{server}/details?key=%2Flibrary%2Fmetadata%2F{args.CurrentTrack.CleanParentKey}";

              string fileLabel = $"<a href=\"{artistUri}\">{args.CurrentTrack.GrandparentTitle}</a>{Environment.NewLine}";
              fileLabel += $"<a href=\"{albumUri}\">{args.CurrentTrack.ParentTitle}</a>{Environment.NewLine}";
              if (args.CurrentTrack.Index == 0)
                fileLabel += $"{args.CurrentTrack.Title}";
              else
              {
                if (args.CurrentTrack.ParentIndex == 0)
                  fileLabel += $"Track: {args.CurrentTrack.Index} - {args.CurrentTrack.Title}";
                else
                  fileLabel += $"Disc {args.CurrentTrack.ParentIndex} | Track {args.CurrentTrack.Index} - {args.CurrentTrack.Title}";
              }

              result = dlg.RatingsClash($"{fileLabel}", normalisedPlexRating, currentNormalisedSourceRating);
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

      if (currentRatingInPlexFormat == null) currentRatingInPlexFormat = -1;

      int? plexDbRating = args.CurrentPlexRating;

      string message = string.Format("Updating Plex rating for file \"{0}\" from {1} to {2}",
          args.CurrentLocalFile.FullName,
          plexDbRating == null ? 0 : plexDbRating,
          currentRatingInPlexFormat == null ? 0 : currentRatingInPlexFormat);

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
          message);

      args.ReportProgress(SyncArgs.ProgressType.IncrementUpdatedCount);

      // Update a rating entry using the rest API
      var result = RestClient.Create(new Uri($"{Settings.PlexUri}:/rate"), RestClient.httpMethod.Put, string.Empty)
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
#endif

      so.Properties.System.Rating.Value = newFileRating;

      args.ReportProgress(SyncArgs.ProgressType.IncrementUpdatedCount);

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
