using DS.Controls;
using DS.PlexRatingsSync.Classes;
using DS.PlexRatingsSync.Managers;
using MailKit.Security;
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

      EnableDisableRatingsOptions(chkSyncRatings.Checked);

      EnableDisableEmailOptions(chkSendEmail.Checked);

      SetupFolderGrid();

      LoadMusicFolders();
    }

    private void PopulateDropdowns()
    {
      cboSyncSource.DataSource = EnumHelper.GetAll<SyncSources>();

      cboSecurity.DataSource = EnumHelper.GetAll<SecureSocketOptions>();
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

      chkSendEmail.Checked = Settings.SendEmailSummaryOfUnmatched;

      txtSendToName.Text = Settings.EmailToName;

      txtSendToEmail.Text = Settings.EmailToEmailAddess;

      txtFromName.Text = Settings.EmailFromName;

      txtFromEmail.Text = Settings.EmailFromEmailAddess;

      txtSmtpServer.Text = Settings.SmtpServer;

      txtSmtpPort.Text = Settings.SmtpPort.ToString();

      chkUseSsl.Checked = Settings.UseSsl;

      foreach (EnumHelper.EnumValue item in cboSecurity.Items)
      {
        if (item.EnumItem.Equals(Settings.SecurityOption))
        {
          cboSecurity.SelectedItem = item;

          break;
        }
      }

      txtSmtpUsername.Text = Settings.SmtpUsername;

      txtSmtpPassword.Text = Settings.SmtpPassword;
    }

    private void SavePreferences()
    {
      Settings.PlexUsername = txtPlexUsername.Text;

      Settings.PlexPassword = txtPlexPassword.Text;

      Settings.PlexUri = txtPlexUri.Text;

      Settings.PlexFolderMappings = grdMappings.DataSource as List<PlexFolderMapping>;

      Settings.SyncRatings = chkSyncRatings.Checked;

      Settings.SyncSource = SelectedSource;

      if (cboSyncMode.SelectedItem is EnumHelper.EnumValue mode)
        Settings.SyncHandling = (SyncModes)mode.EnumItem;

      if (cboClashWinner.SelectedItem is EnumHelper.EnumValue clash)
        Settings.ClashHandling = (ClashWinner)clash.EnumItem;

      Settings.SendEmailSummaryOfUnmatched = chkSendEmail.Checked;

      Settings.EmailToName = txtSendToName.Text;

      Settings.EmailToEmailAddess = txtSendToEmail.Text;

      Settings.EmailFromName = txtFromName.Text;

      Settings.EmailFromEmailAddess = txtFromEmail.Text;

      Settings.SmtpServer = txtSmtpServer.Text;

      if (int.TryParse(txtSmtpPort.Text, out int port))
        Settings.SmtpPort = port;

      Settings.UseSsl = chkUseSsl.Checked;

      if (cboSecurity.SelectedItem is EnumHelper.EnumValue security)
        Settings.SecurityOption = (SecureSocketOptions)security.EnumItem;

      Settings.SmtpUsername = txtSmtpUsername.Text;

      Settings.SmtpPassword = txtSmtpPassword.Text;

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

    private void EnableDisableEmailOptions(bool enable)
    {
      lblSendToName.Enabled = enable;

      txtSendToName.Enabled = enable;

      lblSendToEmail.Enabled = enable;

      txtSendToEmail.Enabled = enable;

      lblFromName.Enabled = enable;

      txtFromName.Enabled = enable;

      lblFromEmail.Enabled = enable;

      txtFromEmail.Enabled = enable;

      lblSmtpServer.Enabled = enable;

      txtSmtpServer.Enabled = enable;

      lblSmtpPort.Enabled = enable;

      txtSmtpPort.Enabled = enable;

      chkUseSsl.Enabled = enable;

      lblSecurity.Enabled = enable;

      cboSecurity.Enabled = enable;

      lblSmtpUsername.Enabled = enable;

      txtSmtpUsername.Enabled = enable;

      lblSmtpPassword.Enabled = enable;

      txtSmtpPassword.Enabled = enable;
    }

    private void SetupFolderGrid()
    {
      grdMappings.Columns.Clear();

      grdMappings.Columns.Add("PlexPath", "Plex Path");
      grdMappings.Columns["PlexPath"].DataPropertyName = "PlexFolder";
      grdMappings.Columns["PlexPath"].Width = 500;
      grdMappings.Columns["PlexPath"].ReadOnly = true;

      DataGridViewButtonColumn col2 = new DataGridViewButtonColumn
      {
        Name = "LocalPath",
        Text = "Local Path",
        HeaderText = "Local Path",
        DataPropertyName = "LocalFolder",
        Width = 500,
        UseColumnTextForButtonValue = false
      };

      grdMappings.Columns.Add(col2);

      grdMappings.RowHeadersVisible = false;
      grdMappings.AutoGenerateColumns = false;
      grdMappings.AllowUserToResizeRows = false;
      grdMappings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      grdMappings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      grdMappings.ColumnHeadersHeight = 20;

      grdMappings.CellClick += GrdMappings_CellClick;
    }

    private void GrdMappings_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (grdMappings.Columns[e.ColumnIndex].Name == "LocalPath")
      {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

        var rowData = grdMappings.CurrentRow.DataBoundItem as PlexFolderMapping;

        if (!string.IsNullOrWhiteSpace(rowData.LocalFolder))
          folderBrowserDialog.SelectedPath = rowData.LocalFolder;

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
          rowData.LocalFolder = folderBrowserDialog.SelectedPath;
        }
      }
    }

    private void LoadMusicFolders()
    {
      if (string.IsNullOrWhiteSpace(txtPlexUsername.Text)
        || string.IsNullOrWhiteSpace(txtPlexPassword.Text)
        || string.IsNullOrWhiteSpace(txtPlexUri.Text))
        return;

      // Try to get an auth token
      try
      {
        var plexUser = PlexApiManager.GetUser();

        if (string.IsNullOrWhiteSpace(plexUser?.user?.authToken))
          return;

        var sections = PlexApiManager.GetLibrarySections(plexUser, txtPlexUri.Text);

        var artistSections = sections.Directory
          .Where(d => d.Type == "artist");

        List<PlexFolderMapping> folders = new List<PlexFolderMapping>();

        foreach (var artist in artistSections)
        {
          foreach (var location in artist.Location)
          {
            var mappedFolder = Settings.PlexFolderMappings.FirstOrDefault(m => m.PlexFolder == location.Path);

            if (mappedFolder == null)
            {
              mappedFolder = new PlexFolderMapping { PlexFolder = location.Path };

              Settings.PlexFolderMappings.Add(mappedFolder);
            }

            folders.Add(mappedFolder);
          }
        }

        grdMappings.DataSource = folders;
      }
      catch
      {
        grdMappings.DataSource = null;
      }
    }

    private void CmdOk_Click(object sender, EventArgs e)
    {
      SavePreferences();

      Close();
    }

    private void txtPlexUsername_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      LoadMusicFolders();
    }

    private void txtPlexPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      LoadMusicFolders();
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

    private void chkSendEmail_CheckedChanged(object sender, EventArgs e)
    {
      EnableDisableEmailOptions(chkSendEmail.Checked);
    }

    private class SelectedPlaylist
    {
      public string Playlist { get; set; }

      public bool Selected { get; set; }
    }
  }
}
