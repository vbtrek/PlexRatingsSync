using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync.Classes.PlexTvApi
{
  public class PlexTvSubscription
  {
    public bool active { get; set; }

    public string status { get; set; }

    public object plan { get; set; }

    public List<string> features { get; set; }
  }
}
