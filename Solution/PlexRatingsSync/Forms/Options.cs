using DS.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DS.PlexRatingsSync
{
    public partial class Options : Form
    {
        private List<ItunesPlaylist> m_ItunesPlaylists = new List<ItunesPlaylist>();

        public Options(List<ItunesPlaylist> itunesPlaylists)
        {
            InitializeComponent();

            m_ItunesPlaylists = itunesPlaylists;
        }

        private void Options_Load(object sender, EventArgs e)
        {
            GetPreferences();
        }

        private void grdConfig_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SetPropertyVisibility();
            SavePreferences();
        }

        private void GetPreferences()
        {
            grdConfig.SuspendLayout();

            grdConfig.Item.Clear();

            int id = grdConfig.Item.Add("Plex Database", Settings.PlexDatabase,
                false, "General", "Plex database file location", true);
            grdConfig.Item[id].UseFileNameEditor = true;
            grdConfig.Item[id].FileNameFilter = "Plex Database Files|com.plexapp.plugins.library.db";

            grdConfig.Item.Add("Plex Account", Settings.PlexAccount,
                false, "General", "Plex account to sync with", true);

            grdConfig.Item.Add("Sync Playlists", Settings.SyncPlaylists,
                false, "Playlists", "Sync iTunes playlist to Plex. Once enabled you will need to close and re-open the settings to choose the list of playlists you want to sync.", true);

            id = grdConfig.Item.Add("iTunes Library", Settings.ItunesLibraryPath,
                false, "Playlists", " iTunes XML Location ", true);
            grdConfig.Item[id].UseFileNameEditor = true;
            grdConfig.Item[id].FileNameFilter = "iTunes Library Files|*.xml";

            grdConfig.Item.Add("Remove Empty Playlists", Settings.RemoveEmptyPlaylists,
                false, "Playlists", "Remove empty playlists from plex", true);

            foreach (var playlist in m_ItunesPlaylists)
            {
                bool export = false;
                if (Settings.ChosenPlaylists.Contains(playlist.FullPlaylistName)) export = true;

                grdConfig.Item.Add(playlist.FullPlaylistName,
                    export,
                    false, "Playlists", "Sync this iTunes playlist to Plex", true);
            }

            grdConfig.Item.Add("Sync Ratings", Settings.SyncRatings,
                false, "Ratings", "Sync Ratings from files to Plex", true);

            grdConfig.Refresh();

            SetPropertyVisibility();

            GetOption("Sync Playlists").Select();

            grdConfig.PerformLayout();
        }

        private void SavePreferences()
        {
            Settings.PlexDatabase = GetOptionValue("Plex Database").ToString();
            Settings.PlexAccount = int.Parse(GetOptionValue("Plex Account").ToString());

            Settings.SyncPlaylists = bool.Parse(GetOptionValue("Sync Playlists").ToString());
            Settings.ItunesLibraryPath = GetOptionValue("iTunes Library").ToString();
            Settings.RemoveEmptyPlaylists = bool.Parse(GetOptionValue("Remove Empty Playlists").ToString());

            Settings.ChosenPlaylists.Clear();

            foreach (var playlist in m_ItunesPlaylists)
            {
                if (GetOptionValue(playlist.FullPlaylistName) != null && (bool)GetOptionValue(playlist.FullPlaylistName) == true)
                {
                    Settings.ChosenPlaylists.Add(playlist.FullPlaylistName);
                }
            }

            Settings.SyncRatings = bool.Parse(GetOptionValue("Sync Ratings").ToString());

            Settings.SavePreferences();
        }

        private void SetPropertyVisibility()
        {
            GridItem exportItem = GetOption("Sync Playlists");

            List<string> exportRelatedItems = m_ItunesPlaylists.ConvertAll(i => i.Name);
            exportRelatedItems.Add("Remove Empty Playlists");
            exportRelatedItems.Add("iTunes Library");

            foreach (CustomProperty item in grdConfig.Item)
            {
                if (exportRelatedItems.Contains(item.Name))
                    item.IsReadOnly = !bool.Parse(exportItem.Value.ToString());
            }

            grdConfig.Refresh();
        }

        private GridItem GetOption(string optionName)
        {
            GridItem gi = grdConfig.EnumerateAllItems().FirstOrDefault(i =>
                              i.PropertyDescriptor != null &&
                              i.PropertyDescriptor.Name == optionName);

            if (gi == null) throw new ArgumentException(optionName + " doesn't exist");

            return gi;
        }

        private object GetOptionValue(string optionName)
        {
            GridItem gi = grdConfig.EnumerateAllItems().FirstOrDefault(i =>
                              i.PropertyDescriptor != null &&
                              i.PropertyDescriptor.Name == optionName);

            if (gi == null) return null;

            return gi.Value;
        }
    }
}
