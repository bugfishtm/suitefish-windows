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
            Header_Version = new Label();
            Header_Logo = new PictureBox();
            Header_Title = new Label();
            Body_Panel = new Panel();
            Navi_Panel = new Panel();
            Navi_List = new ListBox();
            Content_Panel = new Panel();
            CUpdate_Panel = new Panel();
            CSettings_Panel = new Panel();
            CWelcome_Panel = new Panel();
            CLibrary_Panel = new Panel();
            CStore_Panel = new Panel();
            Status_Panel = new Panel();
            tooltip_frame = new ToolTip(components);
            bindingSource1 = new BindingSource(components);
            Background_Panel = new Panel();
            Header_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Header_Logo).BeginInit();
            Body_Panel.SuspendLayout();
            Navi_Panel.SuspendLayout();
            Content_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // Header_Panel
            // 
            Header_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Header_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Header_Panel.Controls.Add(Header_Version);
            Header_Panel.Controls.Add(Header_Logo);
            Header_Panel.Controls.Add(Header_Title);
            Header_Panel.Location = new Point(0, 0);
            Header_Panel.Name = "Header_Panel";
            Header_Panel.Size = new Size(1099, 95);
            Header_Panel.TabIndex = 0;
            Header_Panel.MouseDoubleClick += Header_Panel_MouseDoubleClick;
            Header_Panel.MouseDown += Header_Panel_MouseDown;
            // 
            // Header_Version
            // 
            Header_Version.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Header_Version.AutoSize = true;
            Header_Version.BackColor = Color.Transparent;
            Header_Version.Font = new Font("Segoe UI", 10F);
            Header_Version.ForeColor = Color.White;
            Header_Version.Location = new Point(994, 57);
            Header_Version.Name = "Header_Version";
            Header_Version.Size = new Size(93, 23);
            Header_Version.TabIndex = 4;
            Header_Version.Text = "Version 1.0";
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
            Header_Title.Font = new Font("Segoe UI", 40F);
            Header_Title.ForeColor = Color.White;
            Header_Title.Location = new Point(113, 3);
            Header_Title.Name = "Header_Title";
            Header_Title.Size = new Size(289, 89);
            Header_Title.TabIndex = 3;
            Header_Title.Text = "Suitefish";
            // 
            // Body_Panel
            // 
            Body_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Body_Panel.Controls.Add(Navi_Panel);
            Body_Panel.Controls.Add(Content_Panel);
            Body_Panel.Controls.Add(Status_Panel);
            Body_Panel.Dock = DockStyle.Fill;
            Body_Panel.Location = new Point(0, 0);
            Body_Panel.Name = "Body_Panel";
            Body_Panel.Size = new Size(1099, 620);
            Body_Panel.TabIndex = 1;
            // 
            // Navi_Panel
            // 
            Navi_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Navi_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Navi_Panel.Controls.Add(Navi_List);
            Navi_Panel.Location = new Point(0, 95);
            Navi_Panel.Name = "Navi_Panel";
            Navi_Panel.Size = new Size(222, 487);
            Navi_Panel.TabIndex = 9;
            // 
            // Navi_List
            // 
            Navi_List.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Navi_List.BackColor = Color.FromArgb(32, 32, 32);
            Navi_List.BorderStyle = BorderStyle.None;
            Navi_List.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Navi_List.ForeColor = SystemColors.ControlLightLight;
            Navi_List.FormattingEnabled = true;
            Navi_List.ItemHeight = 28;
            Navi_List.Location = new Point(0, 0);
            Navi_List.Name = "Navi_List";
            Navi_List.Size = new Size(222, 476);
            Navi_List.TabIndex = 1;
            // 
            // Content_Panel
            // 
            Content_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Content_Panel.BackColor = Color.FromArgb(224, 224, 224);
            Content_Panel.Controls.Add(CUpdate_Panel);
            Content_Panel.Controls.Add(CSettings_Panel);
            Content_Panel.Controls.Add(CWelcome_Panel);
            Content_Panel.Controls.Add(CLibrary_Panel);
            Content_Panel.Controls.Add(CStore_Panel);
            Content_Panel.Location = new Point(221, 95);
            Content_Panel.Name = "Content_Panel";
            Content_Panel.Size = new Size(875, 487);
            Content_Panel.TabIndex = 9;
            // 
            // CUpdate_Panel
            // 
            CUpdate_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CUpdate_Panel.BackColor = Color.FromArgb(224, 224, 224);
            CUpdate_Panel.Location = new Point(0, 0);
            CUpdate_Panel.Name = "CUpdate_Panel";
            CUpdate_Panel.Size = new Size(875, 487);
            CUpdate_Panel.TabIndex = 13;
            // 
            // CSettings_Panel
            // 
            CSettings_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CSettings_Panel.BackColor = Color.FromArgb(224, 224, 224);
            CSettings_Panel.Location = new Point(0, 0);
            CSettings_Panel.Name = "CSettings_Panel";
            CSettings_Panel.Size = new Size(875, 487);
            CSettings_Panel.TabIndex = 13;
            // 
            // CWelcome_Panel
            // 
            CWelcome_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CWelcome_Panel.BackColor = Color.FromArgb(224, 224, 224);
            CWelcome_Panel.Location = new Point(0, 0);
            CWelcome_Panel.Name = "CWelcome_Panel";
            CWelcome_Panel.Size = new Size(875, 487);
            CWelcome_Panel.TabIndex = 12;
            // 
            // CLibrary_Panel
            // 
            CLibrary_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CLibrary_Panel.BackColor = Color.FromArgb(224, 224, 224);
            CLibrary_Panel.Location = new Point(0, 0);
            CLibrary_Panel.Name = "CLibrary_Panel";
            CLibrary_Panel.Size = new Size(875, 487);
            CLibrary_Panel.TabIndex = 11;
            // 
            // CStore_Panel
            // 
            CStore_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CStore_Panel.BackColor = Color.FromArgb(224, 224, 224);
            CStore_Panel.Location = new Point(0, 0);
            CStore_Panel.Name = "CStore_Panel";
            CStore_Panel.Size = new Size(875, 487);
            CStore_Panel.TabIndex = 10;
            // 
            // Status_Panel
            // 
            Status_Panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Status_Panel.BackColor = Color.FromArgb(32, 32, 32);
            Status_Panel.Location = new Point(0, 582);
            Status_Panel.Name = "Status_Panel";
            Status_Panel.Size = new Size(1099, 38);
            Status_Panel.TabIndex = 12;
            // 
            // Background_Panel
            // 
            Background_Panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Background_Panel.BackColor = Color.White;
            Background_Panel.Location = new Point(0, 0);
            Background_Panel.Name = "Background_Panel";
            Background_Panel.Size = new Size(1099, 620);
            Background_Panel.TabIndex = 11;
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(1099, 620);
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
            Navi_Panel.ResumeLayout(false);
            Content_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel Header_Panel;
        private Panel Body_Panel;
        private PictureBox Header_Logo;
        private ToolTip tooltip_frame;
        private Label Header_Title;
        private ListBox Navi_List;
        private BindingSource bindingSource1;
        private Label label4;
        private Panel Content_Panel;
        private Panel Navi_Panel;
        private Label Header_Version;
        private Panel Background_Panel;
        private Panel Status_Panel;
        private Panel CStore_Panel;
        private Panel CSettings_Panel;
        private Panel CWelcome_Panel;
        private Panel CLibrary_Panel;
        private Panel CUpdate_Panel;
    }
}
