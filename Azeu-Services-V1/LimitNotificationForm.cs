using System;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public class LimitNotificationForm : Form
    {
        public LimitNotificationForm(string message, Point anchorLocation)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.Width = 250;
            this.Height = 100;
            this.Text = "Pisonet Notice";
            this.ShowInTaskbar = false;

            // Calculate Location: 
            // anchorLocation.Y is either the Top of the Countdown HUD or the bottom of the screen.
            // We place this form 5 pixels above that point.
            this.Location = new Point(anchorLocation.X, anchorLocation.Y - this.Height - 5);

            Label lbl = new Label()
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Padding = new Padding(10)
            };

            // Clicking dismisses the warning
            lbl.Click += (s, e) => this.Close();
            this.Click += (s, e) => this.Close();

            this.Controls.Add(lbl);

            // Auto-close after 10 seconds. 
            // Explicitly naming the namespace to avoid CS0104 Ambiguity Error.
            System.Windows.Forms.Timer autoClose = new System.Windows.Forms.Timer { Interval = 10000 };
            autoClose.Tick += (s, e) => {
                autoClose.Stop();
                autoClose.Dispose();
                this.Close();
            };
            autoClose.Start();
        }
    }
}