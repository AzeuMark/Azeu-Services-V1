using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AzeuServices_V1
{
    public partial class LimitEditorForm : Form
    {
        private AppSettings _settings;
        private bool isInitializing = true;

        public LimitEditorForm()
        {
            InitializeComponent();
            _settings = AppSettings.Load();

            SetupEditor();
            LoadCurrentSettings();

            isInitializing = false;
            UpdatePreview();
        }

        private void SetupEditor()
        {
            string[] topFonts = { "Arial", "Arial Black", "Impact", "Segoe UI", "Verdana", "Tahoma", "Times New Roman" };
            cmbFontFamily.Items.AddRange(topFonts);
            cmbRetFontFamily.Items.AddRange(topFonts);

            string[] colors = { "Black", "White", "Red", "DimGray", "Blue", "Green", "Yellow", "Gray" };
            foreach (var cb in new[] { cmbBgColor, cmbTextColor, cmbRetTextColor }) cb.Items.AddRange(colors);

            cmbImageSize.Items.AddRange(new string[] { "Stretch", "Zoom", "Center" });

            // Wire up live updates for ALL relevant controls
            Control[] liveInputs = {
                txtMessage, txtFontSize, cmbFontFamily, cmbBgColor, cmbTextColor,
                chkShowBypass, cmbImageSize,
                chkShowReturning, cmbRetFontFamily, txtRetFontSize, cmbRetTextColor
            };

            foreach (var ctrl in liveInputs)
            {
                if (ctrl is TextBox tb) tb.TextChanged += (s, e) => UpdatePreview();
                if (ctrl is ComboBox cb) cb.SelectedIndexChanged += (s, e) => UpdatePreview();
                if (ctrl is CheckBox chk) chk.CheckedChanged += (s, e) => UpdatePreview();
            }

            btnSelectImage.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.bmp" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtImagePath.Text = ofd.FileName;
                        UpdatePreview();
                    }
                }
            };

            
            btnSave.Click += (s, e) => SaveAndClose();
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadCurrentSettings()
        {
            // Main Message
            txtMessage.Text = _settings.LimitMessage;
            cmbFontFamily.Text = _settings.LimitFontFamily;
            txtFontSize.Text = _settings.LimitFontSize.ToString();
            cmbBgColor.Text = _settings.LimitBgColor;
            cmbTextColor.Text = _settings.LimitTextColor;

            // Returning Time
            chkShowReturning.Checked = _settings.LimitShowReturningTime;
            cmbRetFontFamily.Text = _settings.LimitReturningFontFamily;
            txtRetFontSize.Text = _settings.LimitReturningFontSize.ToString();
            cmbRetTextColor.Text = _settings.LimitReturningTextColor;

            // Background
            txtImagePath.Text = _settings.LimitDesktopImagePath;
            cmbImageSize.Text = _settings.LimitDesktopImageSizeMode;
            chkShowBypass.Checked = _settings.LimitShowBypassInstructions;

            if (string.IsNullOrEmpty(cmbFontFamily.Text)) cmbFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbRetFontFamily.Text)) cmbRetFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbImageSize.Text)) cmbImageSize.Text = "Stretch";
        }

        private void UpdatePreview()
        {
            if (isInitializing) return;

            float scaleFactor = (float)pnlPreview.Width / Screen.PrimaryScreen.Bounds.Width;
            pnlPreview.BackColor = Color.FromName(cmbBgColor.Text);

            // Background Image logic
            if (!string.IsNullOrEmpty(txtImagePath.Text) && File.Exists(txtImagePath.Text))
            {
                try
                {
                    if (pnlPreview.BackgroundImage != null) pnlPreview.BackgroundImage.Dispose();
                    using (var fs = new FileStream(txtImagePath.Text, FileMode.Open, FileAccess.Read))
                    {
                        pnlPreview.BackgroundImage = Image.FromStream(fs);
                    }
                    pnlPreview.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), cmbImageSize.Text);
                }
                catch { pnlPreview.BackgroundImage = null; }
            }
            else pnlPreview.BackgroundImage = null;

            // 1. STYLE MAIN MESSAGE
            lblPreviewMsg.Text = txtMessage.Text;
            lblPreviewMsg.ForeColor = Color.FromName(cmbTextColor.Text);
            int.TryParse(txtFontSize.Text, out int fSize);
            try { lblPreviewMsg.Font = new Font(cmbFontFamily.Text, fSize * scaleFactor, FontStyle.Bold); } catch { }

            // 2. STYLE RETURNING TIME (SEPARATED)
            lblPreviewReturning.Visible = chkShowReturning.Checked;
            if (chkShowReturning.Checked)
            {
                // Pull schedule from local config temporarily for preview
                var mainCfg = AppSettings.Load();
                lblPreviewReturning.Text = $"Returning at {mainCfg.LimitDesktopHourOpen}:{mainCfg.LimitDesktopMinOpen} {mainCfg.LimitDesktopAMPMOpen}";
                lblPreviewReturning.ForeColor = Color.FromName(cmbRetTextColor.Text);
                int.TryParse(txtRetFontSize.Text, out int rSize);
                try { lblPreviewReturning.Font = new Font(cmbRetFontFamily.Text, rSize * scaleFactor, FontStyle.Bold); } catch { }
            }

            lblPreviewBypass.Visible = chkShowBypass.Checked;
            lblPreviewBypass.ForeColor = Color.FromName(cmbTextColor.Text);
        }

        private void SaveToSettings(AppSettings s)
        {
            s.LimitMessage = txtMessage.Text;
            s.LimitFontFamily = cmbFontFamily.Text;
            int.TryParse(txtFontSize.Text, out int v1); s.LimitFontSize = v1;
            s.LimitBgColor = cmbBgColor.Text;
            s.LimitTextColor = cmbTextColor.Text;
            s.LimitShowBypassInstructions = chkShowBypass.Checked;

            s.LimitShowReturningTime = chkShowReturning.Checked;
            s.LimitReturningFontFamily = cmbRetFontFamily.Text;
            int.TryParse(txtRetFontSize.Text, out int v2); s.LimitReturningFontSize = v2;
            s.LimitReturningTextColor = cmbRetTextColor.Text;

            s.LimitDesktopImagePath = txtImagePath.Text;
            s.LimitDesktopImageSizeMode = cmbImageSize.Text;
        }

        private void SaveAndClose()
        {
            SaveToSettings(_settings);
            AppSettings.Save(_settings);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void RunFullScreenTest()
        {
            // Create a temporary settings object to hold current UI choices
            AppSettings temp = new AppSettings();
            SaveToSettings(temp);

            // Sync the schedule from the main saved settings so the labels are accurate
            var saved = AppSettings.Load();
            temp.LimitDesktopHourOpen = saved.LimitDesktopHourOpen;
            temp.LimitDesktopMinOpen = saved.LimitDesktopMinOpen;
            temp.LimitDesktopAMPMOpen = saved.LimitDesktopAMPMOpen;

            // Open the form in Preview Mode (which is now Full Screen)
            using (LimitClosedForm test = new LimitClosedForm(temp, null, true))
            {
                test.ShowDialog();
            }
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            RunFullScreenTest();    
        }
    }
}