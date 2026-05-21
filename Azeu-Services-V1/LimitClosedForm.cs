using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        private Button? btnFullScreen;
        private Label? lblEscPrompt;

        public LimitClosedForm(AppSettings settings, Action? shutdownAction, bool isPreview = false)
        {
            _settings = settings;
            _shutdownAction = shutdownAction;
            _isPreview = isPreview;
            this.KeyPreview = true;

            this.BackColor = Color.Black;
            this.ShowInTaskbar = isPreview;

            if (isPreview)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Size = new Size(800, 500);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.TopMost = false;
                this.Text = "Preview - Desktop Limit Dialog";
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = Screen.PrimaryScreen.Bounds.Location;

                _delegate = HookCallback;
                _hhk = SetHook(_delegate);
            }

            if (!string.IsNullOrEmpty(settings.LimitDesktopImagePath) && System.IO.File.Exists(settings.LimitDesktopImagePath))
            {
                try
                {
                    this.BackgroundImage = Image.FromFile(settings.LimitDesktopImagePath);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch { }
            }

            SetupUI();

            if (isPreview) SetupPreviewControls();

            // Password/Hotkey Logic
            this.KeyDown += (s, e) =>
            {
                if (_isPreview && e.KeyCode == Keys.Escape)
                {
                    if (this.WindowState == FormWindowState.Maximized) ExitFullScreen();
                    else this.Close();
                    return;
                }

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

            // 3-Minute Auto-Shutdown Timer (Disabled if preview)
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

        private void SetupPreviewControls()
        {
            btnFullScreen = new Button()
            {
                Text = "Full Screen",
                Size = new Size(100, 30),
                Location = new Point(this.ClientSize.Width - 120, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.DimGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            btnFullScreen.Click += (s, e) => EnterFullScreen();

            lblEscPrompt = new Label()
            {
                Text = " Press Esc to exit full screen ",
                ForeColor = Color.Black,
                BackColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(this.ClientSize.Width - 220, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(btnFullScreen);
            this.Controls.Add(lblEscPrompt);

            btnFullScreen.BringToFront();
            lblEscPrompt.BringToFront();
        }

        private void EnterFullScreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            if (btnFullScreen != null) btnFullScreen.Visible = false;
            if (lblEscPrompt != null)
            {
                lblEscPrompt.Visible = true;
                lblEscPrompt.BringToFront(); // Force to top
            }
        }
        private void ExitFullScreen()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
            if (btnFullScreen != null) btnFullScreen.Visible = true;
            if (lblEscPrompt != null) lblEscPrompt.Visible = false;
        }

        private void SetupUI()
        {
            string openTime = $"{_settings.LimitDesktopOpenHour}:{_settings.LimitDesktopOpenMin} {_settings.LimitDesktopOpenAMPM}";
            Label msgLabel = new Label()
            {
                Text = $"PISONET IS NOW CLOSED\nReturning tomorrow at {openTime}",
                ForeColor = Color.White,
                Font = new Font("Arial", 36, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            this.Controls.Add(msgLabel);
        }

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
            if (shutdownTimer != null) shutdownTimer.Stop();
            if (_hhk != IntPtr.Zero) UnhookWindowsHookEx(_hhk);
            base.OnFormClosing(e);
        }
    }
}