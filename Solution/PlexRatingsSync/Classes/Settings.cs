using System;
using System.Collections.Generic;
using DS.PlexRatingsSync.Classes;
using MailKit.Security;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace DS.PlexRatingsSync
{
  public static class Settings
  {
    public static string PlexUsername { get; set; }
    public static string PlexPassword { get; set; }
    public static string PlexUri { get; set; }
    public static List<PlexFolderMapping> PlexFolderMappings { get; set; }

    public static bool SyncRatings { get; set; }
    public static SyncSources SyncSource { get; set; }
    public static SyncModes SyncHandling { get; set; }
    public static ClashWinner ClashHandling { get; set; }

    public static bool SendEmailSummaryOfUnmatched { get; set; }
    public static string EmailFromName { get; set; }
    public static string EmailFromEmailAddess { get; set; }
    public static string EmailToName { get; set; }
    public static string EmailToEmailAddess { get; set; }
    public static string SmtpServer { get; set; }
    public static int SmtpPort { get; set; }
    public static bool UseSsl { get; set; }
    public static SecureSocketOptions SecurityOption { get; set; }
    public static string SmtpUsername { get; set; }
    public static string SmtpPassword { get; set; }

    public static void GetPreferences()
    {
      Registry.CurrentUser.CreateSubKey(@"Software\Derek Smith\PlexRatingsSync");

      PlexUsername = GetOption("PlexUsername", string.Empty);
      PlexPassword = GetOption("PlexPassword", string.Empty);
      PlexUri = GetOption("PlexUri", string.Empty);
      PlexFolderMappings = JsonConvert.DeserializeObject<List<PlexFolderMapping>>(GetOption("PlexFolderMappings", string.Empty)) ?? new List<PlexFolderMapping>();

      SyncRatings = bool.Parse(GetOption("SyncRatings", "false"));
      SyncSource = (SyncSources)Enum.Parse(typeof(SyncSources), GetOption("SyncSource", "0"));
      SyncHandling = (SyncModes)Enum.Parse(typeof(SyncModes), GetOption("SyncHandling", "3"));
      ClashHandling = (ClashWinner)Enum.Parse(typeof(ClashWinner), GetOption("ClashHandling", "0"));

      SendEmailSummaryOfUnmatched = bool.Parse(GetOption("SendEmailSummaryOfUnmatched", "false"));
      EmailFromName = GetOption("EmailFromName", string.Empty);
      EmailFromEmailAddess = GetOption("EmailFromEmailAddess", string.Empty);
      EmailToName = GetOption("EmailToName", string.Empty);
      EmailToEmailAddess = GetOption("EmailToEmailAddess", string.Empty);
      SmtpServer = GetOption("SmtpServer", string.Empty);
      SmtpPort = int.Parse(GetOption("SmtpPort", "587"));
      UseSsl = bool.Parse(GetOption("UseSsl", "false"));
      SecurityOption = (SecureSocketOptions)Enum.Parse(typeof(SecureSocketOptions), GetOption("SecurityOption", "1"));
      SmtpUsername = GetOption("SmtpUsername", string.Empty);
      SmtpPassword = GetOption("SmtpPassword", string.Empty);
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
      SaveOption("PlexFolderMappings", JsonConvert.SerializeObject(PlexFolderMappings));

      SaveOption("SyncRatings", SyncRatings);
      SaveOption("SyncSource", SyncSource);
      SaveOption("SyncHandling", SyncHandling);
      SaveOption("ClashHandling", ClashHandling);

      SaveOption("SendEmailSummaryOfUnmatched", SendEmailSummaryOfUnmatched);
      SaveOption("EmailFromName", EmailFromName);
      SaveOption("EmailFromEmailAddess", EmailFromEmailAddess);
      SaveOption("EmailToName", EmailToName);
      SaveOption("EmailToEmailAddess", EmailToEmailAddess);
      SaveOption("SmtpServer", SmtpServer);
      SaveOption("SmtpPort", SmtpPort);
      SaveOption("UseSsl", UseSsl);
      SaveOption("SecurityOption", SecurityOption);
      SaveOption("SmtpUsername", SmtpUsername);
      SaveOption("SmtpPassword", SmtpPassword);
    }

    private static void SaveOption(string optionName, object optionValue)
    {
      if (optionValue != null)
        Registry.SetValue(@"HKEY_CURRENT_USER\Software\Derek Smith\PlexRatingsSync", optionName, optionValue);
    }
  }
}
