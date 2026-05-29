using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO;

namespace AzeuServices_V1
{
    public partial class NoSmokingEditorForm : Form
    {
        private AppSettings _settings;
        private bool isInitializing = true;

        public NoSmokingEditorForm()
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
            // FONT PERFORMANCE FIX: Load only 10 popular/readable fonts
            cmbFontFamily.BeginUpdate();
            string[] topFonts = {
                "Arial", "Arial Black", "Impact", "Segoe UI",
                "Verdana", "Tahoma", "Times New Roman",
                "Trebuchet MS", "Georgia", "Courier New"
            };
            cmbFontFamily.Items.AddRange(topFonts);
            cmbFontFamily.EndUpdate();

            string[] colors = { "Black", "White", "Red", "DimGray", "Blue", "Green", "Yellow", "Orange", "Gray", "DarkRed" };
            ComboBox[] colorCombos = { cmbBgColor, cmbTextColor, cmbBtnBgColor, cmbBtnTextColor };
            foreach (var cb in colorCombos) cb.Items.AddRange(colors);

            cmbImageSize.Items.AddRange(new string[] { "Stretch", "Zoom", "None" });

            btnPreviewAction.FlatStyle = FlatStyle.Flat;
            btnPreviewAction.FlatAppearance.BorderSize = 0;

            // Live Update Wiring
            Control[] liveInputs = { txtMessage, txtButtonText, txtFontSize, txtBtnWidth, txtBtnHeight, txtBtnFontSize, txtBtnRadius, txtBtnMargin, cmbFontFamily, cmbBgColor, cmbTextColor, cmbBtnBgColor, cmbBtnTextColor, cmbImageSize };
            foreach (var ctrl in liveInputs)
            {
                if (ctrl is TextBox tb) tb.TextChanged += (s, e) => UpdatePreview();
                if (ctrl is ComboBox cb) cb.SelectedIndexChanged += (s, e) => UpdatePreview();
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

            btnPreviewAction.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle bounds = new Rectangle(0, 0, btnPreviewAction.Width, btnPreviewAction.Height);
                float scaleFactor = (float)pnlPreview.Width / Screen.PrimaryScreen.Bounds.Width;
                int.TryParse(txtBtnRadius.Text, out int r);
                int scaledR = (int)(r * scaleFactor * 2.5);
                if (scaledR <= 1) scaledR = 1;
                if (scaledR > bounds.Height) scaledR = bounds.Height;

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(bounds.X, bounds.Y, scaledR, scaledR, 180, 90);
                    path.AddArc(bounds.X + bounds.Width - scaledR, bounds.Y, scaledR, scaledR, 270, 90);
                    path.AddArc(bounds.X + bounds.Width - scaledR, bounds.Y + bounds.Height - scaledR, scaledR, scaledR, 0, 90);
                    path.AddArc(bounds.X, bounds.Y + bounds.Height - scaledR, scaledR, scaledR, 90, 90);
                    path.CloseAllFigures();
                    btnPreviewAction.Region = new Region(path);
                    using (Brush b = new SolidBrush(Color.FromName(cmbBtnBgColor.Text))) e.Graphics.FillPath(b, path);
                    TextRenderer.DrawText(e.Graphics, txtButtonText.Text, btnPreviewAction.Font, bounds, Color.FromName(cmbBtnTextColor.Text), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            };
        }

        private void LoadCurrentSettings()
        {
            txtMessage.Text = _settings.NoSmokingMessage;
            txtButtonText.Text = _settings.NoSmokingButtonText;
            txtFontSize.Text = _settings.NoSmokingFontSize.ToString();
            txtBtnRadius.Text = _settings.NoSmokingButtonRadius.ToString();
            txtDuration.Text = _settings.NoSmokingDuration.ToString();
            cmbFontFamily.Text = _settings.NoSmokingFontFamily;
            cmbBgColor.Text = _settings.NoSmokingBgColor;
            cmbTextColor.Text = _settings.NoSmokingTextColor;
            cmbBtnBgColor.Text = _settings.NoSmokingButtonBgColor;
            cmbBtnTextColor.Text = _settings.NoSmokingButtonTextColor;
            txtBtnMargin.Text = _settings.NoSmokingButtonBottomMargin.ToString();
            txtBtnWidth.Text = _settings.NoSmokingButtonWidth.ToString();
            txtBtnHeight.Text = _settings.NoSmokingButtonHeight.ToString();
            txtBtnFontSize.Text = _settings.NoSmokingButtonFontSize.ToString();
            txtImagePath.Text = _settings.NoSmokingImagePath;
            cmbImageSize.Text = _settings.NoSmokingImageSizeMode;

            if (string.IsNullOrEmpty(cmbFontFamily.Text)) cmbFontFamily.Text = "Arial";
            if (string.IsNullOrEmpty(cmbImageSize.Text)) cmbImageSize.Text = "Stretch";
        }

