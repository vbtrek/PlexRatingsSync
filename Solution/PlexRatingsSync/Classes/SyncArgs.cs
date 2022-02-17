using DS.Library.MessageHandling;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync
{
  public class SyncArgs : IDisposable
  {
    #region Enums

    public enum ProgressType
    {
      IncrementProgressBar,
      IncrementUpdatedCount,
      IncrementNewCount,
      ResetCounters
    }

    #endregion

    #region Private members

    private PlexRatingsData _CurrentFile = null;

    private int? _CachedFileRating = null;

    private bool _DisposedValue;

    #endregion

    #region Public properties

    public BackgroundWorker Worker { get; set; }

    public PlexDatabaseControlller PlexDb { get; set; }

    public ItunesManager ItunesData { get; set; }

    public PlexRatingsData CurrentFile
    {
      get => _CurrentFile;
      set { _CachedFileRating = null; _CurrentFile = value; }
    }

    public int? CurrentPlexRating => (int?)CurrentFile.rating;

    public int? CurrentFileRating => ReadFileRating(CurrentFile?.file);

    public int? CurrentItunesRating => ReadItunesRating();

    #endregion

    #region Constructor

    public SyncArgs(BackgroundWorker worker)
    {
      Worker = worker;

      PlexDb = new PlexDatabaseControlller(Settings.PlexDatabase);

      ItunesData = new ItunesManager(Settings.ItunesLibraryPath);

      if (Settings.SyncPlaylists || (Settings.SyncRatings && Settings.SyncSource == SyncSources.ITunesLibrary))
      {
        ReportProgress("Reading Playlists from iTunes...");

        ItunesData.ReadItunesData(true, true);
      }
    }

    #endregion

    #region Public methods

    public void ReportProgress(ProgressType type)
    {
      if (Worker == null) return;

      switch (type)
      {
        case ProgressType.IncrementProgressBar:
          Worker.ReportProgress(-1);
          break;
        case ProgressType.IncrementUpdatedCount:
          Worker.ReportProgress(-2);
          break;
        case ProgressType.IncrementNewCount:
          Worker.ReportProgress(-3);
          break;
        case ProgressType.ResetCounters:
          Worker.ReportProgress(4);
          break;
      }
    }

    public void ReportProgress(string status)
    {
      if (Worker == null) return;

      Worker.ReportProgress(0, status);
    }

    public void ReportProgress(int percentProgress)
    {
      if (Worker == null) return;

      Worker.ReportProgress(percentProgress);
    }

    // Use 1-5 as a Normalised rating
    // Plex ratings are 2,4,6,8,10
    public int? NormalisedRating(int? rating, RatingConvert fromType)
    {
      // Convert from fromType to Normalised
      switch (fromType)
      {
        case RatingConvert.Plex:
          return NormalisePlexRating(rating);

        case RatingConvert.File:
          return NormaliseFileRating(rating);

        case RatingConvert.Itunes:
          return NormaliseItunesRating(rating);
      }

      return null;
    }

    // Use 1-5 as a Normalised rating
    // Plex ratings are 2,4,6,8,10
    public int? ConvertRating(int? rating, RatingConvert fromType, RatingConvert toType)
    {
      int? normalisedRating = NormalisedRating(rating, fromType);

      // Convert from Normalised to toType
      switch (toType)
      {
        case RatingConvert.Plex:
          return PlexRatingFromNormalised(normalisedRating);

        case RatingConvert.File:
          return FileRatingFromNormalised(normalisedRating);

        case RatingConvert.Itunes:
          return ItunesRatingFromNormalised(normalisedRating);
      }

      return null;
    }

    public string PlexTitle
    {
      get
      {
        string sql = "SELECT title FROM metadata_items WHERE guid = '{0}' AND metadata_type = 10";

        sql = string.Format(sql, CurrentFile.guid);

        return (string)PlexDb.ReadPlexValue(sql);
      }
    }

    #endregion

    #region Private methods

    private int? ReadFileRating(string file)
    {
      int? fileRating = null;

      if (_CachedFileRating == null)
      {
        try
        {
          ShellFile so = ShellFile.FromFilePath(file);

          if (so.Properties.System.Rating.Value != null)
            fileRating = Convert.ToInt32(so.Properties.System.Rating.Value);
        }
        catch (Exception ex)
        {
          MessageManager.Instance.ExceptionWrite(new object(), ex);

          return null;
        }

        _CachedFileRating = fileRating;
      }
      else
        fileRating = _CachedFileRating;

      return fileRating;
    }

    private int? ReadItunesRating()
    {
      // TODO_DS1 Need to get current iTunes Rating somehow
      Debug.Assert(false);

      return 0;
    }

    private static int? NormaliseFileRating(int? fileRating)
    {
      if (fileRating == null)
        return null;

      switch (fileRating)
      {
        case int n when n < 1:
          return null;
        case int n when n < 13:
          return 1;
        case int n when n < 38:
          return 2;
        case int n when n < 63:
          return 3;
        case int n when n < 87:
          return 4;
        default:
          return 5;
      }
    }

    private static int? NormalisePlexRating(int? plexRating)
    {
      switch (plexRating)
      {
        case 2:
          return 1;
        case 4:
          return 2;
        case 6:
          return 3;
        case 8:
          return 4;
        case 10:
          return 5;
        default:
          return null;
      }
    }

    private static int? NormaliseItunesRating(int? itunesRating)
    {
      switch (itunesRating)
      {
        case 20:
          return 1;
        case 40:
          return 2;
        case 60:
          return 3;
        case 80:
          return 4;
        case 100:
          return 5;
        default:
          return null;
      }
    }

    private static int? PlexRatingFromNormalised(int? normalisedRating)
    {
      switch (normalisedRating)
      {
        case 1:
          return 2;
        case 2:
          return 4;
        case 3:
          return 6;
        case 4:
          return 8;
        case 5:
          return 10;
        default:
          return null;
      }
    }

    private static int? FileRatingFromNormalised(int? normalisedRating)
    {
      if (normalisedRating == null)
        return null;

      switch (normalisedRating)
      {
        case 1:
          return 1;
        case 2:
          return 25;
        case 3:
          return 50;
        case 4:
          return 75;
        case 5:
          return 99;
        default:
          return null;
      }
    }

    private static int? ItunesRatingFromNormalised(int? normalisedRating)
    {
      switch (normalisedRating)
      {
        case 1:
          return 20;
        case 2:
          return 40;
        case 3:
          return 60;
        case 4:
          return 80;
        case 5:
          return 100;
        default:
          return null;
      }
    }

    #endregion

    #region Dispose pattern

    protected virtual void Dispose(bool disposing)
    {
      if (!_DisposedValue)
      {
        if (disposing)
        {
          // Dispose managed state (managed objects)
          if (PlexDb != null) PlexDb.Dispose();
        }

        // Free any unmanaged resources (unmanaged objects) and override finalizer
        // Set large fields to null

        _DisposedValue = true;
      }
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);

      GC.SuppressFinalize(this);
    }

    #endregion
  }
}
