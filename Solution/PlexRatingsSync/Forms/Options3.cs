using DS.Controls;
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
