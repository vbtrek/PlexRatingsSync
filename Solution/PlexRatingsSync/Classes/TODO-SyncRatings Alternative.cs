using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using iTunesLib;
using WMPLib;
using Microsoft.WindowsAPICodePack.Shell;
using System.IO;

// TODO_DS1 Review and merge in Clash Handling and Sync Modes
namespace iTunesLibraryUpdater
{
  public enum SyncModes
  {
    FileToItunes = 1,
    ItunesToFile = 2,
    TwoWay = 3
  }

  public enum ClashWinner
  {
    Itunes = 1,
    File = 2,
    Prompt = 3,
    AlwaysPrompt = 4
  }

  public class SyncRatings
  {
    private enum SyncState
    {
      Initializing,
      Syncing,
      Done,
      Cancelled,
      Error
    }

    private SyncState state;
    private double progress;
    private string currentFile;
    private string currentError;
    private bool cancelled;

    public int RatingsUpdated;

    private SyncInfo m_SyncInfo { get; set; }

    public delegate void ProgressEventHandler(object sender, MyProgressChangedEventArgs e);
    public event ProgressEventHandler Progress;

    public void StartSync(SyncInfo syncInfo)
    {
      if (syncInfo == null)
        throw new InvalidOperationException();

      if (!syncInfo.SyncRatings)
        return;

      m_SyncInfo = syncInfo;

      state = SyncState.Initializing;
      progress = 0;
      currentFile = string.Empty;
      currentError = null;
      cancelled = false;
      RatingsUpdated = 0;

      if (m_SyncInfo.UseWMP)
        DoWorkWMP();
      else
      {
        //Thread thread = new Thread(new ThreadStart(DoWorkFileSystem));
        //thread.Start();
        //thread.Join();
        DoWorkFileSystem();
      }
    }

