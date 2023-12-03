using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.PlexRatingsSync.Classes.PlexTvApi
{
  public class PlexTvUser
  {
    public int id { get; set; }

    public string uuid { get; set; }
    
    public string email { get; set; }
    
    public DateTime joined_at { get; set; }
    
    public string username { get; set; }
    
    public string title { get; set; }
    
    public string thumb { get; set; }
    
    public bool hasPassword { get; set; }
    
    public string authToken { get; set; }
    
    public string authentication_token { get; set; }
    
    public PlexTvSubscription subscription { get; set; }
    
    public PlexTvRoles roles { get; set; }
    
    public List<object> entitlements { get; set; }
    
    public DateTime confirmedAt { get; set; }
    
    public object forumId { get; set; }
    
    public bool rememberMe { get; set; }
  }
}
