using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexTableMediaItems
  {
    public Int64 id { get; set; }
    public Int64 library_section_id { get; set; }
    public Int64 section_location_id { get; set; }
    public Int64 metadata_item_id { get; set; }
    public Int64 type_id { get; set; }
    public Int64 width { get; set; }
    public Int64 height { get; set; }
    public Int64 size { get; set; }
    public Int64 duration { get; set; }
    public Int64 bitrate { get; set; }
    public string container { get; set; }
    public string video_codec { get; set; }
    public string audio_codec { get; set; }
    public double display_aspect_ratio { get; set; }
    public double frames_per_second { get; set; }
    public Int64 audio_channels { get; set; }
    public bool interlaced { get; set; }
    public string source { get; set; }
    public string hints { get; set; }
    public Int64 display_offset { get; set; }
    public string settings { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public bool optimized_for_streaming { get; set; }
    public DateTime deleted_at { get; set; }
    public Int64 media_analysis_version { get; set; }
    public double sample_aspect_ratio { get; set; }
    public string extra_data { get; set; }
  }
}
