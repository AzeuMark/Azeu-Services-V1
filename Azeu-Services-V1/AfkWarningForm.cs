using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class AfkWarningForm : Form
    {
        // Customizable properties for the Designer appearance
        private int cornerRadius = 20;
        private int borderThickness = 2;
        private Label lblWarning;
        private Label label1;
        private Button closeAFKWarnDialogBtn;
        private Color borderColor = Color.Red;

        public AfkWarningForm()
        {
            InitializeComponent();
            // This ensures the form redraws correctly when you resize it in the designer
            this.Resize += (s, e) => UpdateRegion();
        }

        protected override bool ShowWithoutActivation => true;

        public void UpdateCountdown(int seconds)
        {
            if (this.IsHandleCreated)
            {
                lblWarning.Text = $"Shutting down in {seconds} seconds.";
            }
        }

        private void UpdateRegion()
        {
            // Create a rounded rectangle path for the form shape
            using (GraphicsPath path = new GraphicsPath())
            {
                float radius = cornerRadius;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                this.Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the 2px Stroke Outline inside the rounded region
            using (GraphicsPath path = new GraphicsPath())
            {
                float radius = cornerRadius;
                // Subtract half thickness to keep the border fully visible inside the region
                float offset = borderThickness / 2f;
                float w = this.Width - borderThickness;
                float h = this.Height - borderThickness;

                path.AddArc(offset, offset, radius, radius, 180, 90);
                path.AddArc(w - radius + offset, offset, radius, radius, 270, 90);
                path.AddArc(w - radius + offset, h - radius + offset, radius, radius, 0, 90);
                path.AddArc(offset, h - radius + offset, radius, radius, 90, 90);
                path.CloseAllFigures();

                using (Pen pen = new Pen(borderColor, borderThickness))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        // --- DESIGNER COMPONENTS ---
        private System.ComponentModel.IContainer components = null;
        private Label lblWarning2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AfkWarningForm));
            lblWarning2 = new Label();
            lblWarning = new Label();
            label1 = new Label();
            closeAFKWarnDialogBtn = new Button();
            SuspendLayout();
            // 
            // lblWarning2
            // 
            lblWarning2.BackColor = Color.Snow;
            lblWarning2.Dock = DockStyle.Top;
            lblWarning2.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            lblWarning2.ForeColor = Color.Red;
            lblWarning2.Location = new Point(0, 0);
            lblWarning2.Name = "lblWarning2";
            lblWarning2.Size = new Size(700, 91);
            lblWarning2.TabIndex = 0;
            lblWarning2.Text = "AFK DETECTED!";
            lblWarning2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblWarning
            // 
            lblWarning.Dock = DockStyle.Fill;
            lblWarning.Font = new Font("Segoe UI", 15F);
            lblWarning.ForeColor = SystemColors.HotTrack;
            lblWarning.Location = new Point(0, 227);
            lblWarning.Name = "lblWarning";
            lblWarning.Size = new Size(700, 73);
            lblWarning.TabIndex = 1;
            lblWarning.Text = "Shutting down in 30 seconds.";
            lblWarning.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI", 20F);
            label1.ForeColor = SystemColors.InactiveCaptionText;
            label1.Location = new Point(0, 91);
            label1.Name = "label1";
            label1.Size = new Size(700, 136);
            label1.TabIndex = 2;
            label1.Text = "PLEASE MOVE YOUR KEYBOARD OR MOUSE";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // closeAFKWarnDialogBtn
            // 
            closeAFKWarnDialogBtn.BackgroundImage = (Image)resources.GetObject("closeAFKWarnDialogBtn.BackgroundImage");
            closeAFKWarnDialogBtn.BackgroundImageLayout = ImageLayout.Stretch;
            closeAFKWarnDialogBtn.FlatAppearance.BorderSize = 0;
            closeAFKWarnDialogBtn.FlatAppearance.MouseDownBackColor = Color.Silver;
            closeAFKWarnDialogBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 64, 64);
            closeAFKWarnDialogBtn.FlatStyle = FlatStyle.Flat;
            closeAFKWarnDialogBtn.Location = new Point(626, 12);
            closeAFKWarnDialogBtn.Name = "closeAFKWarnDialogBtn";
            closeAFKWarnDialogBtn.Size = new Size(62, 62);
            closeAFKWarnDialogBtn.TabIndex = 3;
            closeAFKWarnDialogBtn.UseVisualStyleBackColor = true;
            closeAFKWarnDialogBtn.Click += closeAFKWarnDialogBtn_Click;
            // 
            // AfkWarningForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(700, 300);
            ControlBox = false;
            Controls.Add(closeAFKWarnDialogBtn);
            Controls.Add(lblWarning);
            Controls.Add(label1);
            Controls.Add(lblWarning2);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AfkWarningForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "IDLE DETECTION ALERT";
            TopMost = true;
            ResumeLayout(false);
        }

        private void closeAFKWarnDialogBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}