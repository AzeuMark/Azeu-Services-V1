using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices; // Required for the fix
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteMessageForm : Form
    {
        private static List<RemoteMessageForm> _activeForms = new List<RemoteMessageForm>();
        private const int MaxForms = 5;
        private const int autoCloseSeconds = 5;
        private System.Windows.Forms.Timer _autoCloseTimer;

        // --- Win32 API to force TopMost ---
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_SHOWWINDOW = 0x0040;
        // ----------------------------------

        public RemoteMessageForm(string message)
        {
            InitializeComponent();
            this.Text = "System Message";
            this.TopMost = true; // Still keep this
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(30, 30, 30);

            lblMessage.Text = message;

            if (_activeForms.Count >= MaxForms)
            {
                if (_activeForms[0] != null && !_activeForms[0].IsDisposed)
                {
                    _activeForms[0].Close();
                }
            }
            _activeForms.Add(this);

            _autoCloseTimer = new System.Windows.Forms.Timer { Interval = autoCloseSeconds * 1000 };
            _autoCloseTimer.Tick += (s, e) => this.Close();
            _autoCloseTimer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RearrangeStacks();

            // CRITICAL FIX: Force the window to the top of the Z-Order immediately on load
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        protected override bool ShowWithoutActivation => true; // Prevents stealing keyboard focus but stays on top

        private static void RearrangeStacks()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int baseX = workingArea.Right - 300 - 10;
            int baseY = workingArea.Bottom - 140 - 80;

            for (int i = 0; i < _activeForms.Count; i++)
            {
                int positionFromBottom = _activeForms.Count - 1 - i;
                int newY = baseY - (positionFromBottom * (140 + 10));
                _activeForms[i].Location = new Point(baseX, newY);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _activeForms.Remove(this);
            RearrangeStacks();

            if (_autoCloseTimer != null)
            {
                _autoCloseTimer.Stop();
                _autoCloseTimer.Dispose();
            }
            base.OnFormClosed(e);
        }

        // --- DESIGNER CODE ---
        private System.ComponentModel.IContainer components = null;
        private Label lblMessage;
        private Button btnOk;
        private Label lblHeader;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.btnOk = new Button();
            this.lblHeader = new Label();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.BackColor = Color.FromArgb(45, 45, 48);
            this.lblHeader.Dock = DockStyle.Top;
            this.lblHeader.ForeColor = Color.White;
            this.lblHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblHeader.Location = new Point(0, 0);
            this.lblHeader.Size = new Size(300, 25);
            this.lblHeader.Text = "  ADMINISTRATOR MESSAGE";
            this.lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = Color.White;
            this.lblMessage.Font = new Font("Segoe UI", 10F);
            this.lblMessage.Location = new Point(12, 35);
            this.lblMessage.Size = new Size(276, 60);
            this.lblMessage.TextAlign = ContentAlignment.TopLeft;
            // 
            // btnOk
            // 
            this.btnOk.BackColor = Color.FromArgb(0, 122, 204);
            this.btnOk.FlatStyle = FlatStyle.Flat;
            this.btnOk.ForeColor = Color.White;
            this.btnOk.Location = new Point(213, 102);
            this.btnOk.Size = new Size(75, 26);
            this.btnOk.Text = "Got it";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new EventHandler(btnOk_Click);
            // 
            // RemoteMessageForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(300, 140);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblHeader);
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true; // Ensure this is set
            this.ResumeLayout(false);
        }
    }
}