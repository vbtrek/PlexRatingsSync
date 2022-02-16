using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DS.Library.MessageHandling;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;

namespace DS.PlexRatingsSync
{
  public partial class Main : Form
  {
    private AutoResetEvent m_ResetEvent = new AutoResetEvent(false);
    private PlexDatabaseControlller m_PlexDb;
    private int m_UpdateCount = 0;
    private int m_AddCount = 0;

    public Main()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null) components.Dispose();
        if (m_PlexDb != null) m_PlexDb.Dispose();
      }
      base.Dispose(disposing);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
          "Starting up...");

      Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      this.Text = String.Format("{0} v{1}", this.Text, currentVersion);

      label1.Text = string.Empty;
      label2.Text = string.Empty;
      label3.Text = string.Empty;
      label4.Text = string.Empty;

      Settings.GetPreferences();
    }

    private void Form1_Shown(object sender, EventArgs e)
    {
      bool ok = true;

      if (!IsSettingValid(false))
      {
        cmdOptions.Enabled = false;

        using (Options2 frm = new Options2())
        {
          frm.ShowDialog(this);
        }

        cmdOptions.Enabled = true;

        if (!IsSettingValid(true)) ok = false;
      }

      if (ok)
        StartProcessing();
      else
        Close();
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!bwProcess.IsBusy) return;

      bwProcess.CancelAsync();

      m_ResetEvent.WaitOne();
    }

    private bool IsSettingValid(bool showMessage)
    {
      if (string.IsNullOrWhiteSpace(Settings.PlexDatabase))
      {
        if (showMessage)
          MessageBox.Show("You must select a Plex Database.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (!File.Exists(Settings.PlexDatabase))
      {
        if (showMessage)
          MessageBox.Show("Plex Database cannot be found in the location specified.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (!Settings.SyncRatings && !Settings.SyncPlaylists)
      {
        if (showMessage)
          MessageBox.Show("You must select at least one thing to sync.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (Settings.SyncPlaylists && string.IsNullOrWhiteSpace(Settings.ItunesLibraryPath))
      {
        if (showMessage)
          MessageBox.Show("You have selected to sync playlists but not selected an iTunes library file.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (Settings.SyncPlaylists && !File.Exists(Settings.ItunesLibraryPath))
      {
        if (showMessage)
          MessageBox.Show("You have selected to sync playlists but the iTunes library file cannot be found in the location specified.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      return true;
    }

    private void StartProcessing()
    {
      label1.Text = "Connecting to database...";
      label1.Refresh();
      label2.Text = $"Updated: 0 | New: 0";
      label2.Refresh();

      if (string.IsNullOrWhiteSpace(Settings.PlexDatabase))
      {
        string dbPath = @"%LOCALAPPDATA%\Plex Media Server\Plug-in Support\Databases\com.plexapp.plugins.library.db";
        Settings.PlexDatabase = Environment.ExpandEnvironmentVariables(dbPath);

        if (!File.Exists(Settings.PlexDatabase)) Settings.PlexDatabase = string.Empty;
      }

      label3.Text = string.Format("Plex:   {0}", Settings.PlexDatabase.EllipsisString(60));
      label4.Text = string.Format("iTunes: {0}", Settings.ItunesLibraryPath.EllipsisString(60));

      m_PlexDb = new PlexDatabaseControlller(Settings.PlexDatabase);

      progressBar1.Value = 0;
      m_UpdateCount = 0;
      m_AddCount = 0;

      bwProcess.RunWorkerAsync();
    }

    private void bwProcess_DoWork(object sender, DoWorkEventArgs e)
    {
      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      if (!e.Cancel && m_PlexDb.IsDbConnected) Process();

      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      m_ResetEvent.Set();
    }

    private void bwProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      // Progress messagees
      if (e.ProgressPercentage == 0)
      {
        label1.Text = e.UserState as string;
        label1.Refresh();
        return;
      }

      // Increment progress
      if (e.ProgressPercentage == -1)
      {
        progressBar1.Increment(1);
        progressBar1.Refresh();
        return;
      }

      // Updated
      if (e.ProgressPercentage == -2)
      {
        m_UpdateCount++;
        label2.Text = $"Updated: {m_UpdateCount} | New: {m_AddCount}";
        label2.Refresh();
        return;
      }

      // New
      if (e.ProgressPercentage == -3)
      {
        m_AddCount++;
        label2.Text = $"Updated: {m_UpdateCount} | New: {m_AddCount}";
        label2.Refresh();
        return;
      }

      // Reset counters
      if (e.ProgressPercentage == -4)
      {
        m_AddCount = 0;
        m_UpdateCount = 0;
        progressBar1.Value = progressBar1.Minimum;
        label2.Text = $"Updated: {m_UpdateCount} | New: {m_AddCount}";
        label2.Refresh();
        return;
      }

      // Set progress max
      progressBar1.Maximum = e.ProgressPercentage;
    }

    private void bwProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (!e.Cancelled) Close();
    }

    private void Process()
    {
      try
      {
        string sql = string.Empty;

        // Sync ratings
        RatingsManager.SyncRatings(new SyncArgs(bwProcess, m_PlexDb));

        // Now sync playlists
        PlaylistManager.SyncPlaylists(new SyncArgs(bwProcess, m_PlexDb));
      }
      catch (Exception ex)
      {
        MessageManager.Instance.ExceptionWrite(this, ex);
      }
    }

    private void cmdOptions_Click(object sender, EventArgs e)
    {
      cmdOptions.Enabled = false;
      bwProcess.CancelAsync();
      m_ResetEvent.WaitOne();

      using (Options2 frm = new Options2())
      {
        frm.ShowDialog(this);
      }

      cmdOptions.Enabled = true;
      StartProcessing();
    }
  }
}
