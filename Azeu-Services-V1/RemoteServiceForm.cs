using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class RemoteServiceForm : Form
    {
        private AppSettings _settings;
        private System.Windows.Forms.Timer logUpdateTimer; // Explicitly defined to fix ambiguity

        public RemoteServiceForm()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            LoadFields();

            // Setup real-time log viewer using the Windows Forms Timer
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
            chkEnable.Checked = _settings.EnableRemoteService;
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
                    // Only read the last 50 lines to keep the UI smooth
                    string[] allLines = File.ReadAllLines(logPath);
                    int start = Math.Max(0, allLines.Length - 50);
                    txtLogs.Text = string.Join(Environment.NewLine, allLines, start, allLines.Length - start);

                    // Auto-scroll to the bottom of the log
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

            _settings.EnableRemoteService = chkEnable.Checked;
            _settings.WebSocketUrl = txtUrl.Text;
            _settings.WebSocketToken = txtToken.Text;

            AppSettings.Save(_settings);

            // Apply changes immediately by restarting the manager
            RemoteServiceManager.Instance.Start();

            MessageBox.Show("Remote settings saved and service restarted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // Update settings temporarily in the manager to test the connection
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

        // --- DESIGNER COMPONENTS ---
        private System.ComponentModel.IContainer components = null;
        private CheckBox chkEnable;
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
            this.chkEnable = new CheckBox();
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
            // chkEnable
            // 
            this.chkEnable.Text = "Enable Remote Management Service";
            this.chkEnable.Location = new Point(20, 20);
            this.chkEnable.AutoSize = true;
            // 
            // label1
            // 
            this.label1.Text = "WebSocket URL (ws://ip:port):";
            this.label1.Location = new Point(20, 55);
            this.label1.AutoSize = true;
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new Point(20, 75);
            this.txtUrl.Width = 340;
            // 
            // label2
            // 
            this.label2.Text = "Security Token:";
            this.label2.Location = new Point(20, 110);
            this.label2.AutoSize = true;
            // 
            // txtToken
            // 
            this.txtToken.Location = new Point(20, 130);
            this.txtToken.Width = 340;
            // 
            // lblStatus
            // 
            this.lblStatus.Text = "Status: Disconnected";
            this.lblStatus.Location = new Point(20, 165);
            this.lblStatus.ForeColor = Color.DarkBlue;
            this.lblStatus.Width = 300;
            // 
            // txtLogs
            // 
            this.txtLogs.Location = new Point(20, 195);
            this.txtLogs.Size = new Size(340, 150);
            this.txtLogs.ReadOnly = true;
            this.txtLogs.BackColor = Color.Black;
            this.txtLogs.ForeColor = Color.Lime;
            this.txtLogs.Font = new Font("Consolas", 8F);
            // 
            // btnSave
            // 
            this.btnSave.Text = "Save & Apply";
            this.btnSave.Location = new Point(260, 360);
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(btnSave_Click);
            // 
            // btnTest
            // 
            this.btnTest.Text = "Quick Connect";
            this.btnTest.Location = new Point(20, 360);
            this.btnTest.Width = 100;
            this.btnTest.Click += new EventHandler(btnTest_Click);
            // 
            // RemoteServiceForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(385, 410);
            this.Text = "Remote Service Configuration";
            this.Controls.AddRange(new Control[] { chkEnable, label1, txtUrl, label2, txtToken, lblStatus, txtLogs, btnSave, btnTest });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}