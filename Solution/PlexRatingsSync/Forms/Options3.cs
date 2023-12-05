using DS.Controls;
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
    public Options3()
    {
      InitializeComponent();
    }

    private void Options3_Load(object sender, EventArgs e)
    {
      PopulateDropdowns();

      GetPreferences();
    }

    private void PopulateDropdowns()
    {
      cboSyncSource.DataSource = EnumHelper.GetAll<SyncSources>();
    }

    private void GetPreferences()
    {
      txtPlexUsername.Text = Settings.PlexUsername;

      txtPlexPassword.Text = Settings.PlexPassword;

      txtPlexUri.Text = Settings.PlexUri;

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

    private void SavePreferences()
    {
      Settings.PlexUsername = txtPlexUsername.Text;

      Settings.PlexPassword = txtPlexPassword.Text;

      Settings.PlexUri = txtPlexUri.Text;

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

        return SyncModes.FileToPlex;
      }
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

      if (string.IsNullOrWhiteSpace(txtPlexUsername.Text))
      {
        var path = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Plex Media Server\Plug-in Support\Databases");

        if (Directory.Exists(path))
          openFileDialog.InitialDirectory = path;
      }
      else
        openFileDialog.FileName = txtPlexUsername.Text;

      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        txtPlexUsername.Text = openFileDialog.FileName;
      }
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
            if (mode.EnumItem.Equals(SyncModes.FileToPlex))
              mode.Description = "File Properties to Plex";

            if (mode.EnumItem.Equals(SyncModes.PlexToFile))
              mode.Description = "Plex to File Properties";

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
            if (clashWinner.EnumItem.Equals(ClashWinner.File))
              clashWinner.Description = "File Properties Always Wins";

            break;
        }

        switch (SelectedSyncMode)
        {
          case SyncModes.FileToPlex:
            if (clashWinner.EnumItem.Equals(ClashWinner.Plex))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Skip))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.Prompt))
              clashWinners.Remove(clashWinner);

            if (clashWinner.EnumItem.Equals(ClashWinner.AlwaysPrompt))
              clashWinners.Remove(clashWinner);

            break;

          case SyncModes.PlexToFile:
            if (clashWinner.EnumItem.Equals(ClashWinner.File))
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
