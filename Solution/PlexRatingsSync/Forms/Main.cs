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
    private AutoResetEvent _ResetEvent = new AutoResetEvent(false);
    private PlexDatabaseControlller _PlexDb;
    private int _UpdateCount = 0;
    private int _AddCount = 0;

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
        if (_PlexDb != null) _PlexDb.Dispose();
      }
      base.Dispose(disposing);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
          "Starting up...");

      Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      this.Text = String.Format("{0} v{1}", this.Text, currentVersion);

      lblStatus.Text = string.Empty;
      lblTotals.Text = string.Empty;
      lblPlex.Text = string.Empty;
      lblItunes.Text = string.Empty;

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

      _ResetEvent.WaitOne();
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
      lblStatus.Text = "Connecting to database...";
      lblStatus.Refresh();
      lblTotals.Text = $"Updated: 0 | New: 0";
      lblTotals.Refresh();

      if (string.IsNullOrWhiteSpace(Settings.PlexDatabase))
      {
        string dbPath = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Plex Media Server\Plug-in Support\Databases\com.plexapp.plugins.library.db");

        if (File.Exists(Settings.PlexDatabase))
          Settings.PlexDatabase = dbPath;
      }

      lblPlex.Text = $"Plex:   {Settings.PlexDatabase.EllipsisString(60)}";
      lblItunes.Text = $"iTunes: {Settings.ItunesLibraryPath.EllipsisString(60)}";

      _PlexDb = new PlexDatabaseControlller(Settings.PlexDatabase);

      progressBar1.Value = 0;
      _UpdateCount = 0;
      _AddCount = 0;

      bwProcess.RunWorkerAsync();
    }

    private void bwProcess_DoWork(object sender, DoWorkEventArgs e)
    {
      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      if (!e.Cancel && _PlexDb.IsDbConnected) Process();

      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      _ResetEvent.Set();
    }

    private void bwProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      // Progress messagees
      if (e.ProgressPercentage == 0)
      {
        lblStatus.Text = e.UserState as string;
        lblStatus.Refresh();
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
        _UpdateCount++;
        lblTotals.Text = $"Updated: {_UpdateCount} | New: {_AddCount}";
        lblTotals.Refresh();
        return;
      }

      // New
      if (e.ProgressPercentage == -3)
      {
        _AddCount++;
        lblTotals.Text = $"Updated: {_UpdateCount} | New: {_AddCount}";
        lblTotals.Refresh();
        return;
      }

      // Reset counters
      if (e.ProgressPercentage == -4)
      {
        _AddCount = 0;
        _UpdateCount = 0;
        progressBar1.Value = progressBar1.Minimum;
        lblTotals.Text = $"Updated: {_UpdateCount} | New: {_AddCount}";
        lblTotals.Refresh();
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
        RatingsManager.SyncRatings(new SyncArgs(bwProcess, _PlexDb));

        // Now sync playlists
        PlaylistManager.SyncPlaylists(new SyncArgs(bwProcess, _PlexDb));
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
      _ResetEvent.WaitOne();

      using (Options2 frm = new Options2())
      {
        frm.ShowDialog(this);
      }

      cmdOptions.Enabled = true;
      StartProcessing();
    }
  }
}
