using DS.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DS.PlexRatingsSync
{
  public partial class Options2 : Form
  {
    private ItunesManager m_Itunes = new ItunesManager();

    public Options2()
    {
      InitializeComponent();
    }

    private void Options2_Load(object sender, EventArgs e)
    {
      m_Itunes.GetItunesPlayLists(Settings.ItunesLibraryPath, false);

      PopulateDropdowns();

      GetPreferences();
    }

    private void PopulateDropdowns()
    {
      cboSyncSource.DataSource = EnumHelper.GetAll<SyncSources>();
    }

    private void GetPreferences()
    {
      txtPlexDatabase.Text = Settings.PlexDatabase;

      cboPlexAccount.Text = Settings.PlexAccount;

      chkSyncPlaylists.Checked = Settings.SyncPlaylists;

      txtItunesLibrary.Text = Settings.ItunesLibraryPath;

      chkRemoveEmptyPlaylists.Checked = Settings.RemoveEmptyPlaylists;

      AddPlaylists();

      chkSyncRatings.Checked = Settings.SyncRatings;
    }

    private void AddPlaylists()
    {
      var selectedPlaylists = (from playlist in m_Itunes.ItunesPlaylists
                              select new SelectedPlaylist() { Playlist = playlist.FullPlaylistName, Selected = false })
                              .ToList();

      foreach (var playlist in selectedPlaylists)
      {
        playlist.Selected = Settings.ChosenPlaylists.Contains(playlist.Playlist);
      }

      grdPlaylists.AutoGenerateColumns = false;

      grdPlaylists.DataSource = selectedPlaylists;
    }

    private void SavePreferences()
    {
      Settings.PlexDatabase = txtPlexDatabase.Text;
      Settings.PlexAccount = cboPlexAccount.Text;
      Settings.SyncPlaylists = chkSyncPlaylists.Checked;
      Settings.ItunesLibraryPath = txtItunesLibrary.Text;
      Settings.RemoveEmptyPlaylists = chkRemoveEmptyPlaylists.Checked;

      Settings.ChosenPlaylists.Clear();

      List<SelectedPlaylist> playlists = grdPlaylists.DataSource as List<SelectedPlaylist>;

      foreach (var playlist in playlists)
      {
        if (playlist.Selected)
          Settings.ChosenPlaylists.Add(playlist.Playlist);
      }

      Settings.SyncRatings = chkSyncRatings.Checked;
      
      Settings.SavePreferences();
    }

    private List<PlexTableAccounts> GetPlexAccounts()
    {
      PlexDatabaseControlller plex = new PlexDatabaseControlller(txtPlexDatabase.Text);

      if (plex.IsDbConnected)
      {
        string sql = @"SELECT id, 'Master' AS name FROM accounts WHERE id = 0
UNION ALL
SELECT id, name FROM accounts WHERE id > 0;";

        return plex.ReadPlexAndMap<PlexTableAccounts>(sql);
      }

      return new List<PlexTableAccounts>();
    }

    private void cmdPlexDatabase_Click(object sender, EventArgs e)
    {
      openFileDialog.Filter = "Plex Database Files|com.plexapp.plugins.library.db";

      openFileDialog.FileName = txtPlexDatabase.Text;

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        txtPlexDatabase.Text = openFileDialog.FileName;
      }
    }

    private void txtPlexDatabase_TextChanged(object sender, EventArgs e)
    {
      var accounts = GetPlexAccounts().Select(a => a.id + " - " + a.name).ToList();

      cboPlexAccount.DataSource = accounts;
    }

    private void chkSyncPlaylists_CheckedChanged(object sender, EventArgs e)
    {
      lblItunesLibrary.Enabled = chkSyncPlaylists.Checked;
      txtItunesLibrary.Enabled = chkSyncPlaylists.Checked;
      cmdItunesLibrary.Enabled = chkSyncPlaylists.Checked;
      grdPlaylists.Enabled = chkSyncPlaylists.Checked;
      chkRemoveEmptyPlaylists.Enabled = chkSyncPlaylists.Checked;
    }

    private void cmdItunesLibrary_Click(object sender, EventArgs e)
    {
      openFileDialog.Filter = "iTunes Library Files|*.xml";

      openFileDialog.FileName = txtItunesLibrary.Text;

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        txtItunesLibrary.Text = openFileDialog.FileName;
      }
    }

    private void txtItunesLibrary_TextChanged(object sender, EventArgs e)
    {
      m_Itunes.GetItunesPlayLists(txtItunesLibrary.Text, false);

      AddPlaylists();
    }

    private void CmdOk_Click(object sender, EventArgs e)
    {
      SavePreferences();

      Close();
    }
  }

  public class SelectedPlaylist
  {
    public string Playlist { get; set; }

    public bool Selected { get; set; }
  }
}
