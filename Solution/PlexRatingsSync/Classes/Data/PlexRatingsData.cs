using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS.PlexRatingsSync
{
  public class PlexRatingsData
  {
    public string guid { get; set; }
    public string file { get; set; }
    public double? rating { get; set; }
  }
}
