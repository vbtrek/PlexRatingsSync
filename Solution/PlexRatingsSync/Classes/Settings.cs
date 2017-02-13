using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace DS.PlexRatingsSync
{
    public static class Settings
    {
        public static string PlexDatabase { get; set; }
        public static string PlexAccount { get; set; }
        public static bool SyncRatings { get; set; }
        public static string ItunesLibraryPath { get; set; }
        public static bool SyncPlaylists { get; set; }
        public static bool RemoveEmptyPlaylists { get; set; }
        public static List<string> ChosenPlaylists { get; set; }

        public static void GetPreferences()
        {
            Registry.CurrentUser.CreateSubKey(@"Software\Derek Smith\PlexRatingsSync");

            PlexDatabase = 
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "PlexDatabase", "").ToString();

            PlexAccount = 
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "PlexAccount", "").ToString();

            SyncRatings = bool.Parse(
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "SyncRatings", "false").ToString());

            ItunesLibraryPath = 
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "ItunesLibraryPath", "").ToString();

            SyncPlaylists = 
                bool.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "SyncPlaylists", "false").ToString());

            RemoveEmptyPlaylists = 
                bool.Parse(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "RemoveEmptyPlaylists", "true").ToString());

            ChosenPlaylists = 
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "Playlists", String.Empty)
                .ToString().Split(',').ToList();
        }

        public static void SavePreferences()
        {
            SaveOption("PlexDatabase", PlexDatabase);
            SaveOption("PlexAccount", PlexAccount);

            SaveOption("SyncRatings", SyncRatings);
            SaveOption("ItunesLibraryPath", ItunesLibraryPath);

            SaveOption("SyncPlaylists", SyncPlaylists);
            SaveOption("RemoveEmptyPlaylists", RemoveEmptyPlaylists);

            string playlistString = String.Join(",", ChosenPlaylists.ToArray<string>());

            SaveOption("Playlists", playlistString);
        }

        private static void SaveOption(string optionName, object optionValue)
        {
            if (optionValue != null)
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", optionName, optionValue);
        }

        public static int PlexAccountId
        {
            get
            {
                if (PlexAccount == null) return 0;
                if (PlexAccount.Contains(" - "))
                {
                    int id;
                    int.TryParse(PlexAccount.Substring(0, PlexAccount.IndexOf(" - ")), out id);
                    return id;
                }

                return 0;
            }
        }
    }
}
