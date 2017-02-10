using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
    public class PlexTableDirectories
    {
        public Int64 id { get; set; }
        public Int64 library_section_id { get; set; }
        public Int64 parent_directory_id { get; set; }
        public string path { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
    }
}
