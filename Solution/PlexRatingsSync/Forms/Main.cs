using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DS.Library.MessageHandling;

namespace DS.PlexRatingsSync
{
    public partial class Main : Form
    {
        private bool m_LoadPlaylistsOnly = false;
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

                m_LoadPlaylistsOnly = true;
                StartProcessing();

                while (bwProcess.IsBusy) Application.DoEvents();

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

        private bool IsSettingValid()
        {
            bool settingsOk = true;

            if (string.IsNullOrWhiteSpace(Settings.PlexDatabase)) settingsOk = false;
            if (!File.Exists(Settings.PlexDatabase)) settingsOk = false;
            if (!Settings.SyncRatings && !Settings.SyncPlaylists) settingsOk = false;
            if (Settings.SyncPlaylists && string.IsNullOrWhiteSpace(Settings.ItunesLibraryPath)) settingsOk = false;
            if (!File.Exists(Settings.ItunesLibraryPath)) settingsOk = false;

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

            m_PlexDb = new PlexDatabaseControlller();
            m_Itunes = new ItunesManager();

            progressBar1.Value = 0;
            m_UpdateCount = 0;
            m_AddCount = 0;

            bwProcess.RunWorkerAsync();
        }

        private void bwProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.SyncPlaylists)
            {
                bwProcess.ReportProgress(0, "Reading Playlists from iTunes...");
                m_Itunes.GetItunesPlayLists(Settings.ItunesLibraryPath);
            }

            if (m_LoadPlaylistsOnly)
            {
                e.Cancel = true;
                return;
            }

            if (bwProcess.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (m_PlexDb.IsDbConnected) Process();

            if (bwProcess.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
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

                // Get all the accounts to store ratings against
// TODO_DS Add an option to choose the Plex account to sync
                sql = @"SELECT * FROM accounts WHERE id = 1;";
                List<PlexTableAccounts> accounts = m_PlexDb.ReadPlexAndMap<PlexTableAccounts>(sql);

                // Sync ratings
                SyncRatings(accounts);

                // Now sync playlists
                SyncPlaylists(accounts);
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(this, ex);
            }
        }

        private void SyncRatings(List<PlexTableAccounts> accounts)
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
LEFT JOIN metadata_item_settings MTIS ON MTIS.guid = MTI.guid AND MTIS.account_id = 1
WHERE LS.section_type = 8";
            List<PlexRatingsData> ratingdata = m_PlexDb.ReadPlexAndMap<PlexRatingsData>(sql);

            bwProcess.ReportProgress(ratingdata.Count);
            bwProcess.ReportProgress(0, "Syncing Ratings...");

            // Process all the files
            foreach (PlexRatingsData file in ratingdata)
            {
                if (bwProcess.CancellationPending) return;

                bwProcess.ReportProgress(0, $"Syncing \"{new FileInfo(file.file).Name}\"...");

                SyncRating(file, accounts);

                bwProcess.ReportProgress(-1);
            }
        }

        private void SyncPlaylists(List<PlexTableAccounts> accounts)
        {
            if (!Settings.SyncPlaylists) return;

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
                    m_PlexDb.ExecutePlexSql(sql);

                    // Update the playlist back to blank
                    sql = @"
UPDATE metadata_items
SET media_item_count = 0, duration = 0, 
updated_at = datetime('now'), extra_data = 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1'
WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
                    m_PlexDb.ExecutePlexSql(sql);
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
                    m_PlexDb.ExecutePlexSql(sql);

                    playlistId = GetPlexPlaylistId(playlist);
                }

                // Ensure there is a metadata_item_accounts record for each playlist
                foreach (PlexTableAccounts account in accounts)
                {
                    sql = "SELECT id FROM metadata_item_accounts WHERE account_id = {0} AND metadata_item_id = {1}";
                    sql = string.Format(sql, account.id, playlistId);
                    int? id = (int?)m_PlexDb.ReadPlexValue(sql);

                    if (id == null)
                    {
                        sql = @"
INSERT INTO metadata_item_accounts (account_id, metadata_item_id)
VALUES ({0}, {1})";
                        sql = string.Format(sql, account.id, playlistId);
                        m_PlexDb.ExecutePlexSql(sql);
                    }
                }

                // Insert the items into the playlist
                int addDuration = 0;
                int orderIncrement = 1000;

                sql = "SELECT MAX([order]) FROM play_queue_generators WHERE playlist_id = {0}";
                sql = string.Format(sql, playlistId);
                int? currentOrder = m_PlexDb.ReadPlexValue(sql);
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
                        m_PlexDb.ExecutePlexSql(sql);

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
                    m_PlexDb.ExecutePlexSql(sql);

                    sql = @"
