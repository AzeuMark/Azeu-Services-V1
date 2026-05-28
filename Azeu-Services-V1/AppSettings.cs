using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public class AppSettings
    {
        private static string RemoteUrl = "https://your-website.com/settings.json";

        // Basics
        public bool ShutdownIfAFK { get; set; } = false;
        public bool MinimizeToTray { get; set; } = false;
        public bool LaunchOnStartup { get; set; } = false;
        public string AdminPassword { get; set; } = "1234";
        public int CountdownMinutes { get; set; } = 1;
        public int CountdownOpacity { get; set; } = 80;
        public bool ShowCountdown { get; set; } = true;
        public bool CountdownTopMost { get; set; } = true;
        public bool EnableOpacity { get; set; } = true;
        public bool ApplicationHighPriority { get; set; } = false;
        public bool AdminShutdown { get; set; } = false;
        public bool ApplicationServiceActive { get; set; } = false;
        public bool IsAppRunningState { get; set; } = false;
        public bool StartInTray { get; set; } = false;

        // Remote Service Settings
        public bool EnableRemoteService { get; set; } = false;
        public string WebSocketUrl { get; set; } = "";
        public string WebSocketToken { get; set; } = "azeu_websocket_token";

        // No Smoking Settings
        public bool EnableNoSmoking { get; set; } = false;
        public string NoSmokingMessage { get; set; } = "NO SMOKING INSIDE THE PISONET";
        public string NoSmokingButtonText { get; set; } = "I Understand";
        public int NoSmokingFontSize { get; set; } = 48;
        public string NoSmokingBgColor { get; set; } = "Black";
        public string NoSmokingTextColor { get; set; } = "White";
        public int NoSmokingButtonRadius { get; set; } = 15;
        public int NoSmokingDuration { get; set; } = 3;
        public string NoSmokingButtonBgColor { get; set; } = "DimGray";
        public string NoSmokingButtonTextColor { get; set; } = "White";
        public string NoSmokingFontFamily { get; set; } = "Arial";
        public int NoSmokingButtonBottomMargin { get; set; } = 100;
        public int NoSmokingButtonWidth { get; set; } = 400;
        public int NoSmokingButtonHeight { get; set; } = 100;
        public int NoSmokingButtonFontSize { get; set; } = 20;
        public string NoSmokingImagePath { get; set; } = "";
        public string NoSmokingImageSizeMode { get; set; } = "Stretch";

        // Desktop Curfew Settings
        public bool LimitDesktopUsage { get; set; } = false;
        public string LimitDesktopHour { get; set; } = "12";
        public string LimitDesktopMin { get; set; } = "00";
        public string LimitDesktopAMPM { get; set; } = "PM";
        public string LimitDesktopHourOpen { get; set; } = "08";
        public string LimitDesktopMinOpen { get; set; } = "00";
        public string LimitDesktopAMPMOpen { get; set; } = "AM";
        public string LimitDesktopAction { get; set; } = "Shutdown";
        public string LimitDesktopImagePath { get; set; } = "";
        public string LimitDesktopImageSizeMode { get; set; } = "Stretch";
        public bool LimitShow5min { get; set; } = false;
        public bool LimitShow10min { get; set; } = false;
        public bool LimitShow30min { get; set; } = false;
        public bool LimitShutdownAfter3Min { get; set; } = false;

        public string LimitMessage { get; set; } = "PISONET IS NOW CLOSED";
        public string LimitFontFamily { get; set; } = "Arial";
        public int LimitFontSize { get; set; } = 48;
        public string LimitBgColor { get; set; } = "Black";
        public string LimitTextColor { get; set; } = "White";
        public bool LimitShowBypassInstructions { get; set; } = false;

        public bool LimitShowReturningTime { get; set; } = true;
        public string LimitReturningFontFamily { get; set; } = "Arial";
        public int LimitReturningFontSize { get; set; } = 24;
        public string LimitReturningTextColor { get; set; } = "White";
        public int LimitReturningBottomMargin { get; set; } = 20;
        public bool LimitShowShutdownCountdown { get; set; } = true;

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? LastBypassDate { get; set; } = null;
        private static string FilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static void Save(AppSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static AppSettings Load()
        {
            AppSettings settings = null;
            if (File.Exists(FilePath))
            {
                try
                {
                    string localJson = File.ReadAllText(FilePath);
                    settings = JsonSerializer.Deserialize<AppSettings>(localJson);
                }
                catch { settings = new AppSettings(); }
            }
            if (settings == null && !string.IsNullOrEmpty(RemoteUrl) && RemoteUrl.StartsWith("http"))
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(2);
                        var task = Task.Run(() => client.GetStringAsync(RemoteUrl));
                        if (task.Wait(2000))
                        {
                            settings = JsonSerializer.Deserialize<AppSettings>(task.Result);
                        }
                    }
                }
                catch { settings = new AppSettings(); }
            }
            if (settings == null) settings = new AppSettings();
            if (settings.CountdownMinutes < 1) settings.CountdownMinutes = 1;
            Save(settings);
            return settings;
        }
    }
}