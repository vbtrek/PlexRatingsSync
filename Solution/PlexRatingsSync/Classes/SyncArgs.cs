using DS.Library.MessageHandling;
using DS.PlexRatingsSync.Classes.PlexApi;
using DS.PlexRatingsSync.Classes.PlexTvApi;
using Microsoft.WindowsAPICodePack.Shell;
using Sentry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
      ResetCounters,
      UpdateSubLabel
    }

    #endregion

    #region Private members

    private Dictionary<string, string> _MusicFolderMappings;

    private Track _CurrentTrack = null;

    private int? _CachedFileRating = null;

    private bool _DisposedValue;

    #endregion

    #region Public properties

    public BackgroundWorker Worker { get; set; }

    public PlexTvRoot PlexUser { get; set; }

    public Dictionary<string, string> MusicFolderMappings
    {
      get => _MusicFolderMappings;
      set { _MusicFolderMappings = value; }
    }

    public Track CurrentTrack
    {
      get => _CurrentTrack;
      set { _CachedFileRating = null; _CurrentTrack = value; }
    }

    public FileInfo CurrentLocalFile
    {
      get
      {
        try
        {
          if (_MusicFolderMappings != null && _CurrentTrack != null)
          {
            if (_CurrentTrack.Media?.Part?.File == null)
              return null;

            string file = _CurrentTrack.Media.Part.File;

            foreach (var folder in _MusicFolderMappings)
            {
              if (file.StartsWith(folder.Key))
                return new FileInfo($"{folder.Value}{file.Substring(folder.Key.Length)}");
            }
          }
        }
        catch
        { }

        return null;
      }
    }

    public int? CurrentPlexRating => (int?)_CurrentTrack.UserRating;

    // TODO_DS1 Need to resolve the file location correctly
    public int? CurrentFileRating => ReadFileRating();

    #endregion

    #region Constructor

    public SyncArgs(BackgroundWorker worker)
    {
      Worker = worker;
    }

    #endregion

    #region Public methods

    public void ReportProgress(ProgressType type)
    {
      ReportProgress(type, string.Empty);
    }

    public void ReportProgress(ProgressType type, string info)
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
          Worker.ReportProgress(-4);
          break;
        case ProgressType.UpdateSubLabel:
          Worker.ReportProgress(-5, info);
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
      }

      return null;
    }

    public string PlexTitle => _CurrentTrack?.Title ?? string.Empty;

    #endregion

    #region Private methods

    private int? ReadFileRating()
    {
      int? fileRating = null;

      if (_CachedFileRating == null)
      {
        try
        {
          ShellFile so = ShellFile.FromFilePath(CurrentLocalFile.FullName);

          if (so.Properties.System.Rating.Value != null)
            fileRating = Convert.ToInt32(so.Properties.System.Rating.Value);
        }
        catch (Exception ex)
        {
          SentrySdk.CaptureException(ex);

          MessageManager.Instance.ExceptionWrite(new object(), ex);

          return null;
        }

        _CachedFileRating = fileRating;
      }
      else
        fileRating = _CachedFileRating;

      return fileRating;
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

    #endregion

    #region Dispose pattern

    protected virtual void Dispose(bool disposing)
    {
      if (!_DisposedValue)
      {
        if (disposing)
        {
          // Dispose managed state (managed objects)
          //if (PlexDb != null) PlexDb.Dispose();
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
