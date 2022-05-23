using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DS.Library.MessageHandling;
using Sentry;

namespace DS.PlexRatingsSync
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();

      Application.SetCompatibleTextRenderingDefault(false);

      using (SentrySdk.Init(o =>
      {
        o.Dsn = "https://87dcfda6c7314f2585eae326392dc543@o338272.ingest.sentry.io/6434180";
        
        // When configuring for the first time, to see what the SDK is doing:
        //o.Debug = true;
        
        // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
        // We recommend adjusting this value in production.
        //o.TracesSampleRate = 1.0;
      }))
      {
        // App code goes here. Dispose the SDK before exiting to flush events.
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

        // Force the current directory to be the directory of the exe
        Directory.SetCurrentDirectory(Path.GetDirectoryName(assembly.Location));

        // Setup error handling
        string logPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Logs");

        if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

        foreach (FileInfo file in new DirectoryInfo(logPath).GetFiles())
          file.Delete();

        MessageManager.Instance.LoggingFolder = logPath;

        MessageManager.Instance.InitialiseServiceLogging();

        // Always do debug logging in this tool
        MessageManager.Instance.LoggingLevelSet(MessageItem.MessageLevel.Debug);

        System.Reflection.AssemblyName assemblyName = assembly.GetName();

        if (args.Any(a => a.Equals("/options", StringComparison.OrdinalIgnoreCase))
          || args.Any(a => a.Equals("/settings", StringComparison.OrdinalIgnoreCase)))
        {
          Settings.GetPreferences();

          using (Options2 frm = new Options2())
            frm.ShowDialog();
        }
        else
          Application.Run(new Main());
      }
    }
  }
}
