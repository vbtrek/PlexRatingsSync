﻿using DS.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DS.PlexRatingsSync
{
  public partial class Options3 : Form
  {
    private ItunesManager _ItunesManager = new ItunesManager(Settings.ItunesLibraryPath);

    public Options3()
    {
      InitializeComponent();
    }

    private void Options3_Load(object sender, EventArgs e)
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

      txtItunesLibrary.Text = Settings.ItunesLibraryPath;

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
    }

    private void SavePreferences()
    {
      Settings.PlexDatabase = txtPlexDatabase.Text;
      Settings.PlexAccount = cboPlexAccount.Text;
      Settings.SyncPlaylists = false;
      Settings.ItunesLibraryPath = txtItunesLibrary.Text;
      Settings.RemoveEmptyPlaylists = false;

      Settings.ChosenPlaylists.Clear();

      List<SelectedPlaylist> playlists = new List<SelectedPlaylist>();

      foreach (var playlist in playlists)
      {
        if (playlist.Selected)
          Settings.ChosenPlaylists.Add(playlist.Playlist);
      }

      Settings.SyncRatings = chkSyncRatings.Checked;

      Settings.SyncSource = SelectedSource;

      if (cboSyncMode.SelectedItem is EnumHelper.EnumValue mode)
        Settings.SyncHandling = (SyncModes)mode.EnumItem;

      if (cboClashWinner.SelectedItem is EnumHelper.EnumValue clash)
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

    private SyncModes SelectedSyncMode
    {
      get
      {
        if (cboSyncMode.SelectedItem is EnumHelper.EnumValue mode)
          return (SyncModes)mode.EnumItem;

        return SyncModes.FileOrItunesToPlex;
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
          openFileDialog.InitialDirectory = path;
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

      for (int i = modes.Count - 1; i >= 0; i--)
      {
        var mode = modes[i];

        switch (SelectedSource)
        {
          case SyncSources.FileProperties:
            if (mode.EnumItem.Equals(SyncModes.FileOrItunesToPlex))
              mode.Description = "File Properties to Plex";

            if (mode.EnumItem.Equals(SyncModes.PlexToFileOrItunes))
              mode.Description = "Plex to File Properties";

            break;

          case SyncSources.ITunesLibrary:
            if (mode.EnumItem.Equals(SyncModes.FileOrItunesToPlex))
              mode.Description = "iTunes Library to Plex";

            if (mode.EnumItem.Equals(SyncModes.PlexToFileOrItunes))
              modes.Remove(mode);

            if (mode.EnumItem.Equals(SyncModes.TwoWay))
              modes.Remove(mode);

            break;
        }
      }

      cboSyncMode.DataSource = modes;
    }

    private void cboSyncMode_SelectedIndexChanged(object sender, EventArgs e)
    {
      var clashWinners = EnumHelper.GetAll<ClashWinner>();

      for (int i = clashWinners.Count - 1; i >= 0; i--)
      {
        var clashWinner = clashWinners[i];

        switch (SelectedSource)
        {
          case SyncSources.FileProperties:
            if (clashWinner.EnumItem.Equals(ClashWinner.FileOrItunes))
              clashWinner.Description = "File Properties Always Wins";

            break;

          case SyncSources.ITunesLibrary:
            if (clashWinner.EnumItem.Equals(ClashWinner.FileOrItunes))
              clashWinner.Description = "iTunes Library Always Wins";

            break;
        }

        switch (SelectedSyncMode)
        {
          case SyncModes.FileOrItunesToPlex:
            if (clashWinner.EnumItem.Equals(ClashWinner.Plex))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Skip))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Prompt))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.AlwaysPrompt))
              clashWinners.Remove(clashWinner);

            break;

          case SyncModes.PlexToFileOrItunes:
            if (clashWinner.EnumItem.Equals(ClashWinner.FileOrItunes))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Skip))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Prompt))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.AlwaysPrompt))
              clashWinners.Remove(clashWinner);

            break;

          case SyncModes.TwoWay:
            break;
        }
      }

      cboClashWinner.DataSource = clashWinners;
    }

    private class SelectedPlaylist
    {
      public string Playlist { get; set; }

      public bool Selected { get; set; }
    }
  }
}
