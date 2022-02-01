using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexTableMetadataItemSettings
  {
    public Int64 id { get; set; }
    public Int64 account_id { get; set; }
    public string guid { get; set; }
    public double rating { get; set; }
    public Int64 view_offset { get; set; }
    public Int64 view_count { get; set; }
    public DateTime last_viewed_at { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
  }
}
