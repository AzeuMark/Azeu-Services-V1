namespace AzeuServices_V1
{
    partial class Form1
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
            shutdownAFKCheckbox = new CheckBox();
            suspiciousKeysLabel = new Label();
            kboardStatusLabel = new Label();
            mouseStatusLabel = new Label();
            countdownMinutesLabel = new Label();
            countdownTextbox = new TextBox();
            noSmokingDialogCheckbox = new CheckBox();
            noSmokingMsgLabel = new Label();
            viewNoSmokingDialog = new Button();
            currentPasswordTextbox = new TextBox();
            newPasswordTextbox = new TextBox();
            confirmPasswordTextbox = new TextBox();
            showCountdownCheckbox = new CheckBox();
            countdownTopMostCheckbox = new CheckBox();
            countdownOpacityCheckbox = new CheckBox();
            countdownOpacityTextbox = new TextBox();
            adminShutdownCheckbox = new CheckBox();
            applicationServiceCheckbox = new CheckBox();
            applicationHighPriorityCheckbox = new CheckBox();
            startupCheckbox = new CheckBox();
            minimizeTrayCheckbox = new CheckBox();
            settingStatusLabel = new Label();
            saveSettingsBtn = new Button();
            limitDesktopUsageCheckbox = new CheckBox();
            limitDesktopAMorPMComboBox = new ComboBox();
            limitDesktopOpenAMorPMComboBox = new ComboBox();
            limitDesktopActionComboBox = new ComboBox();
            limitDesktopShowDialog5minCheckbox = new CheckBox();
            limitDesktopShowDialog10minCheckbox = new CheckBox();
            limitDesktopShowDialog30minCheckbox = new CheckBox();
            limitDesktopShutdownAfter3Minutes = new CheckBox();
            viewLimitDesktopActionDialogBtn = new Button();
            btnRemoteSettings = new Button();
            afkWarningThresholdLabel = new Label();
            afkWarningThresholdTextbox = new TextBox();
            remoteServiceCheckbox = new CheckBox();
            limitDesktopHourCombo = new ComboBox();
            limitDesktopHourOpenCombo = new ComboBox();
            limitDesktopMinCombo = new ComboBox();
            limitDesktopMinOpenCombo = new ComboBox();
            SuspendLayout();
            // 
            // shutdownAFKCheckbox
            // 
            shutdownAFKCheckbox.AutoSize = true;
            shutdownAFKCheckbox.Location = new Point(24, 91);
            shutdownAFKCheckbox.Name = "shutdownAFKCheckbox";
            shutdownAFKCheckbox.Size = new Size(114, 19);
            shutdownAFKCheckbox.TabIndex = 0;
            shutdownAFKCheckbox.Text = "Shutdown if AFK";
            shutdownAFKCheckbox.UseVisualStyleBackColor = true;
            shutdownAFKCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // suspiciousKeysLabel
            // 
            suspiciousKeysLabel.AutoSize = true;
            suspiciousKeysLabel.Location = new Point(48, 113);
            suspiciousKeysLabel.Name = "suspiciousKeysLabel";
            suspiciousKeysLabel.Size = new Size(52, 15);
            suspiciousKeysLabel.TabIndex = 1;
            suspiciousKeysLabel.Text = "susLabel";
            // 
            // kboardStatusLabel
            // 
            kboardStatusLabel.AutoSize = true;
            kboardStatusLabel.Location = new Point(48, 128);
            kboardStatusLabel.Name = "kboardStatusLabel";
            kboardStatusLabel.Size = new Size(92, 15);
            kboardStatusLabel.TabIndex = 2;
            kboardStatusLabel.Text = "Keyboard Status";
            // 
            // mouseStatusLabel
            // 
            mouseStatusLabel.AutoSize = true;
            mouseStatusLabel.Location = new Point(48, 143);
            mouseStatusLabel.Name = "mouseStatusLabel";
            mouseStatusLabel.Size = new Size(78, 15);
            mouseStatusLabel.TabIndex = 3;
            mouseStatusLabel.Text = "Mouse Status";
            // 
            // countdownMinutesLabel
            // 
            countdownMinutesLabel.AutoSize = true;
            countdownMinutesLabel.Location = new Point(15, 161);
            countdownMinutesLabel.Name = "countdownMinutesLabel";
            countdownMinutesLabel.Size = new Size(119, 15);
            countdownMinutesLabel.TabIndex = 4;
            countdownMinutesLabel.Text = "Countdown minutes:";
            // 
            // countdownTextbox
            // 
            countdownTextbox.Location = new Point(155, 158);
            countdownTextbox.Name = "countdownTextbox";
            countdownTextbox.Size = new Size(40, 23);
            countdownTextbox.TabIndex = 5;
            countdownTextbox.TextChanged += OnSettingChanged;
            countdownTextbox.KeyPress += countdownTextbox_KeyPress;
            countdownTextbox.Leave += countdownTextbox_Leave;
            // 
            // noSmokingDialogCheckbox
            // 
            noSmokingDialogCheckbox.AutoSize = true;
            noSmokingDialogCheckbox.Location = new Point(15, 404);
            noSmokingDialogCheckbox.Name = "noSmokingDialogCheckbox";
            noSmokingDialogCheckbox.Size = new Size(167, 19);
            noSmokingDialogCheckbox.TabIndex = 6;
            noSmokingDialogCheckbox.Text = "Enable No Smoking Dialog";
            noSmokingDialogCheckbox.UseVisualStyleBackColor = true;
            noSmokingDialogCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // noSmokingMsgLabel
            // 
            noSmokingMsgLabel.AutoSize = true;
            noSmokingMsgLabel.Location = new Point(24, 465);
            noSmokingMsgLabel.Name = "noSmokingMsgLabel";
            noSmokingMsgLabel.Size = new Size(100, 15);
            noSmokingMsgLabel.TabIndex = 15;
            noSmokingMsgLabel.Text = "Configure Dialog:";
            // 
            // viewNoSmokingDialog
            // 
            viewNoSmokingDialog.Location = new Point(155, 461);
            viewNoSmokingDialog.Name = "viewNoSmokingDialog";
            viewNoSmokingDialog.Size = new Size(140, 23);
            viewNoSmokingDialog.TabIndex = 16;
            viewNoSmokingDialog.Text = "Open Smoking Editor";
            viewNoSmokingDialog.UseVisualStyleBackColor = true;
            viewNoSmokingDialog.Click += viewNoSmokingDialog_Click;
            // 
            // currentPasswordTextbox
            // 
            currentPasswordTextbox.Location = new Point(380, 88);
            currentPasswordTextbox.Name = "currentPasswordTextbox";
            currentPasswordTextbox.PlaceholderText = "Enter current password";
            currentPasswordTextbox.Size = new Size(180, 23);
            currentPasswordTextbox.TabIndex = 21;
            currentPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // newPasswordTextbox
            // 
            newPasswordTextbox.Location = new Point(380, 117);
            newPasswordTextbox.Name = "newPasswordTextbox";
            newPasswordTextbox.PlaceholderText = "Enter new password";
            newPasswordTextbox.Size = new Size(180, 23);
            newPasswordTextbox.TabIndex = 22;
            newPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // confirmPasswordTextbox
            // 
            confirmPasswordTextbox.Location = new Point(380, 146);
            confirmPasswordTextbox.Name = "confirmPasswordTextbox";
            confirmPasswordTextbox.PlaceholderText = "Confirm new password";
            confirmPasswordTextbox.Size = new Size(180, 23);
            confirmPasswordTextbox.TabIndex = 23;
            confirmPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // showCountdownCheckbox
            // 
            showCountdownCheckbox.AutoSize = true;
            showCountdownCheckbox.Location = new Point(320, 191);
            showCountdownCheckbox.Name = "showCountdownCheckbox";
            showCountdownCheckbox.Size = new Size(162, 19);
            showCountdownCheckbox.TabIndex = 24;
            showCountdownCheckbox.Text = "Show Countdown Widget";
            showCountdownCheckbox.UseVisualStyleBackColor = true;
            showCountdownCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownTopMostCheckbox
            // 
            countdownTopMostCheckbox.AutoSize = true;
            countdownTopMostCheckbox.Location = new Point(340, 215);
            countdownTopMostCheckbox.Name = "countdownTopMostCheckbox";
            countdownTopMostCheckbox.Size = new Size(141, 19);
            countdownTopMostCheckbox.TabIndex = 25;
            countdownTopMostCheckbox.Text = "Countdown Top Most";
            countdownTopMostCheckbox.UseVisualStyleBackColor = true;
            countdownTopMostCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownOpacityCheckbox
            // 
            countdownOpacityCheckbox.AutoSize = true;
            countdownOpacityCheckbox.Location = new Point(340, 239);
            countdownOpacityCheckbox.Name = "countdownOpacityCheckbox";
            countdownOpacityCheckbox.Size = new Size(174, 19);
            countdownOpacityCheckbox.TabIndex = 26;
            countdownOpacityCheckbox.Text = "Enable Countdown Opacity:";
            countdownOpacityCheckbox.UseVisualStyleBackColor = true;
            countdownOpacityCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownOpacityTextbox
            // 
            countdownOpacityTextbox.Location = new Point(530, 237);
            countdownOpacityTextbox.Name = "countdownOpacityTextbox";
            countdownOpacityTextbox.Size = new Size(30, 23);
            countdownOpacityTextbox.TabIndex = 27;
            countdownOpacityTextbox.TextChanged += OnSettingChanged;
            countdownOpacityTextbox.KeyPress += countdownOpacityTextbox_KeyPress;
            countdownOpacityTextbox.Leave += countdownOpacityTextbox_Leave;
            // 
            // adminShutdownCheckbox
            // 
            adminShutdownCheckbox.AutoSize = true;
            adminShutdownCheckbox.Location = new Point(365, 482);
            adminShutdownCheckbox.Name = "adminShutdownCheckbox";
            adminShutdownCheckbox.Size = new Size(160, 19);
            adminShutdownCheckbox.TabIndex = 28;
            adminShutdownCheckbox.Text = "Administrative Shutdown";
            adminShutdownCheckbox.UseVisualStyleBackColor = true;
            adminShutdownCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // applicationServiceCheckbox
            // 
            applicationServiceCheckbox.AutoSize = true;
            applicationServiceCheckbox.Location = new Point(365, 507);
            applicationServiceCheckbox.Name = "applicationServiceCheckbox";
            applicationServiceCheckbox.Size = new Size(146, 19);
            applicationServiceCheckbox.TabIndex = 29;
            applicationServiceCheckbox.Text = "Keep App Alive Service";
            applicationServiceCheckbox.UseVisualStyleBackColor = true;
            applicationServiceCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // applicationHighPriorityCheckbox
            // 
            applicationHighPriorityCheckbox.AutoSize = true;
            applicationHighPriorityCheckbox.Location = new Point(365, 532);
            applicationHighPriorityCheckbox.Name = "applicationHighPriorityCheckbox";
            applicationHighPriorityCheckbox.Size = new Size(153, 19);
            applicationHighPriorityCheckbox.TabIndex = 30;
            applicationHighPriorityCheckbox.Text = "High CPU Priority Mode";
            applicationHighPriorityCheckbox.UseVisualStyleBackColor = true;
            applicationHighPriorityCheckbox.CheckedChanged += applicationHighPriority_CheckedChanged;
            // 
            // startupCheckbox
            // 
            startupCheckbox.AutoSize = true;
            startupCheckbox.Location = new Point(440, 557);
            startupCheckbox.Name = "startupCheckbox";
            startupCheckbox.Size = new Size(122, 19);
            startupCheckbox.TabIndex = 31;
            startupCheckbox.Text = "Launch on startup";
            startupCheckbox.UseVisualStyleBackColor = true;
            startupCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // minimizeTrayCheckbox
            // 
            minimizeTrayCheckbox.AutoSize = true;
            minimizeTrayCheckbox.Location = new Point(395, 582);
            minimizeTrayCheckbox.Name = "minimizeTrayCheckbox";
            minimizeTrayCheckbox.Size = new Size(159, 19);
            minimizeTrayCheckbox.TabIndex = 32;
            minimizeTrayCheckbox.Text = "Minimize to tray if closed";
            minimizeTrayCheckbox.UseVisualStyleBackColor = true;
            minimizeTrayCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // settingStatusLabel
            // 
            settingStatusLabel.AutoSize = true;
            settingStatusLabel.Location = new Point(365, 607);
            settingStatusLabel.Name = "settingStatusLabel";
            settingStatusLabel.Size = new Size(70, 15);
            settingStatusLabel.TabIndex = 33;
            settingStatusLabel.Text = "No changes";
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Location = new Point(495, 603);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(75, 23);
            saveSettingsBtn.TabIndex = 34;
            saveSettingsBtn.Text = "Save";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += saveSettingsBtn_Click;
            // 
            // limitDesktopUsageCheckbox
            // 
            limitDesktopUsageCheckbox.AutoSize = true;
            limitDesktopUsageCheckbox.Location = new Point(610, 100);
            limitDesktopUsageCheckbox.Name = "limitDesktopUsageCheckbox";
            limitDesktopUsageCheckbox.Size = new Size(148, 19);
            limitDesktopUsageCheckbox.TabIndex = 35;
            limitDesktopUsageCheckbox.Text = "Enable Desktop Curfew";
            limitDesktopUsageCheckbox.UseVisualStyleBackColor = true;
            limitDesktopUsageCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // limitDesktopAMorPMComboBox
            // 
            limitDesktopAMorPMComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            limitDesktopAMorPMComboBox.Location = new Point(755, 125);
            limitDesktopAMorPMComboBox.Name = "limitDesktopAMorPMComboBox";
            limitDesktopAMorPMComboBox.Size = new Size(50, 23);
            limitDesktopAMorPMComboBox.TabIndex = 9;
            limitDesktopAMorPMComboBox.SelectedIndexChanged += OnSettingChanged;
            // 
            // limitDesktopOpenAMorPMComboBox
            // 
            limitDesktopOpenAMorPMComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopOpenAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            limitDesktopOpenAMorPMComboBox.Location = new Point(755, 183);
            limitDesktopOpenAMorPMComboBox.Name = "limitDesktopOpenAMorPMComboBox";
            limitDesktopOpenAMorPMComboBox.Size = new Size(50, 23);
            limitDesktopOpenAMorPMComboBox.TabIndex = 8;
            limitDesktopOpenAMorPMComboBox.SelectedIndexChanged += OnSettingChanged;
            // 
            // limitDesktopActionComboBox
            // 
            limitDesktopActionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopActionComboBox.Items.AddRange(new object[] { "Shutdown", "Show Image Dialog" });
            limitDesktopActionComboBox.Location = new Point(635, 240);
            limitDesktopActionComboBox.Name = "limitDesktopActionComboBox";
            limitDesktopActionComboBox.Size = new Size(150, 23);
            limitDesktopActionComboBox.TabIndex = 7;
            limitDesktopActionComboBox.SelectedIndexChanged += OnUIStateChanged;
            // 
            // limitDesktopShowDialog5minCheckbox
            // 
            limitDesktopShowDialog5minCheckbox.Location = new Point(620, 310);
            limitDesktopShowDialog5minCheckbox.Name = "limitDesktopShowDialog5minCheckbox";
            limitDesktopShowDialog5minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog5minCheckbox.TabIndex = 4;
            limitDesktopShowDialog5minCheckbox.Text = "Show 5 min warning";
            limitDesktopShowDialog5minCheckbox.UseVisualStyleBackColor = true;
            limitDesktopShowDialog5minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShowDialog10minCheckbox
            // 
            limitDesktopShowDialog10minCheckbox.Location = new Point(620, 340);
            limitDesktopShowDialog10minCheckbox.Name = "limitDesktopShowDialog10minCheckbox";
            limitDesktopShowDialog10minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog10minCheckbox.TabIndex = 3;
            limitDesktopShowDialog10minCheckbox.Text = "Show 10 min warning";
            limitDesktopShowDialog10minCheckbox.UseVisualStyleBackColor = true;
            limitDesktopShowDialog10minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShowDialog30minCheckbox
            // 
            limitDesktopShowDialog30minCheckbox.Location = new Point(620, 370);
            limitDesktopShowDialog30minCheckbox.Name = "limitDesktopShowDialog30minCheckbox";
            limitDesktopShowDialog30minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog30minCheckbox.TabIndex = 2;
            limitDesktopShowDialog30minCheckbox.Text = "Show 30 min warning";
            limitDesktopShowDialog30minCheckbox.UseVisualStyleBackColor = true;
            limitDesktopShowDialog30minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShutdownAfter3Minutes
            // 
            limitDesktopShutdownAfter3Minutes.Location = new Point(620, 400);
            limitDesktopShutdownAfter3Minutes.Name = "limitDesktopShutdownAfter3Minutes";
            limitDesktopShutdownAfter3Minutes.Size = new Size(200, 23);
            limitDesktopShutdownAfter3Minutes.TabIndex = 1;
            limitDesktopShutdownAfter3Minutes.Text = "Shutdown after 3 min";
            limitDesktopShutdownAfter3Minutes.UseVisualStyleBackColor = true;
            limitDesktopShutdownAfter3Minutes.CheckedChanged += OnSettingChanged;
            // 
            // viewLimitDesktopActionDialogBtn
            // 
            viewLimitDesktopActionDialogBtn.Location = new Point(635, 430);
            viewLimitDesktopActionDialogBtn.Name = "viewLimitDesktopActionDialogBtn";
            viewLimitDesktopActionDialogBtn.Size = new Size(150, 23);
            viewLimitDesktopActionDialogBtn.TabIndex = 0;
            viewLimitDesktopActionDialogBtn.Text = "Preview Curfew screen";
            viewLimitDesktopActionDialogBtn.UseVisualStyleBackColor = true;
            viewLimitDesktopActionDialogBtn.Click += viewLimitDesktopActionDialogBtn_Click;
            // 
            // btnRemoteSettings
            // 
            btnRemoteSettings.Location = new Point(340, 381);
            btnRemoteSettings.Name = "btnRemoteSettings";
            btnRemoteSettings.Size = new Size(111, 23);
            btnRemoteSettings.TabIndex = 40;
            btnRemoteSettings.Text = "Remote Service";
            btnRemoteSettings.UseVisualStyleBackColor = true;
            btnRemoteSettings.Click += btnRemoteSettings_Click;
            // 
            // afkWarningThresholdLabel
            // 
            afkWarningThresholdLabel.AutoSize = true;
            afkWarningThresholdLabel.Location = new Point(17, 186);
            afkWarningThresholdLabel.Name = "afkWarningThresholdLabel";
            afkWarningThresholdLabel.Size = new Size(121, 15);
            afkWarningThresholdLabel.TabIndex = 42;
            afkWarningThresholdLabel.Text = "Show Warning (Secs):";
            // 
            // afkWarningThresholdTextbox
            // 
            afkWarningThresholdTextbox.Location = new Point(155, 185);
            afkWarningThresholdTextbox.Name = "afkWarningThresholdTextbox";
            afkWarningThresholdTextbox.Size = new Size(40, 23);
            afkWarningThresholdTextbox.TabIndex = 41;
            afkWarningThresholdTextbox.TextChanged += OnSettingChanged;
            afkWarningThresholdTextbox.KeyPress += afkWarningThresholdTextbox_KeyPress;
            afkWarningThresholdTextbox.Leave += afkWarningThresholdTextbox_Leave;
            // 
            // remoteServiceCheckbox
            // 
            remoteServiceCheckbox.AutoSize = true;
            remoteServiceCheckbox.Location = new Point(339, 354);
            remoteServiceCheckbox.Name = "remoteServiceCheckbox";
            remoteServiceCheckbox.Size = new Size(154, 19);
            remoteServiceCheckbox.TabIndex = 41;
            remoteServiceCheckbox.Text = "remoteServiceCheckbox";
            remoteServiceCheckbox.UseVisualStyleBackColor = true;
            remoteServiceCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // limitDesktopHourCombo
            // 
            limitDesktopHourCombo.AutoCompleteCustomSource.AddRange(new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourCombo.FormattingEnabled = true;
            limitDesktopHourCombo.Items.AddRange(new object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourCombo.Location = new Point(635, 125);
            limitDesktopHourCombo.Name = "limitDesktopHourCombo";
            limitDesktopHourCombo.Size = new Size(50, 23);
            limitDesktopHourCombo.TabIndex = 43;
            limitDesktopHourCombo.KeyPress += AMandPM_KeyPress;
            limitDesktopHourCombo.Leave += AMandPM_Leave;
            // 
            // limitDesktopHourOpenCombo
            // 
            limitDesktopHourOpenCombo.AutoCompleteCustomSource.AddRange(new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourOpenCombo.FormattingEnabled = true;
            limitDesktopHourOpenCombo.Items.AddRange(new object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourOpenCombo.Location = new Point(635, 183);
            limitDesktopHourOpenCombo.Name = "limitDesktopHourOpenCombo";
            limitDesktopHourOpenCombo.Size = new Size(50, 23);
            limitDesktopHourOpenCombo.TabIndex = 44;
            limitDesktopHourOpenCombo.KeyPress += AMandPM_KeyPress;
            limitDesktopHourOpenCombo.Leave += AMandPM_Leave;
            // 
            // limitDesktopMinCombo
            // 
            limitDesktopMinCombo.FormattingEnabled = true;
            limitDesktopMinCombo.Items.AddRange(new object[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "", "", "```", "", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" });
            limitDesktopMinCombo.Location = new Point(695, 125);
            limitDesktopMinCombo.Name = "limitDesktopMinCombo";
            limitDesktopMinCombo.Size = new Size(50, 23);
            limitDesktopMinCombo.TabIndex = 45;
            limitDesktopMinCombo.KeyPress += MinutesLimit_KeyPress;
            limitDesktopMinCombo.Leave += MinutesLimit_Leave;
            // 
            // limitDesktopMinOpenCombo
            // 
            limitDesktopMinOpenCombo.FormattingEnabled = true;
            limitDesktopMinOpenCombo.Items.AddRange(new object[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "", "", "```", "", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59" });
            limitDesktopMinOpenCombo.Location = new Point(695, 183);
            limitDesktopMinOpenCombo.Name = "limitDesktopMinOpenCombo";
            limitDesktopMinOpenCombo.Size = new Size(50, 23);
            limitDesktopMinOpenCombo.TabIndex = 46;
            limitDesktopMinOpenCombo.KeyPress += MinutesLimit_KeyPress;
            limitDesktopMinOpenCombo.Leave += MinutesLimit_Leave;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(880, 650);
            Controls.Add(limitDesktopMinOpenCombo);
            Controls.Add(limitDesktopMinCombo);
            Controls.Add(limitDesktopHourOpenCombo);
            Controls.Add(limitDesktopHourCombo);
            Controls.Add(remoteServiceCheckbox);
            Controls.Add(btnRemoteSettings);
            Controls.Add(viewLimitDesktopActionDialogBtn);
            Controls.Add(limitDesktopShutdownAfter3Minutes);
            Controls.Add(limitDesktopShowDialog30minCheckbox);
            Controls.Add(limitDesktopShowDialog10minCheckbox);
            Controls.Add(limitDesktopShowDialog5minCheckbox);
            Controls.Add(limitDesktopActionComboBox);
            Controls.Add(limitDesktopOpenAMorPMComboBox);
            Controls.Add(limitDesktopAMorPMComboBox);
            Controls.Add(limitDesktopUsageCheckbox);
            Controls.Add(saveSettingsBtn);
            Controls.Add(settingStatusLabel);
            Controls.Add(minimizeTrayCheckbox);
            Controls.Add(startupCheckbox);
            Controls.Add(applicationHighPriorityCheckbox);
            Controls.Add(applicationServiceCheckbox);
            Controls.Add(adminShutdownCheckbox);
            Controls.Add(countdownOpacityTextbox);
            Controls.Add(countdownOpacityCheckbox);
            Controls.Add(countdownTopMostCheckbox);
            Controls.Add(showCountdownCheckbox);
            Controls.Add(confirmPasswordTextbox);
            Controls.Add(newPasswordTextbox);
            Controls.Add(currentPasswordTextbox);
            Controls.Add(viewNoSmokingDialog);
            Controls.Add(noSmokingMsgLabel);
            Controls.Add(noSmokingDialogCheckbox);
            Controls.Add(countdownTextbox);
            Controls.Add(countdownMinutesLabel);
            Controls.Add(mouseStatusLabel);
            Controls.Add(kboardStatusLabel);
            Controls.Add(suspiciousKeysLabel);
            Controls.Add(shutdownAFKCheckbox);
            Controls.Add(afkWarningThresholdLabel);
            Controls.Add(afkWarningThresholdTextbox);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Azeu Services V1";
            FormClosing += OnFormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox shutdownAFKCheckbox;
        private Label suspiciousKeysLabel;
        private Label kboardStatusLabel;
        private Label mouseStatusLabel;
        private Label countdownMinutesLabel;
        private TextBox countdownTextbox;
        private CheckBox noSmokingDialogCheckbox;
        private Label noSmokingMsgLabel;
        private Button viewNoSmokingDialog;
        private TextBox currentPasswordTextbox;
        private TextBox newPasswordTextbox;
        private TextBox confirmPasswordTextbox;
        private CheckBox showCountdownCheckbox;
        private CheckBox countdownTopMostCheckbox;
        private CheckBox countdownOpacityCheckbox;
        private TextBox countdownOpacityTextbox;
        private CheckBox adminShutdownCheckbox;
        private CheckBox applicationServiceCheckbox;
        private CheckBox applicationHighPriorityCheckbox;
        private CheckBox startupCheckbox;
        private CheckBox minimizeTrayCheckbox;
        private Label settingStatusLabel;
        private Button saveSettingsBtn;
        private CheckBox limitDesktopUsageCheckbox;
        private ComboBox limitDesktopAMorPMComboBox;
        private ComboBox limitDesktopOpenAMorPMComboBox;
        private ComboBox limitDesktopActionComboBox;
        private CheckBox limitDesktopShowDialog5minCheckbox;
        private CheckBox limitDesktopShowDialog10minCheckbox;
        private CheckBox limitDesktopShowDialog30minCheckbox;
        private CheckBox limitDesktopShutdownAfter3Minutes;
        private Button viewLimitDesktopActionDialogBtn;
        private Button btnRemoteSettings;
        private Label afkWarningThresholdLabel;
        private TextBox afkWarningThresholdTextbox;
        private CheckBox remoteServiceCheckbox;
        private ComboBox limitDesktopHourCombo;
        private ComboBox limitDesktopHourOpenCombo;
        private ComboBox limitDesktopMinCombo;
        private ComboBox limitDesktopMinOpenCombo;
    }
}