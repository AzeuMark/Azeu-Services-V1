namespace AzeuServices_V1
{
    partial class LimitEditorForm
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
            lblPreviewReturning = new Label();
            lblPreviewBypass = new Label();
            lblPreviewMsg = new Label();
            grpReturning = new GroupBox();
            txtRetMargin = new TextBox();
            label9 = new Label();
            chkShowReturning = new CheckBox();
            cmbRetFontFamily = new ComboBox();
            txtRetFontSize = new TextBox();
            cmbRetTextColor = new ComboBox();
            label2 = new Label();
            label7 = new Label();
            label8 = new Label();
            grpMessage = new GroupBox();
            chkShowBypass = new CheckBox();
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
            btnCancel = new Button();
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
            grpReturning.SuspendLayout();
            grpMessage.SuspendLayout();
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
            splitContainer1.Panel2.Controls.Add(grpReturning);
            splitContainer1.Panel2.Controls.Add(grpMessage);
            splitContainer1.Panel2.Controls.Add(btnFullScreen);
            splitContainer1.Panel2.Controls.Add(btnSave);
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(grpImage);
            splitContainer1.Panel2.Padding = new Padding(15);
            splitContainer1.Size = new Size(1063, 630);
            splitContainer1.SplitterDistance = 723;
            splitContainer1.TabIndex = 0;
            // 
            // pnlPreview
            // 
            pnlPreview.BackColor = Color.Black;
            pnlPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlPreview.Controls.Add(lblPreviewReturning);
            pnlPreview.Controls.Add(lblPreviewBypass);
            pnlPreview.Controls.Add(lblPreviewMsg);
            pnlPreview.Location = new Point(12, 10);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(700, 400);
            pnlPreview.TabIndex = 0;
            // 
            // lblPreviewReturning
            // 
            lblPreviewReturning.BackColor = Color.Transparent;
            lblPreviewReturning.Dock = DockStyle.Bottom;
            lblPreviewReturning.ForeColor = Color.White;
            lblPreviewReturning.Location = new Point(0, 323);
            lblPreviewReturning.Name = "lblPreviewReturning";
            lblPreviewReturning.Size = new Size(698, 44);
            lblPreviewReturning.TabIndex = 3;
            lblPreviewReturning.Text = "Returning at 08:00 AM";
            lblPreviewReturning.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPreviewBypass
            // 
            lblPreviewBypass.BackColor = Color.Transparent;
            lblPreviewBypass.Dock = DockStyle.Bottom;
            lblPreviewBypass.ForeColor = Color.White;
            lblPreviewBypass.Location = new Point(0, 367);
            lblPreviewBypass.Name = "lblPreviewBypass";
            lblPreviewBypass.Size = new Size(698, 31);
            lblPreviewBypass.TabIndex = 2;
            lblPreviewBypass.Text = "Staff? Press Ctrl + X to unlock";
            lblPreviewBypass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPreviewMsg
            // 
            lblPreviewMsg.BackColor = Color.Transparent;
            lblPreviewMsg.Dock = DockStyle.Top;
            lblPreviewMsg.ForeColor = Color.White;
            lblPreviewMsg.Location = new Point(0, 0);
            lblPreviewMsg.Name = "lblPreviewMsg";
            lblPreviewMsg.Size = new Size(698, 240);
            lblPreviewMsg.TabIndex = 1;
            lblPreviewMsg.Text = "PISONET IS NOW CLOSED";
            lblPreviewMsg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpReturning
            // 
            grpReturning.Controls.Add(txtRetMargin);
            grpReturning.Controls.Add(label9);
            grpReturning.Controls.Add(chkShowReturning);
            grpReturning.Controls.Add(cmbRetFontFamily);
            grpReturning.Controls.Add(txtRetFontSize);
            grpReturning.Controls.Add(cmbRetTextColor);
            grpReturning.Controls.Add(label2);
            grpReturning.Controls.Add(label7);
            grpReturning.Controls.Add(label8);
            grpReturning.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpReturning.Location = new Point(10, 262);
            grpReturning.Name = "grpReturning";
            grpReturning.Size = new Size(300, 166);
            grpReturning.TabIndex = 1;
            grpReturning.TabStop = false;
            grpReturning.Text = "2. Returning Time Styles";
            // 
            // txtRetMargin
            // 
            txtRetMargin.Font = new Font("Segoe UI", 9F);
            txtRetMargin.Location = new Point(155, 134);
            txtRetMargin.Name = "txtRetMargin";
            txtRetMargin.Size = new Size(130, 23);
            txtRetMargin.TabIndex = 9;
            // 
            // label9
            // 
            label9.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label9.Location = new Point(155, 116);
            label9.Name = "label9";
            label9.Size = new Size(130, 15);
            label9.TabIndex = 8;
            label9.Text = "Bottom Margin:";
            // 
            // chkShowReturning
            // 
            chkShowReturning.AutoSize = true;
            chkShowReturning.Location = new Point(10, 125);
            chkShowReturning.Name = "chkShowReturning";
            chkShowReturning.Size = new Size(147, 19);
            chkShowReturning.TabIndex = 7;
            chkShowReturning.Text = "Show Returning Time";
            chkShowReturning.UseVisualStyleBackColor = true;
            // 
            // cmbRetFontFamily
            // 
            cmbRetFontFamily.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRetFontFamily.Font = new Font("Segoe UI", 9F);
            cmbRetFontFamily.Location = new Point(10, 40);
            cmbRetFontFamily.Name = "cmbRetFontFamily";
            cmbRetFontFamily.Size = new Size(130, 23);
            cmbRetFontFamily.TabIndex = 1;
            // 
            // txtRetFontSize
            // 
            txtRetFontSize.Font = new Font("Segoe UI", 9F);
            txtRetFontSize.Location = new Point(155, 40);
            txtRetFontSize.Name = "txtRetFontSize";
            txtRetFontSize.Size = new Size(130, 23);
            txtRetFontSize.TabIndex = 3;
            // 
            // cmbRetTextColor
            // 
            cmbRetTextColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRetTextColor.Font = new Font("Segoe UI", 9F);
            cmbRetTextColor.Location = new Point(10, 90);
            cmbRetTextColor.Name = "cmbRetTextColor";
            cmbRetTextColor.Size = new Size(275, 23);
            cmbRetTextColor.TabIndex = 5;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.Location = new Point(10, 22);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 0;
            label2.Text = "Font Family:";
            // 
            // label7
            // 
            label7.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label7.Location = new Point(155, 22);
            label7.Name = "label7";
            label7.Size = new Size(80, 15);
            label7.TabIndex = 2;
            label7.Text = "Font Size:";
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label8.Location = new Point(10, 72);
            label8.Name = "label8";
            label8.Size = new Size(130, 15);
            label8.TabIndex = 4;
            label8.Text = "Text Color:";
            // 
            // grpMessage
            // 
            grpMessage.Controls.Add(chkShowBypass);
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
            grpMessage.Size = new Size(300, 246);
            grpMessage.TabIndex = 0;
            grpMessage.TabStop = false;
            grpMessage.Text = "1. Main Lock Message Styles";
            // 
            // chkShowBypass
            // 
            chkShowBypass.AutoSize = true;
            chkShowBypass.Location = new Point(10, 212);
            chkShowBypass.Name = "chkShowBypass";
            chkShowBypass.Size = new Size(201, 19);
            chkShowBypass.TabIndex = 10;
            chkShowBypass.Text = "Show Bypass Instructions (Hint)";
            chkShowBypass.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            txtMessage.Font = new Font("Segoe UI", 9F);
            txtMessage.Location = new Point(10, 43);
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
            txtFontSize.Name = "txtFontSize";
            txtFontSize.Size = new Size(130, 23);
            txtFontSize.TabIndex = 5;
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
            label1.Text = "Lock Message:";
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
            btnFullScreen.Location = new Point(10, 550);
            btnFullScreen.Name = "btnFullScreen";
            btnFullScreen.Size = new Size(300, 30);
            btnFullScreen.TabIndex = 4;
            btnFullScreen.Text = "Full Screen Test";
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Green;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(10, 586);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(145, 30);
            btnSave.TabIndex = 5;
            btnSave.Text = "Save Changes";
            btnSave.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Gray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(165, 586);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(145, 30);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // grpImage
            // 
            grpImage.Controls.Add(cmbImageSize);
            grpImage.Controls.Add(txtImagePath);
            grpImage.Controls.Add(label15);
            grpImage.Controls.Add(btnSelectImage);
            grpImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpImage.Location = new Point(10, 434);
            grpImage.Name = "grpImage";
            grpImage.Size = new Size(300, 110);
            grpImage.TabIndex = 2;
            grpImage.TabStop = false;
            grpImage.Text = "3. Background Image";
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
            txtImagePath.Size = new Size(245, 23);
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
            btnSelectImage.Location = new Point(260, 25);
            btnSelectImage.Name = "btnSelectImage";
            btnSelectImage.Size = new Size(30, 23);
            btnSelectImage.TabIndex = 2;
            btnSelectImage.Text = "...";
            // 
            // LimitEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1063, 630);
            Controls.Add(splitContainer1);
            MinimumSize = new Size(1000, 400);
            Name = "LimitEditorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Desktop Curfew Dialog Editor";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            pnlPreview.ResumeLayout(false);
            grpReturning.ResumeLayout(false);
            grpReturning.PerformLayout();
            grpMessage.ResumeLayout(false);
            grpMessage.PerformLayout();
            grpImage.ResumeLayout(false);
            grpImage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel pnlPreview;
        private Label lblPreviewMsg;
        private Label lblPreviewReturning;
        private Label lblPreviewBypass;
        private GroupBox grpMessage;
        private CheckBox chkShowBypass;
        private TextBox txtMessage;
        private ComboBox cmbFontFamily;
        private TextBox txtFontSize;
        private ComboBox cmbBgColor;
        private ComboBox cmbTextColor;
        private GroupBox grpReturning;
        private TextBox txtRetMargin;
        private Label label9;
        private CheckBox chkShowReturning;
        private ComboBox cmbRetFontFamily;
        private TextBox txtRetFontSize;
        private ComboBox cmbRetTextColor;
        private GroupBox grpImage;
        private ComboBox cmbImageSize;
        private TextBox txtImagePath;
        private Button btnSelectImage;
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
        private Label label15;
    }
}