    private void DoWorkFileSystem()
    {
      iTunesApp itunes = null;

      try
      {
        itunes = new iTunesApp();

        state = SyncState.Syncing;

        IITTrackCollection itunesTracks = itunes.LibraryPlaylist.Tracks;

        for (int i = 1; i <= itunesTracks.Count; i++)
        {
          if (cancelled)
          {
            state = SyncState.Cancelled;
            return;
          }

          //                    Thread.Sleep(20);
          System.Windows.Forms.Application.DoEvents();

          IITTrack itunesTrack = itunesTracks[i];

          if (itunesTrack.Kind == ITTrackKind.ITTrackKindFile)
          {
            IITFileOrCDTrack itunesFileTrack = (IITFileOrCDTrack)itunesTrack;

            currentFile = itunesFileTrack.Location;

            MyProgressChangedEventArgs e = new MyProgressChangedEventArgs();
            e.PercentComplete = (int)progress;
            e.CurrentPathFile = currentFile;
            OnProgressChanged(e);

            if (currentFile != null && File.Exists(currentFile))
            {
              uint? fileRating = null;
              ShellFile so = ShellFile.FromFilePath(currentFile);

              if (so.Properties.System.Rating.Value != null)
                fileRating = (uint)so.Properties.System.Rating.Value;

              try
              {
                int? itunesStandardisedRating = itunesFileTrack.ratingKind == ITRatingKind.ITRatingKindUser ? RatingHelper.FromItunesRating(itunesFileTrack.Rating) : null;
                int? fileStandardisedRating = RatingHelper.FromWmpRating(Convert.ToString(fileRating));
                int iRating = itunesStandardisedRating == null ? 0 : (int)itunesStandardisedRating;
                int fRating = fileStandardisedRating == null ? 0 : (int)fileStandardisedRating;

                if (m_SyncInfo.SyncRatings)
                {
                  try
                  {
                    if (iRating != fRating)
                    {
                      RatingsClashReturn rtn = RatingsClashReturn.Cancel;

                      switch (m_SyncInfo.SyncHandling)
                      {
                        case SyncModes.FileToItunes:
                          rtn = RatingsClashReturn.UseFile;
                          break;
                        case SyncModes.ItunesToFile:
                          rtn = RatingsClashReturn.UseItunes;
                          break;
                        case SyncModes.TwoWay:
                          if (m_SyncInfo.ClashHandling != ClashWinner.AlwaysPrompt)
                          {
                            if (iRating == 0 && fRating > 0) rtn = RatingsClashReturn.UseFile;
                            if (fRating == 0 && iRating > 0) rtn = RatingsClashReturn.UseItunes;
                          }

                          if (rtn == RatingsClashReturn.Cancel)
                          {
                            switch (m_SyncInfo.ClashHandling)
                            {
                              case ClashWinner.File:
                                rtn = RatingsClashReturn.UseFile;
                                break;
                              case ClashWinner.Itunes:
                                rtn = RatingsClashReturn.UseItunes;
                                break;
                            }

                            if (rtn == RatingsClashReturn.Cancel && !Program.m_runSilent)
                            {
                              FileInfo fi = new FileInfo(currentFile);
                              TaskDialogs dlg = new TaskDialogs();
                              rtn = dlg.RatingsClash(itunesFileTrack.Album + "\r\n" + fi.Name, iRating, fRating);
                            }
                          }

                          if (rtn == RatingsClashReturn.Cancel)
                          {
                            Program.DoDebug("SKIPPED: Auto sync of '" + so.Name + "'. iTunes rating: " + iRating.ToString() + ", file rating: " + fRating.ToString());
                          }
                          break;
                      }


                      string fromRating = null;
                      string toRating = null;

                      switch (rtn)
                      {
                        case RatingsClashReturn.UseFile:
                          if (itunesFileTrack.Rating != RatingHelper.ToItunesRating(fileStandardisedRating))
                          {
                            fromRating = RatingHelper.FromItunesRating(itunesFileTrack.Rating).ToString();
                            if (String.IsNullOrEmpty(fromRating)) fromRating = "0";
                            toRating = RatingHelper.ToItunesRating(fileStandardisedRating).ToString();
                            if (String.IsNullOrEmpty(toRating)) toRating = "0";

                            m_SyncInfo.ReportList.Items.Add(
                                String.Format("{0} - '{1}' - iTunes rating updated from {2} to {3} stars",
                                    itunesFileTrack.Album,
                                    so.Name,
                                    itunesFileTrack.Rating.ToString(),
                                    (fileStandardisedRating == null ? "0" : fileStandardisedRating.ToString())
                                ));
                            m_SyncInfo.UI.ShowCounts();
                            Program.DoDebug("SYNCED: Auto sync of '" + so.Name + "'. iTunes rating updated to " + toRating + " stars");
                            RatingsUpdated += 1;
                            itunesFileTrack.Rating = RatingHelper.ToItunesRating(fileStandardisedRating);
                          }
                          break;

                        case RatingsClashReturn.UseItunes:
                          fromRating = RatingHelper.FromWmpRating(so.Properties.System.Rating.Value.ToString()).ToString();
                          if (String.IsNullOrEmpty(fromRating)) fromRating = "0";
                          toRating = itunesStandardisedRating.ToString();
                          if (String.IsNullOrEmpty(toRating)) toRating = "0";

                          m_SyncInfo.ReportList.Items.Add(
                              String.Format("{0} - '{1}' - File rating updated from {2} to {3} stars",
                                  itunesFileTrack.Album,
                                  so.Name,
                                  fromRating,
                                  toRating
                              ));
                          m_SyncInfo.UI.ShowCounts();
                          Program.DoDebug("SYNCED: Auto sync of '" + so.Name + "'. File rating updated to " + toRating + " stars");
                          RatingsUpdated += 1;
                          uint? rating = Convert.ToUInt32(RatingHelper.ToWmpRating(itunesStandardisedRating));
                          if (rating == 0) rating = null;
                          so.Properties.System.Rating.Value = rating;
                          break;
                        case RatingsClashReturn.Cancel:
                          m_SyncInfo.ReportList.Items.Add(itunesFileTrack.Album + " - '" + so.Name + "' - Rating difference skipped");
                          m_SyncInfo.UI.ShowCounts();
                          break;
                      }
                    }

                    /*
                    if (itunesStandardisedRating == null)
                    {
                        if (itunesFileTrack.Rating != RatingHelper.ToItunesRating(fileStandardisedRating))
                        {
                            RatingsUpdated += 1;
                            itunesFileTrack.Rating = RatingHelper.ToItunesRating(fileStandardisedRating);
                            //so.Properties.System.Rating.Value = (uint)rating;
                        }
                    }

                    if (Program.m_runSilent)
                    {
                    }
                    */
                  }
                  catch { }
                }
              }
              catch { }
            }
          }

          progress = 100.0 * i / itunesTracks.Count;
        }

        progress = 100.0;

        if (!cancelled)
        {
          state = SyncState.Done;
        }
      }
      catch (Exception)
      {
        if (!cancelled)
        {
          currentError = string.Format("An unexpected error occurred.{0}{0}Make sure that WMP and iTunes are installed, and that you have the required permissions to read and modify the libraries.{0}{0}Do not close iTunes while Orzeszek Ratings is running.", Environment.NewLine);
          state = SyncState.Error;
        }
      }
    }

