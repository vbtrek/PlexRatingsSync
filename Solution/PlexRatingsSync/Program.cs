using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DS.MessageHandling;

namespace DS.PlexRatingsSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

            // Force the current directory to be the directory of the exe
            Directory.SetCurrentDirectory(Path.GetDirectoryName(assembly.Location));

            // Setup error handling
            string logPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Logs");
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

            foreach (FileInfo file in new DirectoryInfo(logPath).GetFiles())
            {
                file.Delete();
            }

            MessageManager.Instance.LoggingFolder = logPath;
            MessageManager.Instance.InitialiseServiceLogging();

            // Always do debug logging in this tool
            MessageManager.Instance.LoggingLevelSet(MessageItem.MessageLevel.Debug);

            System.Reflection.AssemblyName assemblyName = assembly.GetName();

            Application.Run(new Main());
        }
    }
}
