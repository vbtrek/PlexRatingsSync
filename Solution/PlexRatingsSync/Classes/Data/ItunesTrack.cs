using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class ItunesTrack
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public int Rating { get; set; }

    public string ProperLocation
    {
      get
      {
        try
        {
          var uri = new System.Uri(Location, UriKind.Absolute);

          string decoded = Uri.UnescapeDataString(uri.AbsolutePath);

          if (decoded.StartsWith("/")) decoded = decoded.Substring(1);

          return Path.GetFullPath(decoded);
        }
        catch
        {
          return null;
        }
      }
    }
  }
}