    private void DoWorkWMP()
    {
      iTunesApp itunes = null;
      WindowsMediaPlayer wmp = null;

      try
      {
        itunes = new iTunesApp();
        wmp = new WindowsMediaPlayer();

        int wmpPlayCountAtom = wmp.mediaCollection.getMediaAtom("UserPlayCount");
        int wmpRatingAtom = wmp.mediaCollection.getMediaAtom("UserRating");

        state = SyncState.Syncing;

        IITTrackCollection itunesTracks = itunes.LibraryPlaylist.Tracks;

        for (int i = 1; i <= itunesTracks.Count; i++)
        {
          if (cancelled)
          {
            state = SyncState.Cancelled;

            if (wmp != null)
              wmp.close();

            return;
          }

          IITTrack itunesTrack = itunesTracks[i];
          if (itunesTrack.Kind == ITTrackKind.ITTrackKindFile)
          {
            IITFileOrCDTrack itunesFileTrack = (IITFileOrCDTrack)itunesTrack;
            IWMPPlaylist wmpTrackSearch = null;

            currentFile = itunesFileTrack.Location;

            MyProgressChangedEventArgs e = new MyProgressChangedEventArgs();
            e.PercentComplete = (int)progress;
            e.CurrentPathFile = currentFile;
            OnProgressChanged(e);

            try
            {
              wmpTrackSearch = wmp.mediaCollection.getByAttribute("SourceURL", itunesFileTrack.Location);
              if (wmpTrackSearch.count > 0)
              {
                IWMPMedia wmpTrack = wmpTrackSearch.get_Item(0);

                int itunesPlayCount = itunesFileTrack.PlayedCount;
                int wmpPlayCount = 0;
                int.TryParse(wmpTrack.getItemInfoByAtom(wmpPlayCountAtom), out wmpPlayCount);

                int? itunesRating = itunesFileTrack.ratingKind == ITRatingKind.ITRatingKindUser ? RatingHelper.FromItunesRating(itunesFileTrack.Rating) : null;
                int? wmpRating = RatingHelper.FromWmpRating(wmpTrack.getItemInfoByAtom(wmpRatingAtom));

                if (m_SyncInfo.SyncPlayCounts != null && (bool)m_SyncInfo.SyncPlayCounts)
                {
                  if (m_SyncInfo.OnlySyncHighestPlayCount != null && (bool)m_SyncInfo.OnlySyncHighestPlayCount)
                    try
                    {
                      if (itunesPlayCount < wmpPlayCount)
                        itunesFileTrack.PlayedCount = wmpPlayCount;
                    }
                    catch (Exception)
                    {
                    }
                  else if (m_SyncInfo.AggregateMovePlayCount != null && (bool)m_SyncInfo.AggregateMovePlayCount)
                    try
                    {
                      if (wmpPlayCount != 0)
                      {
                        itunesFileTrack.PlayedCount = itunesPlayCount + wmpPlayCount;
                        wmpTrack.setItemInfo("UserPlayCount", "0");
                      }
                    }
                    catch (Exception)
                    {
                    }
                  else
                    try
                    {
                      if (itunesPlayCount != wmpPlayCount)
                        itunesFileTrack.PlayedCount = wmpPlayCount;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (m_SyncInfo.SyncRatings)
                {
                  try
                  {
                    if (m_SyncInfo.OnlySyncMissingRatings == null || !(bool)m_SyncInfo.OnlySyncMissingRatings || itunesRating == null)
                    {
                      if (itunesFileTrack.Rating != RatingHelper.ToItunesRating(wmpRating))
                      {
                        RatingsUpdated += 1;
                        itunesFileTrack.Rating = RatingHelper.ToItunesRating(wmpRating);
                      }
                    }
                  }
                  catch (Exception)
                  {
                  }
                }
              }
            }
            catch (Exception)
            {
            }
            finally
            {
              if (wmpTrackSearch != null)
                wmpTrackSearch.clear();
            }
          }

          progress = 100.0 * i / itunesTracks.Count;
        }

        progress = 100.0;

        if (!cancelled)
        {
          state = SyncState.Done;
          //                    OnDone();
          //                    StatusBarManager.AddMessage("Done.", MessageType.Success, MessageLength.Short);
        }
      }
      catch (Exception)
      {
        if (!cancelled)
        {
          currentError = string.Format("An unexpected error occurred.{0}{0}Make sure that WMP and iTunes are installed, and that you have the required permissions to read and modify the libraries.{0}{0}Do not close iTunes while Orzeszek Ratings is running.", Environment.NewLine);
          state = SyncState.Error;
          //                    OnDone();
          //                    StatusBarManager.AddMessage("An unexpected error occurred.", MessageType.Error, MessageLength.Short);
        }
      }
      finally
      {
        if (wmp != null)
          wmp.close();
      }
    }

    protected virtual void OnProgressChanged(MyProgressChangedEventArgs e)
    {
      if (Progress != null) Progress(this, e);
    }
  }

  public class SyncInfo
  {
    public Main UI { get; set; }
    public System.Windows.Forms.ListBox ReportList { get; set; }
    public bool? SyncPlayCounts { get; set; }
    public bool SyncRatings { get; set; }

    public bool? OnlySyncMissingRatings { get; set; }
    public bool? OnlySyncHighestPlayCount { get; set; }
    public bool? AggregateMovePlayCount { get; set; }
    public bool UseWMP { get; set; }

    public SyncModes SyncHandling { get; set; }
    public ClashWinner ClashHandling { get; set; }
  }

  public class SyncInfoEventArgs : EventArgs
  {
    public SyncInfo SyncInfo { get; set; }

    public SyncInfoEventArgs(SyncInfo syncInfo)
    {
      SyncInfo = syncInfo;
    }
  }
}
