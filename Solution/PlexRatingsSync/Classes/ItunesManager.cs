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
        private List<ItunesTrack> m_ItunesTracks = new List<ItunesTrack>();
        private List<ItunesPlaylist> m_AllItunesPlaylists = new List<ItunesPlaylist>();
        private List<ItunesPlaylist> m_ItunesPlaylists = new List<ItunesPlaylist>();

        public void GetItunesPlayLists(string itunesLibrary)
        {
            m_ItunesTracks = new List<ItunesTrack>();
            m_AllItunesPlaylists = new List<ItunesPlaylist>();
            m_ItunesPlaylists = new List<ItunesPlaylist>();

            if (string.IsNullOrWhiteSpace(itunesLibrary)) return;
            if (!File.Exists(itunesLibrary)) return;

            Cursor.Current = Cursors.WaitCursor;

            // Get Itunes Tracks
            string xpath = "/plist/dict/dict/dict";

            XPathDocument docNav = new XPathDocument(itunesLibrary);
            XPathNavigator nav = docNav.CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            XPathNodeIterator nodes = nav.Select(expr);

            XmlReaderSettings set2 = new XmlReaderSettings();
            set2.ConformanceLevel = ConformanceLevel.Fragment;

            m_ItunesTracks.Clear();

            foreach (XPathNavigator node in nodes)
            {
                XPathDocument doc2 = new XPathDocument(
                    XmlReader.Create(new StringReader(node.InnerXml), set2));

                XPathNavigator nav2 = doc2.CreateNavigator();

                ItunesTrack track = new ItunesTrack();
                track.Id = nav2.SelectSingleNode("key[.='Track ID']/following-sibling::*").ValueAsInt;
                track.Name = nav2.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
                track.Location = nav2.SelectSingleNode("key[.='Location']/following-sibling::*").ToString();
                m_ItunesTracks.Add(track);
            }

            // Get Itunes playlists
            xpath = "/plist/dict/array/dict";

            expr = nav.Compile(xpath);
            nodes = nav.Select(expr);

            m_ItunesPlaylists.Clear();

            foreach (XPathNavigator node in nodes)
            {
                if (!node.InnerXml.Contains("<key>Master</key>")
                    && !node.InnerXml.Contains("<key>Distinguished Kind</key>"))
                {
                    XPathDocument doc2 = new XPathDocument(
                        XmlReader.Create(new StringReader(node.InnerXml), set2));

                    XPathNavigator nav2 = doc2.CreateNavigator();

                    // TODO Need to build a path reading "Playlist Persistent ID" and "Parent Persistent ID"
                    ItunesPlaylist playlist = new ItunesPlaylist();
                    playlist.PlaylistPersistentID = nav2.SelectSingleNode("key[.='Playlist Persistent ID']/following-sibling::*")?.ToString();
                    playlist.ParentPersistentID = nav2.SelectSingleNode("key[.='Parent Persistent ID']/following-sibling::*")?.ToString();
                    playlist.Name = nav2.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
                    playlist.FullPlaylistName = FullPlaylistName(playlist);

                    m_AllItunesPlaylists.Add(playlist);

                    if (!node.InnerXml.Contains("<key>Folder</key>"))
                    {
                        var trackListXml = nav2.Select("key[.='Playlist Items']/following-sibling::*");

                        trackListXml.MoveNext();
                        string tracksXml = trackListXml.Current.InnerXml;

                        doc2 = new XPathDocument(XmlReader.Create(new StringReader(tracksXml), set2));
                        nav2 = doc2.CreateNavigator();

                        var trackIdList = nav2.Select("dict/key[.='Track ID']/following-sibling::*");

                        while (trackIdList.MoveNext())
                        {
                            int trackId = trackIdList.Current.ValueAsInt;
                            ItunesTrack track = m_ItunesTracks.FirstOrDefault(t => t.Id == trackId);

                            if (track != null) playlist.Tracks.Add(track);
                        }

                        m_ItunesPlaylists.Add(playlist);
                    }
                }
            }

            Cursor.Current = Cursors.Default;
        }

        private string FullPlaylistName(ItunesPlaylist playlist)
        {
            string name = playlist.Name;

            if (!string.IsNullOrWhiteSpace(playlist.ParentPersistentID))
            {
                ItunesPlaylist parent = m_AllItunesPlaylists.FirstOrDefault(p => p.PlaylistPersistentID == playlist.ParentPersistentID);

                if (parent != null) name = string.Format("{0} - {1}", FullPlaylistName(parent), name);
            }

            return name;
        }

        public List<ItunesTrack> ItunesTracks
        {
            get { return m_ItunesTracks; }
        }

        public List<ItunesPlaylist> AllItunesPlaylists
        {
            get { return m_AllItunesPlaylists; }
        }

        public List<ItunesPlaylist> ItunesPlaylists
        {
            get { return m_ItunesPlaylists; }
        }
    }
}
