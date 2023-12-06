using DS.Library.MessageHandling;
using DS.PlexRatingsSync.Classes;
using DS.PlexRatingsSync.Classes.PlexApi;
using DS.PlexRatingsSync.Classes.PlexTvApi;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.WindowsAPICodePack.Shell;
using MimeKit;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

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
      var media = RestClient.Create(
        new Uri($"{Settings.PlexUri}library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      MediaContainer mediaContainer = MediaContainer.Parse(media);


      // https://www.plexopedia.com/plex-media-server/api/server/identity/
      var server = RestClient.Create(
        new Uri($"{Settings.PlexUri}identity"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      MediaContainer serverDetails = MediaContainer.Parse(server);

      args.InvalidTracks.Clear();

      args.PlexServerIdentitfier = serverDetails.MachineIdentifier;

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

      SendInvalidTracksEmail(args);
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

            int? currentNormalisedPlexRating = args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex);

            if (currentNormalisedSourceRating != args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex))
            {
              switch (DetermineClashHandling(args))
              {
                case RatingsClashResult.UpdatePlex:
                  UpdatePlexRating(args);

                  ValidateRating(args, currentNormalisedSourceRating);

                  break;

                case RatingsClashResult.UpdateFile:
                  if (Settings.SyncSource == SyncSources.FileProperties)
                    UpdateFileRating(args);

                  ValidateRating(args, currentNormalisedPlexRating);

                  break;

                case RatingsClashResult.Cancel:
                  ValidateRating(args, -1);

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

    private static void ValidateRating(SyncArgs args, int? chosenRating)
    {
      // Read the plex file using the API
      var media = RestClient.Create(
        new Uri($"{Settings.PlexUri}library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", args.PlexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .AddParameter("id", args.CurrentTrack.CleanKey)
        .SendRequestWithExceptionResponse();

      MediaContainer mediaContainer = MediaContainer.Parse(media);

      int? currentPlexRating = args.NormalisedRating(Convert.ToInt32(mediaContainer.Tracks[0].UserRating), RatingConvert.Plex);

      // Read the file rating
      int? currentFileRating = CurrentNormalisedSourceRating(args);

      // confirm they both match chosenRating
      if ((currentPlexRating ?? 0) != (chosenRating ?? 0) || (currentFileRating ?? 0) != (chosenRating ?? 0))
        args.AddInvalidTrack(chosenRating, currentPlexRating, currentFileRating);
    }

    private static void SendInvalidTracksEmail(SyncArgs args)
    {
      string allTrackInfo = string.Empty;

      foreach (var item in args.InvalidTracks)
      {
        if (!string.IsNullOrWhiteSpace(allTrackInfo))
          allTrackInfo += "<hr/>";

        allTrackInfo += item;
      }

      string body = $@"<h1>Mismatched Tracks</h1>
Total Tracks: {args.InvalidTracks.Count}<br/>
<br/>
{allTrackInfo}
";

      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(Settings.EmailFromName, Settings.EmailFromEmailAddess));
      message.To.Add(new MailboxAddress(Settings.EmailToName, Settings.EmailToEmailAddess));
      
      message.Subject = "Plex Ratings Sync";

      message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = body
      };

      using (var client = new SmtpClient())
      {
        client.Connect(Settings.SmtpServer, Settings.SmtpPort, Settings.SecurityOption);
        client.Authenticate(Settings.SmtpUsername, Settings.SmtpPassword);
        client.Send(message);
        client.Disconnect(true);
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

              case ClashWinner.LargestRating:
                if ((currentNormalisedSourceRating ?? 0) > (normalisedPlexRating ?? 0))
                  result = RatingsClashResult.UpdatePlex;

                if ((normalisedPlexRating ?? 0) > (currentNormalisedSourceRating ?? 0))
                  result = RatingsClashResult.UpdateFile;
                break;
            }

            if (result == RatingsClashResult.Cancel && Settings.ClashHandling != ClashWinner.Skip)
            {
              TaskDialogs dlg = new TaskDialogs();

              string artistUri = args.Artist(true, true);
              string albumUri = args.Album(true, true);
              string track = args.Track();

              string fileLabel = $@"{artistUri}
{albumUri}
{track}";

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
