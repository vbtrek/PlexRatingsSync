using System;
using System.Windows.Forms;
using DS.PlexRatingsSync.Classes;
using DS.PlexRatingsSync.Classes.PlexApi;
using DS.PlexRatingsSync.Classes.PlexTvApi;
using Newtonsoft.Json;

namespace DS.PlexRatingsSync.Managers
{
  public static class PlexApiManager
  {
    public static PlexTvRoot GetUser()
    {
      string username = Settings.PlexUsername;
      string password = Settings.PlexPassword;

      return GetUser(username, password);
    }

    public static PlexTvRoot GetUser(string username, string password)
    {
      var loginResult = RestClient.Create(new Uri("https://plex.tv/users/sign_in.json"), RestClient.httpMethod.Post, string.Empty)
        .AddHeader("X-Plex-Client-Identifier", "Plexapi")
        .AddHeader("X-Plex-Product", "PlexRatingsSync")
        .AddHeader("X-Plex-Version", Application.ProductVersion)
        .AddHeader("X-Plex-Device", Environment.MachineName)
        .AddHeader("X-Plex-Platform", "Desktop")
        .AcceptHeader(RestClient.httpContentType.Json)
        .AuthorizationBasic(username, password)
        .SendRequestWithExceptionResponse();

      return JsonConvert.DeserializeObject<PlexTvRoot>(loginResult);
    }

    public static MediaContainer GetAllTracks(PlexTvRoot plexUser)
    {
      // Get all tracks
      var media = RestClient.Create(
        new Uri($"{Settings.PlexUri}library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      return MediaContainer.Parse(media);
    }

    public static MediaContainer GetTrack(PlexTvRoot plexUser, string key)
    {
      // Read the plex file using the API
      var media = RestClient.Create(
        new Uri($"{Settings.PlexUri}library/all"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .AddParameter("id", key)
        .SendRequestWithExceptionResponse();

      return MediaContainer.Parse(media);
    }

    /// <seealso cref="https://www.plexopedia.com/plex-media-server/api/server/identity/"/>
    /// <param name="plexUser"></param>
    /// <returns></returns>
    public static MediaContainer GetServerIdentity(PlexTvRoot plexUser)
    {
      var server = RestClient.Create(
        new Uri($"{Settings.PlexUri}identity"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("type", "10") // 10=tracks
        .SendRequestWithExceptionResponse();

      return MediaContainer.Parse(server);
    }

    public static void SetRating(PlexTvRoot plexUser, string key, int? rating)
    {
      if (rating == null) rating = -1;

      // Update a rating entry using the rest API
      var result = RestClient.Create(new Uri($"{Settings.PlexUri}:/rate"), RestClient.httpMethod.Put, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .AddParameter("key", key)
        .AddParameter("identifier", "com.plexapp.plugins.library")
        .AddParameter("rating", rating.ToString())
        .SendRequestWithExceptionResponse();
    }

    public static MediaContainer GetLibrarySections(PlexTvRoot plexUser)
    {
      var sections = RestClient.Create(
        new Uri($"{Settings.PlexUri}library/sections"), RestClient.httpMethod.Get, string.Empty)
        .AddHeader("X-Plex-Token", plexUser.user.authToken)
        .SendRequestWithExceptionResponse();

      return MediaContainer.Parse(sections);
    }
  }
}
