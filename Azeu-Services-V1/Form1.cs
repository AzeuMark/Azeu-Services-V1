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
        private bool isRestartInProgress = false;
        public AfkWarningForm? activeAfkWarning = null;


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

            // --- LINK REMOTE COMMANDS TO LOCAL POWER FUNCTIONS ---
            RemoteServiceManager.Instance.OnRequestShutdown = (reason) =>
            {
                if (this.IsHandleCreated) this.Invoke(new Action(() => PerformShutdown(reason)));
            };

            RemoteServiceManager.Instance.OnRequestRestart = (reason) =>
            {
                if (this.IsHandleCreated) this.Invoke(new Action(() => PerformRestart(reason)));
            };

            RemoteServiceManager.Instance.OnRequestBypassCurfew = () =>
            {
                if (this.IsHandleCreated) this.Invoke(new Action(() => PerformRemoteBypass()));
            };


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
            // FIX: If the Settings window (Form1) is currently visible, 
            // we must prevent the Curfew lock from triggering.
            if (this.Visible)
            {
                // If the lock screen was already open, close it so the Admin can work
                if (activeLockScreen != null && !activeLockScreen.IsDisposed)
                {
                    activeLockScreen.DialogResult = DialogResult.OK; // Set OK to bypass closing cancel logic
                    activeLockScreen.Close();
                    activeLockScreen = null;
                }
                return;
            }

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

            // 5. Warning Logic
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
            kboardStatusLabel.Text = monitor.IsKbAfk ? "AFK" : "Active";
            mouseStatusLabel.Text = monitor.IsMouseAfk ? "AFK" : "Active";

            kboardStatusLabel.ForeColor = monitor.IsKbAfk ? Color.Red : Color.Green;
            mouseStatusLabel.ForeColor = monitor.IsMouseAfk ? Color.Red : Color.Green;


            if (monitor.IsSuspicious)
            {
                suspiciousKeysLabel.Text = "Yes";
                suspiciousKeysLabel.ForeColor = Color.Red;
            }
            else
            {
                suspiciousKeysLabel.Text = "Normal";
                suspiciousKeysLabel.ForeColor = Color.Green;
            }

            CheckDesktopLimit();
            ManageCountdownLogic();
            UpdateSettingsStatus();

            // --- REMOTE STATUS SYNC (AFK + UPTIME + LOCK STATE) ---
            if (lastSavedSettings.EnableRemoteService)
            {
                string countdownText;
                if (!lastSavedSettings.ShutdownIfAFK) countdownText = "Turned OFF";
                else
                {
                    int mins = currentSecondsLeft / 60;
                    int secs = currentSecondsLeft % 60;
                    countdownText = string.Format("{0:00}:{1:00}", mins, secs);
                }

                TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                string uptimeText;
                if (uptime.TotalDays >= 1)
                    uptimeText = string.Format("{0}d {1:D2}h {2:D2}m", (int)uptime.TotalDays, uptime.Hours, uptime.Minutes);
                else
                    uptimeText = string.Format("{0:D2}h {1:D2}m {2:D2}s", uptime.Hours, uptime.Minutes, uptime.Seconds);

                // NEW: Detect if Curfew Form is currently active
                bool isLocked = activeLockScreen != null && !activeLockScreen.IsDisposed;

                _ = RemoteServiceManager.Instance.SendStatusUpdate(countdownText, uptimeText, isLocked);
            }
        }

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            // 1. If the new password fields are enabled, it means the current password was correct.
            // We only validate the new password if the user actually typed something there.
            if (newPasswordTextbox.Enabled && !string.IsNullOrEmpty(newPasswordTextbox.Text))
            {
                if (newPasswordTextbox.Text != confirmPasswordTextbox.Text)
                {
                    MessageBox.Show("New passwords do not match. Please try again.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 2. Save the configuration to settings.json
            SaveConfig();

            // 3. Reset the UI: Clear the current password to trigger the auto-lock
            currentPasswordTextbox.Clear();
            newPasswordTextbox.Clear();
            confirmPasswordTextbox.Clear();

            // 4. Update UI Status and Notify success
            UpdateSettingsStatus();
            MessageBox.Show("Settings saved successfully!", "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                FinalCleanupAndExit();
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            lastSavedSettings.LastBypassDate = null;
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (settingStatusLabel.Text == "Settings not saved")
                {
                    DialogResult result = MessageBox.Show("Save changes before closing?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        saveSettingsBtn_Click(this, EventArgs.Empty);
                        if (settingStatusLabel.Text == "Settings not saved")
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (result == DialogResult.No)
                    {
                        LoadConfig();
                    }
                }

                // Determine if we stay alive in tray or exit fully
                if (minimizeTrayCheckbox.Checked || shutdownAFKCheckbox.Checked)
                {
                    e.Cancel = true;
                    this.allowVisible = false;
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    this.WindowState = FormWindowState.Minimized;
                    this.Hide();

                    // If "Minimize to tray" is OFF but AFK is ON, we hide the tray icon (Stealth mode)
                    if (trayIcon != null) trayIcon.Visible = minimizeTrayCheckbox.Checked;

                    ManageCountdownLogic();
                }
                else
                {
                    // Exit fully using the optimized cleanup
                    FinalCleanupAndExit();
                }
            }
            else
            {
                // OS is shutting down or process is being killed externally
                FinalCleanupAndExit();
            }
        }

        private void SaveConfig()
        {
            int.TryParse(countdownTextbox.Text, out int minutes);
            if (minutes < 1) minutes = 1;

            int.TryParse(countdownOpacityTextbox.Text, out int opacity);
            if (opacity < 10) opacity = 10;
            if (opacity > 100) opacity = 100;

            int.TryParse(afkWarningThresholdTextbox.Text, out int warningSecs);
            if (warningSecs < 10) warningSecs = 10;
            if (warningSecs > 60) warningSecs = 60;

            string finalPassword = string.IsNullOrEmpty(newPasswordTextbox.Text) ? lastSavedSettings.AdminPassword : newPasswordTextbox.Text;

            var newSettings = new AppSettings
            {
                // 1. Basics & AFK
                ShutdownIfAFK = shutdownAFKCheckbox.Checked,
                CountdownMinutes = minutes,
                AfkWarningThreshold = warningSecs,
                ShowCountdown = showCountdownCheckbox.Checked,
                CountdownTopMost = countdownTopMostCheckbox.Checked,
                EnableOpacity = countdownOpacityCheckbox.Checked,
                CountdownOpacity = opacity,

                // 2. No Smoking (UI toggles + PRESERVING EDITOR SETTINGS)
                EnableNoSmoking = noSmokingDialogCheckbox.Checked,
                NoSmokingMessage = lastSavedSettings.NoSmokingMessage,
                NoSmokingButtonText = lastSavedSettings.NoSmokingButtonText,
                NoSmokingFontSize = lastSavedSettings.NoSmokingFontSize,
                NoSmokingBgColor = lastSavedSettings.NoSmokingBgColor,
                NoSmokingTextColor = lastSavedSettings.NoSmokingTextColor,
                NoSmokingButtonRadius = lastSavedSettings.NoSmokingButtonRadius,
                NoSmokingDuration = lastSavedSettings.NoSmokingDuration,
                NoSmokingButtonBgColor = lastSavedSettings.NoSmokingButtonBgColor,
                NoSmokingButtonTextColor = lastSavedSettings.NoSmokingButtonTextColor,
                NoSmokingFontFamily = lastSavedSettings.NoSmokingFontFamily,
                NoSmokingButtonBottomMargin = lastSavedSettings.NoSmokingButtonBottomMargin,
                NoSmokingButtonWidth = lastSavedSettings.NoSmokingButtonWidth,
                NoSmokingButtonHeight = lastSavedSettings.NoSmokingButtonHeight,
                NoSmokingButtonFontSize = lastSavedSettings.NoSmokingButtonFontSize,
                NoSmokingImagePath = lastSavedSettings.NoSmokingImagePath, // THE FIX: Preserve Image Path
                NoSmokingImageSizeMode = lastSavedSettings.NoSmokingImageSizeMode, // THE FIX: Preserve Size Mode

                // 3. Remote Service (Preserving URL and Token)
                EnableRemoteService = remoteServiceCheckbox.Checked,
                WebSocketUrl = lastSavedSettings.WebSocketUrl,
                WebSocketToken = lastSavedSettings.WebSocketToken,

                // 4. Desktop Curfew (UI settings + PRESERVING EDITOR SETTINGS)
                LimitDesktopUsage = limitDesktopUsageCheckbox.Checked,
                LimitDesktopHour = limitDesktopHourCombo.Text,
                LimitDesktopMin = limitDesktopMinCombo.Text,
                LimitDesktopAMPM = limitDesktopAMorPMComboBox.Text,
                LimitDesktopHourOpen = limitDesktopHourOpenCombo.Text,
                LimitDesktopMinOpen = limitDesktopMinOpenCombo.Text,
                LimitDesktopAMPMOpen = limitDesktopOpenAMorPMComboBox.Text,
                LimitDesktopAction = limitDesktopActionComboBox.Text,
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
                LimitShowShutdownCountdown = lastSavedSettings.LimitShowShutdownCountdown,
                LimitShowReturningTime = lastSavedSettings.LimitShowReturningTime,
                LimitReturningFontFamily = lastSavedSettings.LimitReturningFontFamily,
                LimitReturningFontSize = lastSavedSettings.LimitReturningFontSize,
                LimitReturningTextColor = lastSavedSettings.LimitReturningTextColor,
                LimitReturningBottomMargin = lastSavedSettings.LimitReturningBottomMargin,
                LimitDesktopImagePath = lastSavedSettings.LimitDesktopImagePath, // THE FIX: Preserve Image Path
                LimitDesktopImageSizeMode = lastSavedSettings.LimitDesktopImageSizeMode, // THE FIX: Preserve Size Mode

                // 5. App & Security
                AdminPassword = finalPassword,
                LaunchOnStartup = startupCheckbox.Checked,
                MinimizeToTray = minimizeTrayCheckbox.Checked,
                ApplicationHighPriority = applicationHighPriorityCheckbox.Checked,
                AdminShutdown = adminShutdownCheckbox.Checked,
                ApplicationServiceActive = applicationServiceCheckbox.Checked,
                IsAppRunningState = true,
                LastBypassDate = lastSavedSettings.LastBypassDate
            };

            // Save to file and update current memory
            AppSettings.Save(newSettings);
            lastSavedSettings = newSettings;

            // Apply internal logic
            startSeconds = minutes * 60;
            currentSecondsLeft = startSeconds;

            SetStartup(newSettings.LaunchOnStartup);
            ApplyHighPriorityLogic(newSettings.ApplicationHighPriority);
            ManageWatchdog(newSettings.ApplicationServiceActive);
            ResetWarningFlags();

            // Re-sync Remote Service
            if (newSettings.EnableRemoteService) RemoteServiceManager.Instance.Start(newSettings);
            else RemoteServiceManager.Instance.Stop();

            UpdateSettingsStatus();
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
                afkWarningThresholdTextbox.Text = lastSavedSettings.AfkWarningThreshold.ToString();
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

                // Remote Service
                remoteServiceCheckbox.Checked = lastSavedSettings.EnableRemoteService;

                // Desktop Curfew (Closing)
                limitDesktopUsageCheckbox.Checked = lastSavedSettings.LimitDesktopUsage;
                limitDesktopHourCombo.Text = lastSavedSettings.LimitDesktopHour;
                limitDesktopMinCombo.Text = lastSavedSettings.LimitDesktopMin;
                limitDesktopAMorPMComboBox.Text = lastSavedSettings.LimitDesktopAMPM;

                // Desktop Curfew (Opening)
                limitDesktopHourOpenCombo.Text = lastSavedSettings.LimitDesktopHourOpen;
                limitDesktopMinOpenCombo.Text = lastSavedSettings.LimitDesktopMinOpen;
                limitDesktopOpenAMorPMComboBox.Text = lastSavedSettings.LimitDesktopAMPMOpen;

                // Actions & Warnings
                limitDesktopActionComboBox.Text = lastSavedSettings.LimitDesktopAction;
                limitDesktopShowDialog5minCheckbox.Checked = lastSavedSettings.LimitShow5min;
                limitDesktopShowDialog10minCheckbox.Checked = lastSavedSettings.LimitShow10min;
                limitDesktopShowDialog30minCheckbox.Checked = lastSavedSettings.LimitShow30min;
                limitDesktopShutdownAfter3Minutes.Checked = lastSavedSettings.LimitShutdownAfter3Min;

                // Clear password boxes on load to ensure they start in a locked state
                currentPasswordTextbox.Clear();
                newPasswordTextbox.Clear();
                confirmPasswordTextbox.Clear();

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
            if (limitDesktopActionComboBox.Items.Count == 0) limitDesktopActionComboBox.Items.AddRange(new object[] { "Shutdown", "Show Image" });
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

            // 1. AFK & Countdown Settings Comparison
            if (shutdownAFKCheckbox.Checked != lastSavedSettings.ShutdownIfAFK) hasChanges = true;
            if (countdownTextbox.Text != lastSavedSettings.CountdownMinutes.ToString()) hasChanges = true;
            if (afkWarningThresholdTextbox.Text != lastSavedSettings.AfkWarningThreshold.ToString()) hasChanges = true;
            if (showCountdownCheckbox.Checked != lastSavedSettings.ShowCountdown) hasChanges = true;
            if (countdownTopMostCheckbox.Checked != lastSavedSettings.CountdownTopMost) hasChanges = true;
            if (countdownOpacityCheckbox.Checked != lastSavedSettings.EnableOpacity) hasChanges = true;
            if (countdownOpacityTextbox.Text != lastSavedSettings.CountdownOpacity.ToString()) hasChanges = true;

            // 2. No Smoking & Remote Service Comparison
            if (noSmokingDialogCheckbox.Checked != lastSavedSettings.EnableNoSmoking) hasChanges = true;
            if (remoteServiceCheckbox.Checked != lastSavedSettings.EnableRemoteService) hasChanges = true; // NEW FIX

            // 3. Curfew Schedule Comparison
            if (limitDesktopUsageCheckbox.Checked != lastSavedSettings.LimitDesktopUsage) hasChanges = true;
            if (limitDesktopHourCombo.Text != lastSavedSettings.LimitDesktopHour) hasChanges = true;
            if (limitDesktopMinCombo.Text != lastSavedSettings.LimitDesktopMin) hasChanges = true;
            if (limitDesktopAMorPMComboBox.Text != lastSavedSettings.LimitDesktopAMPM) hasChanges = true;
            if (limitDesktopHourOpenCombo.Text != lastSavedSettings.LimitDesktopHourOpen) hasChanges = true;
            if (limitDesktopMinOpenCombo.Text != lastSavedSettings.LimitDesktopMinOpen) hasChanges = true;
            if (limitDesktopOpenAMorPMComboBox.Text != lastSavedSettings.LimitDesktopAMPMOpen) hasChanges = true;

            // 4. Curfew Action & Warnings Comparison
            if (limitDesktopShowDialog5minCheckbox.Checked != lastSavedSettings.LimitShow5min) hasChanges = true;
            if (limitDesktopShowDialog10minCheckbox.Checked != lastSavedSettings.LimitShow10min) hasChanges = true;
            if (limitDesktopShowDialog30minCheckbox.Checked != lastSavedSettings.LimitShow30min) hasChanges = true;
            if (limitDesktopShutdownAfter3Minutes.Checked != lastSavedSettings.LimitShutdownAfter3Min) hasChanges = true;

            // 5. Basic Application Settings Comparison
            if (adminShutdownCheckbox.Checked != lastSavedSettings.AdminShutdown) hasChanges = true;
            if (applicationServiceCheckbox.Checked != lastSavedSettings.ApplicationServiceActive) hasChanges = true;
            if (applicationHighPriorityCheckbox.Checked != lastSavedSettings.ApplicationHighPriority) hasChanges = true;
            if (startupCheckbox.Checked != lastSavedSettings.LaunchOnStartup) hasChanges = true;
            if (minimizeTrayCheckbox.Checked != lastSavedSettings.MinimizeToTray) hasChanges = true;

            // 6. Security (Password Boxes)
            if (!string.IsNullOrEmpty(newPasswordTextbox.Text)) hasChanges = true;

            // Update the UI Label
            if (hasChanges)
            {
                settingStatusLabel.Text = "Settings not saved";
                settingStatusLabel.ForeColor = Color.OrangeRed;
            }
            else
            {
                settingStatusLabel.Text = "No changes";
                settingStatusLabel.ForeColor = Color.Gray;
            }
        }

        private void PerformRemoteBypass()
        {
            if (activeLockScreen != null && !activeLockScreen.IsDisposed)
            {
                // 1. Mark as bypassed for today in settings
                lastSavedSettings.LastBypassDate = DateTime.Today;
                AppSettings.Save(lastSavedSettings);

                // 2. Force close the Curfew form
                activeLockScreen.DialogResult = DialogResult.OK; // Important to bypass the OnClosing cancel logic
                activeLockScreen.Close();
                activeLockScreen = null;

                RemoteServiceManager.Instance.WriteLog("Remote Curfew Bypass Executed.");
            }
        }

        public void PerformShutdown(string reason)
        {
            if (isShutdownInProgress || isRestartInProgress) return;
            isShutdownInProgress = true;

            bool isAdministrative = adminShutdownCheckbox.Checked;

            MessageBox.Show(
                $"SHUTDOWN TRIGGERED (DEBUG MODE)\n" +
                $"Source: {reason}\n" +
                $"Type: {(isAdministrative ? "Forced" : "Normal")}\n\n" +
                $"System command is currently commented out for testing.",
                "System Shutdown Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            // ACTUAL COMMAND TO SHUTDOWN (COMMENTED FOR DEBUGGING)
            // ExecuteSilentCommand("shutdown", $"/s {(isAdministrative ? "/f" : "")} /t 0"); 

            Task.Delay(5000).ContinueWith(t => isShutdownInProgress = false);
        }

        public void PerformRestart(string reason)
        {
            if (isShutdownInProgress || isRestartInProgress) return;
            isRestartInProgress = true;

            MessageBox.Show(
                $"RESTART TRIGGERED (DEBUG MODE)\n" +
                $"Source: {reason}\n\n" +
                $"System command is currently commented out for testing.",
                "System Restart Notice",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            // ACTUAL COMMAND TO RESTART (COMMENTED FOR DEBUGGING)
            // ExecuteSilentCommand("shutdown", "/r /f /t 0");

            Task.Delay(5000).ContinueWith(t => isRestartInProgress = false);
        }

        private void ManageCountdownLogic()
        {
            // If settings window is open, or widget is hidden, or AFK is off, kill everything.
            if (this.Visible || !showCountdownCheckbox.Checked || isWidgetManuallyHidden || !shutdownAFKCheckbox.Checked)
            {
                if (countdownWindow != null) { countdownWindow.AllowClose = true; countdownWindow.Close(); countdownWindow = null; }
                if (activeAfkWarning != null) { activeAfkWarning.Close(); activeAfkWarning = null; }
                isCountingDown = false;
                return;
            }

            // 1. Manage the small Countdown HUD Widget
            if (countdownWindow == null || countdownWindow.IsDisposed)
            {
                countdownWindow = new CountdownForm();
                countdownWindow.PositionBottomRight();
                countdownWindow.OnRequestOpen = () => TryOpenFromTray();
                countdownWindow.OnRequestToggle = () => ToggleWidgetManual();
                countdownWindow.OnRequestExit = () => TryExitApp();
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

            // 2. Detection Logic
            bool isTriggered = (monitor.IsKbAfk && monitor.IsMouseAfk) ||
                               (monitor.IsKbSuspicious && monitor.IsMouseAfk) ||
                               (monitor.IsMouseClickSuspicious && monitor.IsKbAfk);

            if (isTriggered)
            {
                isCountingDown = true;
                countdownWindow.SetAlertMode(true);
                currentSecondsLeft--;

                // --- DYNAMIC AFK WARNING DIALOG LOGIC ---
                // Fetch threshold from UI. Default to 30 if parsing fails somehow.
                if (!int.TryParse(afkWarningThresholdTextbox.Text, out int threshold)) threshold = 30;

                if (currentSecondsLeft <= threshold && currentSecondsLeft > 0)
                {
                    if (activeAfkWarning == null || activeAfkWarning.IsDisposed)
                    {
                        activeAfkWarning = new AfkWarningForm();
                        activeAfkWarning.Show();
                    }
                    activeAfkWarning.UpdateCountdown(currentSecondsLeft);
                }

                // Perform actual shutdown
                if (currentSecondsLeft <= 0 && startupGraceSeconds <= 0)
                {
                    if (activeAfkWarning != null) { activeAfkWarning.Close(); activeAfkWarning = null; }
                    PerformShutdown("AFK Detection");
                }
            }
            else
            {
                // NO LONGER AFK: Reset everything
                isCountingDown = false;

                if (int.TryParse(countdownTextbox.Text, out int minutes))
                    currentSecondsLeft = minutes * 60;
                else
                    currentSecondsLeft = lastSavedSettings.CountdownMinutes * 60;

                countdownWindow.SetAlertMode(false);

                // Close warning automatically if movement detected
                if (activeAfkWarning != null)
                {
                    activeAfkWarning.Close();
                    activeAfkWarning = null;
                }
            }

            if (startupGraceSeconds > 0) startupGraceSeconds--;
            if (countdownWindow != null) countdownWindow.UpdateTime(currentSecondsLeft);
        }

        private void AMandPM_KeyPress(object sender, KeyPressEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (char.IsDigit(e.KeyChar) && int.TryParse(cmb.Text + e.KeyChar, out int val) && val <= 12)
                return; // Allow

            if (!char.IsControl(e.KeyChar))
                e.Handled = true; // Block everything else
        }

        private void AMandPM_Leave(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null) return;

            // Try to parse the value
            if (int.TryParse(cmb.Text, out int value))
            {
                // Check if within 1-12 range
                if (value < 1 || value > 12)
                {
                    cmb.Text = "01";
                }
                else
                {
                    cmb.Text = value.ToString("00");
                }
            }
            else
            {
                // Invalid input
                cmb.Text = "01";
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
            // 1. AFK Master Group
            bool isAFKEnabled = shutdownAFKCheckbox.Checked;
            countdownMinutesLabel.Enabled = isAFKEnabled;
            countdownTextbox.Enabled = isAFKEnabled;
            afkWarningThresholdLabel.Enabled = isAFKEnabled;
            afkWarningThresholdTextbox.Enabled = isAFKEnabled;
            showCountdownCheckbox.Enabled = isAFKEnabled;

            // 2. Countdown Widget Sub-Group
            bool isWidgetAllowed = isAFKEnabled && showCountdownCheckbox.Checked;
            countdownTopMostCheckbox.Enabled = isWidgetAllowed;
            countdownOpacityCheckbox.Enabled = isWidgetAllowed;
            countdownOpacityTextbox.Enabled = isWidgetAllowed && countdownOpacityCheckbox.Checked;

            // 3. No Smoking Group
            viewNoSmokingDialog.Enabled = noSmokingDialogCheckbox.Checked;

            // 4. Remote Service Group
            btnRemoteSettings.Enabled = remoteServiceCheckbox.Checked;

            // 5. Curfew Group
            bool isCurfewEnabled = limitDesktopUsageCheckbox.Checked;
            limitDesktopHourCombo.Enabled = isCurfewEnabled;
            limitDesktopMinCombo.Enabled = isCurfewEnabled;
            limitDesktopAMorPMComboBox.Enabled = isCurfewEnabled;
            limitDesktopHourOpenCombo.Enabled = isCurfewEnabled;
            limitDesktopMinOpenCombo.Enabled = isCurfewEnabled;
            limitDesktopOpenAMorPMComboBox.Enabled = isCurfewEnabled;
            limitDesktopActionComboBox.Enabled = isCurfewEnabled;

            // Check if the selected action is "Show Image Dialog"
            bool isImageAction = limitDesktopActionComboBox.Text == "Show Image";

            // Controls that only make sense if the "Show Image Dialog" action is selected
            viewLimitDesktopActionDialogBtn.Enabled = isCurfewEnabled && isImageAction;
            limitDesktopShutdownAfter3Minutes.Enabled = isCurfewEnabled && isImageAction; // FIXED: Linked to Image Action

            // Warnings can be shown regardless of whether the final action is shutdown or dialog
            limitDesktopShowDialog5minCheckbox.Enabled = isCurfewEnabled;
            limitDesktopShowDialog10minCheckbox.Enabled = isCurfewEnabled;
            limitDesktopShowDialog30minCheckbox.Enabled = isCurfewEnabled;

            // --- PASSWORD SECURITY SECTION ---
            bool isPasswordCorrect = (!string.IsNullOrEmpty(currentPasswordTextbox.Text) && currentPasswordTextbox.Text == lastSavedSettings.AdminPassword);
            newPasswordTextbox.Enabled = isPasswordCorrect;
            confirmPasswordTextbox.Enabled = isPasswordCorrect;

            if (!isPasswordCorrect)
            {
                newPasswordTextbox.Clear();
                confirmPasswordTextbox.Clear();
            }
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

        private bool isProcessingWatchdog = false;
        private void ManageWatchdog(bool enable)
        {
            if (isProcessingWatchdog) return;
            isProcessingWatchdog = true;

            Task.Run(() =>
            {
                try
                {
                    string appDir = AppDomain.CurrentDomain.BaseDirectory;
                    string scriptPath = Path.Combine(appDir, "watchdog.vbs");
                    string stopSignalPath = Path.Combine(appDir, "watchdog.stop");
                    string customWscriptPath = Path.Combine(appDir, "AzeuWatchdog.exe");
                    string systemWscript = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wscript.exe");

                    if (enable)
                    {
                        // Only kill and restart if we are ENABLING (to prevent multiple instances)
                        using (Process killProc = new Process())
                        {
                            killProc.StartInfo.FileName = "taskkill";
                            killProc.StartInfo.Arguments = "/F /IM AzeuWatchdog.exe /T";
                            killProc.StartInfo.CreateNoWindow = true;
                            killProc.StartInfo.UseShellExecute = false;
                            killProc.Start();
                            killProc.WaitForExit(500);
                        }

                        if (!File.Exists(customWscriptPath) && File.Exists(systemWscript))
                            File.Copy(systemWscript, customWscriptPath, true);

                        if (File.Exists(stopSignalPath)) File.Delete(stopSignalPath);

                        string exeName = Path.GetFileName(Application.ExecutablePath);
                        string exePath = Application.ExecutablePath;
                        string vbsContent = $@"Set shell = CreateObject(""WScript.Shell"")
Set fso = CreateObject(""Scripting.FileSystemObject"")
stopFile = ""{stopSignalPath.Replace("\\", "\\\\")}""
Do
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
        If Not fso.FileExists(stopFile) Then shell.Run """"""{exePath}""""""
    End If
    WScript.Sleep 3000 
Loop";

                        if (!File.Exists(scriptPath) || File.ReadAllText(scriptPath) != vbsContent)
                            File.WriteAllText(scriptPath, vbsContent);

                        ProcessStartInfo psi = new ProcessStartInfo(customWscriptPath)
                        {
                            Arguments = $"\"{scriptPath}\"",
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        Process.Start(psi);
                    }
                    else
                    {
                        // LIGHTWEIGHT DISABLE: Just drop the signal file. 
                        // AzeuWatchdog.exe will see this file and close itself automatically within 3 seconds.
                        // We avoid TaskKill here to prevent the "Cursor Freeze".
                        File.WriteAllText(stopSignalPath, "STOP");
                    }
                }
                catch (Exception ex) { Debug.WriteLine("Watchdog Error: " + ex.Message); }
                finally { isProcessingWatchdog = false; }
            });
        }
        private void FinalCleanupAndExit()
        {
            // 1. Drop the stop signal immediately (Fastest I/O)
            try
            {
                string stopSignalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "watchdog.stop");
                File.WriteAllText(stopSignalPath, "STOP");
            }
            catch { }

            // 2. Stop Hooks and UI components
            if (monitor != null) monitor.Stop();

            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            // 3. Update State
            UpdateAppState(false);

            // 4. Force Exit
            Environment.Exit(0);
        }

        private void viewNoSmokingDialog_Click(object sender, EventArgs e)
        {
            using (NoSmokingEditorForm editor = new NoSmokingEditorForm())
            {
                if (editor.ShowDialog() == DialogResult.OK) lastSavedSettings = AppSettings.Load();
            }
        }

        private void viewLimitDesktopActionDialogBtn_Click(object sender, EventArgs e)
        {
            using (LimitEditorForm editor = new LimitEditorForm())
            {
                if (editor.ShowDialog() == DialogResult.OK) { lastSavedSettings = AppSettings.Load(); UpdateSettingsStatus(); }
            }
        }

        private void OnSettingChanged(object sender, EventArgs e) => UpdateSettingsStatus();

        private void PasswordTextbox_TextChanged(object sender, EventArgs e)
        {
            // Trigger the master UI state check
            RefreshUIEnableState();

            // Notify the user if changes are detected
            UpdateSettingsStatus();
        }

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

        private void afkWarningThresholdTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void afkWarningThresholdTextbox_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(afkWarningThresholdTextbox.Text, out int val)) val = 30;
            if (val < 10) val = 10;
            if (val > 60) val = 60;
            afkWarningThresholdTextbox.Text = val.ToString();
            UpdateSettingsStatus();
        }

        private void MinutesLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            ComboBox? cmb = sender as ComboBox;

            if (char.IsDigit(e.KeyChar))
            {
                string potentialText = cmb.Text + e.KeyChar;
                if (int.TryParse(potentialText, out int value))
                {
                    if (value > 59)
                        e.Handled = true;
                }
            }
            else if (!char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void MinutesLimit_Leave(object sender, EventArgs e)
        {
            ComboBox? cmb = sender as ComboBox;
            if (cmb == null) return;

            if (int.TryParse(cmb.Text, out int value))
            {
                if (value < 0) cmb.Text = "00";
                else if (value > 59) cmb.Text = "59";
                else cmb.Text = value.ToString("00");
            }
            else
            {
                cmb.Text = "00";
            }
        }


        private void facebookUrlLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://www.facebook.com/azeu.dev";

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show("Unable to open browser", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}