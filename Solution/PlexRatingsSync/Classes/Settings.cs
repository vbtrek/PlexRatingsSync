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

    public static bool SyncRatings { get; set; }
    public static SyncSources SyncSource { get; set; }
    public static SyncModes SyncHandling { get; set; }
    public static ClashWinner ClashHandling { get; set; }

    public static void GetPreferences()
    {
      Registry.CurrentUser.CreateSubKey(@"Software\Derek Smith\PlexRatingsSync");

      PlexUsername =
          Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "PlexUsername", "").ToString();

      PlexPassword =
          Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "PlexPassword", "").ToString();

      SyncRatings = bool.Parse(
          Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "SyncRatings", "false").ToString());

      var value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "SyncSource", "0").ToString();

      SyncSource = (SyncSources)Enum.Parse(typeof(SyncSources), value);

      value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "SyncHandling", "3").ToString();

      SyncHandling = (SyncModes)Enum.Parse(typeof(SyncModes), value);

      value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", "ClashHandling", "0").ToString();

      ClashHandling = (ClashWinner)Enum.Parse(typeof(ClashWinner), value);
    }

    public static void SavePreferences()
    {
      SaveOption("PlexUsername", PlexUsername);
      SaveOption("PlexPassword", PlexPassword);

      SaveOption("SyncSource", SyncSource);
      SaveOption("SyncRatings", SyncRatings);
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
