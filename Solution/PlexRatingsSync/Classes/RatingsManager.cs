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
  public enum SyncModes
  {
    FileToPlex = 1,
    PlexToFile = 2,
    TwoWay = 3
  }

  public enum ClashWinner
  {
    Skip = 0,
    Plex = 1,
    File = 2,
    Prompt = 3,
    AlwaysPrompt = 4
  }

  public enum RatingsClashResult
  {
    Cancel = 0,
    UsePlex = 1,
    UseFile = 2
  }

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
            if (args.FileRatingInPlexFormat != args.PlexDatabaseRating)
            {
              switch (DetermineClashHandling(args))
              {
                case RatingsClashResult.UsePlex:
                  UpdatePlexDbRating(args);
                  break;

                case RatingsClashResult.UseFile:
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
        case SyncModes.FileToPlex:
          result = RatingsClashResult.UseFile;
          break;

        case SyncModes.PlexToFile:
          result = RatingsClashResult.UsePlex;
          break;

        case SyncModes.TwoWay:
          if (args.ClashHandling != ClashWinner.AlwaysPrompt)
          {
            if (args.PlexDatabaseRating == 0 && args.FileRatingInPlexFormat > 0)
              result = RatingsClashResult.UseFile;

            if (args.FileRatingInPlexFormat == 0 && args.PlexDatabaseRating > 0)
              result = RatingsClashResult.UsePlex;
          }

          if (result == RatingsClashResult.Cancel)
          {
            switch (args.ClashHandling)
            {
              case ClashWinner.File:
                result = RatingsClashResult.UseFile;
                break;

              case ClashWinner.Plex:
                result = RatingsClashResult.UsePlex;
                break;
            }

            if (result == RatingsClashResult.Cancel && args.ClashHandling != ClashWinner.Skip)
            {
              FileInfo fi = new FileInfo(currentFile);

              TaskDialogs dlg = new TaskDialogs();
              
              result = dlg.RatingsClash(itunesFileTrack.Album + "\r\n" + fi.Name, iRating, fRating);
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
      int? plexFileRating = args.FileRatingInPlexFormat;

      int? plexDbRating = args.PlexDatabaseRating;

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
      uint? fileRating = FileRating(args.CurrentFile.file);

      int? plexFileRating = args.FileRatingInPlexFormat;

      int? plexDbRating = (int?)args.CurrentFile.rating;

      uint? newRating = Convert.ToUInt32(FileRatingFromPlexRating(plexDbRating));

      if (newRating == 0) newRating = null;

      string message = string.Format("Updating file rating for file \"{0}\", from {1} to {2}",
          args.CurrentFile.file,
          fileRating == null ? 0 : fileRating,
          newRating == null ? 0 : newRating);

      ShellFile so = ShellFile.FromFilePath(args.CurrentFile.file);

#if DEBUG
      Debug.Print(message);
#else
      so.Properties.System.Rating.Value = newRating;
#endif

      MessageManager.Instance.MessageWrite(new object(), MessageItem.MessageLevel.Information, message);
    }

    private static uint? FileRating(string file)
    {
      uint? fileRating = null;

      try
      {
        ShellFile so = ShellFile.FromFilePath(file);

        if (so.Properties.System.Rating.Value != null)
          fileRating = (uint)so.Properties.System.Rating.Value;
      }
      catch (Exception ex)
      {
        MessageManager.Instance.ExceptionWrite(new RatingsManager(), ex);

        return null;
      }

      return fileRating;
    }

    private static int? FileRatingFromPlexRating(int? plexRating)
    {
      try
      {
        switch (plexRating)
        {
          case 2:
            return 1;
          case 4:
            return 25;
          case 6:
            return 50;
          case 8:
            return 75;
          case 10:
            return 99;
          default:
            return null;
        }
      }
      catch (Exception ex)
      {
        MessageManager.Instance.ExceptionWrite(new RatingsManager(), ex);
        return null;
      }
    }
  }

  public class SyncRatingsArgs
  {
    public BackgroundWorker Worker { get; set; }

    public PlexDatabaseControlller PlexDb { get; set; }

    public PlexRatingsData CurrentFile { get; set; }

    public SyncRatingsArgs(BackgroundWorker worker, PlexDatabaseControlller plexDb)
    {
      Worker = worker;

      PlexDb = plexDb;
    }

    public void ReportProgress(string staus)
    {
      if (Worker == null) return;

      Worker.ReportProgress(0, staus);
    }

    public void ReportProgress(int percentProgress)
    {
      if (Worker == null) return;

      Worker.ReportProgress(percentProgress);
    }

    public int? FileRatingInPlexFormat => PlexRatingFromFile(CurrentFile?.file);

    public int? PlexDatabaseRating => (int?)CurrentFile.rating;

    private static int? PlexRatingFromFile(string file)
    {
      try
      {
        uint? fileRating = null;

        ShellFile so = ShellFile.FromFilePath(file);

        if (so.Properties?.System?.Rating?.Value != null)
          fileRating = (uint)so.Properties.System.Rating.Value;

        if (fileRating == null)
          return null;

        if (fileRating < 1)
          return null;
        else if (fileRating < 13)
          return 2;
        else if (fileRating < 38)
          return 4;
        else if (fileRating < 63)
          return 6;
        else if (fileRating < 87)
          return 8;
        else
          return 10;
      }
      catch (Exception ex)
      {
        MessageManager.Instance.ExceptionWrite(new RatingsManager(), ex);
        return null;
      }
    }
  }
}
