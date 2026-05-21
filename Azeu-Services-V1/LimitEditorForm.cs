using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            // Populate Font List
            string[] topFonts = { "Arial", "Arial Black", "Impact", "Segoe UI", "Verdana", "Tahoma", "Times New Roman" };
            cmbFontFamily.Items.AddRange(topFonts);
            cmbRetFontFamily.Items.AddRange(topFonts);

            // Populate Colors
            string[] colors = { "Black", "White", "Red", "DimGray", "Blue", "Green", "Yellow", "Gray", "DarkRed", "Orange" };
            foreach (var cb in new[] { cmbBgColor, cmbTextColor, cmbRetTextColor })
            {
                cb.Items.AddRange(colors);
            }

            cmbImageSize.Items.AddRange(new string[] { "Stretch", "Zoom", "Center" });

            // Wire up live updates for ALL relevant controls
            Control[] liveInputs = {
                txtMessage, txtFontSize, cmbFontFamily, cmbBgColor, cmbTextColor,
                chkShowBypass, cmbImageSize,
                chkShowReturning, cmbRetFontFamily, txtRetFontSize, cmbRetTextColor, txtRetMargin
            };

            foreach (var ctrl in liveInputs)
            {
                if (ctrl is TextBox tb) tb.TextChanged += (s, e) => UpdatePreview();
                if (ctrl is ComboBox cb) cb.SelectedIndexChanged += (s, e) => UpdatePreview();
                if (ctrl is CheckBox chk) chk.CheckedChanged += (s, e) => UpdatePreview();
            }

            // Image Selection
            btnSelectImage.Click += (s, e) => {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png;*.bmp" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtImagePath.Text = ofd.FileName;
                        UpdatePreview();
                    }
                }
            };

            btnFullScreen.Click += (s, e) => RunFullScreenTest();
            btnSave.Click += (s, e) => SaveAndClose();
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadCurrentSettings()
        {
            // Main Message Segment
            txtMessage.Text = _settings.LimitMessage;
            cmbFontFamily.Text = _settings.LimitFontFamily;
            txtFontSize.Text = _settings.LimitFontSize.ToString();
            cmbBgColor.Text = _settings.LimitBgColor;
            cmbTextColor.Text = _settings.LimitTextColor;
            chkShowBypass.Checked = _settings.LimitShowBypassInstructions;

            // Returning Time Segment (Separated)
            chkShowReturning.Checked = _settings.LimitShowReturningTime;
            cmbRetFontFamily.Text = _settings.LimitReturningFontFamily;
            txtRetFontSize.Text = _settings.LimitReturningFontSize.ToString();
            cmbRetTextColor.Text = _settings.LimitReturningTextColor;
            txtRetMargin.Text = _settings.LimitReturningBottomMargin.ToString();

            // Background Segment
            txtImagePath.Text = _settings.LimitDesktopImagePath;
            cmbImageSize.Text = _settings.LimitDesktopImageSizeMode;

            // Defaults if empty
            if (string.IsNullOrEmpty(cmbFontFamily.Text)) cmbFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbRetFontFamily.Text)) cmbRetFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbImageSize.Text)) cmbImageSize.Text = "Stretch";
        }

        private void UpdatePreview()
        {
            if (isInitializing) return;

            // Calculate scaling to make the preview panel look like a real monitor
            float scaleFactor = (float)pnlPreview.Width / Screen.PrimaryScreen.Bounds.Width;
            pnlPreview.BackColor = Color.FromName(cmbBgColor.Text);

            // 1. Background Image Preview
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

            // 2. Main Message Preview
            lblPreviewMsg.Text = txtMessage.Text;
            lblPreviewMsg.ForeColor = Color.FromName(cmbTextColor.Text);
            int.TryParse(txtFontSize.Text, out int fSize);
            try { lblPreviewMsg.Font = new Font(cmbFontFamily.Text, fSize * scaleFactor, FontStyle.Bold); } catch { }

            // 3. Returning Time Preview (With its own styles)
            lblPreviewReturning.Visible = chkShowReturning.Checked;
            if (chkShowReturning.Checked)
            {
                // Pull actual schedule from settings for the preview text
                var mainCfg = AppSettings.Load();
                lblPreviewReturning.Text = $"Returning at {mainCfg.LimitDesktopHourOpen}:{mainCfg.LimitDesktopMinOpen} {mainCfg.LimitDesktopAMPMOpen}";
                lblPreviewReturning.ForeColor = Color.FromName(cmbRetTextColor.Text);

                int.TryParse(txtRetFontSize.Text, out int rSize);
                try { lblPreviewReturning.Font = new Font(cmbRetFontFamily.Text, rSize * scaleFactor, FontStyle.Bold); } catch { }

                // Apply dynamic margin to the preview (scaled)
                int.TryParse(txtRetMargin.Text, out int margin);
                lblPreviewReturning.Padding = new Padding(0, 0, 0, (int)(margin * scaleFactor));
            }

            // 4. Bypass Instruction Preview
            lblPreviewBypass.Visible = chkShowBypass.Checked;
            lblPreviewBypass.ForeColor = Color.FromName(cmbTextColor.Text);
        }

        private void SaveToSettings(AppSettings s)
        {
            // Main Message
            s.LimitMessage = txtMessage.Text;
            s.LimitFontFamily = cmbFontFamily.Text;
            int.TryParse(txtFontSize.Text, out int v1); s.LimitFontSize = v1;
            s.LimitBgColor = cmbBgColor.Text;
            s.LimitTextColor = cmbTextColor.Text;
            s.LimitShowBypassInstructions = chkShowBypass.Checked;

            // Returning Time
            s.LimitShowReturningTime = chkShowReturning.Checked;
            s.LimitReturningFontFamily = cmbRetFontFamily.Text;
            int.TryParse(txtRetFontSize.Text, out int v2); s.LimitReturningFontSize = v2;
            s.LimitReturningTextColor = cmbRetTextColor.Text;
            int.TryParse(txtRetMargin.Text, out int v3); s.LimitReturningBottomMargin = v3;

            // Background
            s.LimitDesktopImagePath = txtImagePath.Text;
            s.LimitDesktopImageSizeMode = cmbImageSize.Text;
        }

        private void SaveAndClose()
        {
            string sourcePath = txtImagePath.Text;
            string finalImagePath = sourcePath;

            // Handle Image Persistence (Copy image to local app folder)
            if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(appDir, "images");
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                string extension = Path.GetExtension(sourcePath);
                string targetPath = Path.Combine(imagesFolder, "curfew-bg" + extension);

                if (string.Compare(sourcePath, targetPath, true) != 0)
                {
                    try { File.Copy(sourcePath, targetPath, true); finalImagePath = targetPath; }
                    catch (Exception ex) { MessageBox.Show("Image Save Error: " + ex.Message); }
                }
            }

            SaveToSettings(_settings);
            _settings.LimitDesktopImagePath = finalImagePath;
            AppSettings.Save(_settings);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void RunFullScreenTest()
        {
            // Create temporary settings from UI for the test
            AppSettings temp = new AppSettings();
            SaveToSettings(temp);

            // Pull the actual schedule from saved settings so the test is realistic
            var saved = AppSettings.Load();
            temp.LimitDesktopHourOpen = saved.LimitDesktopHourOpen;
            temp.LimitDesktopMinOpen = saved.LimitDesktopMinOpen;
            temp.LimitDesktopAMPMOpen = saved.LimitDesktopAMPMOpen;

            using (LimitClosedForm test = new LimitClosedForm(temp, null, true))
            {
                test.ShowDialog();
            }
        }
    }
}