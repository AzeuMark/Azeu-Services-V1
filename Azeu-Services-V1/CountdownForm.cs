using System;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class CountdownForm : Form
    {
        public Action? OnRequestOpen;
        public Action? OnRequestExit;
        public Action? OnRequestToggle;

        private ToolStripMenuItem? toggleMenuItem;

        public CountdownForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            
            // REMOVED: hardcoded this.TopMost = true;

            SetupContextMenu();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        // REMOVED: CreateParams override that was forcing WS_EX_TOPMOST (0x00000008)
        // Standard WinForms .TopMost property will now handle this correctly.

        private Image? GetImageFromResource(byte[] resourceData)
        {
            using (var ms = new System.IO.MemoryStream(resourceData))
            {
                return Image.FromStream(ms);
            }
        }

        public void PositionBottomRight()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(workingArea.Right - this.Width - 10,
                                      workingArea.Bottom - this.Height - 10);
        }

        public void UpdateTime(int secondsRemaining)
        {
            if (secondsRemaining < 0) secondsRemaining = 0;

            int mins = secondsRemaining / 60;
            int secs = secondsRemaining % 60;

            lblTimer.Text = string.Format("{0:00}:{1:00}", mins, secs);
        }

        public void SetAlertMode(bool isAlert)
        {
            this.BackColor = isAlert ? Color.Red : Color.FromArgb(45, 45, 48);
        }

        public void ApplyOpacity(int percent)
        {
            this.Opacity = percent / 100.0;
        }

        public void SetupContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            Image? openImg = GetImageFromResource(Properties.Resources.open_icon);
            menu.Items.Add("Open Settings", openImg, (s, e) => OnRequestOpen?.Invoke());

            Image? hideImg = GetImageFromResource(Properties.Resources.hide_icon);
            toggleMenuItem = new ToolStripMenuItem("Hide Widget", hideImg, (s, e) => OnRequestToggle?.Invoke());
            menu.Items.Add(toggleMenuItem);

            menu.Items.Add(new ToolStripSeparator());

            Image? exitImg = GetImageFromResource(Properties.Resources.exit_icon);
            menu.Items.Add("Exit System", exitImg, (s, e) => OnRequestExit?.Invoke());

            this.ContextMenuStrip = menu;
            lblTimer.ContextMenuStrip = menu;
        }

        public void UpdateMenuUI(bool isHidden)
        {
            if (toggleMenuItem == null) return;

            toggleMenuItem.Text = isHidden ? "Show Widget" : "Hide Widget";
            byte[] iconBytes = isHidden ? Properties.Resources.show_icon : Properties.Resources.hide_icon;
            toggleMenuItem.Image = GetImageFromResource(iconBytes);
        }

        private void CountdownForm_DoubleClick(object sender, EventArgs e)
        {
            OnRequestOpen?.Invoke();
        }

        public bool AllowClose = false;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !AllowClose)
            {
                e.Cancel = true;
                PositionBottomRight();
            }
            base.OnFormClosing(e);
        }
    }
}