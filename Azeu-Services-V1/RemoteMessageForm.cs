using System;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteMessageForm : Form
    {
        public RemoteMessageForm(string message)
        {
            InitializeComponent();
            this.Text = "System Message";
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            //this.BackColor = Color.FromArgb(30, 30, 30); // Dark theme

            lblMessage.Text = message;

            // Positioning Logic: Bottom Right, above the Countdown widget
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            // Shift up by 80 pixels to avoid overlapping the Countdown widget
            this.Location = new Point(workingArea.Right - this.Width - 5,
                                      workingArea.Bottom - this.Height - 80);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RemoteMessageForm_Click(object sender, EventArgs e)
        {
            this.Close(); // Dismiss on click anywhere
        }

        // Standard WinForms Designer code embedded for simplicity
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
            lblMessage = new Label();
            btnOk = new Button();
            lblHeader = new Label();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.AutoEllipsis = true;
            lblMessage.Font = new Font("Segoe UI", 10F);
            lblMessage.ForeColor = SystemColors.MenuText;
            lblMessage.Location = new Point(12, 35);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(276, 60);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "ahahahahahaha HAHAHAHAHAHAHAHAHAHAA";
            lblMessage.Click += RemoteMessageForm_Click;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.FromArgb(0, 122, 204);
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(213, 102);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 26);
            btnOk.TabIndex = 0;
            btnOk.Text = "Nigga";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += btnOk_Click;
            // 
            // lblHeader
            // 
            lblHeader.BackColor = Color.FromArgb(45, 45, 48);
            lblHeader.Dock = DockStyle.Top;
            lblHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Location = new Point(0, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(300, 25);
            lblHeader.TabIndex = 2;
            lblHeader.Text = "  AZEUMARK MESSAGE";
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RemoteMessageForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLightLight;
            ClientSize = new Size(300, 140);
            Controls.Add(btnOk);
            Controls.Add(lblMessage);
            Controls.Add(lblHeader);
            FormBorderStyle = FormBorderStyle.None;
            Name = "RemoteMessageForm";
            StartPosition = FormStartPosition.Manual;
            Click += RemoteMessageForm_Click;
            ResumeLayout(false);
        }
    }
}