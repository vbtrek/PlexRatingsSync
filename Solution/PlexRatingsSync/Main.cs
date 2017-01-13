using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Web;
using DS.Library.MessageHandling;

namespace DS.PlexRatingsSync
{
    public partial class Main : Form
    {
        private bool m_LoadPlaylistsOnly = false;
        private SQLiteConnection m_DbConnection = null;
        private int m_UpdateCount = 0;
        private int m_AddCount = 0;
        private List<ItunesTrack> m_ItunesTracks = new List<ItunesTrack>();
        private List<ItunesPlaylist> m_AllItunesPlaylists = new List<ItunesPlaylist>();
        private List<ItunesPlaylist> m_ItunesPlaylists = new List<ItunesPlaylist>();

        public Main()
        {
            InitializeComponent();
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

                using (Options frm = new Options(m_ItunesPlaylists))
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

            if (!string.IsNullOrWhiteSpace(Settings.PlexDatabase))
            {
                m_DbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", Settings.PlexDatabase));
                m_DbConnection.Open();
            }

            progressBar1.Value = 0;
            m_UpdateCount = 0;
            m_AddCount = 0;

            bwProcess.RunWorkerAsync();
        }

        private void GetItunesPlayLists()
        {
            if (string.IsNullOrWhiteSpace(Settings.ItunesLibraryPath)) return;

            bwProcess.ReportProgress(0, "Reading Playlists from iTunes...");
            
            string xpath = "/plist/dict/dict/dict";

            XPathDocument docNav = new XPathDocument(Settings.ItunesLibraryPath);
            XPathNavigator nav = docNav.CreateNavigator();
            XPathExpression expr = nav.Compile(xpath);
            XPathNodeIterator nodes = nav.Select(expr);

            XmlReaderSettings set2 = new XmlReaderSettings();
            set2.ConformanceLevel = ConformanceLevel.Fragment;

            m_ItunesTracks.Clear();

            foreach (XPathNavigator node in nodes)
            {
                XPathDocument doc2 = new XPathDocument(
                    XmlReader.Create(new StringReader(node.InnerXml), set2));

                XPathNavigator nav2 = doc2.CreateNavigator();

                ItunesTrack track = new ItunesTrack();
                track.Id = nav2.SelectSingleNode("key[.='Track ID']/following-sibling::*").ValueAsInt;
                track.Name = nav2.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
                track.Location = nav2.SelectSingleNode("key[.='Location']/following-sibling::*").ToString();
                m_ItunesTracks.Add(track);
            }


            xpath = "/plist/dict/array/dict";

            expr = nav.Compile(xpath);
            nodes = nav.Select(expr);

            m_ItunesPlaylists.Clear();

            foreach (XPathNavigator node in nodes)
            {
                if (!node.InnerXml.Contains("<key>Master</key>")
                    && !node.InnerXml.Contains("<key>Distinguished Kind</key>"))
                {
                    XPathDocument doc2 = new XPathDocument(
                        XmlReader.Create(new StringReader(node.InnerXml), set2));

                    XPathNavigator nav2 = doc2.CreateNavigator();

                    // TODO Need to build a path reading "Playlist Persistent ID" and "Parent Persistent ID"
                    ItunesPlaylist playlist = new ItunesPlaylist();
                    playlist.PlaylistPersistentID = nav2.SelectSingleNode("key[.='Playlist Persistent ID']/following-sibling::*")?.ToString();
                    playlist.ParentPersistentID = nav2.SelectSingleNode("key[.='Parent Persistent ID']/following-sibling::*")?.ToString();
                    playlist.Name = nav2.SelectSingleNode("key[.='Name']/following-sibling::*").ToString();
                    playlist.FullPlaylistName = FullPlaylistName(playlist);

                    m_AllItunesPlaylists.Add(playlist);

                    if (!node.InnerXml.Contains("<key>Folder</key>"))
                    {
                        var trackListXml = nav2.Select("key[.='Playlist Items']/following-sibling::*");

                        trackListXml.MoveNext();
                        string tracksXml = trackListXml.Current.InnerXml;

                        doc2 = new XPathDocument(XmlReader.Create(new StringReader(tracksXml), set2));
                        nav2 = doc2.CreateNavigator();

                        var trackIdList = nav2.Select("dict/key[.='Track ID']/following-sibling::*");

                        while (trackIdList.MoveNext())
                        {
                            int trackId = trackIdList.Current.ValueAsInt;
                            ItunesTrack track = m_ItunesTracks.FirstOrDefault(t => t.Id == trackId);

                            if (track != null) playlist.Tracks.Add(track);
                        }

                        //ItunesTrack track = m_ItunesTracks.Find(t=>t.Id==)
                        //playlist.Tracks.Add(track);

                        m_ItunesPlaylists.Add(playlist);
                    }
                }
            }
        }

