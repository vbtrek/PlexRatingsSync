using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace DS.PlexRatingsSync
{
  public static class Settings
  {
    public static string PlexUsername { get; set; }
    public static string PlexPassword { get; set; }
    public static string PlexUri { get; set; }

    public static bool SyncRatings { get; set; }
    public static SyncSources SyncSource { get; set; }
    public static SyncModes SyncHandling { get; set; }
    public static ClashWinner ClashHandling { get; set; }

    public static void GetPreferences()
    {
      Registry.CurrentUser.CreateSubKey(@"Software\Derek Smith\PlexRatingsSync");

      PlexUsername = GetOption("PlexUsername", string.Empty);
      PlexPassword = GetOption("PlexPassword", string.Empty);
      PlexUri = GetOption("PlexUri", string.Empty);

      SyncRatings = bool.Parse(GetOption("SyncRatings", "false"));
      SyncSource = (SyncSources)Enum.Parse(typeof(SyncSources), GetOption("SyncSource", "0"));
      SyncHandling = (SyncModes)Enum.Parse(typeof(SyncModes), GetOption("SyncHandling", "3"));
      ClashHandling = (ClashWinner)Enum.Parse(typeof(ClashWinner), GetOption("ClashHandling", "0"));
    }

    private static string GetOption(string optionName, string defaultValue)
    {
      if (string.IsNullOrWhiteSpace(optionName))
        return string.Empty;

      if (defaultValue == null)
        defaultValue = string.Empty;

      return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", optionName, defaultValue).ToString();
    }

    public static void SavePreferences()
    {
      if (!PlexUri.EndsWith("/"))
        PlexUri += "/";

      SaveOption("PlexUsername", PlexUsername);
      SaveOption("PlexPassword", PlexPassword);
      SaveOption("PlexUri", PlexUri);

      SaveOption("SyncRatings", SyncRatings);
      SaveOption("SyncSource", SyncSource);
      SaveOption("SyncHandling", SyncHandling);
      SaveOption("ClashHandling", ClashHandling);
    }

    private static void SaveOption(string optionName, object optionValue)
    {
      if (optionValue != null)
        Registry.SetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", optionName, optionValue);
    }
  }
}
