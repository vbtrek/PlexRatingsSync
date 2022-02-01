using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexTableLibrarySections
  {
    public Int64 id { get; set; }
    public Int64 library_id { get; set; }
    public string name { get; set; }
    public string name_sort { get; set; }
    public Int64 section_type { get; set; }
    public string language { get; set; }
    public string agent { get; set; }
    public string scanner { get; set; }
    public string user_thumb_url { get; set; }
    public string user_art_url { get; set; }
    public string user_theme_music_url { get; set; }
    public bool IsPublic { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime scanned_at { get; set; }
    public bool display_secondary_level { get; set; }
    public string user_fields { get; set; }
    public string query_xml { get; set; }
    public Int64 query_type { get; set; }
  }
}
