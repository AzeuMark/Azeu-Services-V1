namespace AzeuServices_V1
{
    partial class CountdownForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary> 
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CountdownForm));
            lblTimer = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // lblTimer
            // 
            lblTimer.Font = new Font("Segoe UI", 24.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTimer.Location = new Point(0, 13);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(149, 45);
            lblTimer.TabIndex = 0;
            lblTimer.Text = "05:00";
            lblTimer.TextAlign = ContentAlignment.MiddleCenter;
            lblTimer.DoubleClick += CountdownForm_DoubleClick;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(149, 24);
            label1.TabIndex = 1;
            label1.Text = "AUTO SHUTDOWN";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.DoubleClick += CountdownForm_DoubleClick;
            // 
            // CountdownForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(149, 59);
            Controls.Add(label1);
            Controls.Add(lblTimer);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CountdownForm";
            Text = "CountdownForm";
            ResumeLayout(false);
        }

        #endregion

        private Label lblTimer;
        private Label label1;
    }
}