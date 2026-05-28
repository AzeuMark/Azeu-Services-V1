using System;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class AfkWarningForm : Form
    {
        public AfkWarningForm()
        {
            InitializeComponent();
        }

        // This property allows the form to show up without stealing focus
        // from the user's active game or application.
        protected override bool ShowWithoutActivation => true;

        public void UpdateCountdown(int seconds)
        {
            if (this.IsHandleCreated)
            {
                lblWarning.Text = $"If you don't move your Keyboard or Mouse right now the PC will shutdown in {seconds} seconds.";
            }
        }

        // --- DESIGNER COMPONENTS ---
        private System.ComponentModel.IContainer components = null;
        private Label lblWarning;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblWarning = new Label();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.Dock = DockStyle.Fill;
            this.lblWarning.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblWarning.ForeColor = Color.Red;
            this.lblWarning.Location = new Point(0, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new Size(500, 150);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "If you don't move your Keyboard or Mouse right now the PC will shutdown in 30 seconds.";
            this.lblWarning.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AfkWarningForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(30, 30, 30); // Dark theme
            this.ClientSize = new Size(500, 150);
            this.ControlBox = false; // Removes X, Min, Max buttons
            this.Controls.Add(this.lblWarning);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AfkWarningForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "IDLE DETECTION ALERT";
            this.TopMost = true;
            this.ResumeLayout(false);
        }
    }
}