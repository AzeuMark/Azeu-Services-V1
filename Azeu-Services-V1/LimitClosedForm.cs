using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private IntPtr _hhk = IntPtr.Zero;
        private LowLevelKeyboardProc? _delegate;
        private bool _isPreview;

        // UI Controls for Preview Mode
        private Button? btnExitPreview;
        private Label? lblEscPrompt;

        public LimitClosedForm(AppSettings settings, Action? shutdownAction, bool isPreview = false)
        {
            _settings = settings;
            _shutdownAction = shutdownAction;
            _isPreview = isPreview;
            this.KeyPreview = true;

            // 1. Apply Dynamic Background Color
            this.BackColor = Color.FromName(_settings.LimitBgColor);
            this.ShowInTaskbar = isPreview;

            // 2. Window State Management (FIXED FOR FULL SCREEN PREVIEW)
            if (isPreview)
            {
                // Even in preview, we want full screen to see the actual result
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = Screen.PrimaryScreen.Bounds.Location;
                this.Text = "Preview Mode - Press ESC to close";
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = Screen.PrimaryScreen.Bounds.Location;

                // Set up Keyboard Hook only in live mode to block system keys (Alt+Tab, etc.)
                _delegate = HookCallback;
                _hhk = SetHook(_delegate);
            }

            // 3. Apply Background Image
            if (!string.IsNullOrEmpty(_settings.LimitDesktopImagePath) && File.Exists(_settings.LimitDesktopImagePath))
            {
                try
                {
                    using (var fs = new FileStream(_settings.LimitDesktopImagePath, FileMode.Open, FileAccess.Read))
                    {
                        this.BackgroundImage = Image.FromStream(fs);
                    }
                    this.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), _settings.LimitDesktopImageSizeMode);
                }
                catch { }
            }

            // 4. Initialize UI
            SetupUI();

            // 5. Preview specific prompt
            if (_isPreview) SetupPreviewControls();

            // 6. Key Handlers
            this.KeyDown += (s, e) =>
            {
                // ESC always closes in preview mode so the admin doesn't get locked out
                if (_isPreview && e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }

                // Ctrl + X Bypass logic (Standard functionality)
                if (e.Control && e.KeyCode == Keys.X)
                {
                    if (PasswordDialog.Authenticate())
                    {
                        _settings.LastBypassDate = DateTime.Today;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            };

            // 7. Auto-Shutdown Timer (Disabled in Preview)
            if (_settings.LimitShutdownAfter3Min && !isPreview && _shutdownAction != null)
            {
                shutdownTimer = new System.Windows.Forms.Timer { Interval = 180000 };
                shutdownTimer.Tick += (s, e) =>
                {
                    shutdownTimer.Stop();
                    _shutdownAction.Invoke();
                };
                shutdownTimer.Start();
            }
        }

        private void SetupUI()
        {
            // Use TableLayoutPanel to manage segments
            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3, // 0: Msg, 1: ReturningTime, 2: BypassHint
                BackColor = Color.Transparent
            };

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f)); // Msg occupies top half
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f)); // ReturningTime occupies bottom half
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Hint at the very bottom

            // 1. ADD MAIN MESSAGE
            Label lblMsg = new Label
            {
                Text = _settings.LimitMessage,
                ForeColor = Color.FromName(_settings.LimitTextColor),
                Font = new Font(_settings.LimitFontFamily, _settings.LimitFontSize, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblMsg, 0, 0);

            // 2. ADD RETURNING TIME (SEPARATE ATTRIBUTES)
            if (_settings.LimitShowReturningTime)
            {
                string openTime = $"{_settings.LimitDesktopHourOpen}:{_settings.LimitDesktopMinOpen} {_settings.LimitDesktopAMPMOpen}";
                Label lblRet = new Label
                {
                    Text = $"Returning at {openTime}",
                    ForeColor = Color.FromName(_settings.LimitReturningTextColor),
                    Font = new Font(_settings.LimitReturningFontFamily, _settings.LimitReturningFontSize, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.TopCenter, // Placed below the message
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(lblRet, 0, 1);
            }

            // 3. ADD BYPASS HINT
            if (_settings.LimitShowBypassInstructions)
            {
                Label lblHint = new Label
                {
                    Text = "Staff? Press Ctrl + X to unlock",
                    ForeColor = Color.FromName(_settings.LimitTextColor),
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 60,
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(lblHint, 0, 2);
            }

            this.Controls.Add(panel);
            panel.BringToFront();
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

        // --- Low Level Keyboard Hook Logic (Blocks WinKey, Alt+Tab, Alt+F4) ---

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT { public int vkCode; public int scanCode; public int flags; }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT info = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                bool isAltTab = (info.vkCode == 9 && (info.flags & 32) != 0);
                bool isWinKey = (info.vkCode == 91 || info.vkCode == 92);
                bool isAltF4 = (info.vkCode == 115 && (info.flags & 32) != 0);

                if (isAltTab || isWinKey || isAltF4) return (IntPtr)1;
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
            // If it's a live lock and timer is still running, prevent closing unless authorized
            if (!_isPreview && e.CloseReason == CloseReason.UserClosing && this.DialogResult != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }

            if (shutdownTimer != null) shutdownTimer.Stop();
            if (_hhk != IntPtr.Zero) UnhookWindowsHookEx(_hhk);
            base.OnFormClosing(e);
        }
    }
}