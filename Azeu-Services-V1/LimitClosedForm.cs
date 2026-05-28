using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace AzeuServices_V1
{
    public partial class LimitClosedForm : Form
    {
        private AppSettings _settings;
        private Action? _shutdownAction;
        private System.Windows.Forms.Timer? shutdownTimer;
        private int _secondsUntilShutdown = 180;
        private IntPtr _hhk = IntPtr.Zero;
        private LowLevelKeyboardProc? _delegate;
        private bool _isPreview;

        private const int WM_HOTKEY = 0x0312;
        private const int STAFF_BYPASS_HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x0002;
        private const uint VK_X = 0x58;

        [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private Label? lblEscPrompt;
        private Label? lblShutdownCountdown;

        public LimitClosedForm(AppSettings settings, Action? shutdownAction, bool isPreview = false)
        {
            _settings = settings;
            _shutdownAction = shutdownAction;
            _isPreview = isPreview;
            this.KeyPreview = true;

            this.BackColor = Color.FromName(_settings.LimitBgColor);
            this.ShowInTaskbar = isPreview;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Screen.PrimaryScreen.Bounds.Location;

            if (!isPreview)
            {
                _delegate = HookCallback;
                _hhk = SetHook(_delegate);
            }

            if (!string.IsNullOrEmpty(_settings.LimitDesktopImagePath) && File.Exists(_settings.LimitDesktopImagePath))
            {
                try
                {
                    using (var fs = new FileStream(_settings.LimitDesktopImagePath, FileMode.Open, FileAccess.Read))
                        this.BackgroundImage = Image.FromStream(fs);
                    this.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), _settings.LimitDesktopImageSizeMode);
                }
                catch { }
            }

            SetupUI();
            if (_isPreview) SetupPreviewControls();

            // --- SHUTDOWN TIMER LOGIC ---
            if (_settings.LimitShutdownAfter3Min)
            {
                //_secondsUntilShutdown = 180;

                if (lblShutdownCountdown != null)
                    lblShutdownCountdown.Visible = _settings.LimitShowShutdownCountdown;

                shutdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                shutdownTimer.Tick += (s, e) =>
                {
                    if (_secondsUntilShutdown > 0) _secondsUntilShutdown--;

                    if (lblShutdownCountdown != null && lblShutdownCountdown.Visible)
                    {
                        string prefix = _isPreview ? "[PREVIEW] " : "";
                        lblShutdownCountdown.Text = $"{prefix}This computer will shutdown in {_secondsUntilShutdown} seconds.";
                    }

                    // CRITICAL SAFETY CHECK: Only shutdown if NOT in preview mode
                    if (_secondsUntilShutdown <= 0)
                    {
                        if (_isPreview)
                        {
                            // In preview, just reset to 180 so the admin can keep testing the UI
                            _secondsUntilShutdown = 180;
                        }
                        else if (_shutdownAction != null)
                        {
                            shutdownTimer.Stop();
                            _shutdownAction.Invoke();
                        }
                    }
                };
                shutdownTimer.Start();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RegisterHotKey(this.Handle, STAFF_BYPASS_HOTKEY_ID, MOD_CONTROL, VK_X);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == STAFF_BYPASS_HOTKEY_ID)
            {
                if (PasswordDialog.Authenticate())
                {
                    _settings.LastBypassDate = DateTime.Today;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_isPreview && keyData == Keys.Escape) { this.Close(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetupUI()
        {
            TableLayoutPanel panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = Color.Transparent };

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // 1. Main Message
            Label lblMsg = new Label
            {
                Text = _settings.LimitMessage,
                ForeColor = Color.FromName(_settings.LimitTextColor),
                Font = new Font(_settings.LimitFontFamily, _settings.LimitFontSize, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblMsg, 0, 0);

            // 2. Returning Time
            if (_settings.LimitShowReturningTime)
            {
                string openTime = $"{_settings.LimitDesktopHourOpen}:{_settings.LimitDesktopMinOpen} {_settings.LimitDesktopAMPMOpen}";
                Label lblRet = new Label
                {
                    Text = $"Returning at {openTime}",
                    ForeColor = Color.FromName(_settings.LimitReturningTextColor),
                    Font = new Font(_settings.LimitReturningFontFamily, _settings.LimitReturningFontSize, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.BottomCenter,
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, _settings.LimitReturningBottomMargin)
                };
                panel.Controls.Add(lblRet, 0, 1);
            }

            // 3. Shutdown Countdown
            lblShutdownCountdown = new Label
            {
                Text = (_isPreview ? "[PREVIEW] " : "") + "This computer will shutdown in 180 seconds.",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Visible = false,
                Padding = new Padding(0, 10, 0, 10)
            };
            panel.Controls.Add(lblShutdownCountdown, 0, 2);

            // 4. Bypass Hint
            if (_settings.LimitShowBypassInstructions)
            {
                Label lblHint = new Label
                {
                    Text = "Staff? Press Ctrl + X to unlock",
                    ForeColor = Color.FromName(_settings.LimitTextColor),
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = true,
                    Padding = new Padding(0, 0, 0, 20)
                };
                panel.Controls.Add(lblHint, 0, 3);
            }

            this.Controls.Add(panel);
        }

        private void SetupPreviewControls()
        {
            lblEscPrompt = new Label
            {
                Text = "PREVIEW MODE: PRESS ESC TO CLOSE",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                AutoSize = true,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblEscPrompt);
            lblEscPrompt.BringToFront();
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT { public int vkCode; public int scanCode; public int flags; }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT info = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                bool blocked = (info.vkCode == 9 && (info.flags & 32) != 0) || (info.vkCode == 91 || info.vkCode == 92) || (info.vkCode == 115 && (info.flags & 32) != 0);
                if (blocked) return (IntPtr)1;
            }
            return CallNextHookEx(_hhk, nCode, wParam, lParam);
        }

        [DllImport("user32.dll")] private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string lpModuleName);

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curP = Process.GetCurrentProcess())
            using (ProcessModule curM = curP.MainModule!)
                return SetWindowsHookEx(13, proc, GetModuleHandle(curM.ModuleName!), 0);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_isPreview && e.CloseReason == CloseReason.UserClosing && this.DialogResult != DialogResult.OK) { e.Cancel = true; return; }
            if (shutdownTimer != null) shutdownTimer.Stop();
            if (_hhk != IntPtr.Zero) UnhookWindowsHookEx(_hhk);
            UnregisterHotKey(this.Handle, STAFF_BYPASS_HOTKEY_ID);
            base.OnFormClosing(e);
        }
    }
}