DELETE metadata_items WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
                    m_PlexDb.ExecutePlexSql(sql);
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
                    m_PlexDb.ExecutePlexSql(sql);
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

        private void SyncRating(PlexRatingsData currentFile, List<PlexTableAccounts> accounts)
        {
            try
            {
                if (currentFile != null && File.Exists(currentFile.file))
                {
                    try
                    {
                        int? plexFileRating = RatingsManager.PlexRatingFromFile(currentFile.file);

                        foreach (PlexTableAccounts account in accounts)
                        {
                            int? plexDbRating = PlexRatingFromDatabase(account.id, currentFile.file);

                            if (plexFileRating != plexDbRating)
                            {
                                string sql = string.Empty;

                                // the rating(s) of a given file
                                sql = string.Format(
                                    @"SELECT * FROM metadata_item_settings WHERE account_id = {0} AND guid = '{1}';",
                                    account.id, currentFile.guid);

                                if (m_PlexDb.RecordsExists(sql))
                                {
                                    MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                                        string.Format("Updating rating for file \"{0}\" from {1} to {2}", 
                                        currentFile.file, plexDbRating, plexFileRating));

                                    bwProcess.ReportProgress(-2);

                                    // Update a rating entry
                                    sql = @"UPDATE metadata_item_settings SET rating = {0} WHERE account_id = {1} AND guid = '{2}'";

                                    sql = string.Format(sql, plexFileRating, account.id, currentFile.guid);
                                }
                                else
                                {
                                    MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                                        string.Format("Creating rating for file \"{0}\", rating {1}",
                                        currentFile.file, plexFileRating));

                                    bwProcess.ReportProgress(-3);

                                    // Create a rating entry
                                    sql = @"
INSERT INTO metadata_item_settings ([account_id], [guid], [rating], [view_offset], [view_count], [last_viewed_at], [created_at], [updated_at])
VALUES({0}, '{1}', {2}, NULL, 0, NULL, DATE('now'), DATE('now'));";

                                    sql = string.Format(sql, account.id, currentFile.guid, plexFileRating);
                                }

                                m_PlexDb.ExecutePlexSql(sql);
                            }
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

        public int? PlexRatingFromDatabase(Int64 accountId, string file)
        {
            int? rating = null;

            // the rating(s) of a given file
            string sql = @"SELECT MTIS.rating
FROM metadata_item_settings MTIS
INNER JOIN metadata_items MTI ON MTI.guid = MTIS.guid
INNER JOIN media_items MI ON MI.metadata_item_id = MTI.id
INNER JOIN media_parts MP ON MP.id = MI.id
WHERE MTIS.account_id = {0}
AND MP.file = '{1}';";

            sql = string.Format(sql, accountId, file.Replace("'", "''"));

            rating = (int?)m_PlexDb.ReadPlexValue(sql);

            return rating;
        }

        private void cmdOptions_Click(object sender, EventArgs e)
        {
            cmdOptions.Enabled = false;
            bwProcess.CancelAsync();

            while (bwProcess.IsBusy)
                Application.DoEvents();

            using (Options frm = new Options())
            {
                frm.ShowDialog(this);
            }

            cmdOptions.Enabled = true;
            StartProcessing();
        }
    }
}
