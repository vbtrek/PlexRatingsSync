using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync.Classes.PlexTvApi
{
  // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
  public class PlexTvRoot
  {
    public PlexTvUser user { get; set; }
  }
}
