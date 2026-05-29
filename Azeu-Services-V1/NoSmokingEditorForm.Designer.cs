namespace AzeuServices_V1
{
    partial class NoSmokingEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            pnlPreview = new Panel();
            btnPreviewAction = new Button();
            lblPreviewMsg = new Label();
            grpMessage = new GroupBox();
            txtMessage = new TextBox();
            cmbFontFamily = new ComboBox();
            txtFontSize = new TextBox();
            cmbBgColor = new ComboBox();
            cmbTextColor = new ComboBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            btnFullScreen = new Button();
            btnSave = new Button();
            grpBehavior = new GroupBox();
            txtDuration = new TextBox();
            label10 = new Label();
            btnCancel = new Button();
            grpButton = new GroupBox();
            txtButtonText = new TextBox();
            txtBtnWidth = new TextBox();
            txtBtnHeight = new TextBox();
            txtBtnFontSize = new TextBox();
            cmbBtnBgColor = new ComboBox();
            cmbBtnTextColor = new ComboBox();
            txtBtnRadius = new TextBox();
            txtBtnMargin = new TextBox();
            label2 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label14 = new Label();
            grpImage = new GroupBox();
            cmbImageSize = new ComboBox();
            txtImagePath = new TextBox();
            label15 = new Label();
            btnSelectImage = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            pnlPreview.SuspendLayout();
            grpMessage.SuspendLayout();
            grpBehavior.SuspendLayout();
            grpButton.SuspendLayout();
            grpImage.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel2;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = Color.FromArgb(20, 20, 20);
            splitContainer1.Panel1.Controls.Add(pnlPreview);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.BackColor = Color.White;
            splitContainer1.Panel2.Controls.Add(grpMessage);
            splitContainer1.Panel2.Controls.Add(btnFullScreen);
            splitContainer1.Panel2.Controls.Add(btnSave);
            splitContainer1.Panel2.Controls.Add(grpBehavior);
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(grpButton);
            splitContainer1.Panel2.Controls.Add(grpImage);
            splitContainer1.Panel2.Padding = new Padding(15);
            splitContainer1.Size = new Size(1063, 421);
            splitContainer1.SplitterDistance = 723;
            splitContainer1.TabIndex = 0;
            // 
            // pnlPreview
            // 
            pnlPreview.BackColor = Color.Black;
            pnlPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlPreview.Controls.Add(btnPreviewAction);
            pnlPreview.Controls.Add(lblPreviewMsg);
            pnlPreview.Location = new Point(12, 10);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(700, 400);
            pnlPreview.TabIndex = 0;
            // 
            // btnPreviewAction
            // 
            btnPreviewAction.FlatStyle = FlatStyle.Flat;
            btnPreviewAction.Location = new Point(225, 300);
            btnPreviewAction.Name = "btnPreviewAction";
            btnPreviewAction.Size = new Size(250, 60);
            btnPreviewAction.TabIndex = 0;
            btnPreviewAction.Text = "I Understand";
            // 
            // lblPreviewMsg
            // 
            lblPreviewMsg.BackColor = Color.Transparent;
            lblPreviewMsg.Dock = DockStyle.Top;
            lblPreviewMsg.ForeColor = Color.White;
            lblPreviewMsg.Location = new Point(0, 0);
            lblPreviewMsg.Name = "lblPreviewMsg";
            lblPreviewMsg.Size = new Size(698, 280);
            lblPreviewMsg.TabIndex = 1;
            lblPreviewMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpMessage
            // 
            grpMessage.Controls.Add(txtMessage);
            grpMessage.Controls.Add(cmbFontFamily);
            grpMessage.Controls.Add(txtFontSize);
            grpMessage.Controls.Add(cmbBgColor);
            grpMessage.Controls.Add(cmbTextColor);
            grpMessage.Controls.Add(label1);
            grpMessage.Controls.Add(label3);
            grpMessage.Controls.Add(label4);
            grpMessage.Controls.Add(label5);
            grpMessage.Controls.Add(label6);
            grpMessage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpMessage.Location = new Point(10, 10);
            grpMessage.Name = "grpMessage";
            grpMessage.Size = new Size(300, 210);
            grpMessage.TabIndex = 0;
            grpMessage.TabStop = false;
            grpMessage.Text = "1. Main Message Styles";
            // 
            // txtMessage
            // 
            txtMessage.Font = new Font("Segoe UI", 9F);
            txtMessage.Location = new Point(10, 43);
            txtMessage.MaxLength = 100;
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(280, 45);
            txtMessage.TabIndex = 1;
            // 
            // cmbFontFamily
            // 
            cmbFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFontFamily.Font = new Font("Segoe UI", 9F);
            cmbFontFamily.Location = new Point(10, 118);
            cmbFontFamily.Name = "cmbFontFamily";
            cmbFontFamily.Size = new Size(130, 23);
            cmbFontFamily.TabIndex = 3;
            // 
            // txtFontSize
            // 
            txtFontSize.Font = new Font("Segoe UI", 9F);
            txtFontSize.Location = new Point(155, 118);
            txtFontSize.MaxLength = 3;
            txtFontSize.Name = "txtFontSize";
            txtFontSize.Size = new Size(130, 23);
            txtFontSize.TabIndex = 5;
            txtFontSize.KeyPress += NumberLimit_KeyPress;
            txtFontSize.Leave += NumberLimit_Leave;
            // 
            // cmbBgColor
            // 
            cmbBgColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBgColor.Font = new Font("Segoe UI", 9F);
            cmbBgColor.Location = new Point(10, 174);
            cmbBgColor.Name = "cmbBgColor";
            cmbBgColor.Size = new Size(130, 23);
            cmbBgColor.TabIndex = 7;
            // 
            // cmbTextColor
            // 
            cmbTextColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTextColor.Font = new Font("Segoe UI", 9F);
            cmbTextColor.Location = new Point(155, 174);
            cmbTextColor.Name = "cmbTextColor";
            cmbTextColor.Size = new Size(130, 23);
            cmbTextColor.TabIndex = 9;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label1.Location = new Point(10, 25);
            label1.Name = "label1";
            label1.Size = new Size(280, 15);
            label1.TabIndex = 0;
            label1.Text = "Dialog Message:";
            // 
            // label3
            // 
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label3.Location = new Point(10, 100);
            label3.Name = "label3";
            label3.Size = new Size(130, 15);
            label3.TabIndex = 2;
            label3.Text = "Font Family:";
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label4.Location = new Point(155, 100);
            label4.Name = "label4";
            label4.Size = new Size(80, 15);
            label4.TabIndex = 4;
            label4.Text = "Font Size:";
            // 
            // label5
            // 
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label5.Location = new Point(10, 156);
            label5.Name = "label5";
            label5.Size = new Size(130, 15);
            label5.TabIndex = 6;
            label5.Text = "Background Color:";
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label6.Location = new Point(155, 156);
            label6.Name = "label6";
            label6.Size = new Size(130, 15);
            label6.TabIndex = 8;
            label6.Text = "Text Color:";
            // 
            // btnFullScreen
            // 
            btnFullScreen.Location = new Point(10, 694);
            btnFullScreen.Name = "btnFullScreen";
            btnFullScreen.Size = new Size(300, 30);
            btnFullScreen.TabIndex = 4;
            btnFullScreen.Text = "Full Screen Test";
            btnFullScreen.Click += btnFullScreen_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Green;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(10, 734);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(145, 40);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save Changes";
            btnSave.UseVisualStyleBackColor = false;
            // 
            // grpBehavior
            // 
            grpBehavior.Controls.Add(txtDuration);
            grpBehavior.Controls.Add(label10);
            grpBehavior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpBehavior.Location = new Point(10, 608);
            grpBehavior.Name = "grpBehavior";
            grpBehavior.Size = new Size(300, 80);
            grpBehavior.TabIndex = 3;
            grpBehavior.TabStop = false;
            grpBehavior.Text = "4. Behavioral Settings";
            // 
            // txtDuration
            // 
            txtDuration.Font = new Font("Segoe UI", 9F);
            txtDuration.Location = new Point(10, 43);
            txtDuration.MaxLength = 2;
            txtDuration.Name = "txtDuration";
            txtDuration.Size = new Size(280, 23);
            txtDuration.TabIndex = 1;
            txtDuration.KeyPress += NumberLimit_KeyPress;
            txtDuration.Leave += NumberLimit_Leave;
            // 
            // label10
            // 
            label10.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(10, 25);
            label10.Name = "label10";
            label10.Size = new Size(280, 15);
            label10.TabIndex = 0;
            label10.Text = "Lock Duration (Seconds):";
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Gray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(165, 734);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(145, 40);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // grpButton
            // 
            grpButton.Controls.Add(txtButtonText);
            grpButton.Controls.Add(txtBtnWidth);
            grpButton.Controls.Add(txtBtnHeight);
            grpButton.Controls.Add(txtBtnFontSize);
            grpButton.Controls.Add(cmbBtnBgColor);
            grpButton.Controls.Add(cmbBtnTextColor);
            grpButton.Controls.Add(txtBtnRadius);
            grpButton.Controls.Add(txtBtnMargin);
            grpButton.Controls.Add(label2);
            grpButton.Controls.Add(label11);
            grpButton.Controls.Add(label12);
            grpButton.Controls.Add(label13);
            grpButton.Controls.Add(label7);
            grpButton.Controls.Add(label8);
            grpButton.Controls.Add(label9);
            grpButton.Controls.Add(label14);
            grpButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpButton.Location = new Point(10, 226);
            grpButton.Name = "grpButton";
            grpButton.Size = new Size(300, 260);
            grpButton.TabIndex = 1;
            grpButton.TabStop = false;
            grpButton.Text = "2. Button Appearance";
            // 
            // txtButtonText
            // 
            txtButtonText.Font = new Font("Segoe UI", 9F);
            txtButtonText.Location = new Point(10, 43);
            txtButtonText.MaxLength = 15;
            txtButtonText.Name = "txtButtonText";
            txtButtonText.Size = new Size(280, 23);
            txtButtonText.TabIndex = 1;
            // 
            // txtBtnWidth
            // 
            txtBtnWidth.Font = new Font("Segoe UI", 9F);
            txtBtnWidth.Location = new Point(10, 98);
            txtBtnWidth.MaxLength = 3;
            txtBtnWidth.Name = "txtBtnWidth";
            txtBtnWidth.Size = new Size(80, 23);
            txtBtnWidth.TabIndex = 3;
            txtBtnWidth.KeyPress += NumberLimit_KeyPress;
            txtBtnWidth.Leave += NumberLimit_Leave;
            // 
            // txtBtnHeight
            // 
            txtBtnHeight.Font = new Font("Segoe UI", 9F);
            txtBtnHeight.Location = new Point(105, 98);
            txtBtnHeight.MaxLength = 3;
            txtBtnHeight.Name = "txtBtnHeight";
            txtBtnHeight.Size = new Size(80, 23);
            txtBtnHeight.TabIndex = 5;
            txtBtnHeight.KeyPress += NumberLimit_KeyPress;
            txtBtnHeight.Leave += NumberLimit_Leave;
            // 
            // txtBtnFontSize
            // 
            txtBtnFontSize.Font = new Font("Segoe UI", 9F);
            txtBtnFontSize.Location = new Point(200, 98);
            txtBtnFontSize.MaxLength = 3;
            txtBtnFontSize.Name = "txtBtnFontSize";
            txtBtnFontSize.Size = new Size(80, 23);
            txtBtnFontSize.TabIndex = 7;
            txtBtnFontSize.KeyPress += NumberLimit_KeyPress;
            txtBtnFontSize.Leave += NumberLimit_Leave;
            // 
            // cmbBtnBgColor
            // 
            cmbBtnBgColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBtnBgColor.Font = new Font("Segoe UI", 9F);
            cmbBtnBgColor.Location = new Point(10, 153);
            cmbBtnBgColor.Name = "cmbBtnBgColor";
            cmbBtnBgColor.Size = new Size(130, 23);
            cmbBtnBgColor.TabIndex = 9;
            // 
            // cmbBtnTextColor
            // 
            cmbBtnTextColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBtnTextColor.Font = new Font("Segoe UI", 9F);
            cmbBtnTextColor.Location = new Point(155, 153);
            cmbBtnTextColor.Name = "cmbBtnTextColor";
            cmbBtnTextColor.Size = new Size(130, 23);
            cmbBtnTextColor.TabIndex = 11;
            // 
            // txtBtnRadius
            // 
            txtBtnRadius.Font = new Font("Segoe UI", 9F);
            txtBtnRadius.Location = new Point(10, 213);
            txtBtnRadius.MaxLength = 3;
            txtBtnRadius.Name = "txtBtnRadius";
            txtBtnRadius.Size = new Size(130, 23);
            txtBtnRadius.TabIndex = 13;
            txtBtnRadius.KeyPress += NumberLimit_KeyPress;
            txtBtnRadius.Leave += NumberLimit_Leave;
            // 
            // txtBtnMargin
            // 
            txtBtnMargin.Font = new Font("Segoe UI", 9F);
            txtBtnMargin.Location = new Point(155, 213);
            txtBtnMargin.MaxLength = 3;
            txtBtnMargin.Name = "txtBtnMargin";
            txtBtnMargin.Size = new Size(130, 23);
            txtBtnMargin.TabIndex = 15;
            txtBtnMargin.KeyPress += NumberLimit_KeyPress;
            txtBtnMargin.Leave += NumberLimit_Leave;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.Location = new Point(10, 25);
            label2.Name = "label2";
            label2.Size = new Size(280, 15);
            label2.TabIndex = 0;
            label2.Text = "Button Text:";
            // 
            // label11
            // 
            label11.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label11.Location = new Point(10, 80);
            label11.Name = "label11";
            label11.Size = new Size(80, 15);
            label11.TabIndex = 2;
            label11.Text = "Width:";
            // 
            // label12
            // 
            label12.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label12.Location = new Point(105, 80);
            label12.Name = "label12";
            label12.Size = new Size(80, 15);
            label12.TabIndex = 4;
            label12.Text = "Height:";
            // 
            // label13
            // 
            label13.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label13.Location = new Point(200, 80);
            label13.Name = "label13";
            label13.Size = new Size(80, 15);
            label13.TabIndex = 6;
            label13.Text = "Font Size:";
            // 
            // label7
            // 
            label7.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label7.Location = new Point(10, 135);
            label7.Name = "label7";
            label7.Size = new Size(130, 15);
            label7.TabIndex = 8;
            label7.Text = "Button BG:";
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label8.Location = new Point(155, 135);
            label8.Name = "label8";
            label8.Size = new Size(130, 15);
            label8.TabIndex = 10;
            label8.Text = "Button Text:";
            // 
            // label9
            // 
            label9.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label9.Location = new Point(10, 195);
            label9.Name = "label9";
            label9.Size = new Size(130, 15);
            label9.TabIndex = 12;
            label9.Text = "Radius:";
            // 
            // label14
            // 
            label14.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label14.Location = new Point(155, 195);
            label14.Name = "label14";
            label14.Size = new Size(130, 15);
            label14.TabIndex = 14;
            label14.Text = "Bottom Margin:";
            // 
            // grpImage
            // 
            grpImage.Controls.Add(cmbImageSize);
            grpImage.Controls.Add(txtImagePath);
            grpImage.Controls.Add(label15);
            grpImage.Controls.Add(btnSelectImage);
            grpImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpImage.Location = new Point(10, 492);
            grpImage.Name = "grpImage";
            grpImage.Size = new Size(300, 110);
            grpImage.TabIndex = 2;
            grpImage.TabStop = false;
            grpImage.Text = "3. Background Image Styles";
            // 
            // cmbImageSize
            // 
            cmbImageSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImageSize.Font = new Font("Segoe UI", 9F);
            cmbImageSize.Location = new Point(90, 62);
            cmbImageSize.Name = "cmbImageSize";
            cmbImageSize.Size = new Size(200, 23);
            cmbImageSize.TabIndex = 1;
            // 
            // txtImagePath
            // 
            txtImagePath.Font = new Font("Segoe UI", 9F);
            txtImagePath.Location = new Point(10, 25);
            txtImagePath.Name = "txtImagePath";
            txtImagePath.ReadOnly = true;
            txtImagePath.Size = new Size(225, 23);
            txtImagePath.TabIndex = 3;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label15.Location = new Point(10, 65);
            label15.Name = "label15";
            label15.Size = new Size(66, 15);
            label15.TabIndex = 0;
            label15.Text = "Size Mode:";
            // 
            // btnSelectImage
            // 
            btnSelectImage.Font = new Font("Segoe UI", 9F);
            btnSelectImage.Location = new Point(241, 25);
            btnSelectImage.Name = "btnSelectImage";
            btnSelectImage.Size = new Size(49, 23);
            btnSelectImage.TabIndex = 2;
            btnSelectImage.Text = "Select";
            // 
            // NoSmokingEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1063, 421);
            Controls.Add(splitContainer1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimumSize = new Size(1000, 400);
            Name = "NoSmokingEditorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "No Smoking Dialog Editor";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pnlPreview.ResumeLayout(false);
            grpMessage.ResumeLayout(false);
            grpMessage.PerformLayout();
            grpBehavior.ResumeLayout(false);
            grpBehavior.PerformLayout();
            grpButton.ResumeLayout(false);
            grpButton.PerformLayout();
            grpImage.ResumeLayout(false);
            grpImage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel pnlPreview;
        private Label lblPreviewMsg;
        private Button btnPreviewAction;
        private GroupBox grpMessage;
        private GroupBox grpButton;
        private GroupBox grpImage;
        private GroupBox grpBehavior;
        private TextBox txtMessage;
        private ComboBox cmbFontFamily;
        private TextBox txtFontSize;
        private ComboBox cmbBgColor;
        private ComboBox cmbTextColor;
        private TextBox txtButtonText;
        private TextBox txtBtnWidth;
        private TextBox txtBtnHeight;
        private TextBox txtBtnFontSize;
        private ComboBox cmbBtnBgColor;
        private ComboBox cmbBtnTextColor;
        private TextBox txtBtnRadius;
        private TextBox txtBtnMargin;
        private TextBox txtImagePath;
        private Button btnSelectImage;
        private ComboBox cmbImageSize;
        private TextBox txtDuration;
        private Button btnFullScreen;
        private Button btnSave;
        private Button btnCancel;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
    }
}