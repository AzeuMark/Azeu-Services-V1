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

        public Form1()
        {
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            InitializeComponent();

            var forceHandle = this.Handle;
            SystemEvents.SessionEnding += OnSystemSessionEnding;

            monitor = new ActivityMonitor();
            SetupTrayIcon();

            uiTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            uiTimer.Tick += UiTimer_Tick;

            LoadConfig();
            uiTimer.Start();
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
            if (lastSavedSettings.LastBypassDate.HasValue && lastSavedSettings.LastBypassDate.Value.Date == DateTime.Today) return;

            string timeStr = lastSavedSettings.LimitDesktopHour;
            string minStr = lastSavedSettings.LimitDesktopMin;
            string ampm = lastSavedSettings.LimitDesktopAMPM;

            if (!DateTime.TryParse($"{timeStr}:{minStr} {ampm}", out DateTime targetTime)) return;

            TimeSpan timeRemaining = targetTime - DateTime.Now;
            double totalMinutesRemaining = timeRemaining.TotalMinutes;

            if (lastSavedSettings.LimitShow30min && totalMinutesRemaining <= 30 && totalMinutesRemaining > 29 && !hasShown30mWarning)
            {
                hasShown30mWarning = true;
                ShowLimitWarning($"PC will close at {timeStr}:{minStr} {ampm}");
            }
            else if (lastSavedSettings.LimitShow10min && totalMinutesRemaining <= 10 && totalMinutesRemaining > 9 && !hasShown10mWarning)
            {
                hasShown10mWarning = true;
                ShowLimitWarning($"PC will close at {timeStr}:{minStr} {ampm}");
            }
            else if (lastSavedSettings.LimitShow5min && totalMinutesRemaining <= 5 && totalMinutesRemaining > 0 && !hasShown5mWarning)
            {
                hasShown5mWarning = true;
                ShowLimitWarning($"PC will close at {timeStr}:{minStr} {ampm}");
            }

            if (DateTime.Now >= targetTime && !limitDialogShownToday)
            {
                limitDialogShownToday = true;
                if (lastSavedSettings.LimitDesktopAction == "Shutdown") PerformShutdown("Curfew Reached");
                else
                {
                    using (LimitClosedForm closedForm = new LimitClosedForm(lastSavedSettings, () => PerformShutdown("Desktop Limit Auto-Close")))
                    {
                        if (closedForm.ShowDialog() == DialogResult.OK)
                        {
                            limitDialogShownToday = false;
                            ResetWarningFlags();
                        }
                    }
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
            Point spawnLoc = new Point(Screen.PrimaryScreen.WorkingArea.Right - 300, Screen.PrimaryScreen.WorkingArea.Bottom - 150);
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
            if (allowVisible) { if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal; this.Show(); this.Activate(); return; }
            if (PasswordDialog.Authenticate())
            {
                allowVisible = true; this.Opacity = 1.0; this.ShowInTaskbar = true; this.WindowState = FormWindowState.Normal; this.Show(); this.Activate();
                isWidgetManuallyHidden = false;
                if (trayToggleItem != null) { trayToggleItem.Text = "Hide Widget"; trayToggleItem.Image = GetImageFromResource(Properties.Resources.hide_icon); }
                if (countdownWindow != null) { countdownWindow.AllowClose = true; countdownWindow.Close(); countdownWindow = null; }
            }
        }

        private void ExecuteSilentCommand(string command, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo(command, arguments) { CreateNoWindow = true, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden };
            Process.Start(psi);
        }

        private void TryExitApp()
        {
            if (PasswordDialog.Authenticate()) { UpdateAppState(false); ManageWatchdog(false); monitor.Stop(); trayIcon.Dispose(); Application.Exit(); }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (minimizeTrayCheckbox.Checked && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; allowVisible = false; this.Opacity = 0; this.ShowInTaskbar = false; this.WindowState = FormWindowState.Minimized; this.Hide(); ManageCountdownLogic();
            }
            else { SystemEvents.SessionEnding -= OnSystemSessionEnding; UpdateAppState(false); ManageWatchdog(false); if (monitor != null) monitor.Stop(); if (trayIcon != null) trayIcon.Dispose(); }
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

                LimitDesktopUsage = limitDesktopUsageCheckbox.Checked,
                LimitDesktopHour = limitDesktopHourTextbox.Text,
                LimitDesktopMin = limitDesktopMinTextbox.Text,
                LimitDesktopAMPM = limitDesktopAMorPMComboBox.Text,
                LimitDesktopOpenHour = limitDesktopHourOpenTextbox.Text,
                LimitDesktopOpenMin = limitDesktopMinOpenTextbox.Text,
                LimitDesktopOpenAMPM = limitDesktopOpenAMorPMComboBox.Text,
                LimitDesktopAction = limitDesktopActionComboBox.Text,
                LimitDesktopImagePath = limitDesktopImagePathTexbox.Text,
                LimitShow5min = limitDesktopShowDialog5minCheckbox.Checked,
                LimitShow10min = limitDesktopShowDialog10minCheckbox.Checked,
                LimitShow30min = limitDesktopShowDialog30minCheckbox.Checked,
                LimitShutdownAfter3Min = limitDesktopShutdownAfter3Minutes.Checked
            };

            AppSettings.Save(newSettings);
            lastSavedSettings = newSettings;
            startSeconds = minutes * 60; currentSecondsLeft = startSeconds;
            SetStartup(newSettings.LaunchOnStartup);
            ApplyHighPriorityLogic(newSettings.ApplicationHighPriority);
            ManageWatchdog(newSettings.ApplicationServiceActive);
            ResetWarningFlags();
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
                noSmokingDialogCheckbox.Checked = lastSavedSettings.EnableNoSmoking;

                limitDesktopUsageCheckbox.Checked = lastSavedSettings.LimitDesktopUsage;
                limitDesktopHourTextbox.Text = lastSavedSettings.LimitDesktopHour;
                limitDesktopMinTextbox.Text = lastSavedSettings.LimitDesktopMin;
                limitDesktopAMorPMComboBox.Text = lastSavedSettings.LimitDesktopAMPM;
                limitDesktopHourOpenTextbox.Text = lastSavedSettings.LimitDesktopOpenHour;
                limitDesktopMinOpenTextbox.Text = lastSavedSettings.LimitDesktopOpenMin;
                limitDesktopOpenAMorPMComboBox.Text = lastSavedSettings.LimitDesktopOpenAMPM;
                limitDesktopActionComboBox.Text = lastSavedSettings.LimitDesktopAction;
                limitDesktopImagePathTexbox.Text = lastSavedSettings.LimitDesktopImagePath;
                limitDesktopShowDialog5minCheckbox.Checked = lastSavedSettings.LimitShow5min;
                limitDesktopShowDialog10minCheckbox.Checked = lastSavedSettings.LimitShow10min;
                limitDesktopShowDialog30minCheckbox.Checked = lastSavedSettings.LimitShow30min;
                limitDesktopShutdownAfter3Minutes.Checked = lastSavedSettings.LimitShutdownAfter3Min;

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
            bool hasChanges = (shutdownAFKCheckbox.Checked != lastSavedSettings.ShutdownIfAFK) || (noSmokingDialogCheckbox.Checked != lastSavedSettings.EnableNoSmoking) || (limitDesktopUsageCheckbox.Checked != lastSavedSettings.LimitDesktopUsage);
            settingStatusLabel.Text = hasChanges ? "Settings not saved" : "No changes";
            settingStatusLabel.ForeColor = hasChanges ? Color.OrangeRed : Color.Gray;
        }

        private void PerformShutdown(string reason)
        {
            bool isAdministrative = adminShutdownCheckbox.Checked;
            MessageBox.Show($"SHUTDOWN TRIGGERED\nSource: {reason}\nType: {(isAdministrative ? "Forced" : "Normal")}\n\nActual command is commented out.");
            /* ExecuteSilentCommand("shutdown", $"/s {(isAdministrative ? "/f" : "")} /t 0"); */
        }

        private void ManageCountdownLogic()
        {
            if (this.Visible || !showCountdownCheckbox.Checked || isWidgetManuallyHidden || !shutdownAFKCheckbox.Checked)
            {
                if (countdownWindow != null) { countdownWindow.AllowClose = true; countdownWindow.Close(); countdownWindow = null; }
                isCountingDown = false; return;
            }
            if (countdownWindow == null || countdownWindow.IsDisposed)
            {
                countdownWindow = new CountdownForm(); countdownWindow.PositionBottomRight();
                countdownWindow.OnRequestOpen = () => TryOpenFromTray();
                countdownWindow.OnRequestToggle = () => ToggleWidgetManual();
                countdownWindow.OnRequestExit = () => TryExitApp();
                countdownWindow.Show();
            }

            bool isTriggered = (monitor.IsKbAfk && monitor.IsMouseAfk) || (monitor.IsKbSuspicious && monitor.IsMouseAfk) || (monitor.IsMouseClickSuspicious && monitor.IsKbAfk);
            if (isTriggered)
            {
                isCountingDown = true; countdownWindow.SetAlertMode(true); currentSecondsLeft--;
                if (currentSecondsLeft <= 0 && startupGraceSeconds <= 0) PerformShutdown("AFK Detection");
            }
            else { isCountingDown = false; currentSecondsLeft = lastSavedSettings.CountdownMinutes * 60; countdownWindow.SetAlertMode(false); }

            if (startupGraceSeconds > 0) startupGraceSeconds--;
            countdownWindow?.UpdateTime(currentSecondsLeft);
        }

        private void RefreshUIEnableState()
        {
            bool isShutdownEnabled = shutdownAFKCheckbox.Checked;
            countdownMinutesLabel.Enabled = isShutdownEnabled;
            countdownTextbox.Enabled = isShutdownEnabled;
            showCountdownCheckbox.Enabled = isShutdownEnabled;
            viewNoSmokingDialog.Enabled = noSmokingDialogCheckbox.Checked;
            limitDesktopHourTextbox.Enabled = limitDesktopUsageCheckbox.Checked;
        }

        private void OnUIStateChanged(object sender, EventArgs e)
        {
            if (countdownWindow != null) countdownWindow.TopMost = countdownTopMostCheckbox.Checked;
            RefreshUIEnableState(); ManageCountdownLogic(); UpdateSettingsStatus();
        }

        private void applicationHighPriority_CheckedChanged(object sender, EventArgs e) => ApplyHighPriorityLogic(applicationHighPriorityCheckbox.Checked);

        private void ApplyHighPriorityLogic(bool enable)
        {
            Process.GetCurrentProcess().PriorityClass = enable ? ProcessPriorityClass.High : ProcessPriorityClass.Normal;
        }

        private void UpdateAppState(bool isActive) { lastSavedSettings.IsAppRunningState = isActive; AppSettings.Save(lastSavedSettings); }

        private void ManageWatchdog(bool enable) { if (!enable) ExecuteSilentCommand("taskkill", "/F /IM wscript.exe /T"); }

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
            using (LimitClosedForm preview = new LimitClosedForm(lastSavedSettings, null, true)) preview.ShowDialog();
        }

        // --- THE MISSING VALIDATION FUNCTIONS START HERE ---

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
            UpdateSettingsStatus();
        }
    }
}