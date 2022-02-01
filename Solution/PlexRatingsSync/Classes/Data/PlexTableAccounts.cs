using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexTableAccounts
  {
    public Int64 id { get; set; }
    public string name { get; set; }
    public string hashed_password { get; set; }
    public string salt { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string default_audio_language { get; set; }
    public string default_subtitle_language { get; set; }
    public bool auto_select_subtitle { get; set; }
    public bool auto_select_audio { get; set; }
  }
}
