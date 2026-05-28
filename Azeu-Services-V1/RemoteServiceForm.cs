using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteServiceForm : Form
    {
        private AppSettings _settings;
        private System.Windows.Forms.Timer logUpdateTimer;

        public RemoteServiceForm()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            LoadFields();

            // Setup real-time log viewer
            logUpdateTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            logUpdateTimer.Tick += (s, e) => LoadLogs();
            logUpdateTimer.Start();

            // Listen for status changes from the manager
            RemoteServiceManager.Instance.OnStatusChanged += (status) => {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() => lblStatus.Text = "Status: " + status));
                }
            };
        }

        private void LoadFields()
        {
            txtUrl.Text = _settings.WebSocketUrl;
            txtToken.Text = _settings.WebSocketToken;
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "websocket-cache", "websocket.log");
                if (File.Exists(logPath))
                {
                    string[] allLines = File.ReadAllLines(logPath);
                    int start = Math.Max(0, allLines.Length - 50);
                    txtLogs.Text = string.Join(Environment.NewLine, allLines, start, allLines.Length - start);
                    txtLogs.SelectionStart = txtLogs.Text.Length;
                    txtLogs.ScrollToCaret();
                }
            }
            catch { }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtToken.Text.Length < 10)
            {
                MessageBox.Show("Security Token must be at least 10 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save to settings object
            _settings.WebSocketUrl = txtUrl.Text;
            _settings.WebSocketToken = txtToken.Text;

            AppSettings.Save(_settings);

            // Apply changes to the background service immediately
            RemoteServiceManager.Instance.Start();

            MessageBox.Show("Remote credentials saved. If the service is enabled in the main menu, it is now active.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // Temporarily use values from textboxes to test connection without a full save
            _settings.WebSocketUrl = txtUrl.Text;
            _settings.WebSocketToken = txtToken.Text;
            RemoteServiceManager.Instance.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (logUpdateTimer != null)
            {
                logUpdateTimer.Stop();
                logUpdateTimer.Dispose();
            }
            base.OnFormClosing(e);
        }

        // --- DESIGNER COMPONENTS (NO CHECKBOX) ---
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUrl;
        private TextBox txtToken;
        private Label lblStatus;
        private RichTextBox txtLogs;
        private Button btnSave;
        private Button btnTest;
        private Label label1;
        private Label label2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtUrl = new TextBox();
            this.txtToken = new TextBox();
            this.lblStatus = new Label();
            this.txtLogs = new RichTextBox();
            this.btnSave = new Button();
            this.btnTest = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Text = "WebSocket URL (e.g. ws://192.168.1.20:3000):";
            this.label1.Location = new Point(20, 20);
            this.label1.AutoSize = true;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new Point(20, 40);
            this.txtUrl.Width = 340;
            // 
            // label2
            // 
            this.label2.Text = "Security Token (Min 10 chars):";
            this.label2.Location = new Point(20, 75);
            this.label2.AutoSize = true;
            // 
            // txtToken
            // 
            this.txtToken.Location = new Point(20, 95);
            this.txtToken.Width = 340;
            // 
            // lblStatus
            // 
            this.lblStatus.Text = "Status: Disconnected";
            this.lblStatus.Location = new Point(20, 130);
            this.lblStatus.ForeColor = Color.DarkBlue;
            this.lblStatus.Width = 300;
            // 
            // txtLogs
            // 
            this.txtLogs.Location = new Point(20, 160);
            this.txtLogs.Size = new Size(340, 185);
            this.txtLogs.ReadOnly = true;
            this.txtLogs.BackColor = Color.Black;
            this.txtLogs.ForeColor = Color.Lime;
            this.txtLogs.Font = new Font("Consolas", 8F);
            // 
            // btnSave
            // 
            this.btnSave.Text = "Save Credentials";
            this.btnSave.Location = new Point(240, 360);
            this.btnSave.Width = 120;
            this.btnSave.Click += new EventHandler(btnSave_Click);
            // 
            // btnTest
            // 
            this.btnTest.Text = "Test Connection";
            this.btnTest.Location = new Point(20, 360);
            this.btnTest.Width = 120;
            this.btnTest.Click += new EventHandler(btnTest_Click);
            // 
            // RemoteServiceForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(385, 410);
            this.Text = "Remote Credentials";
            this.Controls.AddRange(new Control[] { label1, txtUrl, label2, txtToken, lblStatus, txtLogs, btnSave, btnTest });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}