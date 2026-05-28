using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    internal static class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const string MessageName = "AzeuServices_RequestOpenSettings";

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            // 1. Cleanup Websocket Cache on Startup
            CleanupWebsocketCache();

            IntPtr existingWindowHandle = FindWindow(null, "Azeu Services V1");
            if (existingWindowHandle != IntPtr.Zero)
            {
                uint msg = RegisterWindowMessage(MessageName);
                PostMessage(existingWindowHandle, msg, IntPtr.Zero, IntPtr.Zero);
                return;
            }

            AppSettings settings = AppSettings.Load();

            if (settings.EnableNoSmoking)
            {
                double uptimeSeconds = TimeSpan.FromMilliseconds(Environment.TickCount64).TotalSeconds;
                if (uptimeSeconds < 60)
                {
                    NoSmokingForm warning = new NoSmokingForm(settings);
                    warning.Show();
                }
            }

            Application.Run(new Form1());
        }

        private static void CleanupWebsocketCache()
        {
            try
            {
                string cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websocket-cache");
                if (Directory.Exists(cachePath))
                {
                    Directory.Delete(cachePath, true);
                }
                Directory.CreateDirectory(cachePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cache Cleanup Error: " + ex.Message);
            }
        }
    }
}