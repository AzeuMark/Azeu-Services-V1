using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace AzeuServices_V1
{
    public partial class NoSmokingForm : Form
    {
        private int secondsLeft;
        private AppSettings _settings;
        private IntPtr _hhk = IntPtr.Zero;
        private LowLevelKeyboardProc? _delegate;
        private bool _isPreview;

        private Button? btnClose;
        private Label? lblEscPrompt;

        public NoSmokingForm(AppSettings settings, bool isPreview = false)
        {
            _settings = settings;
            _isPreview = isPreview;
            this.KeyPreview = true;

            InitializeComponent();

            this.BackColor = Color.FromName(settings.NoSmokingBgColor);

            // Apply Background Image
            if (!string.IsNullOrEmpty(settings.NoSmokingImagePath) && File.Exists(settings.NoSmokingImagePath))
            {
                try
                {
                    using (var fs = new FileStream(settings.NoSmokingImagePath, FileMode.Open, FileAccess.Read))
                    {
                        this.BackgroundImage = Image.FromStream(fs);
                    }
                    this.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), settings.NoSmokingImageSizeMode);
                }
                catch { }
            }

            this.ShowInTaskbar = isPreview;

            if (isPreview)
            {
                this.Text = "Preview Mode";
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized; // Force full screen immediately for test
                this.TopMost = true;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                _delegate = HookCallback;
                _hhk = SetHook(_delegate);
            }

            SetupUI();
            StartTimer();

            this.KeyDown += (s, e) => {
                if (_isPreview && e.KeyCode == Keys.Escape) this.Close();
            };
        }

        private void StartTimer()
        {
            this.secondsLeft = _settings.NoSmokingDuration;
            if (btnClose == null) return;

            if (!_isPreview && secondsLeft > 0)
            {
                btnClose.Enabled = false;
                btnClose.Text = $"{_settings.NoSmokingButtonText} ({secondsLeft})";
            }

            System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            countdownTimer.Tick += (s, e) => {
                if (secondsLeft > 0)
                {
                    secondsLeft--;
                    if (!_isPreview) btnClose.Text = $"{_settings.NoSmokingButtonText} ({secondsLeft})";
                }
                else
                {
                    btnClose.Text = _settings.NoSmokingButtonText;
                    btnClose.Enabled = true;
                }
            };
            countdownTimer.Start();
        }

        private void SetupUI()
        {
            TableLayoutPanel panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.Transparent };
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Font messageFont;
            try { messageFont = new Font(_settings.NoSmokingFontFamily, _settings.NoSmokingFontSize, FontStyle.Bold); }
            catch { messageFont = new Font("Arial", _settings.NoSmokingFontSize, FontStyle.Bold); }

            Label lblMsg = new Label
            {
                Text = _settings.NoSmokingMessage,
                ForeColor = Color.FromName(_settings.NoSmokingTextColor),
                Font = messageFont,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            btnClose = new Button
            {
                Size = new Size(_settings.NoSmokingButtonWidth, _settings.NoSmokingButtonHeight),
                Anchor = AnchorStyles.None,
                Font = new Font("Arial", _settings.NoSmokingButtonFontSize, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Text = _settings.NoSmokingButtonText,
                BackColor = Color.FromName(_settings.NoSmokingButtonBgColor),
                ForeColor = Color.FromName(_settings.NoSmokingButtonTextColor)
            };

            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            if (_settings.NoSmokingButtonRadius > 0)
            {
                btnClose.Paint += (s, e) => {
                    Rectangle bounds = new Rectangle(0, 0, btnClose.Width, btnClose.Height);
                    int r = _settings.NoSmokingButtonRadius * 2;
                    if (r > btnClose.Height) r = btnClose.Height;
                    GraphicsPath path = new GraphicsPath();
                    path.AddArc(bounds.X, bounds.Y, r, r, 180, 90);
                    path.AddArc(bounds.X + bounds.Width - r, bounds.Y, r, r, 270, 90);
                    path.AddArc(bounds.X + bounds.Width - r, bounds.Y + bounds.Height - r, r, r, 0, 90);
                    path.AddArc(bounds.X, bounds.Y + bounds.Height - r, r, r, 90, 90);
                    path.CloseAllFigures();
                    btnClose.Region = new Region(path);
                };
            }

            // ESC Prompt for test mode
            if (_isPreview)
            {
                lblEscPrompt = new Label
                {
                    Text = "PRESS ESC TO EXIT FULL SCREEN",
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(150, 0, 0, 0), // Semi-transparent black
                    AutoSize = true,
                    Padding = new Padding(10),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Location = new Point(20, 20)
                };
                this.Controls.Add(lblEscPrompt);
                lblEscPrompt.BringToFront();
            }

            panel.Controls.Add(lblMsg, 0, 0);
            panel.Controls.Add(btnClose, 0, 1);
            panel.Padding = new Padding(0, 0, 0, _settings.NoSmokingButtonBottomMargin);

            this.Controls.Add(panel);
            panel.BringToFront();
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT { public int vkCode; public int scanCode; public int flags; }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT info = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (info.vkCode == 91 || info.vkCode == 92 || (info.vkCode == 9 && (info.flags & 32) != 0)) return (IntPtr)1;
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
            if (!_isPreview && e.CloseReason == CloseReason.UserClosing && !btnClose.Enabled)
            {
                e.Cancel = true;
                return;
            }
            if (_hhk != IntPtr.Zero) UnhookWindowsHookEx(_hhk);
            base.OnFormClosing(e);
        }
    }
}