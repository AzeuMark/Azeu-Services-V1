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
            this.BackColor = Color.FromArgb(30, 30, 30); // Dark theme

            lblMessage.Text = message;

            // Positioning Logic: Bottom Right, above the Countdown widget
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            // Shift up by 80 pixels to avoid overlapping the Countdown widget
            this.Location = new Point(workingArea.Right - this.Width - 10,
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