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
        private TableLayoutPanel? previewLayout;

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

            string[] colors = { "Black", "White", "Red", "DimGray", "Blue", "Green", "Yellow", "Gray", "DarkRed", "Orange" };
            foreach (var cb in new[] { cmbBgColor, cmbTextColor, cmbRetTextColor }) cb.Items.AddRange(colors);

            cmbImageSize.Items.AddRange(new string[] { "Stretch", "Zoom", "Center" });

            Control[] liveInputs = {
                txtMessage, txtFontSize, cmbFontFamily, cmbBgColor, cmbTextColor,
                chkShowBypass, chkShowShutdownCountdown, cmbImageSize,
                chkShowReturning, cmbRetFontFamily, txtRetFontSize, cmbRetTextColor, txtRetMargin
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
                    if (ofd.ShowDialog() == DialogResult.OK) { txtImagePath.Text = ofd.FileName; UpdatePreview(); }
            };

            btnFullScreen.Click += (s, e) => RunFullScreenTest();
            btnSave.Click += (s, e) => SaveAndClose();
            btnCancel.Click += (s, e) => this.Close();

            // Prepare the layout inside the preview panel
            previewLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = Color.Transparent };
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30f));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15f));

            pnlPreview.Controls.Clear();
            pnlPreview.Controls.Add(previewLayout);

            previewLayout.Controls.Add(lblPreviewMsg, 0, 0);
            previewLayout.Controls.Add(lblPreviewReturning, 0, 1);
            previewLayout.Controls.Add(lblPreviewShutdown, 0, 2);
            previewLayout.Controls.Add(lblPreviewBypass, 0, 3);
        }

        private void LoadCurrentSettings()
        {
            txtMessage.Text = _settings.LimitMessage;
            cmbFontFamily.Text = _settings.LimitFontFamily;
            txtFontSize.Text = _settings.LimitFontSize.ToString();
            cmbBgColor.Text = _settings.LimitBgColor;
            cmbTextColor.Text = _settings.LimitTextColor;
            chkShowBypass.Checked = _settings.LimitShowBypassInstructions;
            chkShowShutdownCountdown.Checked = _settings.LimitShowShutdownCountdown;
            chkShowReturning.Checked = _settings.LimitShowReturningTime;
            cmbRetFontFamily.Text = _settings.LimitReturningFontFamily;
            txtRetFontSize.Text = _settings.LimitReturningFontSize.ToString();
            cmbRetTextColor.Text = _settings.LimitReturningTextColor;
            txtRetMargin.Text = _settings.LimitReturningBottomMargin.ToString();
            txtImagePath.Text = _settings.LimitDesktopImagePath;
            cmbImageSize.Text = _settings.LimitDesktopImageSizeMode;

            if (string.IsNullOrEmpty(cmbFontFamily.Text)) cmbFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbRetFontFamily.Text)) cmbRetFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbImageSize.Text)) cmbImageSize.Text = "Stretch";
        }

        private void UpdatePreview()
        {
            if (isInitializing || previewLayout == null) return;

            float scaleFactor = (float)pnlPreview.Width / Screen.PrimaryScreen.Bounds.Width;
            pnlPreview.BackColor = Color.FromName(cmbBgColor.Text);

            // Apply the same Row Logic as the real dialog
            previewLayout.RowStyles.Clear();
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Background Image logic
            if (!string.IsNullOrEmpty(txtImagePath.Text) && File.Exists(txtImagePath.Text))
            {
                try
                {
                    using (var fs = new FileStream(txtImagePath.Text, FileMode.Open, FileAccess.Read))
                        pnlPreview.BackgroundImage = Image.FromStream(fs);
                    pnlPreview.BackgroundImageLayout = (ImageLayout)Enum.Parse(typeof(ImageLayout), cmbImageSize.Text);
                }
                catch { pnlPreview.BackgroundImage = null; }
            }
            else pnlPreview.BackgroundImage = null;

            // 1. Message Preview
            lblPreviewMsg.Text = txtMessage.Text;
            lblPreviewMsg.ForeColor = Color.FromName(cmbTextColor.Text);
            int.TryParse(txtFontSize.Text, out int fSize);
            try { lblPreviewMsg.Font = new Font(cmbFontFamily.Text, fSize * scaleFactor, FontStyle.Bold); } catch { }

            // 2. Returning Time Preview (FIXED)
            lblPreviewReturning.Visible = chkShowReturning.Checked;
            if (chkShowReturning.Checked)
            {
                var mainCfg = AppSettings.Load();
                lblPreviewReturning.Text = $"Returning at {mainCfg.LimitDesktopHourOpen}:{mainCfg.LimitDesktopMinOpen} {mainCfg.LimitDesktopAMPMOpen}";
                lblPreviewReturning.ForeColor = Color.FromName(cmbRetTextColor.Text);

                int.TryParse(txtRetFontSize.Text, out int rSize);
                try { lblPreviewReturning.Font = new Font(cmbRetFontFamily.Text, rSize * scaleFactor, FontStyle.Bold); } catch { }

                // Apply Scaled Padding
                int.TryParse(txtRetMargin.Text, out int margin);
                lblPreviewReturning.Padding = new Padding(0, 0, 0, (int)(margin * scaleFactor));
            }

            // 3. Countdown Preview
            lblPreviewShutdown.Visible = chkShowShutdownCountdown.Checked;
            try { lblPreviewShutdown.Font = new Font("Segoe UI", 16 * scaleFactor, FontStyle.Bold); } catch { }
            lblPreviewShutdown.Padding = new Padding(0, (int)(10 * scaleFactor), 0, (int)(10 * scaleFactor));

            // 4. Bypass Hint Preview
            lblPreviewBypass.Visible = chkShowBypass.Checked;
            lblPreviewBypass.ForeColor = Color.FromName(cmbTextColor.Text);
            try { lblPreviewBypass.Font = new Font("Segoe UI", 12 * scaleFactor, FontStyle.Italic); } catch { }
            lblPreviewBypass.Padding = new Padding(0, 0, 0, (int)(20 * scaleFactor));
        }

        private void SaveToSettings(AppSettings s)
        {
            s.LimitMessage = txtMessage.Text;
            s.LimitFontFamily = cmbFontFamily.Text;
            int.TryParse(txtFontSize.Text, out int v1); s.LimitFontSize = v1;
            s.LimitBgColor = cmbBgColor.Text;
            s.LimitTextColor = cmbTextColor.Text;
            s.LimitShowBypassInstructions = chkShowBypass.Checked;
            s.LimitShowShutdownCountdown = chkShowShutdownCountdown.Checked;
            s.LimitShowReturningTime = chkShowReturning.Checked;
            s.LimitReturningFontFamily = cmbRetFontFamily.Text;
            int.TryParse(txtRetFontSize.Text, out int v2); s.LimitReturningFontSize = v2;
            s.LimitReturningTextColor = cmbRetTextColor.Text;
            int.TryParse(txtRetMargin.Text, out int v3); s.LimitReturningBottomMargin = v3;
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
            AppSettings temp = new AppSettings();
            SaveToSettings(temp);
            var saved = AppSettings.Load();
            temp.LimitDesktopHourOpen = saved.LimitDesktopHourOpen;
            temp.LimitDesktopMinOpen = saved.LimitDesktopMinOpen;
            temp.LimitDesktopAMPMOpen = saved.LimitDesktopAMPMOpen;
            temp.LimitShutdownAfter3Min = saved.LimitShutdownAfter3Min;

            using (LimitClosedForm test = new LimitClosedForm(temp, null, true)) { test.ShowDialog(); }
        }

        private void NumberLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox? txtKP = sender as TextBox;
            if (txtKP == null) return;

            // Define limits per TextBox
            int minValue = 0;
            int maxValue = 0;

            if (txtKP.Name == "txtFontSize" || txtKP.Name == "txtRetFontSize")
            {
                minValue = 0;
                maxValue = 100;
            }
            else if (txtKP.Name == "txtRetMargin")
            {
                minValue = 0;
                maxValue = 50;
            }

            if (char.IsDigit(e.KeyChar))
            {
                string potentialText = txtKP.Text + e.KeyChar;
                if (int.TryParse(potentialText, out int value))
                {
                    if (value < minValue || value > maxValue)
                        e.Handled = true;
                }
            }
            else if (!char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void NumberLimit_Leave(object sender, EventArgs e)
        {
            TextBox? txt = sender as TextBox;
            if (txt == null) return;

            // Define limits per TextBox
            int minValue = 0;
            int maxValue = 0;
            int defaultValue = 0;

            if (txt.Name == "txtFontSize" || txt.Name == "txtRetFontSize")
            {
                minValue = 0;
                maxValue = 100;
                defaultValue = 0;
            }
            else if (txt.Name == "txtRetMargin")
            {
                minValue = 0;
                maxValue = 50;
                defaultValue = 0;
            }

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = defaultValue.ToString();
            }
            else if (int.TryParse(txt.Text, out int value))
            {
                if (value < minValue)
                    txt.Text = minValue.ToString();
                else if (value > maxValue)
                    txt.Text = maxValue.ToString();
                else
                    txt.Text = value.ToString();
            }
            else
            {
                txt.Text = defaultValue.ToString();
            }
        }
    }
}