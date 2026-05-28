using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteMessageForm : Form
    {
        // Static list to track all open message forms
        private static List<RemoteMessageForm> _activeForms = new List<RemoteMessageForm>();
        private const int MaxForms = 5;
        private System.Windows.Forms.Timer _autoCloseTimer;

        public RemoteMessageForm(string message)
        {
            InitializeComponent();
            this.Text = "System Message";
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(30, 30, 30);

            lblMessage.Text = message;

            // Handle limits and add to list
            if (_activeForms.Count >= MaxForms)
            {
                // Close the oldest one (index 0)
                _activeForms[0].Close();
            }

            _activeForms.Add(this);

            // Set up the 30-second auto-close timer
            _autoCloseTimer = new System.Windows.Forms.Timer { Interval = 30000 };
            _autoCloseTimer.Tick += (s, e) => this.Close();
            _autoCloseTimer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Every time a new form loads, we tell ALL open forms to rearrange themselves
            RearrangeStacks();
        }

        /// <summary>
        /// This static method calculates the correct position for every open message.
        /// It ensures they are perfectly stacked above each other.
        /// </summary>
        private static void RearrangeStacks()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // Base coordinates (where the first/newest message sits)
            int baseX = workingArea.Right - 300 - 10; // 300 is form width
            int baseY = workingArea.Bottom - 140 - 80; // 140 is form height, 80 is margin for countdown

            // Loop through the list from newest to oldest
            // We want the NEWEST (last in list) at the bottom, and OLDER ones above it.
            for (int i = 0; i < _activeForms.Count; i++)
            {
                // Index from the end: the most recently added is index 0 for the loop logic
                int positionFromBottom = _activeForms.Count - 1 - i;

                // Calculate Y: Start at bottom, and subtract (Height + 10px margin) for each step up
                int newY = baseY - (positionFromBottom * (140 + 10));

                // Update the location of the form
                _activeForms[i].Location = new Point(baseX, newY);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RemoteMessageForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Remove from tracking list
            _activeForms.Remove(this);

            // Reposition the remaining forms so they "drop down" into the empty space
            RearrangeStacks();

            _autoCloseTimer?.Stop();
            _autoCloseTimer?.Dispose();
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
            this.lblMessage.Click += new EventHandler(RemoteMessageForm_Click);
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
            this.Click += new EventHandler(RemoteMessageForm_Click);
            this.ResumeLayout(false);
        }
    }
}