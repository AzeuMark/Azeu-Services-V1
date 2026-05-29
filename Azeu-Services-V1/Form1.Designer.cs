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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            shutdownAFKCheckbox = new CheckBox();
            suspiciousKeysLabel = new Label();
            kboardStatusLabel = new Label();
            mouseStatusLabel = new Label();
            countdownMinutesLabel = new Label();
            countdownTextbox = new TextBox();
            noSmokingDialogCheckbox = new CheckBox();
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
            afkWarningThresholdLabel = new Label();
            afkWarningThresholdTextbox = new TextBox();
            remoteServiceCheckbox = new CheckBox();
            limitDesktopHourCombo = new ComboBox();
            limitDesktopHourOpenCombo = new ComboBox();
            limitDesktopMinCombo = new ComboBox();
            limitDesktopMinOpenCombo = new ComboBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            panel1 = new Panel();
            label2 = new Label();
            panel2 = new Panel();
            label5 = new Label();
            panel3 = new Panel();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label10 = new Label();
            panel4 = new Panel();
            label14 = new Label();
            label12 = new Label();
            label11 = new Label();
            label13 = new Label();
            panel5 = new Panel();
            label15 = new Label();
            panel6 = new Panel();
            btnRemoteSettings = new Button();
            label16 = new Label();
            panel7 = new Panel();
            panel8 = new Panel();
            facebookUrlLinkLabel = new LinkLabel();
            label20 = new Label();
            label19 = new Label();
            label18 = new Label();
            label17 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            panel7.SuspendLayout();
            panel8.SuspendLayout();
            SuspendLayout();
            // 
            // shutdownAFKCheckbox
            // 
            shutdownAFKCheckbox.AutoSize = true;
            shutdownAFKCheckbox.Font = new Font("Segoe UI", 10F);
            shutdownAFKCheckbox.Location = new Point(11, 22);
            shutdownAFKCheckbox.Name = "shutdownAFKCheckbox";
            shutdownAFKCheckbox.Size = new Size(123, 23);
            shutdownAFKCheckbox.TabIndex = 0;
            shutdownAFKCheckbox.Text = "Enable anti AFK";
            shutdownAFKCheckbox.UseVisualStyleBackColor = true;
            shutdownAFKCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // suspiciousKeysLabel
            // 
            suspiciousKeysLabel.Font = new Font("Segoe UI", 12F);
            suspiciousKeysLabel.Location = new Point(187, 18);
            suspiciousKeysLabel.Name = "suspiciousKeysLabel";
            suspiciousKeysLabel.Size = new Size(63, 23);
            suspiciousKeysLabel.TabIndex = 1;
            suspiciousKeysLabel.Text = "Normal";
            suspiciousKeysLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // kboardStatusLabel
            // 
            kboardStatusLabel.Font = new Font("Segoe UI", 12F);
            kboardStatusLabel.Location = new Point(187, 43);
            kboardStatusLabel.Name = "kboardStatusLabel";
            kboardStatusLabel.Size = new Size(72, 21);
            kboardStatusLabel.TabIndex = 2;
            kboardStatusLabel.Text = "Active";
            kboardStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // mouseStatusLabel
            // 
            mouseStatusLabel.Font = new Font("Segoe UI", 12F);
            mouseStatusLabel.Location = new Point(187, 64);
            mouseStatusLabel.Name = "mouseStatusLabel";
            mouseStatusLabel.Size = new Size(63, 21);
            mouseStatusLabel.TabIndex = 3;
            mouseStatusLabel.Text = "Active";
            mouseStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // countdownMinutesLabel
            // 
            countdownMinutesLabel.AutoSize = true;
            countdownMinutesLabel.Location = new Point(27, 49);
            countdownMinutesLabel.Name = "countdownMinutesLabel";
            countdownMinutesLabel.Size = new Size(180, 15);
            countdownMinutesLabel.TabIndex = 4;
            countdownMinutesLabel.Text = "Countdown threshold (minutes):";
            // 
            // countdownTextbox
            // 
            countdownTextbox.Location = new Point(215, 41);
            countdownTextbox.MaxLength = 3;
            countdownTextbox.Name = "countdownTextbox";
            countdownTextbox.Size = new Size(35, 23);
            countdownTextbox.TabIndex = 5;
            countdownTextbox.TextChanged += OnSettingChanged;
            countdownTextbox.KeyPress += countdownTextbox_KeyPress;
            countdownTextbox.Leave += countdownTextbox_Leave;
            // 
            // noSmokingDialogCheckbox
            // 
            noSmokingDialogCheckbox.AutoSize = true;
            noSmokingDialogCheckbox.Font = new Font("Segoe UI", 10F);
            noSmokingDialogCheckbox.Location = new Point(11, 23);
            noSmokingDialogCheckbox.Name = "noSmokingDialogCheckbox";
            noSmokingDialogCheckbox.Size = new Size(185, 23);
            noSmokingDialogCheckbox.TabIndex = 6;
            noSmokingDialogCheckbox.Text = "Enable no smoking dialog";
            noSmokingDialogCheckbox.UseVisualStyleBackColor = true;
            noSmokingDialogCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // viewNoSmokingDialog
            // 
            viewNoSmokingDialog.Location = new Point(146, 48);
            viewNoSmokingDialog.Name = "viewNoSmokingDialog";
            viewNoSmokingDialog.Size = new Size(107, 23);
            viewNoSmokingDialog.TabIndex = 16;
            viewNoSmokingDialog.Text = "Edit Dialog";
            viewNoSmokingDialog.UseVisualStyleBackColor = true;
            viewNoSmokingDialog.Click += viewNoSmokingDialog_Click;
            // 
            // currentPasswordTextbox
            // 
            currentPasswordTextbox.Location = new Point(22, 42);
            currentPasswordTextbox.Name = "currentPasswordTextbox";
            currentPasswordTextbox.PlaceholderText = "Input current password";
            currentPasswordTextbox.Size = new Size(228, 23);
            currentPasswordTextbox.TabIndex = 21;
            currentPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // newPasswordTextbox
            // 
            newPasswordTextbox.Location = new Point(22, 86);
            newPasswordTextbox.Name = "newPasswordTextbox";
            newPasswordTextbox.PlaceholderText = "Enter new password";
            newPasswordTextbox.Size = new Size(228, 23);
            newPasswordTextbox.TabIndex = 22;
            newPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // confirmPasswordTextbox
            // 
            confirmPasswordTextbox.Location = new Point(22, 132);
            confirmPasswordTextbox.Name = "confirmPasswordTextbox";
            confirmPasswordTextbox.PlaceholderText = "Confirm new password";
            confirmPasswordTextbox.Size = new Size(228, 23);
            confirmPasswordTextbox.TabIndex = 23;
            confirmPasswordTextbox.TextChanged += PasswordTextbox_TextChanged;
            // 
            // showCountdownCheckbox
            // 
            showCountdownCheckbox.AutoSize = true;
            showCountdownCheckbox.Location = new Point(22, 94);
            showCountdownCheckbox.Name = "showCountdownCheckbox";
            showCountdownCheckbox.Size = new Size(155, 19);
            showCountdownCheckbox.TabIndex = 24;
            showCountdownCheckbox.Text = "Show countdown dialog";
            showCountdownCheckbox.UseVisualStyleBackColor = true;
            showCountdownCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownTopMostCheckbox
            // 
            countdownTopMostCheckbox.AutoSize = true;
            countdownTopMostCheckbox.Location = new Point(35, 119);
            countdownTopMostCheckbox.Name = "countdownTopMostCheckbox";
            countdownTopMostCheckbox.Size = new Size(111, 19);
            countdownTopMostCheckbox.TabIndex = 25;
            countdownTopMostCheckbox.Text = "Top most dialog";
            countdownTopMostCheckbox.UseVisualStyleBackColor = true;
            countdownTopMostCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownOpacityCheckbox
            // 
            countdownOpacityCheckbox.AutoSize = true;
            countdownOpacityCheckbox.Location = new Point(35, 143);
            countdownOpacityCheckbox.Name = "countdownOpacityCheckbox";
            countdownOpacityCheckbox.Size = new Size(89, 19);
            countdownOpacityCheckbox.TabIndex = 26;
            countdownOpacityCheckbox.Text = "Use Opacity";
            countdownOpacityCheckbox.UseVisualStyleBackColor = true;
            countdownOpacityCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // countdownOpacityTextbox
            // 
            countdownOpacityTextbox.Location = new Point(215, 139);
            countdownOpacityTextbox.MaxLength = 3;
            countdownOpacityTextbox.Name = "countdownOpacityTextbox";
            countdownOpacityTextbox.Size = new Size(35, 23);
            countdownOpacityTextbox.TabIndex = 27;
            countdownOpacityTextbox.TextChanged += OnSettingChanged;
            countdownOpacityTextbox.KeyPress += countdownOpacityTextbox_KeyPress;
            countdownOpacityTextbox.Leave += countdownOpacityTextbox_Leave;
            // 
            // adminShutdownCheckbox
            // 
            adminShutdownCheckbox.AutoSize = true;
            adminShutdownCheckbox.BackColor = Color.Transparent;
            adminShutdownCheckbox.Font = new Font("Segoe UI", 10F);
            adminShutdownCheckbox.Location = new Point(11, 69);
            adminShutdownCheckbox.Name = "adminShutdownCheckbox";
            adminShutdownCheckbox.Size = new Size(181, 23);
            adminShutdownCheckbox.TabIndex = 28;
            adminShutdownCheckbox.Text = "Administrative shutdown";
            adminShutdownCheckbox.UseVisualStyleBackColor = false;
            adminShutdownCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // applicationServiceCheckbox
            // 
            applicationServiceCheckbox.AutoSize = true;
            applicationServiceCheckbox.BackColor = Color.Transparent;
            applicationServiceCheckbox.Font = new Font("Segoe UI", 10F);
            applicationServiceCheckbox.Location = new Point(11, 115);
            applicationServiceCheckbox.Name = "applicationServiceCheckbox";
            applicationServiceCheckbox.Size = new Size(161, 23);
            applicationServiceCheckbox.TabIndex = 29;
            applicationServiceCheckbox.Text = "Keep app alive service";
            applicationServiceCheckbox.UseVisualStyleBackColor = false;
            applicationServiceCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // applicationHighPriorityCheckbox
            // 
            applicationHighPriorityCheckbox.AutoSize = true;
            applicationHighPriorityCheckbox.BackColor = Color.Transparent;
            applicationHighPriorityCheckbox.Font = new Font("Segoe UI", 10F);
            applicationHighPriorityCheckbox.Location = new Point(11, 92);
            applicationHighPriorityCheckbox.Name = "applicationHighPriorityCheckbox";
            applicationHighPriorityCheckbox.Size = new Size(175, 23);
            applicationHighPriorityCheckbox.TabIndex = 30;
            applicationHighPriorityCheckbox.Text = "High CPU priority mode";
            applicationHighPriorityCheckbox.UseVisualStyleBackColor = false;
            applicationHighPriorityCheckbox.CheckedChanged += applicationHighPriority_CheckedChanged;
            // 
            // startupCheckbox
            // 
            startupCheckbox.AutoSize = true;
            startupCheckbox.BackColor = Color.Transparent;
            startupCheckbox.Font = new Font("Segoe UI", 10F);
            startupCheckbox.Location = new Point(11, 46);
            startupCheckbox.Name = "startupCheckbox";
            startupCheckbox.Size = new Size(197, 23);
            startupCheckbox.TabIndex = 31;
            startupCheckbox.Text = "Launch on windows startup";
            startupCheckbox.UseVisualStyleBackColor = false;
            startupCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // minimizeTrayCheckbox
            // 
            minimizeTrayCheckbox.AutoSize = true;
            minimizeTrayCheckbox.BackColor = Color.Transparent;
            minimizeTrayCheckbox.Font = new Font("Segoe UI", 10F);
            minimizeTrayCheckbox.Location = new Point(11, 23);
            minimizeTrayCheckbox.Name = "minimizeTrayCheckbox";
            minimizeTrayCheckbox.Size = new Size(207, 23);
            minimizeTrayCheckbox.TabIndex = 32;
            minimizeTrayCheckbox.Text = "Minimize to tray when closed";
            minimizeTrayCheckbox.UseVisualStyleBackColor = false;
            minimizeTrayCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // settingStatusLabel
            // 
            settingStatusLabel.AutoSize = true;
            settingStatusLabel.Font = new Font("Segoe UI", 11F);
            settingStatusLabel.Location = new Point(569, 462);
            settingStatusLabel.Name = "settingStatusLabel";
            settingStatusLabel.Size = new Size(129, 20);
            settingStatusLabel.TabIndex = 33;
            settingStatusLabel.Text = "No changes made";
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Location = new Point(753, 454);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(80, 39);
            saveSettingsBtn.TabIndex = 34;
            saveSettingsBtn.Text = "Save";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += saveSettingsBtn_Click;
            // 
            // limitDesktopUsageCheckbox
            // 
            limitDesktopUsageCheckbox.AutoSize = true;
            limitDesktopUsageCheckbox.Font = new Font("Segoe UI", 10F);
            limitDesktopUsageCheckbox.Location = new Point(11, 23);
            limitDesktopUsageCheckbox.Name = "limitDesktopUsageCheckbox";
            limitDesktopUsageCheckbox.Size = new Size(153, 23);
            limitDesktopUsageCheckbox.TabIndex = 35;
            limitDesktopUsageCheckbox.Text = "Enable curfew dialog";
            limitDesktopUsageCheckbox.UseVisualStyleBackColor = true;
            limitDesktopUsageCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // limitDesktopAMorPMComboBox
            // 
            limitDesktopAMorPMComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            limitDesktopAMorPMComboBox.Location = new Point(165, 66);
            limitDesktopAMorPMComboBox.Name = "limitDesktopAMorPMComboBox";
            limitDesktopAMorPMComboBox.Size = new Size(50, 23);
            limitDesktopAMorPMComboBox.TabIndex = 9;
            limitDesktopAMorPMComboBox.SelectedIndexChanged += OnSettingChanged;
            // 
            // limitDesktopOpenAMorPMComboBox
            // 
            limitDesktopOpenAMorPMComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopOpenAMorPMComboBox.Items.AddRange(new object[] { "AM", "PM" });
            limitDesktopOpenAMorPMComboBox.Location = new Point(165, 110);
            limitDesktopOpenAMorPMComboBox.Name = "limitDesktopOpenAMorPMComboBox";
            limitDesktopOpenAMorPMComboBox.Size = new Size(50, 23);
            limitDesktopOpenAMorPMComboBox.TabIndex = 8;
            limitDesktopOpenAMorPMComboBox.SelectedIndexChanged += OnSettingChanged;
            // 
            // limitDesktopActionComboBox
            // 
            limitDesktopActionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            limitDesktopActionComboBox.Items.AddRange(new object[] { "Shutdown", "Show Image" });
            limitDesktopActionComboBox.Location = new Point(53, 232);
            limitDesktopActionComboBox.Name = "limitDesktopActionComboBox";
            limitDesktopActionComboBox.Size = new Size(115, 23);
            limitDesktopActionComboBox.TabIndex = 7;
            limitDesktopActionComboBox.SelectedIndexChanged += OnUIStateChanged;
            // 
            // limitDesktopShowDialog5minCheckbox
            // 
            limitDesktopShowDialog5minCheckbox.BackColor = Color.Transparent;
            limitDesktopShowDialog5minCheckbox.BackgroundImageLayout = ImageLayout.None;
            limitDesktopShowDialog5minCheckbox.Location = new Point(27, 139);
            limitDesktopShowDialog5minCheckbox.Name = "limitDesktopShowDialog5minCheckbox";
            limitDesktopShowDialog5minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog5minCheckbox.TabIndex = 4;
            limitDesktopShowDialog5minCheckbox.Text = "Show 5 min warning";
            limitDesktopShowDialog5minCheckbox.UseVisualStyleBackColor = false;
            limitDesktopShowDialog5minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShowDialog10minCheckbox
            // 
            limitDesktopShowDialog10minCheckbox.BackColor = Color.Transparent;
            limitDesktopShowDialog10minCheckbox.BackgroundImageLayout = ImageLayout.None;
            limitDesktopShowDialog10minCheckbox.Location = new Point(27, 162);
            limitDesktopShowDialog10minCheckbox.Name = "limitDesktopShowDialog10minCheckbox";
            limitDesktopShowDialog10minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog10minCheckbox.TabIndex = 3;
            limitDesktopShowDialog10minCheckbox.Text = "Show 10 min warning";
            limitDesktopShowDialog10minCheckbox.UseVisualStyleBackColor = false;
            limitDesktopShowDialog10minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShowDialog30minCheckbox
            // 
            limitDesktopShowDialog30minCheckbox.BackColor = Color.Transparent;
            limitDesktopShowDialog30minCheckbox.BackgroundImageLayout = ImageLayout.None;
            limitDesktopShowDialog30minCheckbox.Location = new Point(27, 185);
            limitDesktopShowDialog30minCheckbox.Name = "limitDesktopShowDialog30minCheckbox";
            limitDesktopShowDialog30minCheckbox.Size = new Size(200, 23);
            limitDesktopShowDialog30minCheckbox.TabIndex = 2;
            limitDesktopShowDialog30minCheckbox.Text = "Show 30 min warning";
            limitDesktopShowDialog30minCheckbox.UseVisualStyleBackColor = false;
            limitDesktopShowDialog30minCheckbox.CheckedChanged += OnSettingChanged;
            // 
            // limitDesktopShutdownAfter3Minutes
            // 
            limitDesktopShutdownAfter3Minutes.BackColor = Color.Transparent;
            limitDesktopShutdownAfter3Minutes.BackgroundImageLayout = ImageLayout.None;
            limitDesktopShutdownAfter3Minutes.Location = new Point(53, 259);
            limitDesktopShutdownAfter3Minutes.Name = "limitDesktopShutdownAfter3Minutes";
            limitDesktopShutdownAfter3Minutes.Size = new Size(176, 23);
            limitDesktopShutdownAfter3Minutes.TabIndex = 1;
            limitDesktopShutdownAfter3Minutes.Text = "Shutdown after 3 minutes";
            limitDesktopShutdownAfter3Minutes.UseVisualStyleBackColor = false;
            limitDesktopShutdownAfter3Minutes.CheckedChanged += OnSettingChanged;
            // 
            // viewLimitDesktopActionDialogBtn
            // 
            viewLimitDesktopActionDialogBtn.BackColor = Color.Transparent;
            viewLimitDesktopActionDialogBtn.Font = new Font("Segoe UI", 9F);
            viewLimitDesktopActionDialogBtn.Location = new Point(174, 232);
            viewLimitDesktopActionDialogBtn.Name = "viewLimitDesktopActionDialogBtn";
            viewLimitDesktopActionDialogBtn.Size = new Size(76, 23);
            viewLimitDesktopActionDialogBtn.TabIndex = 0;
            viewLimitDesktopActionDialogBtn.Text = "Edit Dialog";
            viewLimitDesktopActionDialogBtn.TextAlign = ContentAlignment.TopCenter;
            viewLimitDesktopActionDialogBtn.UseVisualStyleBackColor = false;
            viewLimitDesktopActionDialogBtn.Click += viewLimitDesktopActionDialogBtn_Click;
            // 
            // afkWarningThresholdLabel
            // 
            afkWarningThresholdLabel.AutoSize = true;
            afkWarningThresholdLabel.Location = new Point(27, 73);
            afkWarningThresholdLabel.Name = "afkWarningThresholdLabel";
            afkWarningThresholdLabel.Size = new Size(162, 15);
            afkWarningThresholdLabel.TabIndex = 42;
            afkWarningThresholdLabel.Text = "Warning threshold (seconds):";
            // 
            // afkWarningThresholdTextbox
            // 
            afkWarningThresholdTextbox.Location = new Point(215, 65);
            afkWarningThresholdTextbox.MaxLength = 2;
            afkWarningThresholdTextbox.Name = "afkWarningThresholdTextbox";
            afkWarningThresholdTextbox.Size = new Size(35, 23);
            afkWarningThresholdTextbox.TabIndex = 41;
            afkWarningThresholdTextbox.TextChanged += OnSettingChanged;
            afkWarningThresholdTextbox.KeyPress += afkWarningThresholdTextbox_KeyPress;
            afkWarningThresholdTextbox.Leave += afkWarningThresholdTextbox_Leave;
            // 
            // remoteServiceCheckbox
            // 
            remoteServiceCheckbox.AutoSize = true;
            remoteServiceCheckbox.Font = new Font("Segoe UI", 10F);
            remoteServiceCheckbox.Location = new Point(11, 23);
            remoteServiceCheckbox.Name = "remoteServiceCheckbox";
            remoteServiceCheckbox.Size = new Size(161, 23);
            remoteServiceCheckbox.TabIndex = 41;
            remoteServiceCheckbox.Text = "Enable remote service";
            remoteServiceCheckbox.UseVisualStyleBackColor = true;
            remoteServiceCheckbox.CheckedChanged += OnUIStateChanged;
            // 
            // limitDesktopHourCombo
            // 
            limitDesktopHourCombo.AutoCompleteCustomSource.AddRange(new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourCombo.FormattingEnabled = true;
            limitDesktopHourCombo.Items.AddRange(new object[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" });
            limitDesktopHourCombo.Location = new Point(53, 66);
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
            limitDesktopHourOpenCombo.Location = new Point(53, 110);
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
            limitDesktopMinCombo.Location = new Point(109, 66);
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
            limitDesktopMinOpenCombo.Location = new Point(109, 110);
            limitDesktopMinOpenCombo.Name = "limitDesktopMinOpenCombo";
            limitDesktopMinOpenCombo.Size = new Size(50, 23);
            limitDesktopMinOpenCombo.TabIndex = 46;
            limitDesktopMinOpenCombo.KeyPress += MinutesLimit_KeyPress;
            limitDesktopMinOpenCombo.Leave += MinutesLimit_Leave;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label1.Location = new Point(11, 19);
            label1.Name = "label1";
            label1.Size = new Size(176, 21);
            label1.TabIndex = 47;
            label1.Text = "SUSPICIOUS ACTIVITY:";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label3.Location = new Point(11, 43);
            label3.Name = "label3";
            label3.Size = new Size(167, 21);
            label3.TabIndex = 49;
            label3.Text = "KEYBOARD ACTIVITY:";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label4.Location = new Point(11, 64);
            label4.Name = "label4";
            label4.Size = new Size(141, 21);
            label4.TabIndex = 50;
            label4.Text = "MOUSE ACTIVITY:";
            label4.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(kboardStatusLabel);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(mouseStatusLabel);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(suspiciousKeysLabel);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(267, 100);
            panel1.TabIndex = 51;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(40, 2);
            label2.Name = "label2";
            label2.Size = new Size(132, 21);
            label2.TabIndex = 52;
            label2.Text = "DESKTOP STATUS";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(shutdownAFKCheckbox);
            panel2.Controls.Add(countdownMinutesLabel);
            panel2.Controls.Add(countdownTextbox);
            panel2.Controls.Add(afkWarningThresholdLabel);
            panel2.Controls.Add(afkWarningThresholdTextbox);
            panel2.Controls.Add(showCountdownCheckbox);
            panel2.Controls.Add(countdownTopMostCheckbox);
            panel2.Controls.Add(countdownOpacityCheckbox);
            panel2.Controls.Add(countdownOpacityTextbox);
            panel2.Location = new Point(12, 129);
            panel2.Name = "panel2";
            panel2.Size = new Size(267, 173);
            panel2.TabIndex = 53;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(40, 119);
            label5.Name = "label5";
            label5.Size = new Size(75, 21);
            label5.TabIndex = 53;
            label5.Text = "ANTI AFK";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(label9);
            panel3.Controls.Add(label8);
            panel3.Controls.Add(label7);
            panel3.Controls.Add(currentPasswordTextbox);
            panel3.Controls.Add(newPasswordTextbox);
            panel3.Controls.Add(confirmPasswordTextbox);
            panel3.Location = new Point(12, 321);
            panel3.Name = "panel3";
            panel3.Size = new Size(267, 173);
            panel3.TabIndex = 54;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(11, 114);
            label9.Name = "label9";
            label9.Size = new Size(132, 15);
            label9.TabIndex = 58;
            label9.Text = "Confirm new password:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(11, 68);
            label8.Name = "label8";
            label8.Size = new Size(115, 15);
            label8.TabIndex = 57;
            label8.Text = "Enter new password:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 24);
            label7.Name = "label7";
            label7.Size = new Size(103, 15);
            label7.TabIndex = 56;
            label7.Text = "Current password:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F);
            label6.Location = new Point(40, 308);
            label6.Name = "label6";
            label6.Size = new Size(79, 21);
            label6.TabIndex = 55;
            label6.Text = "SECURITY";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F);
            label10.Location = new Point(317, -1);
            label10.Name = "label10";
            label10.Size = new Size(169, 21);
            label10.TabIndex = 57;
            label10.Text = "CURFEW TIME DIALOG";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(viewLimitDesktopActionDialogBtn);
            panel4.Controls.Add(label14);
            panel4.Controls.Add(label12);
            panel4.Controls.Add(label11);
            panel4.Controls.Add(limitDesktopUsageCheckbox);
            panel4.Controls.Add(limitDesktopMinCombo);
            panel4.Controls.Add(limitDesktopAMorPMComboBox);
            panel4.Controls.Add(limitDesktopHourCombo);
            panel4.Controls.Add(limitDesktopHourOpenCombo);
            panel4.Controls.Add(limitDesktopMinOpenCombo);
            panel4.Controls.Add(limitDesktopOpenAMorPMComboBox);
            panel4.Controls.Add(limitDesktopShowDialog5minCheckbox);
            panel4.Controls.Add(limitDesktopShowDialog10minCheckbox);
            panel4.Controls.Add(limitDesktopActionComboBox);
            panel4.Controls.Add(limitDesktopShutdownAfter3Minutes);
            panel4.Controls.Add(limitDesktopShowDialog30minCheckbox);
            panel4.Location = new Point(289, 12);
            panel4.Name = "panel4";
            panel4.Size = new Size(267, 290);
            panel4.TabIndex = 56;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.Transparent;
            label14.Location = new Point(22, 212);
            label14.Name = "label14";
            label14.Size = new Size(94, 15);
            label14.TabIndex = 57;
            label14.Text = "● Curfew action:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(22, 92);
            label12.Name = "label12";
            label12.Size = new Size(217, 15);
            label12.TabIndex = 46;
            label12.Text = "● Pisonet open time [Hour] [Min] [Secs]";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(22, 48);
            label11.Name = "label11";
            label11.Size = new Size(217, 15);
            label11.TabIndex = 36;
            label11.Text = "● Pisonet close time [Hour] [Min] [Secs]";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.Location = new Point(594, -1);
            label13.Name = "label13";
            label13.Size = new Size(168, 21);
            label13.TabIndex = 59;
            label13.Text = "NO SMOKING DIALOG";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            panel5.BorderStyle = BorderStyle.FixedSingle;
            panel5.Controls.Add(noSmokingDialogCheckbox);
            panel5.Controls.Add(viewNoSmokingDialog);
            panel5.Location = new Point(566, 12);
            panel5.Name = "panel5";
            panel5.Size = new Size(267, 84);
            panel5.TabIndex = 58;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(596, 106);
            label15.Name = "label15";
            label15.Size = new Size(132, 21);
            label15.TabIndex = 61;
            label15.Text = "REMOTE SERVICE";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel6
            // 
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel6.Controls.Add(remoteServiceCheckbox);
            panel6.Controls.Add(btnRemoteSettings);
            panel6.Location = new Point(568, 119);
            panel6.Name = "panel6";
            panel6.Size = new Size(265, 84);
            panel6.TabIndex = 60;
            // 
            // btnRemoteSettings
            // 
            btnRemoteSettings.Location = new Point(144, 48);
            btnRemoteSettings.Name = "btnRemoteSettings";
            btnRemoteSettings.Size = new Size(107, 23);
            btnRemoteSettings.TabIndex = 40;
            btnRemoteSettings.Text = "Remote Settings";
            btnRemoteSettings.UseVisualStyleBackColor = true;
            btnRemoteSettings.Click += btnRemoteSettings_Click;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 12F);
            label16.Location = new Point(317, 308);
            label16.Name = "label16";
            label16.Size = new Size(140, 21);
            label16.TabIndex = 63;
            label16.Text = "SYSTEM SETTINGS";
            label16.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel7
            // 
            panel7.BorderStyle = BorderStyle.FixedSingle;
            panel7.Controls.Add(adminShutdownCheckbox);
            panel7.Controls.Add(applicationServiceCheckbox);
            panel7.Controls.Add(applicationHighPriorityCheckbox);
            panel7.Controls.Add(startupCheckbox);
            panel7.Controls.Add(minimizeTrayCheckbox);
            panel7.Location = new Point(289, 321);
            panel7.Name = "panel7";
            panel7.Size = new Size(267, 173);
            panel7.TabIndex = 62;
            // 
            // panel8
            // 
            panel8.BorderStyle = BorderStyle.FixedSingle;
            panel8.Controls.Add(facebookUrlLinkLabel);
            panel8.Controls.Add(label20);
            panel8.Controls.Add(label19);
            panel8.Controls.Add(label18);
            panel8.Controls.Add(label17);
            panel8.Location = new Point(568, 209);
            panel8.Name = "panel8";
            panel8.Size = new Size(266, 239);
            panel8.TabIndex = 64;
            // 
            // facebookUrlLinkLabel
            // 
            facebookUrlLinkLabel.AutoSize = true;
            facebookUrlLinkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
            facebookUrlLinkLabel.Location = new Point(184, 206);
            facebookUrlLinkLabel.Name = "facebookUrlLinkLabel";
            facebookUrlLinkLabel.Size = new Size(57, 15);
            facebookUrlLinkLabel.TabIndex = 69;
            facebookUrlLinkLabel.TabStop = true;
            facebookUrlLinkLabel.Text = "click here";
            facebookUrlLinkLabel.LinkClicked += facebookUrlLinkLabel_LinkClicked;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(22, 206);
            label20.Name = "label20";
            label20.Size = new Size(161, 15);
            label20.TabIndex = 68;
            label20.Text = "Social Media Link (Facebook)";
            // 
            // label19
            // 
            label19.Font = new Font("Segoe UI", 10F);
            label19.Location = new Point(0, 148);
            label19.Name = "label19";
            label19.Size = new Size(265, 38);
            label19.TabIndex = 67;
            label19.Text = "This application is created by \r\nUelmark G. Valdehueza";
            label19.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            label18.Font = new Font("Segoe UI", 10F);
            label18.Location = new Point(1, 57);
            label18.Name = "label18";
            label18.Size = new Size(264, 57);
            label18.TabIndex = 66;
            label18.Text = "If you encounter any errors or bug do\r\nnot hesitate to contact and\r\nreport it to the developer.";
            label18.TextAlign = ContentAlignment.TopCenter;
            // 
            // label17
            // 
            label17.BackColor = Color.DodgerBlue;
            label17.Dock = DockStyle.Top;
            label17.Font = new Font("Segoe UI", 12F);
            label17.Location = new Point(0, 0);
            label17.Name = "label17";
            label17.Size = new Size(264, 34);
            label17.TabIndex = 65;
            label17.Text = "DEVELOPER'S NOTICE";
            label17.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(846, 505);
            Controls.Add(panel8);
            Controls.Add(label16);
            Controls.Add(panel7);
            Controls.Add(label15);
            Controls.Add(panel6);
            Controls.Add(label10);
            Controls.Add(panel4);
            Controls.Add(label13);
            Controls.Add(panel5);
            Controls.Add(label6);
            Controls.Add(panel3);
            Controls.Add(label5);
            Controls.Add(panel2);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(saveSettingsBtn);
            Controls.Add(settingStatusLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Azeu Services V1";
            FormClosing += OnFormClosing;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
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
        private Label afkWarningThresholdLabel;
        private TextBox afkWarningThresholdTextbox;
        private CheckBox remoteServiceCheckbox;
        private ComboBox limitDesktopHourCombo;
        private ComboBox limitDesktopHourOpenCombo;
        private ComboBox limitDesktopMinCombo;
        private ComboBox limitDesktopMinOpenCombo;
        private Label label1;
        private Label label3;
        private Label label4;
        private Panel panel1;
        private Label label2;
        private Panel panel2;
        private Label label5;
        private Panel panel3;
        private Label label6;
        private Label label7;
        private Label label9;
        private Label label8;
        private Label label10;
        private Panel panel4;
        private Label label12;
        private Label label11;
        private Label label13;
        private Panel panel5;
        private Label label14;
        private Label label15;
        private Panel panel6;
        private Button btnRemoteSettings;
        private Label label16;
        private Panel panel7;
        private Panel panel8;
        private Label label19;
        private Label label18;
        private Label label17;
        private LinkLabel facebookUrlLinkLabel;
        private Label label20;
    }
}