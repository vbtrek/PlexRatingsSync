using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync
{
  // TODO_DS1 Update UI with new clash handling options
  public enum SyncSources
  {
    [Description("File Properties")]
    FileProperties = 0,
    ITunesLibrary = 1
  }

  // TODO_DS1 Update UI with new clash handling options
  public enum SyncModes
  {
    FileOrItunesToPlex = 1,
    PlexToFileOrItunes = 2,
    TwoWay = 3
  }

  // TODO_DS1 Update UI with new clash handling options
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
    UseFileOrItunes = 2
  }

  public enum RatingConvert
  {
    Plex,
    File,
    Itunes
  }
}
