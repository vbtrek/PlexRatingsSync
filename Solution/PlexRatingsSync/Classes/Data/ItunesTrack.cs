using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
    public class ItunesTrack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public string ProperLocation
        {
            get
            {
                string decoded = Uri.UnescapeDataString(Location);
                decoded = decoded.Substring(0, 16) == "file://localhost" ? "\\" + decoded.Substring(17) : decoded;
                decoded = decoded.Replace('/', '\\');
                return decoded;
            }
        }
    }
}
