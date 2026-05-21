using System;
using System.Diagnostics;
using System.Runtime.InteropServices; // Added
using System.Windows.Forms;

namespace AzeuServices_V1
{
    internal static class Program
    {
        // Import User32 to bring the existing window to front
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        [STAThread]
        static void Main(string[] args) // Add string[] args here
        {
            ApplicationConfiguration.Initialize();

            // 1. Single Instance Logic - Be careful not to kill yourself
            string procName = Process.GetCurrentProcess().ProcessName;
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(procName);

            foreach (Process p in processes)
            {
                if (p.Id != current.Id)
                {
                    // If another instance is already running, focus it and quit this one
                    IntPtr handle = p.MainWindowHandle;
                    ShowWindow(handle, 9); // 9 = SW_RESTORE
                    SetForegroundWindow(handle);
                    return;
                }
            }

            // 2. Load Settings
            AppSettings settings = AppSettings.Load();

            // 3. No Smoking Logic
            if (settings.EnableNoSmoking)
            {
                double uptimeSeconds = TimeSpan.FromMilliseconds(Environment.TickCount64).TotalSeconds;
                // Only show if the PC was turned on less than 60 seconds ago
                if (uptimeSeconds < 60)
                {
                    NoSmokingForm warning = new NoSmokingForm(settings);
                    warning.Show();
                    // We use Show() so Form1 can load in the background
                }
            }

            // 4. Run the main application
            Application.Run(new Form1());
        }
    }
}