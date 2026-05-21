using System;
using System.Drawing;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public class LimitNotificationForm : Form
    {
        public LimitNotificationForm(string message, Point countdownLocation)
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.Width = 250;
            this.Height = 120;
            this.Text = "Pisonet Notice";

            // Position it exactly above the Countdown Form
            this.Location = new Point(countdownLocation.X, countdownLocation.Y - this.Height - 5);

            Label lbl = new Label()
            {
                Text = message,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Button btn = new Button()
            {
                Text = "Dismiss",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            btn.Click += (s, e) => this.Close();

            this.Controls.Add(lbl);
            this.Controls.Add(btn);

            // Clicking anywhere on form closes it
            this.Click += (s, e) => this.Close();
            lbl.Click += (s, e) => this.Close();
        }
    }
}