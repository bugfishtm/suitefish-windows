using System;
using System.Threading;
using System.Windows.Forms;

namespace suitefish
{
    internal static class Program
    {

        private static readonly string MutexName = "btmx658-suitefish-software-mutex";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create a mutex to check for an existing instance
            using (Mutex mutex = new Mutex(false, MutexName, out bool runningSoftware))
            {
                // If an instance is already running, exit the application
                if (!runningSoftware)
                {
                    MessageBox.Show("Another instance of the application is already running.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.Run(new Interface());
            }
        }
    }
}