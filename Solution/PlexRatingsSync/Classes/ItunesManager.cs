using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace DS.PlexRatingsSync
{
  public class ItunesManager
  {
    private string _ItunesLibrary;

    public List<ItunesTrack> ItunesTracks { get; set; }

    public List<ItunesPlaylist> AllItunesPlaylists { get; set; }

    public List<ItunesPlaylist> ItunesPlaylists { get; set; }

    public ItunesManager(string itunesLibrary)
    {
      _ItunesLibrary = itunesLibrary;
    }

    public void ReadItunesData(bool readPlaylists, bool readTracks)
    {
      ItunesTracks = new List<ItunesTrack>();

      AllItunesPlaylists = new List<ItunesPlaylist>();

      ItunesPlaylists = new List<ItunesPlaylist>();

      if (string.IsNullOrWhiteSpace(_ItunesLibrary)) return;

      if (!File.Exists(_ItunesLibrary)) return;

      Cursor.Current = Cursors.WaitCursor;

      XmlReaderSettings documentSettings = new XmlReaderSettings();

      documentSettings.XmlResolver = null;

      documentSettings.DtdProcessing = DtdProcessing.Ignore;

      var xmlReader = XmlTextReader.Create(_ItunesLibrary, documentSettings);

      XPathDocument itunesDocument = new XPathDocument(xmlReader);

      // Get Itunes Tracks
      if (readTracks) ExtractTracks(itunesDocument);

      // Get Itunes playlists
      if (readPlaylists) ExtractPlaylists(itunesDocument, readTracks);

      Cursor.Current = Cursors.Default;
    }

    private void ExtractPlaylists(XPathDocument itunesDocument, bool readTracks)
    {
      string playlistXpath = "/plist/dict/array/dict";

      XPathNavigator playlistNav = itunesDocument.CreateNavigator();

      XPathExpression playlistExpr = playlistNav.Compile(playlistXpath);

      XPathNodeIterator playlistNodes = playlistNav.Select(playlistExpr);

      XmlReaderSettings playlistSettings = new XmlReaderSettings();

      playlistSettings.ConformanceLevel = ConformanceLevel.Fragment;

      ItunesPlaylists.Clear();

      foreach (XPathNavigator node in playlistNodes)
      {
        if (node.InnerXml.Contains("<key>Master</key>") || node.InnerXml.Contains("<key>Distinguished Kind</key>"))
          continue;

        XPathDocument document = new XPathDocument(
            XmlReader.Create(new StringReader(node.InnerXml), playlistSettings));

        XPathNavigator navigator = document.CreateNavigator();

        // TODO Need to build a path reading "Playlist Persistent ID" and "Parent Persistent ID"
        ItunesPlaylist playlist = new ItunesPlaylist();

        playlist.PlaylistPersistentID = navigator.SelectSingleNode("key[.='Playlist Persistent ID']/following-sibling::*")?.ToString();
        
        playlist.ParentPersistentID = navigator.SelectSingleNode("key[.='Parent Persistent ID']/following-sibling::*")?.ToString();
        
        playlist.Name = navigator.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
        
        playlist.FullPlaylistName = FullPlaylistName(playlist);

        AllItunesPlaylists.Add(playlist);

        if (!node.InnerXml.Contains("<key>Folder</key>"))
        {
          if (readTracks)
          {
            var trackListXml = navigator.Select("key[.='Playlist Items']/following-sibling::*");

            trackListXml.MoveNext();
            
            string tracksXml = trackListXml.Current.InnerXml;

            document = new XPathDocument(XmlReader.Create(new StringReader(tracksXml), playlistSettings));
            
            navigator = document.CreateNavigator();

            var trackIdList = navigator.Select("dict/key[.='Track ID']/following-sibling::*");

            while (trackIdList.MoveNext())
            {
              int trackId = trackIdList.Current.ValueAsInt;

              ItunesTrack track = ItunesTracks.FirstOrDefault(t => t.Id == trackId);

              if (track != null) playlist.Tracks.Add(track);
            }
          }

          ItunesPlaylists.Add(playlist);
        }
      }
    }

    private void ExtractTracks(XPathDocument itunesDocument)
    {
      string trackXpath = "/plist/dict/dict/dict";

      XPathNavigator trackNav = itunesDocument.CreateNavigator();

      XPathExpression trackExpr = trackNav.Compile(trackXpath);
      
      XPathNodeIterator trackNodes = trackNav.Select(trackExpr);

      XmlReaderSettings trackSettings = new XmlReaderSettings();
      
      trackSettings.ConformanceLevel = ConformanceLevel.Fragment;

      ItunesTracks.Clear();

      foreach (XPathNavigator node in trackNodes)
      {
        XPathDocument document = new XPathDocument(
            XmlReader.Create(new StringReader(node.InnerXml), trackSettings));

        XPathNavigator navigator = document.CreateNavigator();

        ItunesTrack track = new ItunesTrack();

        track.Id = navigator.SelectSingleNode("key[.='Track ID']/following-sibling::*").ValueAsInt;
        
        track.Name = navigator.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
        
        track.Location = navigator.SelectSingleNode("key[.='Location']/following-sibling::*").ToString();
        
        ItunesTracks.Add(track);
      }
    }

    private string FullPlaylistName(ItunesPlaylist playlist)
    {
      string name = playlist.Name;

      if (!string.IsNullOrWhiteSpace(playlist.ParentPersistentID))
      {
        ItunesPlaylist parent = AllItunesPlaylists.FirstOrDefault(p => p.PlaylistPersistentID == playlist.ParentPersistentID);

        if (parent != null) name = string.Format("{0} - {1}", FullPlaylistName(parent), name);
      }

      return name;
    }
  }
}
