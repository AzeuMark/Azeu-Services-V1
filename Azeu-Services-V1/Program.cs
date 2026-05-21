using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    internal static class Program
    {
        // Import to find the hidden window by its name
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // Unique string to identify our custom message
        public const string MessageName = "AzeuServices_RequestOpenSettings";

        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            // 1. Try to find the window of the already running instance
            // We use the "Text" property of Form1 defined in your Designer ("Azeu Services V1")
            IntPtr existingWindowHandle = FindWindow(null, "Azeu Services V1");

            if (existingWindowHandle != IntPtr.Zero)
            {
                // 2. An instance is already running.
                // Register the same message ID as the first instance.
                uint msg = RegisterWindowMessage(MessageName);

                // 3. Send the message specifically to the existing window.
                PostMessage(existingWindowHandle, msg, IntPtr.Zero, IntPtr.Zero);

                // 4. Close this instance.
                return;
            }

            // 5. No instance found, proceed with normal startup.
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
    }
}