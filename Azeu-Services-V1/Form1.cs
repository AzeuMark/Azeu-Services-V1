using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace AzeuServices_V1
{
    public partial class Form1 : Form
    {
        private bool isWidgetManuallyHidden = false;
        private ToolStripMenuItem? trayToggleItem;
        private bool allowVisible = false;
        private int startupGraceSeconds = 10;

        private AppSettings lastSavedSettings = new AppSettings();
        private ActivityMonitor monitor;
        private System.Windows.Forms.Timer uiTimer;
        private NotifyIcon trayIcon;

        private static int startSeconds = 60;
        private int currentSecondsLeft = startSeconds;
        private bool isCountingDown = false;
        private CountdownForm? countdownWindow;

        private LimitNotificationForm? activeLimitWarning = null;
        private bool limitDialogShownToday = false;
        private bool isLoading = true;

        private bool hasShown30mWarning = false;
        private bool hasShown10mWarning = false;
        private bool hasShown5mWarning = false;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint RegisterWindowMessage(string lpString);
        private uint _openRequestMsg;

        private LimitClosedForm? activeLockScreen = null;

        private bool isShutdownInProgress = false;

        public Form1()
        {
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            InitializeComponent();

            _openRequestMsg = RegisterWindowMessage(Program.MessageName);
            var forceHandle = this.Handle;

            SystemEvents.SessionEnding += OnSystemSessionEnding;
            monitor = new ActivityMonitor();
            SetupTrayIcon();

            uiTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            uiTimer.Tick += UiTimer_Tick;

            LoadConfig();
            uiTimer.Start();

            // START REMOTE SERVICE BACKGROUND MANAGER
            RemoteServiceManager.Instance.Start();
        }




        private void OnSystemSessionEnding(object sender, SessionEndingEventArgs e)
        {
            UpdateAppState(false);
            ManageWatchdog(false);
            if (monitor != null) monitor.Stop();
            if (trayIcon != null) trayIcon.Dispose();
            Application.Exit();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible) base.SetVisibleCore(false);
            else base.SetVisibleCore(value);
        }

        private Image? GetImageFromResource(byte[] resourceData)
        {
            using (var ms = new System.IO.MemoryStream(resourceData))
            {
                return Image.FromStream(ms);
            }
        }

        private void CheckDesktopLimit()
        {
            if (!lastSavedSettings.LimitDesktopUsage) return;

            // 1. Skip if bypassed today
            if (lastSavedSettings.LastBypassDate.HasValue && lastSavedSettings.LastBypassDate.Value.Date == DateTime.Today)
            {
                if (activeLockScreen != null) { activeLockScreen.Close(); activeLockScreen = null; }
                return;
            }

            // 2. Parse Closing and Opening Times
            if (!DateTime.TryParse($"{lastSavedSettings.LimitDesktopHour}:{lastSavedSettings.LimitDesktopMin} {lastSavedSettings.LimitDesktopAMPM}", out DateTime curfewStart)) return;
            if (!DateTime.TryParse($"{lastSavedSettings.LimitDesktopHourOpen}:{lastSavedSettings.LimitDesktopMinOpen} {lastSavedSettings.LimitDesktopAMPMOpen}", out DateTime curfewEnd)) return;

            DateTime now = DateTime.Now;
            bool isInsideCurfew = false;

            // 3. Determine if currently in Curfew (Logic for crossing midnight)
            if (curfewStart > curfewEnd)
            {
                isInsideCurfew = (now >= curfewStart || now < curfewEnd);
            }
            else
            {
                isInsideCurfew = (now >= curfewStart && now < curfewEnd);
            }

            // 4. Handle Lock Screen
            if (isInsideCurfew)
            {
                if (activeLockScreen == null || activeLockScreen.IsDisposed)
                {
                    if (lastSavedSettings.LimitDesktopAction == "Shutdown")
                    {
                        PerformShutdown("Curfew Reached");
                    }
                    else
                    {
                        activeLockScreen = new LimitClosedForm(lastSavedSettings, () => PerformShutdown("Desktop Limit Auto-Close"));
                        activeLockScreen.FormClosed += (s, e) => { if (activeLockScreen?.DialogResult == DialogResult.OK) activeLockScreen = null; };
                        activeLockScreen.Show();
                    }
                }
            }
            else
            {
                if (activeLockScreen != null && !activeLockScreen.IsDisposed)
                {
                    activeLockScreen.Close();
                    activeLockScreen = null;
                    ResetWarningFlags();
                }
            }

            // 5. Warning Logic (The Fix)
            if (!isInsideCurfew)
            {
                // Calculate the NEXT occurrence of the curfewStart
                DateTime nextCurfewTrigger = curfewStart;
                if (now > nextCurfewTrigger)
                {
                    nextCurfewTrigger = nextCurfewTrigger.AddDays(1);
                }

                double totalMinutesRemaining = (nextCurfewTrigger - now).TotalMinutes;

                // Trigger warnings based on thresholds
                if (lastSavedSettings.LimitShow30min && totalMinutesRemaining <= 30 && !hasShown30mWarning)
                {
                    hasShown30mWarning = true;
                    ShowLimitWarning($"PC will close in 30 minutes ({lastSavedSettings.LimitDesktopHour}:{lastSavedSettings.LimitDesktopMin} {lastSavedSettings.LimitDesktopAMPM})");
                }
                else if (lastSavedSettings.LimitShow10min && totalMinutesRemaining <= 10 && !hasShown10mWarning)
                {
                    hasShown10mWarning = true;
                    ShowLimitWarning($"PC will close in 10 minutes ({lastSavedSettings.LimitDesktopHour}:{lastSavedSettings.LimitDesktopMin} {lastSavedSettings.LimitDesktopAMPM})");
                }
                else if (lastSavedSettings.LimitShow5min && totalMinutesRemaining <= 5 && !hasShown5mWarning)
                {
                    hasShown5mWarning = true;
                    ShowLimitWarning($"PC will close in 5 minutes ({lastSavedSettings.LimitDesktopHour}:{lastSavedSettings.LimitDesktopMin} {lastSavedSettings.LimitDesktopAMPM})");
                }
            }
        }

        private void ResetWarningFlags()
        {
            hasShown30mWarning = false;
            hasShown10mWarning = false;
            hasShown5mWarning = false;
            limitDialogShownToday = false;
        }

        private void ShowLimitWarning(string msg)
        {
            // 1. Requirement: Do not show if the main settings window is visible
            if (this.Visible) return;

            // Close existing warning to prevent multiple stacks
            if (activeLimitWarning != null && !activeLimitWarning.IsDisposed)
            {
                activeLimitWarning.Close();
            }

            // 2. Requirement: Position above Countdown HUD
            // Start with the standard bottom position
            int xPos = Screen.PrimaryScreen.WorkingArea.Right - 260;
            int yPos = Screen.PrimaryScreen.WorkingArea.Bottom;

            // If the Countdown HUD exists and is showing, move our target Y coordinate to its TOP
            if (countdownWindow != null && !countdownWindow.IsDisposed && countdownWindow.Visible)
            {
                yPos = countdownWindow.Top;
            }

            Point spawnLoc = new Point(xPos, yPos);

            activeLimitWarning = new LimitNotificationForm(msg, spawnLoc);
            activeLimitWarning.FormClosed += (s, e) => activeLimitWarning = null;
            activeLimitWarning.Show();
        }
        private void SetupTrayIcon()
        {
            trayIcon = new NotifyIcon() { Icon = SystemIcons.Application, Visible = true, Text = "Azeu Services V1" };
            trayIcon.DoubleClick += (s, e) => TryOpenFromTray();
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Open Settings", GetImageFromResource(Properties.Resources.open_icon), (s, e) => TryOpenFromTray());
            trayToggleItem = new ToolStripMenuItem("Hide Widget", GetImageFromResource(Properties.Resources.hide_icon), (s, e) => ToggleWidgetManual());
            menu.Items.Add(trayToggleItem);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit System", GetImageFromResource(Properties.Resources.exit_icon), (s, e) => TryExitApp());
            trayIcon.ContextMenuStrip = menu;
        }

        private void ToggleWidgetManual()
        {
            isWidgetManuallyHidden = !isWidgetManuallyHidden;
            if (trayToggleItem != null)
            {
                trayToggleItem.Text = isWidgetManuallyHidden ? "Show Widget" : "Hide Widget";
                trayToggleItem.Image = GetImageFromResource(isWidgetManuallyHidden ? Properties.Resources.show_icon : Properties.Resources.hide_icon);
            }
            if (countdownWindow != null && !countdownWindow.IsDisposed) countdownWindow.UpdateMenuUI(isWidgetManuallyHidden);
            ManageCountdownLogic();
        }

        private void UiTimer_Tick(object? sender, EventArgs e)
        {
            monitor.Update();
            kboardStatusLabel.Text = monitor.IsKbAfk ? "Keyboard: AFK" : "Keyboard: Active";
            mouseStatusLabel.Text = monitor.IsMouseAfk ? "Mouse: AFK" : "Mouse: Active";
            if (monitor.IsSuspicious) { suspiciousKeysLabel.Text = "Status: Suspicious"; suspiciousKeysLabel.ForeColor = Color.Red; }
            else { suspiciousKeysLabel.Text = "Status: Normal"; suspiciousKeysLabel.ForeColor = Color.Black; }
            CheckDesktopLimit();
            ManageCountdownLogic();
            UpdateSettingsStatus();
        }

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            if (newPasswordTextbox.Text.Length > 0)
            {
                if (currentPasswordTextbox.Text != lastSavedSettings.AdminPassword) { MessageBox.Show("Current password incorrect."); return; }
                if (newPasswordTextbox.Text != confirmPasswordTextbox.Text) { MessageBox.Show("Passwords match error."); return; }
            }
            SaveConfig();
            currentPasswordTextbox.Clear(); newPasswordTextbox.Clear(); confirmPasswordTextbox.Clear();
            UpdateSettingsStatus();
            MessageBox.Show("Settings saved successfully!");
        }

        private void TryOpenFromTray()
        {
            if (!this.IsHandleCreated) return;

            // If the window is already visible, just bring it to the front
            if (allowVisible)
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.WindowState = FormWindowState.Normal;

                this.Show();
                this.Activate();
                this.BringToFront();
                return;
            }

            // Try to authenticate. PasswordDialog will prevent multiple prompts itself.
            if (PasswordDialog.Authenticate())
            {
                allowVisible = true;
                this.Opacity = 1.0;
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
                this.Show();
                this.Activate();
                this.BringToFront();

                // Restore tray icon visibility when opening settings
                if (trayIcon != null) trayIcon.Visible = true;

                isWidgetManuallyHidden = false;

                if (trayToggleItem != null)
                {
                    trayToggleItem.Text = "Hide Widget";
                    trayToggleItem.Image = GetImageFromResource(Properties.Resources.hide_icon);
                }

                if (countdownWindow != null)
                {
                    countdownWindow.AllowClose = true;
                    countdownWindow.Close();
                    countdownWindow = null;
                }
            }
        }

        private void ExecuteSilentCommand(string command, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(command, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Command Error: " + ex.Message);
            }
        }

        private void TryExitApp()
        {
            if (PasswordDialog.Authenticate())
            {
                // 1. Tell the Watchdog to stop and wait for it
                ManageWatchdog(false);

                // 2. Small delay to ensure Windows handles the process termination
                System.Threading.Thread.Sleep(500);

                // 3. Clean up the App's tray icon and resources
                UpdateAppState(false);
                if (monitor != null) monitor.Stop();

                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }

                // 4. Force terminate the application immediately.
                // This stops all threads so the watchdog doesn't see a "zombie" process.
                Environment.Exit(0);
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Check if changes exist by looking at the status label
                if (settingStatusLabel.Text == "Settings not saved")
                {
                    DialogResult result = MessageBox.Show("Save changes before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Trigger the save button logic
                        saveSettingsBtn_Click(this, EventArgs.Empty);

                        // If saving failed (due to validation/password), cancel the close
                        if (settingStatusLabel.Text == "Settings not saved")
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (result == DialogResult.No)
                    {
                        // REVERT: User clicked No. Reload settings from disk to reset UI and logic.
                        LoadConfig();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        // Abort closing and stay on the settings screen
                        e.Cancel = true;
                        return;
                    }
                }

                // --- CLOSING / HIDING LOGIC ---

                // Determine if we should minimize to tray normally
                if (minimizeTrayCheckbox.Checked)
                {
                    e.Cancel = true;
                    this.allowVisible = false;
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();

                    if (trayIcon != null) trayIcon.Visible = true;
                    ManageCountdownLogic();
                    return;
                }

                // If Minimize to Tray is OFF, check if AFK is active to determine if we should "Stealth"
                if (shutdownAFKCheckbox.Checked)
                {
                    e.Cancel = true;
                    this.allowVisible = false;
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();

                    // Stealth mode: Keep alive but hide tray icon
                    if (trayIcon != null) trayIcon.Visible = false;

                    ManageCountdownLogic();
                }
                else
                {
                    // AFK is OFF and Minimize to Tray is OFF: Exit completely
                    string stopSignalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "watchdog.stop");
                    try { File.WriteAllText(stopSignalPath, "STOP"); } catch { }

                    ManageWatchdog(false);
                    UpdateAppState(false);
                    if (monitor != null) monitor.Stop();

                    if (trayIcon != null)
                    {
                        trayIcon.Visible = false;
                        trayIcon.Dispose();
                    }

                    SystemEvents.SessionEnding -= OnSystemSessionEnding;
                    Environment.Exit(0);
                }
            }
            else
            {
                // Handle OS Shutdown or Task Manager kills
                string stopSignalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "watchdog.stop");
                try { File.WriteAllText(stopSignalPath, "STOP"); } catch { }

                ManageWatchdog(false);
                UpdateAppState(false);
                if (monitor != null) monitor.Stop();
                if (trayIcon != null) trayIcon.Dispose();
                SystemEvents.SessionEnding -= OnSystemSessionEnding;
            }
        }

        private void SaveConfig()
        {
            int.TryParse(countdownTextbox.Text, out int minutes); if (minutes < 1) minutes = 1;
            int.TryParse(countdownOpacityTextbox.Text, out int opacity); if (opacity < 10) opacity = 10;
            string finalPassword = string.IsNullOrEmpty(newPasswordTextbox.Text) ? lastSavedSettings.AdminPassword : newPasswordTextbox.Text;

            var newSettings = new AppSettings
            {
                ShutdownIfAFK = shutdownAFKCheckbox.Checked,
                MinimizeToTray = minimizeTrayCheckbox.Checked,
                LaunchOnStartup = startupCheckbox.Checked,
                AdminPassword = finalPassword,
                CountdownMinutes = minutes,
                CountdownOpacity = opacity,
                ShowCountdown = showCountdownCheckbox.Checked,
                CountdownTopMost = countdownTopMostCheckbox.Checked,
                EnableOpacity = countdownOpacityCheckbox.Checked,
                ApplicationHighPriority = applicationHighPriorityCheckbox.Checked,
                AdminShutdown = adminShutdownCheckbox.Checked,
                ApplicationServiceActive = applicationServiceCheckbox.Checked,
                IsAppRunningState = true,

                // Remote Settings
                EnableRemoteService = remoteServiceCheckbox.Checked,
                WebSocketUrl = lastSavedSettings.WebSocketUrl,
                WebSocketToken = lastSavedSettings.WebSocketToken,

                EnableNoSmoking = noSmokingDialogCheckbox.Checked,
                NoSmokingMessage = lastSavedSettings.NoSmokingMessage,
                NoSmokingButtonText = lastSavedSettings.NoSmokingButtonText,
                NoSmokingFontSize = lastSavedSettings.NoSmokingFontSize,
                NoSmokingFontFamily = lastSavedSettings.NoSmokingFontFamily,
                NoSmokingBgColor = lastSavedSettings.NoSmokingBgColor,
                NoSmokingTextColor = lastSavedSettings.NoSmokingTextColor,
                NoSmokingButtonBgColor = lastSavedSettings.NoSmokingButtonBgColor,
                NoSmokingButtonTextColor = lastSavedSettings.NoSmokingButtonTextColor,
                NoSmokingButtonRadius = lastSavedSettings.NoSmokingButtonRadius,
                NoSmokingDuration = lastSavedSettings.NoSmokingDuration,
                NoSmokingImagePath = lastSavedSettings.NoSmokingImagePath,
                NoSmokingImageSizeMode = lastSavedSettings.NoSmokingImageSizeMode,

                LimitDesktopUsage = limitDesktopUsageCheckbox.Checked,
                LimitDesktopHour = limitDesktopHourTextbox.Text,
                LimitDesktopMin = limitDesktopMinTextbox.Text,
                LimitDesktopAMPM = limitDesktopAMorPMComboBox.Text,
                LimitDesktopHourOpen = limitDesktopHourOpenTextbox.Text,
                LimitDesktopMinOpen = limitDesktopMinOpenTextbox.Text,
                LimitDesktopAMPMOpen = limitDesktopOpenAMorPMComboBox.Text,

                LimitDesktopAction = limitDesktopActionComboBox.Text,
                LimitDesktopImagePath = limitDesktopImagePathTexbox.Text,
                LimitShow5min = limitDesktopShowDialog5minCheckbox.Checked,
                LimitShow10min = limitDesktopShowDialog10minCheckbox.Checked,
                LimitShow30min = limitDesktopShowDialog30minCheckbox.Checked,
                LimitShutdownAfter3Min = limitDesktopShutdownAfter3Minutes.Checked,

                LimitMessage = lastSavedSettings.LimitMessage,
                LimitFontFamily = lastSavedSettings.LimitFontFamily,
                LimitFontSize = lastSavedSettings.LimitFontSize,
                LimitBgColor = lastSavedSettings.LimitBgColor,
                LimitTextColor = lastSavedSettings.LimitTextColor,
                LimitShowBypassInstructions = lastSavedSettings.LimitShowBypassInstructions,
                LimitShowReturningTime = lastSavedSettings.LimitShowReturningTime,
                LimitReturningFontFamily = lastSavedSettings.LimitReturningFontFamily,
                LimitReturningFontSize = lastSavedSettings.LimitReturningFontSize,
                LimitReturningTextColor = lastSavedSettings.LimitReturningTextColor,
                LimitReturningBottomMargin = lastSavedSettings.LimitReturningBottomMargin,
                LimitShowShutdownCountdown = lastSavedSettings.LimitShowShutdownCountdown
            };

            AppSettings.Save(newSettings);
            lastSavedSettings = newSettings;

            startSeconds = minutes * 60;
            currentSecondsLeft = startSeconds;
            SetStartup(newSettings.LaunchOnStartup);
            ApplyHighPriorityLogic(newSettings.ApplicationHighPriority);
            ManageWatchdog(newSettings.ApplicationServiceActive);

            // Apply Remote Service Action
            if (newSettings.EnableRemoteService) RemoteServiceManager.Instance.Start();
            else RemoteServiceManager.Instance.Stop();

            ResetWarningFlags();
            UpdateSettingsStatus();
            RefreshUIEnableState(); // Force UI Sync
            MessageBox.Show("Settings saved successfully!");
        }

        private void LoadConfig()
        {
            isLoading = true;
            try
            {
                lastSavedSettings = AppSettings.Load();
                startSeconds = lastSavedSettings.CountdownMinutes * 60;
                currentSecondsLeft = startSeconds;
                SetupComboBoxItems();

                // Basic Settings
                countdownTextbox.Text = lastSavedSettings.CountdownMinutes.ToString();
                countdownOpacityTextbox.Text = lastSavedSettings.CountdownOpacity.ToString();
                shutdownAFKCheckbox.Checked = lastSavedSettings.ShutdownIfAFK;
                minimizeTrayCheckbox.Checked = lastSavedSettings.MinimizeToTray;
                startupCheckbox.Checked = lastSavedSettings.LaunchOnStartup;
                showCountdownCheckbox.Checked = lastSavedSettings.ShowCountdown;
                countdownTopMostCheckbox.Checked = lastSavedSettings.CountdownTopMost;
                countdownOpacityCheckbox.Checked = lastSavedSettings.EnableOpacity;
                applicationHighPriorityCheckbox.Checked = lastSavedSettings.ApplicationHighPriority;
                adminShutdownCheckbox.Checked = lastSavedSettings.AdminShutdown;
                applicationServiceCheckbox.Checked = lastSavedSettings.ApplicationServiceActive;

                // No Smoking
                noSmokingDialogCheckbox.Checked = lastSavedSettings.EnableNoSmoking;

                // Remote Service (Crucial)
                remoteServiceCheckbox.Checked = lastSavedSettings.EnableRemoteService;

                // Desktop Curfew (Closing)
                limitDesktopUsageCheckbox.Checked = lastSavedSettings.LimitDesktopUsage;
                limitDesktopHourTextbox.Text = lastSavedSettings.LimitDesktopHour;
                limitDesktopMinTextbox.Text = lastSavedSettings.LimitDesktopMin;
                limitDesktopAMorPMComboBox.Text = lastSavedSettings.LimitDesktopAMPM;

                // Desktop Curfew (Opening)
                limitDesktopHourOpenTextbox.Text = lastSavedSettings.LimitDesktopHourOpen;
                limitDesktopMinOpenTextbox.Text = lastSavedSettings.LimitDesktopMinOpen;
                limitDesktopOpenAMorPMComboBox.Text = lastSavedSettings.LimitDesktopAMPMOpen;

                // Actions & Warnings
                limitDesktopActionComboBox.Text = lastSavedSettings.LimitDesktopAction;
                limitDesktopImagePathTexbox.Text = lastSavedSettings.LimitDesktopImagePath;
                limitDesktopShowDialog5minCheckbox.Checked = lastSavedSettings.LimitShow5min;
                limitDesktopShowDialog10minCheckbox.Checked = lastSavedSettings.LimitShow10min;
                limitDesktopShowDialog30minCheckbox.Checked = lastSavedSettings.LimitShow30min;
                limitDesktopShutdownAfter3Minutes.Checked = lastSavedSettings.LimitShutdownAfter3Min;

                // Final UI Sync
                RefreshUIEnableState();
                UpdateSettingsStatus();
            }
            catch { }
            finally { isLoading = false; }
        }

        private void SetupComboBoxItems()
        {
            if (limitDesktopAMorPMComboBox.Items.Count == 0) limitDesktopAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            if (limitDesktopOpenAMorPMComboBox.Items.Count == 0) limitDesktopOpenAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            if (limitDesktopActionComboBox.Items.Count == 0) limitDesktopActionComboBox.Items.AddRange(new object[] { "Shutdown", "Show Image Dialog" });
        }

        private void SetStartup(bool start)
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (start) rk.SetValue("AzeuServicesV1", $"\"{Application.ExecutablePath}\" --startup");
                    else rk.DeleteValue("AzeuServicesV1", false);
                }
            }
            catch { }
        }

        private void UpdateSettingsStatus()
        {
            if (isLoading) return;

            bool hasChanges = false;

            // 1. AFK & Basic Settings Comparison
            if (shutdownAFKCheckbox.Checked != lastSavedSettings.ShutdownIfAFK) hasChanges = true;
            if (countdownTextbox.Text != lastSavedSettings.CountdownMinutes.ToString()) hasChanges = true;
            if (showCountdownCheckbox.Checked != lastSavedSettings.ShowCountdown) hasChanges = true;
            if (countdownTopMostCheckbox.Checked != lastSavedSettings.CountdownTopMost) hasChanges = true;
            if (countdownOpacityCheckbox.Checked != lastSavedSettings.EnableOpacity) hasChanges = true;
            if (countdownOpacityTextbox.Text != lastSavedSettings.CountdownOpacity.ToString()) hasChanges = true;

            // 2. No Smoking Comparison
            if (noSmokingDialogCheckbox.Checked != lastSavedSettings.EnableNoSmoking) hasChanges = true;

            // 3. Remote Service Comparison (New)
            if (remoteServiceCheckbox.Checked != lastSavedSettings.EnableRemoteService) hasChanges = true;

            // 4. Curfew Schedule Comparison
            if (limitDesktopUsageCheckbox.Checked != lastSavedSettings.LimitDesktopUsage) hasChanges = true;
            if (limitDesktopHourTextbox.Text != lastSavedSettings.LimitDesktopHour) hasChanges = true;
            if (limitDesktopMinTextbox.Text != lastSavedSettings.LimitDesktopMin) hasChanges = true;
            if (limitDesktopAMorPMComboBox.Text != lastSavedSettings.LimitDesktopAMPM) hasChanges = true;
            if (limitDesktopHourOpenTextbox.Text != lastSavedSettings.LimitDesktopHourOpen) hasChanges = true;
            if (limitDesktopMinOpenTextbox.Text != lastSavedSettings.LimitDesktopMinOpen) hasChanges = true;
            if (limitDesktopOpenAMorPMComboBox.Text != lastSavedSettings.LimitDesktopAMPMOpen) hasChanges = true;

            // 5. Curfew Action Comparison
            if (limitDesktopActionComboBox.Text != lastSavedSettings.LimitDesktopAction) hasChanges = true;
            if (limitDesktopImagePathTexbox.Text != lastSavedSettings.LimitDesktopImagePath) hasChanges = true;
            if (limitDesktopShowDialog5minCheckbox.Checked != lastSavedSettings.LimitShow5min) hasChanges = true;
            if (limitDesktopShowDialog10minCheckbox.Checked != lastSavedSettings.LimitShow10min) hasChanges = true;
            if (limitDesktopShowDialog30minCheckbox.Checked != lastSavedSettings.LimitShow30min) hasChanges = true;
            if (limitDesktopShutdownAfter3Minutes.Checked != lastSavedSettings.LimitShutdownAfter3Min) hasChanges = true;

            // 6. General System Comparison
            if (adminShutdownCheckbox.Checked != lastSavedSettings.AdminShutdown) hasChanges = true;
            if (applicationServiceCheckbox.Checked != lastSavedSettings.ApplicationServiceActive) hasChanges = true;
            if (applicationHighPriorityCheckbox.Checked != lastSavedSettings.ApplicationHighPriority) hasChanges = true;
            if (startupCheckbox.Checked != lastSavedSettings.LaunchOnStartup) hasChanges = true;
            if (minimizeTrayCheckbox.Checked != lastSavedSettings.MinimizeToTray) hasChanges = true;
            if (!string.IsNullOrEmpty(newPasswordTextbox.Text)) hasChanges = true;

            // Update the Label UI
            settingStatusLabel.Text = hasChanges ? "Settings not saved" : "No changes";
            settingStatusLabel.ForeColor = hasChanges ? Color.OrangeRed : Color.Gray;
        }
        private void PerformShutdown(string reason)
        {
            // 1. If a shutdown is already active or in cooldown, exit immediately.
            if (isShutdownInProgress) return;

            // 2. Lock the function
            isShutdownInProgress = true;

            bool isAdministrative = adminShutdownCheckbox.Checked;

            // 3. Show the Notification
            // Note: MessageBox.Show is a blocking call. The code below it won't run until you click OK.
            MessageBox.Show(
                $"SHUTDOWN TRIGGERED\n" +
                $"Source: {reason}\n" +
                $"Type: {(isAdministrative ? "Forced" : "Normal")}\n\n" +
                $"Actual command is commented out for Debug Mode.",
                "System Shutdown Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            // 4. Actual Shutdown Command (Commented for Debugging)
            /* 
            ExecuteSilentCommand("shutdown", $"/s {(isAdministrative ? "/f" : "")} /t 0"); 
            */

            // 5. Cooldown Logic:
            // We wait 5 seconds before allowing the shutdown logic to trigger again.
            // This gives the PC time to actually turn off, or gives you time to change settings during testing.
            System.Threading.Tasks.Task.Delay(5000).ContinueWith(t =>
            {
                isShutdownInProgress = false;
                //MessageBox.Show("Shutdown cooldown complete.");
            });
        }

        private void ManageCountdownLogic()
        {
            // If the settings window is currently visible, we hide the HUD to avoid overlap.
            // Otherwise, we follow the current UI checkbox states.
            if (this.Visible || !showCountdownCheckbox.Checked || isWidgetManuallyHidden || !shutdownAFKCheckbox.Checked)
            {
                if (countdownWindow != null)
                {
                    countdownWindow.AllowClose = true;
                    countdownWindow.Close();
                    countdownWindow = null;
                }
                isCountingDown = false;
                return;
            }

            // Create the HUD if it is enabled and doesn't exist
            if (countdownWindow == null || countdownWindow.IsDisposed)
            {
                countdownWindow = new CountdownForm();
                countdownWindow.PositionBottomRight();
                countdownWindow.OnRequestOpen = () => TryOpenFromTray();
                countdownWindow.OnRequestToggle = () => ToggleWidgetManual();
                countdownWindow.OnRequestExit = () => TryExitApp();

                // Apply properties from the current UI controls
                countdownWindow.TopMost = countdownTopMostCheckbox.Checked;

                if (countdownOpacityCheckbox.Checked)
                {
                    if (int.TryParse(countdownOpacityTextbox.Text, out int op))
                        countdownWindow.ApplyOpacity(op);
                }
                else
                {
                    countdownWindow.ApplyOpacity(100);
                }

                countdownWindow.Show();
            }

            // Logic for triggering the Auto Shutdown sequence
            bool isTriggered = (monitor.IsKbAfk && monitor.IsMouseAfk) ||
                               (monitor.IsKbSuspicious && monitor.IsMouseAfk) ||
                               (monitor.IsMouseClickSuspicious && monitor.IsKbAfk);

            if (isTriggered)
            {
                isCountingDown = true;
                countdownWindow.SetAlertMode(true);
                currentSecondsLeft--;

                // Final shutdown execution
                if (currentSecondsLeft <= 0 && startupGraceSeconds <= 0)
                {
                    PerformShutdown("AFK Detection");
                }
            }
            else
            {
                isCountingDown = false;
                // Revert timer to the duration set in the textbox
                if (int.TryParse(countdownTextbox.Text, out int minutes))
                {
                    currentSecondsLeft = minutes * 60;
                }
                else
                {
                    currentSecondsLeft = lastSavedSettings.CountdownMinutes * 60;
                }

                countdownWindow.SetAlertMode(false);
            }

            if (startupGraceSeconds > 0) startupGraceSeconds--;

            if (countdownWindow != null)
            {
                countdownWindow.UpdateTime(currentSecondsLeft);
            }
        }

        protected override void WndProc(ref Message m)
        {
            // If the message matches our custom "OpenRequest" string
            if (m.Msg == _openRequestMsg && _openRequestMsg != 0)
            {
                // We use BeginInvoke to ensure the UI thread is ready to show the dialog
                this.BeginInvoke(new Action(() =>
                {
                    TryOpenFromTray();
                }));
            }
            base.WndProc(ref m);
        }


        private void RefreshUIEnableState()
        {
            // --- REMOTE SERVICE SECTION ---
            // The button is enabled ONLY if the checkbox is checked.
            btnRemoteSettings.Enabled = remoteServiceCheckbox.Checked;

            // --- AFK SECTION ---
            bool isAFKEnabled = shutdownAFKCheckbox.Checked;
            countdownMinutesLabel.Enabled = isAFKEnabled;
            countdownTextbox.Enabled = isAFKEnabled;
            showCountdownCheckbox.Enabled = isAFKEnabled;

            bool isWidgetAllowed = isAFKEnabled && showCountdownCheckbox.Checked;
            countdownTopMostCheckbox.Enabled = isWidgetAllowed;
            countdownOpacityCheckbox.Enabled = isWidgetAllowed;
            countdownOpacityTextbox.Enabled = isWidgetAllowed && countdownOpacityCheckbox.Checked;

            // --- NO SMOKING SECTION ---
            viewNoSmokingDialog.Enabled = noSmokingDialogCheckbox.Checked;

            // --- CURFEW SECTION ---
            bool isCurfewEnabled = limitDesktopUsageCheckbox.Checked;
            limitDesktopHourTextbox.Enabled = isCurfewEnabled;
            limitDesktopMinTextbox.Enabled = isCurfewEnabled;
            limitDesktopAMorPMComboBox.Enabled = isCurfewEnabled;
            limitDesktopHourOpenTextbox.Enabled = isCurfewEnabled;
            limitDesktopMinOpenTextbox.Enabled = isCurfewEnabled;
            limitDesktopOpenAMorPMComboBox.Enabled = isCurfewEnabled;
            limitDesktopActionComboBox.Enabled = isCurfewEnabled;

            bool isImageAction = limitDesktopActionComboBox.Text == "Show Image Dialog";
            bool enableImageControls = isCurfewEnabled && isImageAction;
            limitDesktopImagePathTexbox.Enabled = enableImageControls;
            limitDesktopSelectImageBtn.Enabled = enableImageControls;
            viewLimitDesktopActionDialogBtn.Enabled = enableImageControls;

            limitDesktopShowDialog5minCheckbox.Enabled = isCurfewEnabled;
            limitDesktopShowDialog10minCheckbox.Enabled = isCurfewEnabled;
            limitDesktopShowDialog30minCheckbox.Enabled = isCurfewEnabled;
            limitDesktopShutdownAfter3Minutes.Enabled = isCurfewEnabled;
        }

        private void OnUIStateChanged(object sender, EventArgs e)
        {
            // 1. Force the UI to update enabled/disabled states immediately
            RefreshUIEnableState();

            // 2. Update the "Settings not saved" status label
            UpdateSettingsStatus();

            // 3. If AFK settings changed, update the countdown logic
            ManageCountdownLogic();

            // 4. Update the live countdown window if it exists
            if (countdownWindow != null && !countdownWindow.IsDisposed)
            {
                countdownWindow.TopMost = countdownTopMostCheckbox.Checked;

                if (countdownOpacityCheckbox.Checked)
                {
                    if (int.TryParse(countdownOpacityTextbox.Text, out int opacityPercent))
                    {
                        countdownWindow.ApplyOpacity(opacityPercent);
                    }
                }
                else
                {
                    countdownWindow.ApplyOpacity(100);
                }
            }
        }

        private void applicationHighPriority_CheckedChanged(object sender, EventArgs e)
        {
            ApplyHighPriorityLogic(applicationHighPriorityCheckbox.Checked);
            UpdateSettingsStatus();
        }

        private void ApplyHighPriorityLogic(bool enable)
        {
            Process.GetCurrentProcess().PriorityClass = enable ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
        }

        private void UpdateAppState(bool isActive) { lastSavedSettings.IsAppRunningState = isActive; AppSettings.Save(lastSavedSettings); }

        private void ManageWatchdog(bool enable)
        {
            string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "watchdog.vbs");
            string stopSignalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "watchdog.stop");

            if (enable)
            {
                try
                {
                    // Delete the stop signal if it exists so the watchdog can run
                    if (File.Exists(stopSignalPath)) File.Delete(stopSignalPath);

                    string exeName = Path.GetFileName(Application.ExecutablePath);
                    string exePath = Application.ExecutablePath;

                    // NEW VBSCRIPT LOGIC: 
                    // 1. Checks if 'watchdog.stop' exists.
                    // 2. If it exists, the script deletes the stop file and exits immediately.
                    // 3. Otherwise, it checks if the app is running.
                    string vbsContent = $@"Set shell = CreateObject(""WScript.Shell"")
Set fso = CreateObject(""Scripting.FileSystemObject"")
stopFile = ""{stopSignalPath.Replace("\\", "\\\\")}""

Do
    ' Check if we were told to stop
    If fso.FileExists(stopFile) Then
        fso.DeleteFile(stopFile)
        WScript.Quit
    End If

    Set service = GetObject(""winmgmts:{{impersonationLevel=impersonate}}!\\.\root\cimv2"")
    Set processes = service.ExecQuery(""SELECT * FROM Win32_Process WHERE Name = '{exeName}'"")
    
    running = False
    For Each process in processes
        running = True
    Next
    
    If Not running Then
        ' Re-check stop file one last time before launching to prevent race condition
        If Not fso.FileExists(stopFile) Then
            shell.Run """"""{exePath}""""""
        End If
    End If
    
    WScript.Sleep 2000 ' Faster check for more responsive exit
Loop";

                    File.WriteAllText(scriptPath, vbsContent);

                    ProcessStartInfo psi = new ProcessStartInfo("wscript.exe")
                    {
                        Arguments = $"\"{scriptPath}\"",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    // Ensure no old instances are running
                    ExecuteSilentCommand("taskkill", "/F /IM wscript.exe /FI \"COMMANDLINE eq *watchdog.vbs*\"");
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Watchdog Start Error: " + ex.Message);
                }
            }
            else
            {
                // DISABLE LOGIC: 
                // 1. Create the stop signal file first.
                // 2. The script will see this and exit on its own.
                // 3. We also call taskkill as a backup.
                try
                {
                    File.WriteAllText(stopSignalPath, "STOP");

                    ProcessStartInfo killOld = new ProcessStartInfo("taskkill", "/F /IM wscript.exe /FI \"COMMANDLINE eq *watchdog.vbs*\"")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    var proc = Process.Start(killOld);
                    proc?.WaitForExit(2000); // Wait for watchdog to die
                }
                catch { }
            }
        }

        private void viewNoSmokingDialog_Click(object sender, EventArgs e)
        {
            using (NoSmokingEditorForm editor = new NoSmokingEditorForm())
            {
                if (editor.ShowDialog() == DialogResult.OK) lastSavedSettings = AppSettings.Load();
            }
        }

        private void limitDesktopSelectImageBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Images|*.jpg;*.png" })
                if (ofd.ShowDialog() == DialogResult.OK) { limitDesktopImagePathTexbox.Text = ofd.FileName; UpdateSettingsStatus(); }
        }

        private void viewLimitDesktopActionDialogBtn_Click(object sender, EventArgs e)
        {
            using (LimitEditorForm editor = new LimitEditorForm())
            {
                if (editor.ShowDialog() == DialogResult.OK) { lastSavedSettings = AppSettings.Load(); UpdateSettingsStatus(); }
            }
        }

        private void OnSettingChanged(object sender, EventArgs e) => UpdateSettingsStatus();

        private void PasswordTextbox_TextChanged(object sender, EventArgs e) => UpdateSettingsStatus();

        private void limitDesktopNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void limitDesktopNumeric_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text)) tb.Text = "00";
            UpdateSettingsStatus();
        }

        private void countdownTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void countdownTextbox_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(countdownTextbox.Text, out int val) || val < 1) countdownTextbox.Text = "1";
            UpdateSettingsStatus();
        }

        private void countdownOpacityTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void countdownOpacityTextbox_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(countdownOpacityTextbox.Text, out int val)) val = 80;
            if (val < 10) val = 10; if (val > 100) val = 100;
            countdownOpacityTextbox.Text = val.ToString();

            // Apply to live window immediately
            if (countdownWindow != null && !countdownWindow.IsDisposed && countdownOpacityCheckbox.Checked)
            {
                countdownWindow.ApplyOpacity(val);
            }

            UpdateSettingsStatus();
        }

        private void btnRemoteSettings_Click(object sender, EventArgs e)
        {
            using (RemoteServiceForm rsForm = new RemoteServiceForm())
            {
                rsForm.ShowDialog();
                // Reload settings in case the user saved new credentials inside the sub-form
                lastSavedSettings = AppSettings.Load();
                UpdateSettingsStatus();
            }

        }
    }
}