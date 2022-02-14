﻿using DS.Library.MessageHandling;
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
            // TODO_DS1 Handle iTunes source
            if (args.NormalisedRating(args.CurrentFileRating, RatingConvert.File) != args.NormalisedRating(args.CurrentPlexRating, RatingConvert.Plex))
            {
              switch (DetermineClashHandling(args))
              {
                case RatingsClashResult.UsePlex:
                  UpdatePlexDbRating(args);
                  break;

                case RatingsClashResult.UseFileOrItunes:
                  UpdateFileRating(args);
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
      // if (args.PlexDatabaseRating != null)
      
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
          if (args.ClashHandling != ClashWinner.AlwaysPrompt)
          {
            if (args.CurrentPlexRating == 0 && args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex) > 0)
              result = RatingsClashResult.UseFileOrItunes;

            if (args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex) == 0 && args.CurrentPlexRating > 0)
              result = RatingsClashResult.UsePlex;
          }

          if (result == RatingsClashResult.Cancel)
          {
            switch (args.ClashHandling)
            {
              case ClashWinner.File:
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
              
              result = dlg.RatingsClash($"{args.PlexTitle}{Environment.NewLine}{fi.Name}", (int)args.CurrentPlexRating, (int)args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex));
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
      int? plexFileRating = args.ConvertRating(args.CurrentFileRating, RatingConvert.File, RatingConvert.Plex);

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
            plexFileRating == null ? 0 : plexFileRating);

        MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
            message);

        args.ReportProgress(-2);

        // Update a rating entry
        sql = @"
UPDATE metadata_item_settings SET rating = {0} WHERE account_id = {1} AND guid = '{2}'";

        sql = string.Format(sql, plexFileRating, Settings.PlexAccountId, args.CurrentFile.guid);
      }
      else
      {
        message = string.Format("Creating Plex rating for file \"{0}\", rating {1}",
            args.CurrentFile.file,
            plexFileRating == null ? 0 : plexFileRating);

        MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information,
            message);

        args.ReportProgress(-3);

        // Create a rating entry
        sql = @"
INSERT INTO metadata_item_settings ([account_id], [guid], [rating], [view_offset], [view_count], [last_viewed_at], [created_at], [updated_at])
VALUES({0}, '{1}', {2}, NULL, 0, NULL, DATE('now'), DATE('now'));";

        sql = string.Format(sql, Settings.PlexAccountId, args.CurrentFile.guid, plexFileRating);
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
//      uint? newFileRating = Convert.ToUInt32(args.PlexRatingInFileFormat);

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
  }
}
