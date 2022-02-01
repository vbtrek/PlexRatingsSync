using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class ItunesPlaylist
  {
    public string PlaylistPersistentID { get; set; }
    public string ParentPersistentID { get; set; }
    public string Name { get; set; }
    public string FullPlaylistName { get; set; }
    public List<ItunesTrack> Tracks { get; set; }

    public ItunesPlaylist()
    {
      Tracks = new List<ItunesTrack>();
    }
  }
}
