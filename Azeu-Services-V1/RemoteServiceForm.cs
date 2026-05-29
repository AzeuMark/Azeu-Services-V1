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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteServiceForm));
            txtUrl = new TextBox();
            txtToken = new TextBox();
            lblStatus = new Label();
            txtLogs = new RichTextBox();
            btnSave = new Button();
            btnTest = new Button();
            btnClearView = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtUrl
            // 
            txtUrl.Location = new Point(20, 40);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new Size(340, 23);
            txtUrl.TabIndex = 1;
            // 
            // txtToken
            // 
            txtToken.Location = new Point(20, 95);
            txtToken.Name = "txtToken";
            txtToken.Size = new Size(340, 23);
            txtToken.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.ForeColor = Color.DarkBlue;
            lblStatus.Location = new Point(20, 130);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(300, 23);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Status: Disconnected";
            // 
            // txtLogs
            // 
            txtLogs.BackColor = Color.Black;
            txtLogs.Font = new Font("Consolas", 8F);
            txtLogs.ForeColor = Color.Lime;
            txtLogs.Location = new Point(20, 160);
            txtLogs.Name = "txtLogs";
            txtLogs.ReadOnly = true;
            txtLogs.Size = new Size(340, 155);
            txtLogs.TabIndex = 5;
            txtLogs.Text = "";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(240, 360);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(120, 23);
            btnSave.TabIndex = 8;
            btnSave.Text = "Save Credentials";
            btnSave.Click += btnSave_Click;
            // 
            // btnTest
            // 
            btnTest.Location = new Point(20, 360);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(120, 23);
            btnTest.TabIndex = 7;
            btnTest.Text = "Test Connection";
            btnTest.Click += btnTest_Click;
            // 
            // btnClearView
            // 
            btnClearView.Location = new Point(20, 320);
            btnClearView.Name = "btnClearView";
            btnClearView.Size = new Size(340, 23);
            btnClearView.TabIndex = 6;
            btnClearView.Text = "Clear Log View";
            btnClearView.Click += btnClearView_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 20);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 0;
            label1.Text = "WebSocket URL:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 75);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 2;
            label2.Text = "Security Token:";
            // 
            // RemoteServiceForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(385, 410);
            Controls.Add(label1);
            Controls.Add(txtUrl);
            Controls.Add(label2);
            Controls.Add(txtToken);
            Controls.Add(lblStatus);
            Controls.Add(txtLogs);
            Controls.Add(btnClearView);
            Controls.Add(btnTest);
            Controls.Add(btnSave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "RemoteServiceForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Remote Credentials";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}