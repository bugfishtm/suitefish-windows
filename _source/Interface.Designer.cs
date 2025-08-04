namespace suitefish
{
    partial class Interface
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interface));
            Header_Panel = new Panel();
            buttonStore = new Button();
            label7 = new Label();
            buttonAbout = new Button();
            buttonSettings = new Button();
            buttonLibrary = new Button();
            Header_Logo = new PictureBox();
            Header_Title = new Label();
            labelUpdate = new Label();
            Body_Panel = new Panel();
            Content_Panel = new Panel();
            panelSelection = new Panel();
            panel5 = new Panel();
            buttonSelectionYesUp = new Button();
            labelSelection = new Label();
            buttonSelectionNo = new Button();
            buttonSelectionNoUp = new Button();
            buttonSelectionYes = new Button();
            Panel_Library = new Panel();
            listLibrary = new ListView();
            panelStore = new Panel();
            listStore = new ListView();
            Panel_Settings = new Panel();
            panel4 = new Panel();
            labelLibraryServerH = new Label();
            labelLibraryServer = new Label();
            inputLibraryServer = new TextBox();
            panel3 = new Panel();
            labelUpdateServerH = new Label();
            labelUpdateServer = new Label();
            inputUpdateServer = new TextBox();
            panel2 = new Panel();
            radioPortugese = new RadioButton();
            radioKorean = new RadioButton();
            radioTurke = new RadioButton();
            radioJap = new RadioButton();
            radioChine = new RadioButton();
            labelLang = new Label();
            radioFrench = new RadioButton();
            radioIndi = new RadioButton();
            radioRuss = new RadioButton();
            radioItaly = new RadioButton();
            labelLanguage = new Label();
            radioGerman = new RadioButton();
            radioEnglish = new RadioButton();
            radioSpain = new RadioButton();
            panel1 = new Panel();
            labelTrayH = new Label();
            labelRadio = new Label();
            radioTrayOn = new RadioButton();
            radioTrayOff = new RadioButton();
            btnSettingsRestore = new Button();
            btnSettingsSave = new Button();
            btnSettingsRevert = new Button();
            panelWait = new Panel();
            labelPleaseWait = new Label();
            Panel_About = new Panel();
            linkLabel3 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            linkLabel1 = new LinkLabel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            Status_Panel = new Panel();
            labelTask2 = new Label();
            progressBar1 = new ProgressBar();
            labelServer = new Label();
            tooltip_frame = new ToolTip(components);
            bindingSource1 = new BindingSource(components);
            Background_Panel = new Panel();
            Header_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Header_Logo).BeginInit();
            Body_Panel.SuspendLayout();
            Content_Panel.SuspendLayout();
            panelSelection.SuspendLayout();
            panel5.SuspendLayout();
            Panel_Library.SuspendLayout();
            panelStore.SuspendLayout();
            Panel_Settings.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            panelWait.SuspendLayout();
            Panel_About.SuspendLayout();
            Status_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // Header_Panel
            // 
            Header_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Header_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Header_Panel.Controls.Add(buttonStore);
            Header_Panel.Controls.Add(label7);
            Header_Panel.Controls.Add(buttonAbout);
            Header_Panel.Controls.Add(buttonSettings);
            Header_Panel.Controls.Add(buttonLibrary);
            Header_Panel.Controls.Add(Header_Logo);
            Header_Panel.Controls.Add(Header_Title);
            Header_Panel.Location = new Point(0, 0);
            Header_Panel.Name = "Header_Panel";
            Header_Panel.Size = new Size(1168, 98);
            Header_Panel.TabIndex = 0;
            Header_Panel.MouseDown += CustomUI_Header_Panel_MouseDown;
            // 
            // buttonStore
            // 
            buttonStore.Anchor = AnchorStyles.Right;
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.FlatStyle = FlatStyle.Popup;
            buttonStore.ForeColor = SystemColors.ButtonHighlight;
            buttonStore.Location = new Point(529, 55);
            buttonStore.Name = "buttonStore";
            buttonStore.Size = new Size(150, 29);
            buttonStore.TabIndex = 10;
            buttonStore.Text = "trs_library";
            buttonStore.UseVisualStyleBackColor = false;
            buttonStore.Click += buttonStore_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.FromArgb(224, 224, 224);
            label7.Location = new Point(123, 64);
            label7.Name = "label7";
            label7.Size = new Size(78, 20);
            label7.TabIndex = 9;
            label7.Text = "trs_update";
            label7.TextAlign = ContentAlignment.TopRight;
            // 
            // buttonAbout
            // 
            buttonAbout.Anchor = AnchorStyles.Right;
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.FlatStyle = FlatStyle.Popup;
            buttonAbout.ForeColor = SystemColors.ButtonHighlight;
            buttonAbout.Location = new Point(997, 55);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new Size(150, 29);
            buttonAbout.TabIndex = 7;
            buttonAbout.Text = "trs_about";
            buttonAbout.UseVisualStyleBackColor = false;
            buttonAbout.Click += buttonAbout_Click;
            // 
            // buttonSettings
            // 
            buttonSettings.Anchor = AnchorStyles.Right;
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.FlatStyle = FlatStyle.Popup;
            buttonSettings.ForeColor = SystemColors.ButtonHighlight;
            buttonSettings.Location = new Point(841, 55);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new Size(150, 29);
            buttonSettings.TabIndex = 6;
            buttonSettings.Text = "trs_settings";
            buttonSettings.UseVisualStyleBackColor = false;
            buttonSettings.Click += buttonSettings_Click;
            // 
            // buttonLibrary
            // 
            buttonLibrary.Anchor = AnchorStyles.Right;
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonLibrary.FlatStyle = FlatStyle.Popup;
            buttonLibrary.ForeColor = SystemColors.ButtonHighlight;
            buttonLibrary.Location = new Point(685, 55);
            buttonLibrary.Name = "buttonLibrary";
            buttonLibrary.Size = new Size(150, 29);
            buttonLibrary.TabIndex = 5;
            buttonLibrary.Text = "trs_library";
            buttonLibrary.UseVisualStyleBackColor = false;
            buttonLibrary.Click += buttonLibrary_Click;
            // 
            // Header_Logo
            // 
            Header_Logo.BackgroundImage = (Image)resources.GetObject("Header_Logo.BackgroundImage");
            Header_Logo.BackgroundImageLayout = ImageLayout.Zoom;
            Header_Logo.Location = new Point(3, 3);
            Header_Logo.Name = "Header_Logo";
            Header_Logo.Size = new Size(104, 95);
            Header_Logo.TabIndex = 1;
            Header_Logo.TabStop = false;
            // 
            // Header_Title
            // 
            Header_Title.AutoSize = true;
            Header_Title.BackColor = Color.Transparent;
            Header_Title.Font = new Font("Segoe UI", 30F);
            Header_Title.ForeColor = Color.White;
            Header_Title.Location = new Point(113, 3);
            Header_Title.Name = "Header_Title";
            Header_Title.Size = new Size(189, 67);
            Header_Title.TabIndex = 3;
            Header_Title.Text = "trs_title";
            // 
            // labelUpdate
            // 
            labelUpdate.Anchor = AnchorStyles.Left;
            labelUpdate.AutoSize = true;
            labelUpdate.Font = new Font("Segoe UI", 8F);
            labelUpdate.ForeColor = Color.FromArgb(255, 128, 0);
            labelUpdate.Location = new Point(6, 3);
            labelUpdate.Name = "labelUpdate";
            labelUpdate.Size = new Size(74, 19);
            labelUpdate.TabIndex = 8;
            labelUpdate.Text = "trs_update";
            labelUpdate.TextAlign = ContentAlignment.TopRight;
            // 
            // Body_Panel
            // 
            Body_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Body_Panel.Controls.Add(Content_Panel);
            Body_Panel.Controls.Add(Status_Panel);
            Body_Panel.Dock = DockStyle.Fill;
            Body_Panel.Location = new Point(0, 0);
            Body_Panel.Name = "Body_Panel";
            Body_Panel.Size = new Size(1168, 662);
            Body_Panel.TabIndex = 1;
            // 
            // Content_Panel
            // 
            Content_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Content_Panel.BackColor = Color.FromArgb(24, 24, 24);
            Content_Panel.Controls.Add(panelSelection);
            Content_Panel.Controls.Add(Panel_Library);
            Content_Panel.Controls.Add(panelStore);
            Content_Panel.Controls.Add(Panel_Settings);
            Content_Panel.Controls.Add(panelWait);
            Content_Panel.Controls.Add(Panel_About);
            Content_Panel.Location = new Point(0, 90);
            Content_Panel.Name = "Content_Panel";
            Content_Panel.Size = new Size(1168, 522);
            Content_Panel.TabIndex = 9;
            // 
            // panelSelection
            // 
            panelSelection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelSelection.BackColor = Color.FromArgb(24, 24, 24);
            panelSelection.Controls.Add(panel5);
            panelSelection.ForeColor = SystemColors.ControlDark;
            panelSelection.Location = new Point(0, 0);
            panelSelection.Name = "panelSelection";
            panelSelection.Size = new Size(1168, 522);
            panelSelection.TabIndex = 16;
            // 
            // panel5
            // 
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel5.Controls.Add(buttonSelectionYesUp);
            panel5.Controls.Add(labelSelection);
            panel5.Controls.Add(buttonSelectionNo);
            panel5.Controls.Add(buttonSelectionNoUp);
            panel5.Controls.Add(buttonSelectionYes);
            panel5.Location = new Point(6, 189);
            panel5.Name = "panel5";
            panel5.Size = new Size(1159, 125);
            panel5.TabIndex = 3;
            // 
            // buttonSelectionYesUp
            // 
            buttonSelectionYesUp.Anchor = AnchorStyles.None;
            buttonSelectionYesUp.BackColor = Color.Green;
            buttonSelectionYesUp.FlatStyle = FlatStyle.Popup;
            buttonSelectionYesUp.ForeColor = SystemColors.ButtonHighlight;
            buttonSelectionYesUp.Location = new Point(355, 86);
            buttonSelectionYesUp.Name = "buttonSelectionYesUp";
            buttonSelectionYesUp.Size = new Size(204, 29);
            buttonSelectionYesUp.TabIndex = 4;
            buttonSelectionYesUp.Text = "button1";
            buttonSelectionYesUp.UseVisualStyleBackColor = false;
            buttonSelectionYesUp.Click += buttonSelectionYesUp_Click;
            // 
            // labelSelection
            // 
            labelSelection.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            labelSelection.Font = new Font("Segoe UI", 30F);
            labelSelection.ForeColor = SystemColors.ControlDarkDark;
            labelSelection.Location = new Point(3, 0);
            labelSelection.Name = "labelSelection";
            labelSelection.Size = new Size(1153, 83);
            labelSelection.TabIndex = 0;
            labelSelection.Text = "label5";
            labelSelection.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonSelectionNo
            // 
            buttonSelectionNo.Anchor = AnchorStyles.None;
            buttonSelectionNo.BackColor = Color.FromArgb(192, 0, 0);
            buttonSelectionNo.FlatStyle = FlatStyle.Popup;
            buttonSelectionNo.ForeColor = SystemColors.ButtonHighlight;
            buttonSelectionNo.Location = new Point(590, 86);
            buttonSelectionNo.Name = "buttonSelectionNo";
            buttonSelectionNo.Size = new Size(205, 29);
            buttonSelectionNo.TabIndex = 2;
            buttonSelectionNo.Text = "button2";
            buttonSelectionNo.UseVisualStyleBackColor = false;
            buttonSelectionNo.Click += buttonSelectionNo_Click;
            // 
            // buttonSelectionNoUp
            // 
            buttonSelectionNoUp.Anchor = AnchorStyles.None;
            buttonSelectionNoUp.BackColor = Color.FromArgb(192, 0, 0);
            buttonSelectionNoUp.FlatStyle = FlatStyle.Popup;
            buttonSelectionNoUp.ForeColor = SystemColors.ButtonHighlight;
            buttonSelectionNoUp.Location = new Point(590, 86);
            buttonSelectionNoUp.Name = "buttonSelectionNoUp";
            buttonSelectionNoUp.Size = new Size(204, 29);
            buttonSelectionNoUp.TabIndex = 3;
            buttonSelectionNoUp.Text = "button2";
            buttonSelectionNoUp.UseVisualStyleBackColor = false;
            buttonSelectionNoUp.Click += buttonSelectionNoUp_Click;
            // 
            // buttonSelectionYes
            // 
            buttonSelectionYes.Anchor = AnchorStyles.None;
            buttonSelectionYes.BackColor = Color.Green;
            buttonSelectionYes.FlatStyle = FlatStyle.Popup;
            buttonSelectionYes.ForeColor = SystemColors.ButtonHighlight;
            buttonSelectionYes.Location = new Point(355, 86);
            buttonSelectionYes.Name = "buttonSelectionYes";
            buttonSelectionYes.Size = new Size(204, 29);
            buttonSelectionYes.TabIndex = 1;
            buttonSelectionYes.Text = "button1";
            buttonSelectionYes.UseVisualStyleBackColor = false;
            buttonSelectionYes.Click += buttonSelectionYes_Click;
            // 
            // Panel_Library
            // 
            Panel_Library.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Library.BackColor = Color.FromArgb(24, 24, 24);
            Panel_Library.Controls.Add(listLibrary);
            Panel_Library.Location = new Point(0, 6);
            Panel_Library.Name = "Panel_Library";
            Panel_Library.Size = new Size(1165, 513);
            Panel_Library.TabIndex = 11;
            // 
            // listLibrary
            // 
            listLibrary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listLibrary.BackColor = Color.FromArgb(24, 24, 24);
            listLibrary.BorderStyle = BorderStyle.None;
            listLibrary.ForeColor = SystemColors.Window;
            listLibrary.Location = new Point(6, 8);
            listLibrary.Name = "listLibrary";
            listLibrary.Size = new Size(1156, 502);
            listLibrary.TabIndex = 0;
            listLibrary.UseCompatibleStateImageBehavior = false;
            // 
            // panelStore
            // 
            panelStore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelStore.BackColor = Color.FromArgb(24, 24, 24);
            panelStore.Controls.Add(listStore);
            panelStore.ForeColor = SystemColors.Window;
            panelStore.Location = new Point(3, 6);
            panelStore.Name = "panelStore";
            panelStore.Size = new Size(1162, 516);
            panelStore.TabIndex = 12;
            // 
            // listStore
            // 
            listStore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listStore.BackColor = Color.FromArgb(24, 24, 24);
            listStore.BorderStyle = BorderStyle.None;
            listStore.ForeColor = SystemColors.Window;
            listStore.Location = new Point(10, 8);
            listStore.Name = "listStore";
            listStore.Size = new Size(1143, 502);
            listStore.TabIndex = 0;
            listStore.UseCompatibleStateImageBehavior = false;
            // 
            // Panel_Settings
            // 
            Panel_Settings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_Settings.BackColor = Color.FromArgb(24, 24, 24);
            Panel_Settings.Controls.Add(panel4);
            Panel_Settings.Controls.Add(panel3);
            Panel_Settings.Controls.Add(panel2);
            Panel_Settings.Controls.Add(panel1);
            Panel_Settings.Controls.Add(btnSettingsRestore);
            Panel_Settings.Controls.Add(btnSettingsSave);
            Panel_Settings.Controls.Add(btnSettingsRevert);
            Panel_Settings.Location = new Point(0, 0);
            Panel_Settings.Name = "Panel_Settings";
            Panel_Settings.Size = new Size(1168, 522);
            Panel_Settings.TabIndex = 10;
            // 
            // panel4
            // 
            panel4.Controls.Add(labelLibraryServerH);
            panel4.Controls.Add(labelLibraryServer);
            panel4.Controls.Add(inputLibraryServer);
            panel4.Location = new Point(33, 32);
            panel4.Name = "panel4";
            panel4.Size = new Size(691, 106);
            panel4.TabIndex = 16;
            // 
            // labelLibraryServerH
            // 
            labelLibraryServerH.AutoSize = true;
            labelLibraryServerH.Font = new Font("Segoe UI", 14F);
            labelLibraryServerH.ForeColor = Color.White;
            labelLibraryServerH.Location = new Point(0, 0);
            labelLibraryServerH.Name = "labelLibraryServerH";
            labelLibraryServerH.Size = new Size(78, 32);
            labelLibraryServerH.TabIndex = 12;
            labelLibraryServerH.Text = "label8";
            // 
            // labelLibraryServer
            // 
            labelLibraryServer.AutoSize = true;
            labelLibraryServer.ForeColor = Color.LightGray;
            labelLibraryServer.Location = new Point(3, 33);
            labelLibraryServer.Name = "labelLibraryServer";
            labelLibraryServer.Size = new Size(23, 20);
            labelLibraryServer.TabIndex = 1;
            labelLibraryServer.Text = "xx";
            // 
            // inputLibraryServer
            // 
            inputLibraryServer.BackColor = Color.FromArgb(24, 24, 24);
            inputLibraryServer.BorderStyle = BorderStyle.FixedSingle;
            inputLibraryServer.ForeColor = Color.White;
            inputLibraryServer.Location = new Point(6, 59);
            inputLibraryServer.Margin = new Padding(10);
            inputLibraryServer.Name = "inputLibraryServer";
            inputLibraryServer.Size = new Size(678, 27);
            inputLibraryServer.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Controls.Add(labelUpdateServerH);
            panel3.Controls.Add(labelUpdateServer);
            panel3.Controls.Add(inputUpdateServer);
            panel3.Location = new Point(33, 144);
            panel3.Name = "panel3";
            panel3.Size = new Size(691, 105);
            panel3.TabIndex = 15;
            // 
            // labelUpdateServerH
            // 
            labelUpdateServerH.AutoSize = true;
            labelUpdateServerH.Font = new Font("Segoe UI", 14F);
            labelUpdateServerH.ForeColor = Color.White;
            labelUpdateServerH.Location = new Point(0, 0);
            labelUpdateServerH.Name = "labelUpdateServerH";
            labelUpdateServerH.Size = new Size(78, 32);
            labelUpdateServerH.TabIndex = 11;
            labelUpdateServerH.Text = "label8";
            // 
            // labelUpdateServer
            // 
            labelUpdateServer.AutoSize = true;
            labelUpdateServer.ForeColor = Color.LightGray;
            labelUpdateServer.Location = new Point(3, 32);
            labelUpdateServer.Name = "labelUpdateServer";
            labelUpdateServer.Size = new Size(23, 20);
            labelUpdateServer.TabIndex = 2;
            labelUpdateServer.Text = "xx";
            // 
            // inputUpdateServer
            // 
            inputUpdateServer.BackColor = Color.FromArgb(24, 24, 24);
            inputUpdateServer.BorderStyle = BorderStyle.FixedSingle;
            inputUpdateServer.ForeColor = Color.White;
            inputUpdateServer.Location = new Point(6, 55);
            inputUpdateServer.Name = "inputUpdateServer";
            inputUpdateServer.Size = new Size(678, 27);
            inputUpdateServer.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(radioPortugese);
            panel2.Controls.Add(radioKorean);
            panel2.Controls.Add(radioTurke);
            panel2.Controls.Add(radioJap);
            panel2.Controls.Add(radioChine);
            panel2.Controls.Add(labelLang);
            panel2.Controls.Add(radioFrench);
            panel2.Controls.Add(radioIndi);
            panel2.Controls.Add(radioRuss);
            panel2.Controls.Add(radioItaly);
            panel2.Controls.Add(labelLanguage);
            panel2.Controls.Add(radioGerman);
            panel2.Controls.Add(radioEnglish);
            panel2.Controls.Add(radioSpain);
            panel2.ForeColor = Color.White;
            panel2.Location = new Point(294, 255);
            panel2.Name = "panel2";
            panel2.Size = new Size(430, 159);
            panel2.TabIndex = 14;
            // 
            // radioPortugese
            // 
            radioPortugese.AutoSize = true;
            radioPortugese.Location = new Point(301, 125);
            radioPortugese.Name = "radioPortugese";
            radioPortugese.Size = new Size(95, 24);
            radioPortugese.TabIndex = 7;
            radioPortugese.TabStop = true;
            radioPortugese.Text = "Português";
            radioPortugese.UseVisualStyleBackColor = true;
            // 
            // radioKorean
            // 
            radioKorean.AutoSize = true;
            radioKorean.Location = new Point(125, 125);
            radioKorean.Name = "radioKorean";
            radioKorean.Size = new Size(75, 24);
            radioKorean.TabIndex = 9;
            radioKorean.TabStop = true;
            radioKorean.Text = "한국인";
            radioKorean.UseVisualStyleBackColor = true;
            // 
            // radioTurke
            // 
            radioTurke.AutoSize = true;
            radioTurke.Location = new Point(125, 95);
            radioTurke.Name = "radioTurke";
            radioTurke.Size = new Size(73, 24);
            radioTurke.TabIndex = 10;
            radioTurke.TabStop = true;
            radioTurke.Text = "Türkçe";
            radioTurke.UseVisualStyleBackColor = true;
            // 
            // radioJap
            // 
            radioJap.AutoSize = true;
            radioJap.Location = new Point(213, 125);
            radioJap.Name = "radioJap";
            radioJap.Size = new Size(75, 24);
            radioJap.TabIndex = 6;
            radioJap.TabStop = true;
            radioJap.Text = "日本語";
            radioJap.UseVisualStyleBackColor = true;
            // 
            // radioChine
            // 
            radioChine.AutoSize = true;
            radioChine.Location = new Point(301, 65);
            radioChine.Name = "radioChine";
            radioChine.Size = new Size(75, 24);
            radioChine.TabIndex = 11;
            radioChine.TabStop = true;
            radioChine.Text = "中国人";
            radioChine.UseVisualStyleBackColor = true;
            // 
            // labelLang
            // 
            labelLang.AutoSize = true;
            labelLang.Font = new Font("Segoe UI", 14F);
            labelLang.ForeColor = Color.White;
            labelLang.Location = new Point(0, 0);
            labelLang.Name = "labelLang";
            labelLang.Size = new Size(78, 32);
            labelLang.TabIndex = 13;
            labelLang.Text = "label8";
            // 
            // radioFrench
            // 
            radioFrench.AutoSize = true;
            radioFrench.Location = new Point(40, 125);
            radioFrench.Name = "radioFrench";
            radioFrench.Size = new Size(83, 24);
            radioFrench.TabIndex = 1;
            radioFrench.TabStop = true;
            radioFrench.Text = "Français";
            radioFrench.UseVisualStyleBackColor = true;
            // 
            // radioIndi
            // 
            radioIndi.AutoSize = true;
            radioIndi.Location = new Point(301, 95);
            radioIndi.Name = "radioIndi";
            radioIndi.Size = new Size(74, 24);
            radioIndi.TabIndex = 4;
            radioIndi.TabStop = true;
            radioIndi.Text = "भारतीय";
            radioIndi.UseVisualStyleBackColor = true;
            // 
            // radioRuss
            // 
            radioRuss.AutoSize = true;
            radioRuss.Location = new Point(40, 95);
            radioRuss.Name = "radioRuss";
            radioRuss.Size = new Size(84, 24);
            radioRuss.TabIndex = 8;
            radioRuss.TabStop = true;
            radioRuss.Text = "Русский";
            radioRuss.UseVisualStyleBackColor = true;
            // 
            // radioItaly
            // 
            radioItaly.AutoSize = true;
            radioItaly.Location = new Point(213, 95);
            radioItaly.Name = "radioItaly";
            radioItaly.Size = new Size(80, 24);
            radioItaly.TabIndex = 2;
            radioItaly.TabStop = true;
            radioItaly.Text = "Italiano";
            radioItaly.UseVisualStyleBackColor = true;
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.ForeColor = Color.LightGray;
            labelLanguage.Location = new Point(3, 32);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(23, 20);
            labelLanguage.TabIndex = 12;
            labelLanguage.Text = "xx";
            // 
            // radioGerman
            // 
            radioGerman.AutoSize = true;
            radioGerman.Location = new Point(125, 65);
            radioGerman.Name = "radioGerman";
            radioGerman.Size = new Size(83, 24);
            radioGerman.TabIndex = 0;
            radioGerman.TabStop = true;
            radioGerman.Text = "Deutsch";
            radioGerman.UseVisualStyleBackColor = true;
            // 
            // radioEnglish
            // 
            radioEnglish.AutoSize = true;
            radioEnglish.Location = new Point(40, 65);
            radioEnglish.Name = "radioEnglish";
            radioEnglish.RightToLeft = RightToLeft.No;
            radioEnglish.Size = new Size(77, 24);
            radioEnglish.TabIndex = 5;
            radioEnglish.TabStop = true;
            radioEnglish.Text = "English";
            radioEnglish.UseVisualStyleBackColor = true;
            // 
            // radioSpain
            // 
            radioSpain.AutoSize = true;
            radioSpain.Location = new Point(213, 65);
            radioSpain.Name = "radioSpain";
            radioSpain.Size = new Size(82, 24);
            radioSpain.TabIndex = 3;
            radioSpain.TabStop = true;
            radioSpain.Text = "Español";
            radioSpain.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(labelTrayH);
            panel1.Controls.Add(labelRadio);
            panel1.Controls.Add(radioTrayOn);
            panel1.Controls.Add(radioTrayOff);
            panel1.ForeColor = SystemColors.Window;
            panel1.Location = new Point(33, 255);
            panel1.Name = "panel1";
            panel1.Size = new Size(247, 159);
            panel1.TabIndex = 13;
            // 
            // labelTrayH
            // 
            labelTrayH.AutoSize = true;
            labelTrayH.Font = new Font("Segoe UI", 14F);
            labelTrayH.ForeColor = Color.White;
            labelTrayH.Location = new Point(0, 0);
            labelTrayH.Name = "labelTrayH";
            labelTrayH.Size = new Size(78, 32);
            labelTrayH.TabIndex = 5;
            labelTrayH.Text = "label8";
            // 
            // labelRadio
            // 
            labelRadio.AutoSize = true;
            labelRadio.ForeColor = Color.LightGray;
            labelRadio.Location = new Point(4, 32);
            labelRadio.Name = "labelRadio";
            labelRadio.Size = new Size(23, 20);
            labelRadio.TabIndex = 4;
            labelRadio.Text = "xx";
            // 
            // radioTrayOn
            // 
            radioTrayOn.AutoSize = true;
            radioTrayOn.ForeColor = Color.White;
            radioTrayOn.Location = new Point(41, 65);
            radioTrayOn.Name = "radioTrayOn";
            radioTrayOn.Size = new Size(38, 24);
            radioTrayOn.TabIndex = 2;
            radioTrayOn.TabStop = true;
            radioTrayOn.Text = "1";
            radioTrayOn.UseVisualStyleBackColor = true;
            // 
            // radioTrayOff
            // 
            radioTrayOff.AutoSize = true;
            radioTrayOff.ForeColor = Color.White;
            radioTrayOff.Location = new Point(41, 95);
            radioTrayOff.Name = "radioTrayOff";
            radioTrayOff.Size = new Size(38, 24);
            radioTrayOff.TabIndex = 3;
            radioTrayOff.TabStop = true;
            radioTrayOff.Text = "0";
            radioTrayOff.UseVisualStyleBackColor = true;
            // 
            // btnSettingsRestore
            // 
            btnSettingsRestore.BackColor = Color.FromArgb(28, 28, 28);
            btnSettingsRestore.FlatStyle = FlatStyle.Popup;
            btnSettingsRestore.ForeColor = SystemColors.ButtonHighlight;
            btnSettingsRestore.Location = new Point(169, 430);
            btnSettingsRestore.Name = "btnSettingsRestore";
            btnSettingsRestore.Size = new Size(163, 34);
            btnSettingsRestore.TabIndex = 10;
            btnSettingsRestore.Text = "restore";
            btnSettingsRestore.UseVisualStyleBackColor = false;
            btnSettingsRestore.Click += button1_Click;
            // 
            // btnSettingsSave
            // 
            btnSettingsSave.BackColor = Color.FromArgb(255, 128, 0);
            btnSettingsSave.FlatStyle = FlatStyle.Popup;
            btnSettingsSave.ForeColor = SystemColors.ButtonHighlight;
            btnSettingsSave.Location = new Point(507, 430);
            btnSettingsSave.Name = "btnSettingsSave";
            btnSettingsSave.Size = new Size(217, 34);
            btnSettingsSave.TabIndex = 9;
            btnSettingsSave.Text = "sav";
            btnSettingsSave.UseVisualStyleBackColor = false;
            btnSettingsSave.Click += btnSettingsSave_Click;
            // 
            // btnSettingsRevert
            // 
            btnSettingsRevert.BackColor = Color.FromArgb(28, 28, 28);
            btnSettingsRevert.FlatStyle = FlatStyle.Popup;
            btnSettingsRevert.ForeColor = SystemColors.ButtonHighlight;
            btnSettingsRevert.Location = new Point(338, 430);
            btnSettingsRevert.Name = "btnSettingsRevert";
            btnSettingsRevert.Size = new Size(163, 34);
            btnSettingsRevert.TabIndex = 8;
            btnSettingsRevert.Text = "rev";
            btnSettingsRevert.UseVisualStyleBackColor = false;
            btnSettingsRevert.Click += btnSettingsRevert_Click;
            // 
            // panelWait
            // 
            panelWait.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelWait.BackColor = Color.FromArgb(24, 24, 24);
            panelWait.Controls.Add(labelPleaseWait);
            panelWait.ForeColor = SystemColors.ControlDark;
            panelWait.Location = new Point(0, 0);
            panelWait.Name = "panelWait";
            panelWait.Size = new Size(1168, 522);
            panelWait.TabIndex = 15;
            // 
            // labelPleaseWait
            // 
            labelPleaseWait.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelPleaseWait.Font = new Font("Segoe UI", 30F);
            labelPleaseWait.ForeColor = SystemColors.ControlDarkDark;
            labelPleaseWait.Location = new Point(13, 205);
            labelPleaseWait.Name = "labelPleaseWait";
            labelPleaseWait.Size = new Size(1134, 67);
            labelPleaseWait.TabIndex = 0;
            labelPleaseWait.Text = "label5";
            labelPleaseWait.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Panel_About
            // 
            Panel_About.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Panel_About.BackColor = Color.FromArgb(24, 24, 24);
            Panel_About.Controls.Add(linkLabel3);
            Panel_About.Controls.Add(linkLabel2);
            Panel_About.Controls.Add(linkLabel1);
            Panel_About.Controls.Add(label4);
            Panel_About.Controls.Add(label3);
            Panel_About.Controls.Add(label2);
            Panel_About.Controls.Add(label1);
            Panel_About.Location = new Point(0, 0);
            Panel_About.Name = "Panel_About";
            Panel_About.Size = new Size(1168, 522);
            Panel_About.TabIndex = 14;
            // 
            // linkLabel3
            // 
            linkLabel3.ActiveLinkColor = Color.FromArgb(255, 128, 0);
            linkLabel3.AutoSize = true;
            linkLabel3.LinkColor = Color.FromArgb(255, 128, 0);
            linkLabel3.Location = new Point(33, 217);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(23, 20);
            linkLabel3.TabIndex = 12;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "xx";
            // 
            // linkLabel2
            // 
            linkLabel2.ActiveLinkColor = Color.FromArgb(255, 128, 0);
            linkLabel2.AutoSize = true;
            linkLabel2.LinkColor = Color.FromArgb(255, 128, 0);
            linkLabel2.Location = new Point(33, 197);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(23, 20);
            linkLabel2.TabIndex = 11;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "xx";
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.FromArgb(255, 128, 0);
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.FromArgb(255, 128, 0);
            linkLabel1.Location = new Point(33, 177);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(23, 20);
            linkLabel1.TabIndex = 10;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "xx";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(33, 118);
            label4.Name = "label4";
            label4.Size = new Size(23, 20);
            label4.TabIndex = 3;
            label4.Text = "xx";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(33, 98);
            label3.Name = "label3";
            label3.Size = new Size(23, 20);
            label3.TabIndex = 2;
            label3.Text = "xx";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(33, 78);
            label2.Name = "label2";
            label2.Size = new Size(23, 20);
            label2.TabIndex = 1;
            label2.Text = "xx";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 19F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(30, 27);
            label1.Name = "label1";
            label1.Size = new Size(50, 45);
            label1.TabIndex = 0;
            label1.Text = "xx";
            // 
            // Status_Panel
            // 
            Status_Panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Status_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Status_Panel.Controls.Add(labelTask2);
            Status_Panel.Controls.Add(labelUpdate);
            Status_Panel.Controls.Add(progressBar1);
            Status_Panel.Controls.Add(labelServer);
            Status_Panel.Location = new Point(0, 615);
            Status_Panel.Name = "Status_Panel";
            Status_Panel.Size = new Size(1168, 47);
            Status_Panel.TabIndex = 12;
            // 
            // labelTask2
            // 
            labelTask2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            labelTask2.BackColor = Color.FromArgb(34, 34, 34);
            labelTask2.FlatStyle = FlatStyle.Popup;
            labelTask2.Font = new Font("Segoe UI", 8F);
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.Location = new Point(758, 6);
            labelTask2.Name = "labelTask2";
            labelTask2.Size = new Size(395, 21);
            labelTask2.TabIndex = 9;
            labelTask2.Text = "label7";
            labelTask2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            progressBar1.Location = new Point(761, 31);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(395, 10);
            progressBar1.TabIndex = 1;
            // 
            // labelServer
            // 
            labelServer.AutoSize = true;
            labelServer.Font = new Font("Segoe UI", 9F);
            labelServer.ForeColor = Color.FromArgb(255, 128, 0);
            labelServer.Location = new Point(6, 21);
            labelServer.Name = "labelServer";
            labelServer.Size = new Size(50, 20);
            labelServer.TabIndex = 0;
            labelServer.Text = "label1";
            // 
            // Background_Panel
            // 
            Background_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Background_Panel.BackColor = Color.FromArgb(24, 24, 24);
            Background_Panel.Location = new Point(0, 0);
            Background_Panel.Name = "Background_Panel";
            Background_Panel.Size = new Size(1168, 662);
            Background_Panel.TabIndex = 11;
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(1168, 662);
            Controls.Add(Header_Panel);
            Controls.Add(Body_Panel);
            Controls.Add(Background_Panel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Interface";
            Text = "Form1";
            Header_Panel.ResumeLayout(false);
            Header_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Header_Logo).EndInit();
            Body_Panel.ResumeLayout(false);
            Content_Panel.ResumeLayout(false);
            panelSelection.ResumeLayout(false);
            panel5.ResumeLayout(false);
            Panel_Library.ResumeLayout(false);
            panelStore.ResumeLayout(false);
            Panel_Settings.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelWait.ResumeLayout(false);
            Panel_About.ResumeLayout(false);
            Panel_About.PerformLayout();
            Status_Panel.ResumeLayout(false);
            Status_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel Header_Panel;
        private Panel Body_Panel;
        private PictureBox Header_Logo;
        private ToolTip tooltip_frame;
        private Label Header_Title;
        private BindingSource bindingSource1;
        private Label label4;
        private Panel Content_Panel;
        private Panel Background_Panel;
        private Panel Status_Panel;
        private Panel Panel_Settings;
        private Panel Panel_Library;
        private Button buttonAbout;
        private Button buttonSettings;
        private Button buttonLibrary;
        private Panel Panel_About;
        private Label labelUpdate;
        private Label labelServer;
        private Label label2;
        private Label label1;
        private ListView listLibrary;
        private Label label3;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel1;
        private RadioButton radioChine;
        private RadioButton radioTurke;
        private RadioButton radioKorean;
        private RadioButton radioRuss;
        private RadioButton radioPortugese;
        private RadioButton radioJap;
        private RadioButton radioEnglish;
        private RadioButton radioIndi;
        private RadioButton radioSpain;
        private RadioButton radioItaly;
        private RadioButton radioFrench;
        private RadioButton radioGerman;
        private RadioButton radioTrayOn;
        private RadioButton radioTrayOff;
        private TextBox inputUpdateServer;
        private TextBox inputLibraryServer;
        private Label labelUpdateServer;
        private Label labelLibraryServer;
        private Label labelLanguage;
        private Label labelRadio;
        private Button btnSettingsSave;
        private Button btnSettingsRevert;
        private Label labelTask2;
        private Button btnSettingsRestore;
        private Label label7;
        private Label labelLibraryServerH;
        private Label labelUpdateServerH;
        private Panel panel1;
        private Label labelTrayH;
        private Panel panel2;
        private Label labelLang;
        private Panel panel4;
        private Panel panel3;
        private Button buttonStore;
        private Panel panelStore;
        private ListView listStore;
        private Panel panelWait;
        private Label labelPleaseWait;
        private ProgressBar progressBar1;
        private Panel panelSelection;
        private Label labelSelection;
        private Button buttonSelectionYes;
        private Button buttonSelectionNo;
        private Panel panel5;
        private Button buttonSelectionYesUp;
        private Button buttonSelectionNoUp;
    }
}