        public string FullPlaylistName(ItunesPlaylist playlist)
        {
            string name = playlist.Name;

            if (!string.IsNullOrWhiteSpace(playlist.ParentPersistentID))
            {
                ItunesPlaylist parent = m_AllItunesPlaylists.FirstOrDefault(p => p.PlaylistPersistentID == playlist.ParentPersistentID);

                if (parent != null) name = string.Format("{0} - {1}", FullPlaylistName(parent), name);
            }

            return name;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_DbConnection != null) m_DbConnection.Close();
        }

        private void bwProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.SyncPlaylists) GetItunesPlayLists();

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

            if (m_DbConnection != null) Process();

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
                sql = @"SELECT * FROM accounts;";
                List<PlexTableAccounts> accounts = ReadPlexAndMap<PlexTableAccounts>(sql);

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

            string sql = @"
SELECT MTI.guid, MP.file, MTIS.rating
FROM media_items MI
INNER JOIN media_parts MP ON MP.id = MI.id
INNER JOIN metadata_items MTI ON MTI.id = MI.metadata_item_id
INNER JOIN library_sections LS ON LS.id = MI.library_section_id
LEFT JOIN metadata_item_settings MTIS ON MTIS.guid = MTI.guid
WHERE LS.section_type = 8";
            List<PlexRatingsData> ratingdata = ReadPlexAndMap<PlexRatingsData>(sql);

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

            foreach (var playlist in m_ItunesPlaylists)
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
                    ExecutePlexSql(sql);

                    // Update the playlist back to blank
                    sql = @"
