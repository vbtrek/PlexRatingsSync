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
        private ItunesManager m_Itunes = new ItunesManager();

        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            m_Itunes.GetItunesPlayLists(Settings.ItunesLibraryPath, false);
            GetPreferences();
        }

        private void grdConfig_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SetPropertyVisibility(e.ChangedItem);
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

            id = grdConfig.Item.Add("Plex Account", Settings.PlexAccount,
                false, "General", "Plex account to sync with", true);
            grdConfig.Item[id].Choices = new CustomChoices(GetPlexAccounts().Select(a => a.id + " - " + a.name).ToArray(), true);
            //grdConfig.Item[id].IsDropdownResizable = true;

            grdConfig.Item.Add("Sync Playlists", Settings.SyncPlaylists,
                false, "Playlist Sync", "Sync iTunes playlist to Plex. Once enabled you will need to close and re-open the settings to choose the list of playlists you want to sync.", true);

            id = grdConfig.Item.Add("iTunes Library", Settings.ItunesLibraryPath,
                false, "Playlist Sync", " iTunes XML Location ", true);
            grdConfig.Item[id].UseFileNameEditor = true;
            grdConfig.Item[id].FileNameFilter = "iTunes Library Files|*.xml";

            grdConfig.Item.Add("Remove Empty Playlists", Settings.RemoveEmptyPlaylists,
                false, "Playlist Sync", "Remove empty playlists from plex", true);

            AddPlaylists();

            grdConfig.Item.Add("Sync Ratings", Settings.SyncRatings,
                false, "Rating Sync", "Sync Ratings from files to Plex", true);

            grdConfig.Refresh();

            SetPropertyVisibility(null);

            GetOption("Sync Playlists").Select();
            
            grdConfig.PerformLayout();
        }

        private void AddPlaylists()
        {
            for (int i = grdConfig.Item.Count - 1; i > 0; i--)
            {
                CustomProperty item = grdConfig.Item[i];

                if (item.Category.Equals("Playlists", StringComparison.OrdinalIgnoreCase) && item.Description.Equals("Sync this iTunes playlist to Plex", StringComparison.OrdinalIgnoreCase))
                    grdConfig.Item.Remove(item.Name);
            }

            foreach (var playlist in m_Itunes.ItunesPlaylists)
            {
                bool export = false;
                if (Settings.ChosenPlaylists.Contains(playlist.FullPlaylistName)) export = true;

                grdConfig.Item.Add(playlist.FullPlaylistName,
                    export,
                    false, "Playlists", "Sync this iTunes playlist to Plex", true);
            }
        }

        private void SavePreferences()
        {
            bool result;

            Settings.PlexDatabase = GetPropertyValue("Plex Database");
            Settings.PlexAccount = GetPropertyValue("Plex Account");

            Settings.SyncPlaylists = bool.TryParse(GetPropertyValue("Sync Playlists"), out result) ? result : false;
            Settings.ItunesLibraryPath = GetPropertyValue("iTunes Library");
            Settings.RemoveEmptyPlaylists = bool.TryParse(GetPropertyValue("Remove Empty Playlists"), out result) ? result : false;

            Settings.ChosenPlaylists.Clear();

            foreach (var playlist in m_Itunes.ItunesPlaylists)
            {
                bool playlistSetting = bool.TryParse(GetPropertyValue(playlist.FullPlaylistName), out result) ? result : false;

                if (playlistSetting)
                    Settings.ChosenPlaylists.Add(playlist.FullPlaylistName);
            }

            Settings.SyncRatings = bool.TryParse(GetPropertyValue("Sync Ratings"), out result) ? result : false;

            Settings.SavePreferences();
        }

        private void SetPropertyVisibility(GridItem changedItem)
        {
            if (changedItem != null)
            {
                switch (changedItem.Label)
                {
                    case "iTunes Library":
                        // Read playlists
                        m_Itunes.GetItunesPlayLists(changedItem.Value.ToString(), false);
                        AddPlaylists();
                        break;
                    case "Plex Database":
                        // Read accounts
                        GetProperty("Plex Account").Choices = new CustomChoices(GetPlexAccounts().Select(a => a.id + " - " + a.name).ToArray(), true);
                        break;
                }
            }

            GridItem exportItem = GetOption("Sync Playlists");

            List<string> exportRelatedItems = m_Itunes.ItunesPlaylists.ConvertAll(i => i.FullPlaylistName);
            exportRelatedItems.Add("Remove Empty Playlists");
            exportRelatedItems.Add("iTunes Library");

            foreach (CustomProperty item in grdConfig.Item)
            {
                if (exportRelatedItems.Contains(item.Name))
                    item.Visible = bool.Parse(exportItem.Value.ToString());
            }

            grdConfig.Refresh();
        }

        private List<PlexTableAccounts> GetPlexAccounts()
        {
            PlexDatabaseControlller plex = new PlexDatabaseControlller(GetProperty("Plex Database").Value.ToString());

            if (plex.IsDbConnected)
            {
                string sql = @"SELECT id, 'Master' AS name FROM accounts WHERE id = 0
UNION ALL
SELECT id, name FROM accounts WHERE id > 0;";
                return plex.ReadPlexAndMap<PlexTableAccounts>(sql);
            }

            return new List<PlexTableAccounts>();
        }

        private CustomProperty GetProperty(string optionName)
        {
            foreach (CustomProperty prop in grdConfig.Item)
            {
                if (prop.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase))
                    return prop;
            }

            return null;
        }

        private string GetPropertyValue(string optionName)
        {
            CustomProperty prop = GetProperty(optionName);
            if (prop == null) return string.Empty;

            if (prop.Visible)
                return prop.Value.ToString();
            else
                return string.Empty;
        }

        private GridItem GetOption(string optionName)
        {
            GridItem gi = grdConfig.EnumerateAllItems().FirstOrDefault(i =>
                              i.PropertyDescriptor != null &&
                              i.PropertyDescriptor.Name == optionName);

            if (gi == null) throw new ArgumentException(optionName + " doesn't exist");

            return gi;
        }
    }
}
