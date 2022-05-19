using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync
{
  public enum SyncSources
  {
    [Description("File Properties")]
    FileProperties = 0,

    [Description("iTunes Library")]
    ITunesLibrary = 1
  }

  public enum SyncModes
  {
    [Description("File or iTunes to Plex")]
    FileOrItunesToPlex = 1,

    [Description("Plex to File or iTunes")]
    PlexToFileOrItunes = 2,

    [Description("2-Way")]
    TwoWay = 3
  }

  public enum ClashWinner
  {
    [Description("Skip")]
    Skip = 0,

    [Description("Plex Always Wins")]
    Plex = 1,

    [Description("File or iTunes Always Wins")]
    FileOrItunes = 2,

    [Description("Prompt If Both Have Different Ratings")]
    Prompt = 3,

    [Description("Always Prompt")]
    AlwaysPrompt = 4
  }

  public enum RatingsClashResult
  {
    Cancel = 0,
    UpdatePlex = 1,
    UpdateFileOrItunes = 2
  }

  public enum RatingConvert
  {
    Plex,
    File,
    Itunes
  }
}
