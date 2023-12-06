using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DS.Library.MessageHandling;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;
using System.Threading;
using Sentry;

namespace DS.PlexRatingsSync
{
  public partial class Main : Form
  {
    private AutoResetEvent _ResetEvent = new AutoResetEvent(false);

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
      }
      base.Dispose(disposing);
    }

    private void Main_Load(object sender, EventArgs e)
    {
      try
      {
        MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
            "Starting up...");

        Version currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        Text = $"{Text} v{currentVersion}";

        lblStatus.Text = string.Empty;
        lblTotals.Text = string.Empty;
        lblPlex.Text = string.Empty;
        lblSubMessage.Text = string.Empty;

        Settings.GetPreferences();
      }
      catch (Exception ex)
      {
        SentrySdk.CaptureException(ex);
      }
    }

    private void Main_Shown(object sender, EventArgs e)
    {
      try
      {
        bool ok = true;

        if (!IsSettingValid(false))
        {
          cmdOptions.Enabled = false;

          using (Options3 frm = new Options3())
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
      catch (Exception ex)
      {
        SentrySdk.CaptureException(ex);
      }
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!bwProcess.IsBusy) return;

      bwProcess.CancelAsync();

      _ResetEvent.WaitOne();
    }

    private void bwProcess_DoWork(object sender, DoWorkEventArgs e)
    {
      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      if (!e.Cancel) Process();

      if (!e.Cancel && bwProcess.CancellationPending)
        e.Cancel = true;

      _ResetEvent.Set();
    }

    private void bwProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      // Progress messages
      if (e.ProgressPercentage == 0)
      {
        UpdateLabel(lblStatus, e.UserState as string);

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

        UpdateLabel(lblTotals, $"Updated: {_UpdateCount}");

        return;
      }

      // New
      if (e.ProgressPercentage == -3)
      {
        _AddCount++;

        UpdateLabel(lblTotals, $"Updated: {_UpdateCount}");
        
        return;
      }

      // Reset counters
      if (e.ProgressPercentage == -4)
      {
        _AddCount = 0;

        _UpdateCount = 0;

        progressBar1.Value = progressBar1.Minimum;

        UpdateLabel(lblTotals, $"Updated: {_UpdateCount}");

        return;
      }

      // Update sub label
      if (e.ProgressPercentage == -5)
      {
        UpdateLabel(lblSubMessage, e.UserState as string);

        return;
      }

      // Set progress max
      progressBar1.Maximum = e.ProgressPercentage;
    }

    private void bwProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (!e.Cancelled) Close();
    }

    private void cmdOptions_Click(object sender, EventArgs e)
    {
      cmdOptions.Enabled = false;
      bwProcess.CancelAsync();
      _ResetEvent.WaitOne();

      using (Options3 frm = new Options3())
      {
        frm.ShowDialog(this);
      }

      cmdOptions.Enabled = true;
      StartProcessing();
    }

    private bool IsSettingValid(bool showMessage)
    {
      if (string.IsNullOrWhiteSpace(Settings.PlexUsername))
      {
        if (showMessage)
          MessageBox.Show("You must enter a Plex Username.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (string.IsNullOrWhiteSpace(Settings.PlexPassword))
      {
        if (showMessage)
          MessageBox.Show("You must enter a Plex Password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (string.IsNullOrWhiteSpace(Settings.PlexUri))
      {
        if (showMessage)
          MessageBox.Show("You must enter a Plex Uri.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (!Settings.SyncRatings)
      {
        if (showMessage)
          MessageBox.Show("You must select at least one thing to sync.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

        return false;
      }

      if (Settings.SendEmailSummaryOfUnmatched)
      {
        if (string.IsNullOrWhiteSpace(Settings.SmtpServer))
        {
          if (showMessage)
            MessageBox.Show("You must enter an SMTP Server Address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

          return false;
        }

        if (string.IsNullOrWhiteSpace(Settings.EmailFromEmailAddess))
        {
          if (showMessage)
            MessageBox.Show("You must enter a from email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

          return false;
        }

        if (string.IsNullOrWhiteSpace(Settings.EmailToEmailAddess))
        {
          if (showMessage)
            MessageBox.Show("You must enter a to email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);

          return false;
        }
      }

      return true;
    }

    private void StartProcessing()
    {
      UpdateLabel(lblStatus, "Connecting...");

      UpdateLabel(lblTotals, $"Updated: 0");

      UpdateLabel(lblPlex, $"Plex:   {Settings.PlexUri.EllipsisString(60)}");

      progressBar1.Value = 0;

      _UpdateCount = 0;
      
      _AddCount = 0;

      bwProcess.RunWorkerAsync();
    }

    private void Process()
    {
      try
      {
        using (var args = new SyncArgs(bwProcess))
        {
          // Sync ratings
          RatingsManager.SyncRatings(args);
        }
      }
      catch (Exception ex)
      {
        SentrySdk.CaptureException(ex);

        MessageManager.Instance.ExceptionWrite(this, ex);
      }
    }

    private void UpdateLabel(Label control, string text)
    {
      control.Text = text;

      control.Refresh();
    }
  }
}
