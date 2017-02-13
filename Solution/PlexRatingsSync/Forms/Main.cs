﻿using System;
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
        private ItunesManager m_Itunes;
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

            if (!IsSettingValid())
            {
                cmdOptions.Enabled = false;

                using (Options frm = new Options())
                {
                    frm.ShowDialog(this);
                }

                cmdOptions.Enabled = true;

                if (!IsSettingValid()) ok = false;
            }

            if (ok)
                StartProcessing();
            else
                Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bwProcess.CancelAsync();
            m_ResetEvent.WaitOne();
        }

        private bool IsSettingValid()
        {
            bool settingsOk = true;

            if (string.IsNullOrWhiteSpace(Settings.PlexDatabase)) settingsOk = false;
            if (!File.Exists(Settings.PlexDatabase)) settingsOk = false;
            if (!Settings.SyncRatings && !Settings.SyncPlaylists) settingsOk = false;
            if (Settings.SyncPlaylists && string.IsNullOrWhiteSpace(Settings.ItunesLibraryPath)) settingsOk = false;
            if (Settings.SyncPlaylists && !File.Exists(Settings.ItunesLibraryPath)) settingsOk = false;

            return settingsOk;
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
            m_Itunes = new ItunesManager();

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
                SyncRatings();

                // Now sync playlists
                SyncPlaylists();
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(this, ex);
            }
        }

        private void SyncRatings()
        {
            if (!Settings.SyncRatings) return;

            // Get all the files to sync ratings for
            bwProcess.ReportProgress(0, "Reading Track Data From Plex...");

// TODO_DS Add an option to choose the Plex account to sync
            string sql = @"
SELECT MTI.guid, MP.file, MTIS.rating
FROM media_items MI
INNER JOIN media_parts MP ON MP.media_item_id = MI.id
INNER JOIN metadata_items MTI ON MTI.id = MI.metadata_item_id
INNER JOIN library_sections LS ON LS.id = MI.library_section_id
LEFT JOIN metadata_item_settings MTIS ON MTIS.guid = MTI.guid AND MTIS.account_id = {0}
WHERE LS.section_type = 8";
            sql = string.Format(sql, Settings.PlexAccountId);

            List<PlexRatingsData> ratingdata = m_PlexDb.ReadPlexAndMap<PlexRatingsData>(sql);

            bwProcess.ReportProgress(ratingdata.Count);
            bwProcess.ReportProgress(0, "Syncing Ratings...");

            // Process all the files
            foreach (PlexRatingsData file in ratingdata)
            {
                if (bwProcess.CancellationPending) return;

                bwProcess.ReportProgress(0, $"Syncing \"{new FileInfo(file.file).Name}\"...");

                SyncRating(file);

                bwProcess.ReportProgress(-1);
            }
        }

        private void SyncPlaylists()
        {
            if (bwProcess.CancellationPending) return;

            if (!Settings.SyncPlaylists) return;

            bwProcess.ReportProgress(0, "Reading Playlists from iTunes...");
            m_Itunes.GetItunesPlayLists(Settings.ItunesLibraryPath, true);

            bwProcess.ReportProgress(Settings.ChosenPlaylists.Count);
            bwProcess.ReportProgress(-4);
            bwProcess.ReportProgress(0, "Syncing Playlists...");

            foreach (var playlist in m_Itunes.ItunesPlaylists)
            {
                if (bwProcess.CancellationPending) return;

                if (!Settings.ChosenPlaylists.Contains(playlist.FullPlaylistName)) continue;

                bwProcess.ReportProgress(0, $"Syncing Playlist \"{playlist.FullPlaylistName}\"...");

                // Find playlist by name
                string sql = string.Empty;
                bool isUpdate = false;
                int? playlistId = GetPlexPlaylistId(playlist);

                if (playlistId != null && playlistId > 0)
                {
                    isUpdate = true;

                    // Delete playlist entries (ready to rebuild)
                    sql = "DELETE FROM play_queue_generators WHERE playlist_id = {0}";
                    sql = string.Format(sql, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif

                    // Update the playlist back to blank
                    sql = @"
UPDATE metadata_items
SET media_item_count = 0, duration = 0, 
updated_at = datetime('now'), extra_data = 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1'
WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif
                }
                else
                {
                    // Create playlist
                    sql = @"
INSERT INTO metadata_items
([guid], [metadata_type], [media_item_count], [title], [title_sort], [index], [absolute_index], [duration], [added_at], [created_at], [updated_at], [extra_data])
VALUES
('com.plexapp.agents.none://{0}', 15, 0, '{1}', '{1}', 
0, 10, 0, datetime('now'), datetime('now'), datetime('now'), 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1')";
                    sql = string.Format(sql, Guid.NewGuid(), playlist.FullPlaylistName);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif

                    playlistId = GetPlexPlaylistId(playlist);
                }

                // Ensure there is a metadata_item_accounts record for each playlist
                sql = "SELECT id FROM metadata_item_accounts WHERE account_id = {0} AND metadata_item_id = {1}";
                sql = string.Format(sql, Settings.PlexAccountId, playlistId);
                int? id = (int?)m_PlexDb.ReadPlexValue(sql);

                if (id == null)
                {
                    sql = @"
INSERT INTO metadata_item_accounts (account_id, metadata_item_id)
VALUES ({0}, {1})";
                    sql = string.Format(sql, Settings.PlexAccountId, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif
                }

                // Insert the items into the playlist
                int addDuration = 0;
                int orderIncrement = 1000;

                sql = "SELECT MAX([order]) FROM play_queue_generators WHERE playlist_id = {0}";
                sql = string.Format(sql, playlistId);
                double? currentOrder = m_PlexDb.ReadPlexValue(sql);
                currentOrder = currentOrder == null ? orderIncrement : currentOrder;

                foreach (var item in playlist.Tracks)
                {
                    // Lookup the file in the DB to get it's id
                    sql = @"
SELECT MI.metadata_item_id
FROM media_parts AS MP
INNER JOIN media_items AS MI ON MI.id = MP.media_item_id
WHERE MP.file = '{0}' COLLATE NOCASE";
                    sql = string.Format(sql, item.ProperLocation.Replace("'", "''"));
                    long? dbItemID = m_PlexDb.ReadPlexValue(sql) as long?;

                    if (dbItemID != null)
                    {
                        // Lookup the duration in the DB
                        sql = @"
SELECT MP.duration
FROM media_parts AS MP
INNER JOIN media_items AS MI ON MI.id = MP.media_item_id
WHERE MI.metadata_item_id = {0}";
                        sql = string.Format(sql, dbItemID);
                        long? dbDuration = m_PlexDb.ReadPlexValue(sql) as long?;
                        if (dbDuration == null) dbDuration = 0;

                        // Create a new playlist entry
                        sql = @"
INSERT INTO play_queue_generators
([playlist_id], [metadata_item_id], [order], [created_at], [updated_at], [uri])
VALUES
({0}, {1}, {2}, datetime('now'), datetime('now'), '')";
                        sql = string.Format(sql, playlistId, dbItemID, currentOrder);
#if DEBUG
                        Debug.Print(sql);
#else
                        m_PlexDb.ExecutePlexSql(sql);
#endif

                        currentOrder += orderIncrement;

                        addDuration += (int)(dbDuration / 1000);
                    }
                }

                if (playlist.Tracks.Count == 0 && Settings.RemoveEmptyPlaylists)
                {
                    // Remove the playlist
                    sql = @"
DELETE metadata_item_accounts WHERE metadata_item_id = {0}";
                    sql = string.Format(sql, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif

                    sql = @"
DELETE metadata_items WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif
                }
                else
                {
                    // Update the playlists info
                    sql = @"
UPDATE metadata_items
SET duration = {0},
media_item_count = {1}
WHERE id = {2}";
                    sql = string.Format(sql, addDuration, playlist.Tracks.Count, playlistId);
#if DEBUG
                    Debug.Print(sql);
#else
                    m_PlexDb.ExecutePlexSql(sql);
#endif
                }

                if (isUpdate)
                    bwProcess.ReportProgress(-2);
                else
                    bwProcess.ReportProgress(-3);

                bwProcess.ReportProgress(-1);
            }
        }

        private int? GetPlexPlaylistId(ItunesPlaylist playlist)
        {
            int? playlistId = null;
            string sql = "SELECT id FROM metadata_items WHERE title = '{0}' AND metadata_type = 15";    //15=PLAYLIST_TYPE
            sql = string.Format(sql, playlist.FullPlaylistName);
            playlistId = (int?)m_PlexDb.ReadPlexValue(sql);

            return playlistId;
        }

        private void SyncRating(PlexRatingsData currentFile)
        {
            try
            {
                if (currentFile != null && File.Exists(currentFile.file))
                {
                    try
                    {
                        int? plexFileRating = RatingsManager.PlexRatingFromFile(currentFile.file);
                        int? plexDbRating = (int?)currentFile.rating;

                        if (plexFileRating != plexDbRating)
                        {
                            // NOTE: Plex wins if both the file and db have ratings that are different
                            if (plexDbRating != null)
                                UpdateFileRating(currentFile);
                            else
                                UpdatePlexDbRating(currentFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageManager.Instance.ExceptionWrite(this, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(this, ex);
            }
        }

        private void UpdatePlexDbRating(PlexRatingsData currentFile)
        {
            int? plexFileRating = RatingsManager.PlexRatingFromFile(currentFile.file);
            int? plexDbRating = (int?)currentFile.rating;

            string sql = string.Empty;
            string message = string.Empty;

            // the rating(s) of a given file
            sql = string.Format(
                @"SELECT * FROM metadata_item_settings WHERE account_id = {0} AND guid = '{1}';",
                Settings.PlexAccountId, currentFile.guid);

            if (m_PlexDb.RecordsExists(sql))
            {
                message = string.Format("Updating Plex rating for file \"{0}\" from {1} to {2}",
                    currentFile.file, 
                    plexDbRating == null ? 0 : plexDbRating, 
                    plexFileRating == null ? 0 : plexFileRating);

                MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                    message);

                bwProcess.ReportProgress(-2);

                // Update a rating entry
                sql = @"
UPDATE metadata_item_settings SET rating = {0} WHERE account_id = {1} AND guid = '{2}'";

                sql = string.Format(sql, plexFileRating, Settings.PlexAccountId, currentFile.guid);
            }
            else
            {
                message = string.Format("Creating Plex rating for file \"{0}\", rating {1}",
                    currentFile.file, 
                    plexFileRating == null ? 0 : plexFileRating);

                MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                    message);

                bwProcess.ReportProgress(-3);

                // Create a rating entry
                sql = @"
INSERT INTO metadata_item_settings ([account_id], [guid], [rating], [view_offset], [view_count], [last_viewed_at], [created_at], [updated_at])
VALUES({0}, '{1}', {2}, NULL, 0, NULL, DATE('now'), DATE('now'));";

                sql = string.Format(sql, Settings.PlexAccountId, currentFile.guid, plexFileRating);
            }
#if DEBUG
            Debug.Print(message);
#else
            m_PlexDb.ExecutePlexSql(sql);
#endif
        }

        private void UpdateFileRating(PlexRatingsData currentFile)
        {
            uint? fileRating = RatingsManager.FileRating(currentFile.file);
            int? plexFileRating = RatingsManager.PlexRatingFromFile(currentFile.file);
            int? plexDbRating = (int?)currentFile.rating;

            uint? newRating = Convert.ToUInt32(RatingsManager.FileRatingFromPlexRating(plexDbRating));
            if (newRating == 0) newRating = null;

            string message = string.Format("Updating file rating for file \"{0}\", from {1} to {2}",
                currentFile.file,
                fileRating == null ? 0 : fileRating,
                newRating == null ? 0 : newRating);

            ShellFile so = ShellFile.FromFilePath(currentFile.file);
#if DEBUG
            Debug.Print(message);
#else
            so.Properties.System.Rating.Value = newRating;
#endif

            MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information, message);
        }

        private void cmdOptions_Click(object sender, EventArgs e)
        {
            cmdOptions.Enabled = false;
            bwProcess.CancelAsync();
            m_ResetEvent.WaitOne();

            using (Options frm = new Options())
            {
                frm.ShowDialog(this);
            }

            cmdOptions.Enabled = true;
            StartProcessing();
        }
    }
}