        private void UpdatePreview()
        {
            if (isInitializing) return;

            float scaleFactor = (float)pnlPreview.Width / Screen.PrimaryScreen.Bounds.Width;
            pnlPreview.BackColor = Color.FromName(cmbBgColor.Text);

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
            else
            {
                pnlPreview.BackgroundImage = null;
            }

            lblPreviewMsg.Text = txtMessage.Text;
            lblPreviewMsg.ForeColor = Color.FromName(cmbTextColor.Text);
            int.TryParse(txtFontSize.Text, out int fSize);
            try { lblPreviewMsg.Font = new Font(cmbFontFamily.Text, fSize * scaleFactor, FontStyle.Bold); } catch { }

            int.TryParse(txtBtnWidth.Text, out int bW);
            int.TryParse(txtBtnHeight.Text, out int bH);
            int.TryParse(txtBtnMargin.Text, out int bM);
            int.TryParse(txtBtnFontSize.Text, out int bFS);

            btnPreviewAction.Width = (int)(bW * scaleFactor);
            btnPreviewAction.Height = (int)(bH * scaleFactor);
            btnPreviewAction.Left = (pnlPreview.Width - btnPreviewAction.Width) / 2;
            btnPreviewAction.Top = pnlPreview.Height - (int)(bM * scaleFactor) - btnPreviewAction.Height;

            

            float totalScale = (bFS * scaleFactor);
            if (totalScale < 1) totalScale = 1;

            btnPreviewAction.Font = new Font("Arial", totalScale, FontStyle.Bold);
            btnPreviewAction.Invalidate();
        }

        private void SaveToSettings(AppSettings s)
        {
            s.NoSmokingMessage = txtMessage.Text;
            s.NoSmokingButtonText = txtButtonText.Text;
            s.NoSmokingFontFamily = cmbFontFamily.Text;
            int.TryParse(txtFontSize.Text, out int v1); s.NoSmokingFontSize = v1;
            s.NoSmokingBgColor = cmbBgColor.Text;
            s.NoSmokingTextColor = cmbTextColor.Text;
            s.NoSmokingButtonBgColor = cmbBtnBgColor.Text;
            s.NoSmokingButtonTextColor = cmbBtnTextColor.Text;
            int.TryParse(txtBtnRadius.Text, out int v2); s.NoSmokingButtonRadius = v2;
            int.TryParse(txtDuration.Text, out int v3); s.NoSmokingDuration = v3;
            int.TryParse(txtBtnMargin.Text, out int v4); s.NoSmokingButtonBottomMargin = v4;
            int.TryParse(txtBtnWidth.Text, out int v5); s.NoSmokingButtonWidth = v5;
            int.TryParse(txtBtnHeight.Text, out int v6); s.NoSmokingButtonHeight = v6;
            int.TryParse(txtBtnFontSize.Text, out int v7); s.NoSmokingButtonFontSize = v7;
            s.NoSmokingImagePath = txtImagePath.Text;
            s.NoSmokingImageSizeMode = cmbImageSize.Text;
        }

        private void SaveAndClose()
        {
            string sourcePath = txtImagePath.Text;
            string finalImagePath = sourcePath;

            if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(appDir, "images");
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                string extension = Path.GetExtension(sourcePath);
                string targetPath = Path.Combine(imagesFolder, "no-smoking-bg" + extension);

                if (string.Compare(sourcePath, targetPath, true) != 0)
                {
                    try { File.Copy(sourcePath, targetPath, true); finalImagePath = targetPath; }
                    catch (Exception ex) { MessageBox.Show("Image Save Error: " + ex.Message); }
                }
            }

            SaveToSettings(_settings);
            _settings.NoSmokingImagePath = finalImagePath;
            AppSettings.Save(_settings);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void RunFullScreenTest()
        {
            AppSettings temp = new AppSettings();
            SaveToSettings(temp);
            // We pass true for isPreview so the form knows to enable ESC exit and prompt
            using (NoSmokingForm test = new NoSmokingForm(temp, true))
            {
                test.ShowDialog();
            }
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            RunFullScreenTest();
        }

        private void NumberLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            // Define limits per TextBox
            int maxValue = 0;

            switch (txt.Name)
            {
                case "txtFontSize":
                case "txtBtnFontSize":
                    maxValue = 100;
                    break;
                case "txtBtnWidth":
                case "txtBtnHeight":
                    maxValue = 900;
                    break;
                case "txtBtnRadius":
                    maxValue = 50;
                    break;
                case "txtBtnMargin":
                    maxValue = 100;
                    break;
                case "txtDuration":
                    maxValue = 30;
                    break;
                default:
                    return;
            }

            if (char.IsDigit(e.KeyChar))
            {
                string potentialText = txt.Text + e.KeyChar;
                if (int.TryParse(potentialText, out int value))
                {
                    // ONLY block if exceeds max (allow numbers below min while typing)
                    if (value > maxValue)
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
            TextBox txt = sender as TextBox;
            if (txt == null) return;

            // Define limits per TextBox
            int minValue = 0;
            int maxValue = 0;
            int defaultValue = 0;

            switch (txt.Name)
            {
                case "txtFontSize":
                    minValue = 0; maxValue = 100; defaultValue = 0;
                    break;
                case "txtBtnFontSize":
                    minValue = 0; maxValue = 100; defaultValue = 0;
                    break;
                case "txtBtnWidth":
                    minValue = 50; maxValue = 900; defaultValue = 300;
                    break;
                case "txtBtnHeight":
                    minValue = 50; maxValue = 900; defaultValue = 100;
                    break;
                case "txtBtnRadius":
                    minValue = 0; maxValue = 50; defaultValue = 8;
                    break;
                case "txtBtnMargin":
                    minValue = 0; maxValue = 100; defaultValue = 20;
                    break;
                case "txtDuration":
                    minValue = 0; maxValue = 30; defaultValue = 5;
                    break;
                default:
                    return;
            }

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = defaultValue.ToString();
                return;
            }

            if (int.TryParse(txt.Text, out int value))
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