UPDATE metadata_items
SET media_item_count = 0, duration = 0, 
updated_at = datetime('now'), extra_data = 'pv%3AdurationInSeconds=1&pv%3AsectionIDs=1'
WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
                    ExecutePlexSql(sql);
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
                    ExecutePlexSql(sql);

                    playlistId = GetPlexPlaylistId(playlist);
                }

                // Ensure there is a metadata_item_accounts record for each playlist
                foreach (PlexTableAccounts account in accounts)
                {
                    sql = "SELECT id FROM metadata_item_accounts WHERE account_id = {0} AND metadata_item_id = {1}";
                    sql = string.Format(sql, account.id, playlistId);
                    int? id = (int?)ReadPlexValue(sql);

                    if (id == null)
                    {
                        sql = @"
INSERT INTO metadata_item_accounts (account_id, metadata_item_id)
VALUES ({0}, {1})";
                        sql = string.Format(sql, account.id, playlistId);
                        ExecutePlexSql(sql);
                    }
                }

                // Insert the items into the playlist
                int addDuration = 0;
                int orderIncrement = 1000;

                sql = "SELECT MAX([order]) FROM play_queue_generators WHERE playlist_id = {0}";
                sql = string.Format(sql, playlistId);
                int? currentOrder = ReadPlexValue(sql);
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
                    long? dbItemID = ReadPlexValue(sql) as long?;

                    if (dbItemID != null)
                    {
                        // Lookup the duration in the DB
                        sql = @"
SELECT MP.duration
FROM media_parts AS MP
INNER JOIN media_items AS MI ON MI.id = MP.media_item_id
WHERE MI.metadata_item_id = {0}";
                        sql = string.Format(sql, dbItemID);
                        long? dbDuration = ReadPlexValue(sql) as long?;
                        if (dbDuration == null) dbDuration = 0;

                        // Create a new playlist entry
                        sql = @"
INSERT INTO play_queue_generators
([playlist_id], [metadata_item_id], [order], [created_at], [updated_at], [uri])
VALUES
({0}, {1}, {2}, datetime('now'), datetime('now'), '')";
                        sql = string.Format(sql, playlistId, dbItemID, currentOrder);
                        ExecutePlexSql(sql);

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
                    ExecutePlexSql(sql);

                    sql = @"
DELETE metadata_items WHERE id = {0}";
                    sql = string.Format(sql, playlistId);
                    ExecutePlexSql(sql);
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
                    ExecutePlexSql(sql);
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
            playlistId = (int?)ReadPlexValue(sql);

            return playlistId;
        }

        private List<T> ReadPlexAndMap<T>(string sql)
            where T : new()
        {
            MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
                string.Format("Executing and mapping SQL{0}{1}",
                Environment.NewLine, sql));

            List<T> data = new List<T>();

            using (SQLiteCommand command = new SQLiteCommand(sql, m_DbConnection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = new T();
                        ReadFields(reader, item);
                        data.Add(item);
                    }
                }
            }

            return data;
        }

        private void ExecutePlexSql(string sql)
        {
            MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
                string.Format("Executing SQL{0}{1}",
                Environment.NewLine, sql));

            using (SQLiteCommand command = new SQLiteCommand(sql, m_DbConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private dynamic ReadPlexValue(string sql)
        {
            MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
                string.Format("Reading SQL Value{0}{1}",
                Environment.NewLine, sql));

            dynamic result = null;

            using (SQLiteCommand command = new SQLiteCommand(sql, m_DbConnection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        if (reader.IsDBNull(0))
                            result = null;
                        else
                            result = reader[0];
                    }
                }
            }
            return result;
        }

        private void SyncRating(PlexRatingsData currentFile, List<PlexTableAccounts> accounts)
        {
            try
            {
                if (currentFile != null && File.Exists(currentFile.file))
                {
                    try
                    {
                        int? plexFileRatng = PlexRatingFromFile(currentFile.file);

                        foreach (PlexTableAccounts account in accounts)
                        {
                            int? plexDbRating = PlexRatingFromDatabase(account.id, currentFile.file);

                            if (plexFileRatng != plexDbRating)
                            {
                                string sql = string.Empty;

                                if (PlexDatabaseRatingExists(account.id, currentFile.guid))
                                {
                                    MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                                        string.Format("Updating rating for file \"{0}\" from {1} to {2}", 
                                        currentFile.file, plexDbRating, plexFileRatng));

                                    bwProcess.ReportProgress(-2);

                                    // Update a rating entry
                                    sql = @"UPDATE metadata_item_settings SET rating = {0} WHERE account_id = {1} AND guid = '{2}'";

                                    sql = string.Format(sql, plexFileRatng, account.id, currentFile.guid);
                                }
                                else
                                {
                                    MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Information,
                                        string.Format("Creating rating for file \"{0}\", rating {1}",
                                        currentFile.file, plexFileRatng));

                                    bwProcess.ReportProgress(-3);

                                    // Create a rating entry
                                    sql = @"
INSERT INTO metadata_item_settings ([account_id], [guid], [rating], [view_offset], [view_count], [last_viewed_at], [created_at], [updated_at])
VALUES({0}, '{1}', {2}, NULL, 0, NULL, DATE('now'), DATE('now'));";

                                    sql = string.Format(sql, account.id, currentFile.guid, plexFileRatng);
                                }

                                ExecutePlexSql(sql);
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

        public int? PlexRatingFromFile(string file)
        {
            try
            {
                uint? fileRating = null;
                ShellFile so = ShellFile.FromFilePath(file);

                if (so.Properties.System.Rating.Value != null)
                    fileRating = (uint)so.Properties.System.Rating.Value;

                if (fileRating == null)
                    return null;

                if (fileRating < 1)
                    return null;
                else if (fileRating < 13)
                    return 2;
                else if (fileRating < 38)
                    return 4;
                else if (fileRating < 63)
                    return 6;
                else if (fileRating < 87)
                    return 8;
                else
                    return 10;
            }
            catch (Exception ex)
            {
                MessageManager.Instance.ExceptionWrite(this, ex);
                return null;
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

            rating = (int?)ReadPlexValue(sql);

            return rating;
        }

        public bool PlexDatabaseRatingExists(Int64 accountId, string guid)
        {
            bool recordExists = false;

            // the rating(s) of a given file
            string sql = @"SELECT * FROM metadata_item_settings WHERE account_id = {0} AND guid = '{1}';";

            sql = string.Format(sql, accountId, guid);

            MessageManager.Instance.MessageWrite(this, MessageItem.MessageLevel.Debug,
                string.Format("Executing SQL{0}{1}",
                Environment.NewLine, sql));

            using (SQLiteCommand command = new SQLiteCommand(sql, m_DbConnection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        recordExists = true;
                    }
                }
            }

            return recordExists;
        }

        private void ReadFields(SQLiteDataReader reader, object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                int fieldPos = reader.GetOrdinal(name);

                if (fieldPos >= 0 && !reader.IsDBNull(fieldPos))
                {
                    dynamic fieldValue = reader[name];
                    property.SetValue(obj, fieldValue, null);
                }
            }
        }

        private void cmdOptions_Click(object sender, EventArgs e)
        {
            cmdOptions.Enabled = false;
            bwProcess.CancelAsync();

            while (bwProcess.IsBusy)
                Application.DoEvents();

            using (Options frm = new Options(m_ItunesPlaylists))
            {
                frm.ShowDialog(this);
            }

            cmdOptions.Enabled = true;
            StartProcessing();
        }
    }

    public class ItunesPlaylist
    {
        public string PlaylistPersistentID { get; set; }
        public string ParentPersistentID { get; set; }
        public string Name { get; set; }
        public string FullPlaylistName { get; set; }
        public List<ItunesTrack> Tracks { get; set; }

        public ItunesPlaylist()
        {
            Tracks = new List<ItunesTrack>();
        }
    }

    public class ItunesTrack
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public string ProperLocation
        {
            get
            {
                string decoded = Uri.UnescapeDataString(Location);
                decoded = decoded.Substring(0, 16) == "file://localhost" ? "\\" + decoded.Substring(17) : decoded;
                decoded = decoded.Replace('/', '\\');
                return decoded;
            }
        }
    }

    public class PlexRatingsData
    {
        public string guid { get; set; }
        public string file { get; set; }
        public double rating { get; set; }
    }

    public class PlexTableAccounts
    {
        public Int64 id { get; set; }
        public string name { get; set; }
        public string hashed_password { get; set; }
        public string salt { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string default_audio_language { get; set; }
        public string default_subtitle_language { get; set; }
        public bool auto_select_subtitle { get; set; }
        public bool auto_select_audio { get; set; }
    }

    public class PlexTableMediaParts
    {
        public Int64 id { get; set; }
        public Int64 media_item_id { get; set; }
        public Int64 directory_id { get; set; }
        public string hash { get; set; }
        public string open_subtitle_hash { get; set; }
        public string file { get; set; }
        public Int64 index { get; set; }
        public Int64 size { get; set; }
        public Int64 duration { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
        public string extra_data { get; set; }
    }

    public class PlexTableMediaItems
    {
        public Int64 id { get; set; }
        public Int64 library_section_id { get; set; }
        public Int64 section_location_id { get; set; }
        public Int64 metadata_item_id { get; set; }
        public Int64 type_id { get; set; }
        public Int64 width { get; set; }
        public Int64 height { get; set; }
        public Int64 size { get; set; }
        public Int64 duration { get; set; }
        public Int64 bitrate { get; set; }
        public string container { get; set; }
        public string video_codec { get; set; }
        public string audio_codec { get; set; }
        public double display_aspect_ratio { get; set; }
        public double frames_per_second { get; set; }
        public Int64 audio_channels { get; set; }
        public bool interlaced { get; set; }
        public string source { get; set; }
        public string hints { get; set; }
        public Int64 display_offset { get; set; }
        public string settings { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool optimized_for_streaming { get; set; }
        public DateTime deleted_at { get; set; }
        public Int64 media_analysis_version { get; set; }
        public double sample_aspect_ratio { get; set; }
        public string extra_data { get; set; }
    }

    public class PlexTableMetadataItems
    {
        public Int64 id { get; set; }
        public Int64 library_section_id { get; set; }
        public Int64 parent_id { get; set; }
        public Int64 metadata_type { get; set; }
        public string guid { get; set; }
        public Int64 media_item_count { get; set; }
        public string title { get; set; }
        public string title_sort { get; set; }
        public string original_title { get; set; }
        public string studio { get; set; }
        public double rating { get; set; }
        public Int64 rating_count { get; set; }
        public string tagline { get; set; }
        public string summary { get; set; }
        public string trivia { get; set; }
        public string quotes { get; set; }
        public string content_rating { get; set; }
        public Int64 content_rating_age { get; set; }
        public Int64 index { get; set; }
        public Int64 absolute_index { get; set; }
        public Int64 duration { get; set; }
        public string user_thumb_url { get; set; }
        public string user_art_url { get; set; }
        public string user_banner_url { get; set; }
        public string user_music_url { get; set; }
        public string user_fields { get; set; }
        public string tags_genre { get; set; }
        public string tags_collection { get; set; }
        public string tags_director { get; set; }
        public string tags_writer { get; set; }
        public string tags_star { get; set; }
        public DateTime originally_available_at { get; set; }
        public DateTime available_at { get; set; }
        public DateTime expires_at { get; set; }
        public DateTime refreshed_at { get; set; }
        public Int64 year { get; set; }
        public DateTime added_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
        public string tags_country { get; set; }
        public string extra_data { get; set; }
    }

    public class PlexTableMetadataItemSettings
    {
        public Int64 id { get; set; }
        public Int64 account_id { get; set; }
        public string guid { get; set; }
        public double rating { get; set; }
        public Int64 view_offset { get; set; }
        public Int64 view_count { get; set; }
        public DateTime last_viewed_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class PlexTableDirectories
    {
        public Int64 id { get; set; }
        public Int64 library_section_id { get; set; }
        public Int64 parent_directory_id { get; set; }
        public string path { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
    }

    public class PlexTableSectionLocations
    {
        public Int64 id { get; set; }
        public Int64 library_section_id { get; set; }
        public string root_path { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime scanned_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class PlexTableLibrarySections
    {
        public Int64 id { get; set; }
        public Int64 library_id { get; set; }
        public string name { get; set; }
        public string name_sort { get; set; }
        public Int64 section_type { get; set; }
        public string language { get; set; }
        public string agent { get; set; }
        public string scanner { get; set; }
        public string user_thumb_url { get; set; }
        public string user_art_url { get; set; }
        public string user_theme_music_url { get; set; }
        public bool IsPublic { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime scanned_at { get; set; }
        public bool display_secondary_level { get; set; }
        public string user_fields { get; set; }
        public string query_xml { get; set; }
        public Int64 query_type { get; set; }
    }
}
