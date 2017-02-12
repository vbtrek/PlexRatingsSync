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
            m_Itunes.GetItunesPlayLists(Settings.ItunesLibraryPath);
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
                false, "Playlists", "Sync iTunes playlist to Plex. Once enabled you will need to close and re-open the settings to choose the list of playlists you want to sync.", true);

            id = grdConfig.Item.Add("iTunes Library", Settings.ItunesLibraryPath,
                false, "Playlists", " iTunes XML Location ", true);
            grdConfig.Item[id].UseFileNameEditor = true;
            grdConfig.Item[id].FileNameFilter = "iTunes Library Files|*.xml";

            grdConfig.Item.Add("Remove Empty Playlists", Settings.RemoveEmptyPlaylists,
                false, "Playlists", "Remove empty playlists from plex", true);

            AddPlaylists();

            grdConfig.Item.Add("Sync Ratings", Settings.SyncRatings,
                false, "Ratings", "Sync Ratings from files to Plex", true);

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
            Settings.PlexDatabase = GetOptionValue("Plex Database").ToString();
            Settings.PlexAccount = int.Parse(GetOptionValue("Plex Account").ToString());

            Settings.SyncPlaylists = bool.Parse(GetOptionValue("Sync Playlists").ToString());
            Settings.ItunesLibraryPath = GetOptionValue("iTunes Library").ToString();
            Settings.RemoveEmptyPlaylists = bool.Parse(GetOptionValue("Remove Empty Playlists").ToString());

            Settings.ChosenPlaylists.Clear();

            foreach (var playlist in m_Itunes.ItunesPlaylists)
            {
                if (GetOptionValue(playlist.FullPlaylistName) != null && (bool)GetOptionValue(playlist.FullPlaylistName) == true)
                {
                    Settings.ChosenPlaylists.Add(playlist.FullPlaylistName);
                }
            }

            Settings.SyncRatings = bool.Parse(GetOptionValue("Sync Ratings").ToString());

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
                        m_Itunes.GetItunesPlayLists(changedItem.Value.ToString());
                        AddPlaylists();
                        break;
                    case "Plex Database":
                        // Read accounts
                        GetProperty("Plex Account").Choices = new CustomChoices(GetPlexAccounts().Select(a => a.id + " - " + a.name).ToArray(), true);
                        break;
                }
            }

            GridItem exportItem = GetOption("Sync Playlists");

            List<string> exportRelatedItems = m_Itunes.ItunesPlaylists.ConvertAll(i => i.Name);
            exportRelatedItems.Add("Remove Empty Playlists");
            exportRelatedItems.Add("iTunes Library");

            foreach (CustomProperty item in grdConfig.Item)
            {
                if (exportRelatedItems.Contains(item.Name))
                    item.IsReadOnly = !bool.Parse(exportItem.Value.ToString());
            }

            grdConfig.Refresh();
        }

        private List<PlexTableAccounts> GetPlexAccounts()
        {
            PlexDatabaseControlller plex = new PlexDatabaseControlller();

            if (plex.IsDbConnected)
            {
                string sql = @"SELECT * FROM accounts;";
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
