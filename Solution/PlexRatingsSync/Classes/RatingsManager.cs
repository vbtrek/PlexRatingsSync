using DS.Library.MessageHandling;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class RatingsManager
  {
    public static void SyncRatings(SyncRatingsArgs args)
    {
      if (!Settings.SyncRatings) return;

      // Get all the files to sync ratings for
      args.ReportProgress("Reading Track Data From Plex...");

      string sql = @"
SELECT MTI.guid, MP.file, MTIS.rating
FROM media_items MI
INNER JOIN media_parts MP ON MP.media_item_id = MI.id
INNER JOIN metadata_items MTI ON MTI.id = MI.metadata_item_id
INNER JOIN library_sections LS ON LS.id = MI.library_section_id
LEFT JOIN metadata_item_settings MTIS ON MTIS.guid = MTI.guid AND MTIS.account_id = {0}
WHERE LS.section_type = 8";

      sql = string.Format(sql, Settings.PlexAccountId);

      List<PlexRatingsData> ratingdata = args.PlexDb.ReadPlexAndMap<PlexRatingsData>(sql);

      args.ReportProgress(ratingdata.Count);
      args.ReportProgress("Syncing Ratings...");

      // Process all the files
      foreach (PlexRatingsData file in ratingdata)
      {
        if (args.Worker.CancellationPending) return;

        args.CurrentFile = file;

        args.ReportProgress($"Syncing \"{new FileInfo(file.file).Name}\"...");

        SyncRating(args);

        args.ReportProgress(-1);
      }
    }

    private static void SyncRating(SyncRatingsArgs args)
    {
      try
      {
        if (args.CurrentFile != null && File.Exists(args.CurrentFile.file))
        {
          try
          {
            int? currentNormalisedSourceRating = CurrentNormalisedSourceRating(args);

            if (currentNormalisedSourceRating != args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex))
            {
              switch (DetermineClashHandling(args))
              {
                case RatingsClashResult.UsePlex:
                  UpdatePlexDbRating(args);
                  break;

                case RatingsClashResult.UseFileOrItunes:
                  if (args.SyncSource == SyncSources.FileProperties) UpdateFileRating(args);
                  if (args.SyncSource == SyncSources.ITunesLibrary) UpdateItunesRating(args);
                  break;
              }
            }
          }
          catch (Exception ex)
          {
            MessageManager.Instance.ExceptionWrite(new object(), ex);
          }
        }
      }
      catch (Exception ex)
      {
        MessageManager.Instance.ExceptionWrite(new object(), ex);
      }
    }

    private static RatingsClashResult DetermineClashHandling(SyncRatingsArgs args)
    {
      var result = RatingsClashResult.Cancel;

      switch (args.SyncHandling)
      {
        case SyncModes.FileOrItunesToPlex:
          result = RatingsClashResult.UseFileOrItunes;
          break;

        case SyncModes.PlexToFileOrItunes:
          result = RatingsClashResult.UsePlex;
          break;

        case SyncModes.TwoWay:
          int? currentNormalisedSourceRating = CurrentNormalisedSourceRating(args);

          if (args.ClashHandling != ClashWinner.AlwaysPrompt)
          {
            if (args.CurrentPlexRating == 0 && currentNormalisedSourceRating > 0)
              result = RatingsClashResult.UseFileOrItunes;

            if (currentNormalisedSourceRating == 0 && args.CurrentPlexRating > 0)
              result = RatingsClashResult.UsePlex;
          }

          if (result == RatingsClashResult.Cancel)
          {
            switch (args.ClashHandling)
            {
              case ClashWinner.FileOrItunes:
                result = RatingsClashResult.UseFileOrItunes;
                break;

              case ClashWinner.Plex:
                result = RatingsClashResult.UsePlex;
                break;
            }

            if (result == RatingsClashResult.Cancel && args.ClashHandling != ClashWinner.Skip)
            {
              FileInfo fi = new FileInfo(args.CurrentFile.file);

              TaskDialogs dlg = new TaskDialogs();
              
              result = dlg.RatingsClash($"{args.PlexTitle}{Environment.NewLine}{fi.Name}", (int)args.CurrentPlexRating, (int)currentNormalisedSourceRating);
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

    private static void UpdatePlexDbRating(SyncRatingsArgs args)
    {
      int? currentNormalisedSourceRating = CurrentNormalisedSourceRating(args);

      int? plexDbRating = args.CurrentPlexRating;

      string sql = string.Empty;
      string message = string.Empty;

      // the rating(s) of a given file
      sql = string.Format(
          @"SELECT * FROM metadata_item_settings WHERE account_id = {0} AND guid = '{1}';",
          Settings.PlexAccountId, args.CurrentFile.guid);

      if (args.PlexDb.RecordsExists(sql))
      {
        message = string.Format("Updating Plex rating for file \"{0}\" from {1} to {2}",
            args.CurrentFile.file,
            plexDbRating == null ? 0 : plexDbRating,
            currentNormalisedSourceRating == null ? 0 : currentNormalisedSourceRating);

        MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
            message);

        args.ReportProgress(-2);

        // Update a rating entry
        sql = @"
UPDATE metadata_item_settings SET rating = {0} WHERE account_id = {1} AND guid = '{2}'";

        sql = string.Format(sql, currentNormalisedSourceRating, Settings.PlexAccountId, args.CurrentFile.guid);
      }
      else
      {
        message = string.Format("Creating Plex rating for file \"{0}\", rating {1}",
            args.CurrentFile.file,
            currentNormalisedSourceRating == null ? 0 : currentNormalisedSourceRating);

        MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
            message);

        args.ReportProgress(-3);

        // Create a rating entry
        sql = @"
INSERT INTO metadata_item_settings ([account_id], [guid], [rating], [view_offset], [view_count], [last_viewed_at], [created_at], [updated_at])
VALUES({0}, '{1}', {2}, NULL, 0, NULL, DATE('now'), DATE('now'));";

        sql = string.Format(sql, Settings.PlexAccountId, args.CurrentFile.guid, currentNormalisedSourceRating);
      }
#if DEBUG
      Debug.Print(message);
#else
      m_PlexDb.ExecutePlexSql(sql);
#endif
    }

    private static void UpdateFileRating(SyncRatingsArgs args)
    {
      int? currentFileRating = args.CurrentFileRating;

      uint? newFileRating = Convert.ToUInt32(args.ConvertRating(args.CurrentPlexRating, RatingConvert.Plex, RatingConvert.File));

      if (newFileRating == 0) newFileRating = null;

      string message = string.Format("Updating file rating for file \"{0}\", from {1} to {2}",
          args.CurrentFile.file,
          currentFileRating == null ? 0 : currentFileRating,
          newFileRating == null ? 0 : newFileRating);

      ShellFile so = ShellFile.FromFilePath(args.CurrentFile.file);

#if DEBUG
      Debug.Print(message);
#else
      so.Properties.System.Rating.Value = newRating;
#endif

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information, message);
    }

    private static void UpdateItunesRating(SyncRatingsArgs args)
    {
      int? currentItunesRating = args.CurrentItunesRating;

      uint? newItunesRating = Convert.ToUInt32(args.ConvertRating(args.CurrentPlexRating, RatingConvert.Plex, RatingConvert.Itunes));

      if (newItunesRating == 0) newItunesRating = null;

      string message = string.Format("Updating iTunes rating for file \"{0}\", from {1} to {2}",
          args.CurrentFile.file,
          currentItunesRating == null ? 0 : currentItunesRating,
          newItunesRating == null ? 0 : newItunesRating);

      // TODO_DS1 Update iTunes

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information, message);
    }

    private static int? CurrentNormalisedSourceRating(SyncRatingsArgs args)
    {
      switch (args.SyncSource)
      {
        case SyncSources.FileProperties:
          return args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex);

        case SyncSources.ITunesLibrary:
          return args.ConvertRating(args.CurrentItunesRating, RatingConvert.Itunes, RatingConvert.Plex);
      }

      return null;
    }
  }
}
