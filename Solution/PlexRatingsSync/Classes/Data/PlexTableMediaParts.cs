using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexTableMediaParts
  {
    public Int64 id { get; set; }
    public Int64 media_item_id { get; set; }
    public Int64 directory_id { get; set; }
    public string hash { get; set; }
    public string open_subtitle_hash { get; set; }
    public string file { get; set; }
    public Int64 index { get; set; }
    public Int64 size { get; set; }
    public Int64 duration { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime deleted_at { get; set; }
    public string extra_data { get; set; }
  }
}
