using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteServiceForm : Form
    {
        private AppSettings _lastSavedSettings;
        private System.Windows.Forms.Timer logUpdateTimer;

        private bool _isLogPaused = false;
        private bool _isWaitingForTest = false;
        private bool _testSuccess = false; // Tracks if the current connection is a successful test
        private bool _isDirty = false;    // Tracks if UI values match settings.json

        public RemoteServiceForm()
        {
            InitializeComponent();
            _lastSavedSettings = AppSettings.Load();
            LoadFields();

            logUpdateTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            logUpdateTimer.Tick += (s, e) => {
                if (!_isLogPaused) LoadLogs();
            };
            logUpdateTimer.Start();

            RemoteServiceManager.Instance.OnStatusChanged += (status) => {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() => {
                        lblStatus.Text = "Status: " + status;

                        if (_isWaitingForTest && status == "Connected")
                        {
                            _isWaitingForTest = false;
                            _testSuccess = true; // Mark as successful test
                            MessageBox.Show("Connection test successful! Current credentials work.", "Remote Service", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }));
                }
            };

            // Wire up text change detection
            txtUrl.TextChanged += (s, e) => { _isDirty = true; _testSuccess = false; };
            txtToken.TextChanged += (s, e) => { _isDirty = true; _testSuccess = false; };
        }

        private void LoadFields()
        {
            txtUrl.Text = _lastSavedSettings.WebSocketUrl;
            txtToken.Text = _lastSavedSettings.WebSocketToken;
            _isDirty = false;
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

            // Update the settings object
            _lastSavedSettings.WebSocketUrl = txtUrl.Text;
            _lastSavedSettings.WebSocketToken = txtToken.Text;

            // Persistent Save to Disk
            AppSettings.Save(_lastSavedSettings);

            _isDirty = false;
            _testSuccess = false;

            // Ensure background service is running with new saved values
            RemoteServiceManager.Instance.Start(_lastSavedSettings);

            MessageBox.Show("Credentials saved to settings.json.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text)) { MessageBox.Show("Please enter a WebSocket URL."); return; }

            _isLogPaused = false;
            txtLogs.Clear();
            RemoteServiceManager.Instance.WriteLog("--- Manual Connection Test Started (Unsaved) ---");

            _isWaitingForTest = true;
            _testSuccess = false;

            // CRITICAL: Create a temporary settings object for testing
            // This does NOT call AppSettings.Save()
            var tempSettings = new AppSettings
            {
                EnableRemoteService = true, // Force enable for test
                WebSocketUrl = txtUrl.Text,
                WebSocketToken = txtToken.Text
            };

            // Pass the temporary settings to the manager
            RemoteServiceManager.Instance.Start(tempSettings);
        }

        private void btnClearView_Click(object sender, EventArgs e)
        {
            txtLogs.Clear();
            _isLogPaused = true;
            RemoteServiceManager.Instance.WriteLog("Log view cleared by user.");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If UI values don't match disk, or we have a successful unsaved test connection running
            if (_isDirty || _testSuccess)
            {
                DialogResult result = MessageBox.Show(
                    "You have unsaved remote credentials. Save changes before closing?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnSave_Click(this, EventArgs.Empty);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    // User clicked NO. 
                    // Revert the background service to whatever is actually saved in settings.json
                    RemoteServiceManager.Instance.Start(AppSettings.Load());
                }
            }

            if (logUpdateTimer != null)
            {
                logUpdateTimer.Stop();
                logUpdateTimer.Dispose();
            }
            base.OnFormClosing(e);
        }

        // --- DESIGNER COMPONENTS ---
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUrl;
        private TextBox txtToken;
        private Label lblStatus;
        private RichTextBox txtLogs;
        private Button btnSave;
        private Button btnTest;
        private Button btnClearView;
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
            this.btnClearView = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Text = "WebSocket URL:";
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
            this.label2.Text = "Security Token:";
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
            this.txtLogs.Size = new Size(340, 155);
            this.txtLogs.ReadOnly = true;
            this.txtLogs.BackColor = Color.Black;
            this.txtLogs.ForeColor = Color.Lime;
            this.txtLogs.Font = new Font("Consolas", 8F);
            // 
            // btnClearView
            // 
            this.btnClearView.Text = "Clear Log View";
            this.btnClearView.Location = new Point(20, 320);
            this.btnClearView.Width = 340;
            this.btnClearView.Click += new EventHandler(btnClearView_Click);
            // 
            // btnTest
            // 
            this.btnTest.Text = "Test Connection";
            this.btnTest.Location = new Point(20, 360);
            this.btnTest.Width = 120;
            this.btnTest.Click += new EventHandler(btnTest_Click);
            // 
            // btnSave
            // 
            this.btnSave.Text = "Save Credentials";
            this.btnSave.Location = new Point(240, 360);
            this.btnSave.Width = 120;
            this.btnSave.Click += new EventHandler(btnSave_Click);
            // 
            // RemoteServiceForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(385, 410);
            this.Text = "Remote Credentials";
            this.Controls.AddRange(new Control[] { label1, txtUrl, label2, txtToken, lblStatus, txtLogs, btnClearView, btnTest, btnSave });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}