using DS.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DS.PlexRatingsSync
{
  public partial class Options2 : Form
  {
    private ItunesManager _ItunesManager = new ItunesManager(Settings.ItunesLibraryPath);

    public Options2()
    {
      InitializeComponent();
    }

    private void Options2_Load(object sender, EventArgs e)
    {
      _ItunesManager.ReadItunesData(true, false);

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

      foreach (EnumHelper.EnumValue item in cboSyncSource.Items)
      {
        if (item.EnumItem.Equals(Settings.SyncSource))
        {
          cboSyncSource.SelectedItem = item;

          break;
        }
      }

      foreach (EnumHelper.EnumValue item in cboSyncMode.Items)
      {
        if (item.EnumItem.Equals(Settings.SyncHandling))
        {
          cboSyncMode.SelectedItem = item;

          break;
        }
      }

      foreach (EnumHelper.EnumValue item in cboClashWinner.Items)
      {
        if (item.EnumItem.Equals(Settings.ClashHandling))
        {
          cboClashWinner.SelectedItem = item;

          break;
        }
      }
    }

    private void AddPlaylists()
    {
      var selectedPlaylists = (from playlist in _ItunesManager.ItunesPlaylists
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

      Settings.SyncSource = SelectedSource;

      if (cboSyncMode.SelectedItem is EnumHelper.EnumValue mode)
        Settings.SyncHandling = (SyncModes)mode.EnumItem;

      if (cboSyncMode.SelectedItem is EnumHelper.EnumValue clash)
        Settings.ClashHandling = (ClashWinner)clash.EnumItem;

      Settings.SavePreferences();
    }

    private SyncSources SelectedSource
    {
      get
      {
        if (cboSyncSource.SelectedItem is EnumHelper.EnumValue source)
          return (SyncSources)source.EnumItem;

        return SyncSources.FileProperties;
      }
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

    private void EnableDisableRatingsOptions(bool enable)
    {
      lblSyncSource.Enabled = enable;

      cboSyncSource.Enabled = enable;

      lblSyncMode.Enabled = enable;

      cboSyncMode.Enabled = enable;

      lblClashWinner.Enabled = enable;

      cboClashWinner.Enabled = enable;
    }

    private void cmdPlexDatabase_Click(object sender, EventArgs e)
    {
      openFileDialog.Filter = "Plex Database Files|com.plexapp.plugins.library.db";

      if (string.IsNullOrWhiteSpace(txtPlexDatabase.Text))
      {
        var path = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Plex Media Server\Plug-in Support\Databases");

        if (Directory.Exists(path))
          openFileDialog.FileName = path;
      }
      else
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
      _ItunesManager = new ItunesManager(txtItunesLibrary.Text);

      _ItunesManager.ReadItunesData(true, false);

      AddPlaylists();
    }

    private void CmdOk_Click(object sender, EventArgs e)
    {
      SavePreferences();

      Close();
    }

    private void chkSyncRatings_CheckedChanged(object sender, EventArgs e)
    {
      EnableDisableRatingsOptions(chkSyncRatings.Checked);
    }

    private void cboSyncSource_SelectedIndexChanged(object sender, EventArgs e)
    {
      var modes = EnumHelper.GetAll<SyncModes>();

      foreach (var item in modes)
      {
        switch (SelectedSource)
        {
          case SyncSources.FileProperties:
            if (item.EnumItem.Equals(SyncModes.FileOrItunesToPlex))
              item.Description = "File Properties to Plex";

            if (item.EnumItem.Equals(SyncModes.PlexToFileOrItunes))
              item.Description = "Plex to File Properties";

            break;

          case SyncSources.ITunesLibrary:
            if (item.EnumItem.Equals(SyncModes.FileOrItunesToPlex))
              item.Description = "iTunes Library to Plex";

            if (item.EnumItem.Equals(SyncModes.PlexToFileOrItunes))
              item.Description = "Plex to iTunes Library";

            break;
        }
      }

      cboSyncMode.DataSource = modes;

      var clashWinner = EnumHelper.GetAll<ClashWinner>();

      foreach (var item in clashWinner)
      {
        switch (SelectedSource)
        {
          case SyncSources.FileProperties:
            if (item.EnumItem.Equals(ClashWinner.FileOrItunes))
              item.Description = "File Properties Always Wins";

            break;

          case SyncSources.ITunesLibrary:
            if (item.EnumItem.Equals(ClashWinner.FileOrItunes))
              item.Description = "iTunes Library Always Wins";

            break;
        }
      }

      cboClashWinner.DataSource = clashWinner;
    }
  }

  public class SelectedPlaylist
  {
    public string Playlist { get; set; }

    public bool Selected { get; set; }
  }
}
