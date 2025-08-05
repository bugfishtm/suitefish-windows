using Microsoft.VisualBasic;
using suitefish.Library;
using suitefish.Properties;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace suitefish
{
    public partial class Interface : Form
    {
        ////////////////////////////////////////////////////////////
        /// Resizing/Moving (No Save, Not Related to Purpose)
        ////////////////////////////////////////////////////////////
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private Point offset;
        private int previousheight;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        // Debug Console
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        // Form Styling
        const int SF_MINIMUM_WIDTH = 1000;
        const int SF_MINIMUM_HEIGHT = 700;
        private int borderWidth = 5;
        private Color borderColor = Color.FromArgb(0x32, 0x32, 0x32);
        private Color buttonColor = Color.FromArgb(0x32, 0x32, 0x32);
        private Color buttonTextColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;
        private string btnMinimizeText = "Minimize";
        private string btnMaximizeText = "Maximize";
        private string btnCloseText = "Close";

        ////////////////////////////////////////////////////////////
        /// Software Related
        ////////////////////////////////////////////////////////////
        private Sqlite sqlite;
        private ContextMenuStrip contextMenuStrip;
        private NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timer_library_update = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timer_software_update = new System.Windows.Forms.Timer();

        ////////////////////////////////////////////////////////////
        /// Debug Constants
        ////////////////////////////////////////////////////////////
        const bool SF_DEBUG_CR = false;

        ////////////////////////////////////////////////////////////
        /// Software Version and Variables
        ////////////////////////////////////////////////////////////
        const string SF_VERSION_CR = "1.0.4";
        private string constant_default_server = "https://suitefish.com";

        ////////////////////////////////////////////////////////////
        /// Software Settings
        ////////////////////////////////////////////////////////////
        private string setting_use_tray_on_exit;
        private string setting_server_store;
        private string setting_server_update;
        private string setting_language;

        ////////////////////////////////////////////////////////////
        /// List Item
        ////////////////////////////////////////////////////////////
        public JsonElement[] current_store_list = [];
        public bool op_running = false;
        public string op_folder = "";
        public ListViewItem op_temp;
        bool cancelbuttondrawout = false;

        public Interface()
        {
            ////////////////////////////////////////////////////////////
            /// Form Settings
            ////////////////////////////////////////////////////////////
            InitializeComponent();
            this.Text = "Suitefish";
            this.Icon = new Icon(new MemoryStream(Properties.Resources.favicon));
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderWidth);
            this.Padding = new Padding(5);
            this.Paint += new PaintEventHandler(CustomUI_Interface_Paint);
            this.Resize += CustomUI_Interface_Resize;
            this.FormClosing += CustomUI_FormClosing;

            ////////////////////////////////////////////////////////////
            /// Debug Console if Debugging Constant is activated
            ////////////////////////////////////////////////////////////
            if (SF_DEBUG_CR)
            {
                AllocConsole();
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Debugging Mode for Suitefish!");
                Console.WriteLine("Whoop whoop!");
                Console.WriteLine("-----------------------------------------------");
            }

            ////////////////////////////////////////////////////////////
            /// Default Settings
            ////////////////////////////////////////////////////////////
            setting_server_store = constant_default_server;
            setting_server_update = constant_default_server;
            setting_use_tray_on_exit = "0";

            ////////////////////////////////////////////////////////////
            /// SQLite Connection
            ////////////////////////////////////////////////////////////
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.db");
            sqlite = new Sqlite(dbPath);
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Settings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Value TEXT NOT NULL, Shortcode TEXT NOT NULL UNIQUE);");

            ////////////////////////////////////////////////////////////
            /// SQLite Content Setup and Settings Init
            ////////////////////////////////////////////////////////////
            if (SettingGet("setting_server_store") == null)
            { SettingSet("setting_server_store", constant_default_server); }
            setting_server_store = SettingGet("setting_server_store");
            if (SettingGet("setting_server_update") == null)
            { SettingSet("setting_server_update", constant_default_server); }
            setting_server_update = SettingGet("setting_server_update");
            if (SettingGet("setting_use_tray_on_exit") == null)
            { SettingSet("setting_use_tray_on_exit", "0"); }
            setting_use_tray_on_exit = SettingGet("setting_use_tray_on_exit");
            if (SettingGet("setting_language") == null)
            { SettingSet("setting_language", "en"); }
            setting_language = SettingGet("setting_language");

            ////////////////////////////////////////////////////////////
            /// Load Translation
            ////////////////////////////////////////////////////////////
            btnMaximizeText = translateKey("maximize");
            btnMinimizeText = translateKey("minimize");
            btnCloseText = translateKey("close");

            ////////////////////////////////////////////////////////////
            /// Draw Buttons
            ////////////////////////////////////////////////////////////
            CustomUI_Buttons_Draw();

            ////////////////////////////////////////////////////////////
            // Create and configure ContextMenuStrip for Tray
            ////////////////////////////////////////////////////////////
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(translateKey("open"), null, (s, e) => { NotifyIcon_EntryShow(); });
            contextMenuStrip.Items.Add(translateKey("maximize"), null, (s, e) => { NotifyIcon_EntryMaximize(); });
            contextMenuStrip.Items.Add(translateKey("exit"), null, (s, e) => { NotifyIcon_EntryExit(s, e); });

            ////////////////////////////////////////////////////////////
            // Folder Creation Preparation
            ////////////////////////////////////////////////////////////
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";

            ////////////////////////////////////////////////////////////
            // Create Temp folder for Updater and Downloads
            ////////////////////////////////////////////////////////////
            folderName = "sf-cache";
            folderPath = Path.Combine(exeDirectory, folderName);
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
            folderName = "sf-cache";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            folderName = "sf-cache/downloading";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            folderName = "sf-cache/extracting";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            folderName = "sf-cache/updating";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);

            ////////////////////////////////////////////////////////////
            // Folder for Finished Software Downloads
            ////////////////////////////////////////////////////////////
            folderName = "sf-apps";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);

            ////////////////////////////////////////////////////////////
            // Show Library Panel on Start
            ////////////////////////////////////////////////////////////
            panelWait.BringToFront();
            Panel_Library.Visible = false;
            panelStore.Visible = false;
            Panel_Settings.Visible = false;
            panelSelection.Visible = false;
            Panel_About.Visible = false;
            buttonLibrary.Visible = false;
            buttonStore.Visible = false;
            buttonAbout.Visible = false;
            buttonSettings.Visible = false;

            ////////////////////////////////////////////////////////////
            // Initial Label Setup
            ////////////////////////////////////////////////////////////
            labelUpdate.Text = translateKey("text_update_available");
            labelUpdate.Visible = false;
            labelTask2.Visible = false;
            progressBar1.Visible = false;
            labelServer.Visible = false;
            Header_Title.Text = "Suitefish";
            //Header_Version.Text = translateKey("version") + ": " + SF_VERSION_CR;
            label7.Text = translateKey("version") + " " + SF_VERSION_CR;

            ////////////////////////////////////////////////////////////
            // Change Button on Top Translation
            ////////////////////////////////////////////////////////////
            buttonAbout.Text = translateKey("about");
            buttonLibrary.Text = translateKey("library");
            buttonSettings.Text = translateKey("settings");
            buttonStore.Text = translateKey("store");
            labelPleaseWait.Text = translateKey("please_wait");
            buttonSelectionNo.Text = translateKey("no");
            buttonSelectionNoUp.Text = translateKey("no");
            buttonSelectionYes.Text = translateKey("yes");
            buttonSelectionYesUp.Text = translateKey("yes");
            labelPleaseWait.AutoSize = false;
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonLibrary.BackColor = Color.FromArgb(255, 128, 0);

            ////////////////////////////////////////////////////////////
            // Setup Library Update Timer
            ////////////////////////////////////////////////////////////
            //  timer_library_update.Interval = 60000;
            // timer_library_update.Tick += timer_library_update_event;
            // timer_library_update.Start();

            ////////////////////////////////////////////////////////////
            // Setup Software Update Timer
            ////////////////////////////////////////////////////////////
            // timer_software_update.Interval = 600000;
            // timer_software_update.Tick += timer_software_update_event;
            // timer_software_update.Start();

            ////////////////////////////////////////////////////////////
            // Create and configure NotifyIcon
            ////////////////////////////////////////////////////////////
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(new MemoryStream(Properties.Resources.favicon)),
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            notifyIcon.Visible = false;
            notifyIcon.Text = "Suitefish";

            ////////////////////////////////////////////////////////////
            // About Page Linklabel Click Events
            ////////////////////////////////////////////////////////////
            linkLabel1.Click += (sender, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://suitefish.com",
                    UseShellExecute = true
                });
            };
            linkLabel2.Click += (sender, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/bugfishtm/suitefish-cms",
                    UseShellExecute = true
                });
            };
            linkLabel3.Click += (sender, e) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://bugfishtm.github.io/suitefish-cms/",
                    UseShellExecute = true
                });
            };

            ////////////////////////////////////////////////////////////
            // List View Setup in Library
            ////////////////////////////////////////////////////////////
            listLibrary.OwnerDraw = true;
            listLibrary.View = View.Details;
            listLibrary.FullRowSelect = true;
            listLibrary.Columns.Add(translateKey("identifier"), 120);
            listLibrary.Columns.Add(translateKey("name"), 200);
            listLibrary.Columns.Add(translateKey("description"), 300);
            listLibrary.Columns.Add(translateKey("version"), 100);
            listLibrary.Columns.Add(translateKey("latest_version"), 100);
            listLibrary.Columns.Add(translateKey("folder"), 100);
            listLibrary.Columns.Add(translateKey("author"), 100);
            listLibrary.Columns.Add(translateKey("license"), 100);
            listLibrary.Columns.Add(translateKey("executable"), 100);
            listLibrary.DrawColumnHeader += (s, e) =>
            {
                // Set your custom colors
                Color headerBackColor = Color.FromArgb(28, 28, 28);       // Background color
                Color headerTextColor = Color.FromArgb(255, 255, 255);      // Text color

                using (SolidBrush backBrush = new SolidBrush(headerBackColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }

                using (SolidBrush textBrush = new SolidBrush(headerTextColor))
                {
                    // Center the header text
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(
                        e.Header.Text,
                        e.Font,
                        textBrush,
                        e.Bounds,
                        sf
                    );
                }

                // Optionally, draw the header border
                e.Graphics.DrawRectangle(Pens.Black, e.Bounds);
            };
            listStore.OwnerDraw = true;
            listStore.View = View.Details;
            listStore.FullRowSelect = true;
            listStore.Columns.Add(translateKey("identifier"), 120);
            listStore.Columns.Add(translateKey("name"), 200);
            listStore.Columns.Add(translateKey("description"), 300);
            listStore.Columns.Add(translateKey("version"), 100);
            listStore.Columns.Add(translateKey("author"), 100);
            listStore.Columns.Add(translateKey("license"), 100);
            listStore.DrawColumnHeader += (s, e) =>
            {
                // Set your custom colors
                Color headerBackColor = Color.FromArgb(28, 28, 28);       // Background color
                Color headerTextColor = Color.FromArgb(255, 255, 255);      // Text color

                using (SolidBrush backBrush = new SolidBrush(headerBackColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }

                using (SolidBrush textBrush = new SolidBrush(headerTextColor))
                {
                    // Center the header text
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(
                        e.Header.Text,
                        e.Font,
                        textBrush,
                        e.Bounds,
                        sf
                    );
                }

                // Optionally, draw the header border
                e.Graphics.DrawRectangle(Pens.Black, e.Bounds);
            };

            listLibrary.MouseDoubleClick += listLibrary_MouseDoubleClick;
            listLibrary.MouseUp += listLibrary_MouseUp;
            listStore.MouseDoubleClick += listStore_MouseDoubleClick;
            listStore.MouseUp += listStore_MouseUp;
            listLibrary.Resize += (s, e) => AdjustLastColumnWidth(listLibrary);
            listStore.Resize += (s, e) => AdjustLastColumnWidth(listStore);
            AdjustLastColumnWidth(listLibrary);
            AdjustLastColumnWidth(listStore);
            listLibrary.DrawItem += (s, e) => e.DrawDefault = true;
            listLibrary.DrawSubItem += (s, e) => e.DrawDefault = true;
            listStore.DrawItem += (s, e) => e.DrawDefault = true;
            listStore.DrawSubItem += (s, e) => e.DrawDefault = true;
            ////////////////////////////////////////////////////////////
            // Init Sync Function Start
            ////////////////////////////////////////////////////////////

            timer_library_update = new System.Windows.Forms.Timer();
            timer_library_update.Interval = 1000; // 2 seconds
            timer_library_update.Tick += Timer_Tick;
            timer_library_update.Start();
        }

        ////////////////////////////////////////////////////////////
        // Init
        ////////////////////////////////////////////////////////////
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer_library_update.Stop();
            reload_external_library();
            init_starter_show_first();
        }

        ////////////////////////////////////////////////////////////
        // Init
        ////////////////////////////////////////////////////////////
        private void init_starter_show_first()
        {
            panelWait.SendToBack();
            Panel_Library.Visible = true;
            Panel_Library.BringToFront();
            buttonLibrary.Visible = true;
            buttonStore.Visible = true;
            buttonAbout.Visible = true;
            buttonSettings.Visible = true;
        }

        private async void lib_sw_update(ListViewItem item)
        {
            string foldervalue = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";
            folderName = "sf-apps";
            string finalfolder = Path.Combine(exeDirectory, folderName);
            string newfilefolder = finalfolder + "\\" + foldervalue;
            if (!IsFolderLocked(newfilefolder))
            {
                panelSelection.BringToFront();
                panelSelection.Visible = true;
                buttonSelectionNo.Visible = false;
                buttonSelectionYes.Visible = false;
                buttonSelectionNoUp.Visible = true;
                buttonSelectionYesUp.Visible = true;
                labelSelection.ForeColor = Color.FromArgb(222, 222, 222);
                labelSelection.Text = translateKey("text_wanna_update");
                op_temp = item;

            }
            else
            {
                panelSelection.BringToFront();
                panelSelection.Visible = true;
                buttonSelectionNo.Visible = false;
                buttonSelectionYes.Visible = false;
                buttonSelectionNoUp.Visible = false;
                buttonSelectionYesUp.Visible = false;
                labelSelection.ForeColor = Color.FromArgb(255, 0, 0);
                labelSelection.Text = translateKey("files_in_use_product");
            }
        }

        private async void buttonSelectionYesUp_Click(object sender, EventArgs e)
        {
            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            progressBar1.Visible = true;
            labelTask2.Visible = true;
            string foldervalue = op_temp.SubItems.Count > 5 ? op_temp.SubItems[5].Text : string.Empty;
            string cver = op_temp.SubItems.Count > 3 ? op_temp.SubItems[3].Text : string.Empty;
            string lver = op_temp.SubItems.Count > 4 ? op_temp.SubItems[4].Text : string.Empty;
            labelTask2.Text = translateKey("update") + ": " + foldervalue;
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            panelSelection.SendToBack();
            panelSelection.Visible = false;
            buttonSelectionNo.Visible = false;
            buttonSelectionYes.Visible = false;
            buttonSelectionNoUp.Visible = false;
            buttonSelectionYesUp.Visible = false;
            Panel_Library.BringToFront();
            labelSelection.Text = translateKey("please_wait");
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 1;
            progressBar1.Value = 20;

            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = true;
            op_folder = foldervalue;

            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";
            folderName = "sf-cache/downloading";
            string downloadfolderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(downloadfolderPath);
            folderName = "sf-cache/extracting";
            string extractfolderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(extractfolderPath);
            folderName = "sf-apps";
            string finalfolder = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(finalfolder);
            string newfilefolder = finalfolder + "\\" + foldervalue;

            labelTask2.Text = translateKey("clearing") + ": " + foldervalue;
            // Remove all folders but  __Output __Config __Persistent
            string[] foldersToKeep = new string[] { "__Output", "__Config", "__Persistent" };
            foreach (var dir in Directory.GetDirectories(newfilefolder))
            {
                // Extract only directory name from full path
                string folderNamexy = Path.GetFileName(dir);

                // If this folder is not one of the folders to keep, delete it
                if (!foldersToKeep.Contains(folderNamexy))
                {
                    Directory.Delete(dir, true); // recursive delete
                }
            }

            labelTask2.Text = translateKey("download") + ": " + foldervalue;
            progressBar1.Value = 40;
            // Download the new Package
            await SF_DownloadFile(setting_server_store + "/_store/module/" + op_temp.Text + "-" + lver + ".zip", downloadfolderPath + "\\" + op_temp.Text + "-" + lver + ".zip");
            labelTask2.Text = translateKey("extract") + ": " + foldervalue;
            progressBar1.Value = 60;
            // Extract the new Package
            SF_Extract(downloadfolderPath + "\\" + op_temp.Text + "-" + lver + ".zip", extractfolderPath + "\\" + op_temp.Text + "-" + lver + "");

            progressBar1.Maximum = 100;
            progressBar1.Minimum = 1;
            labelTask2.Text = translateKey("move") + ": " + foldervalue;
            progressBar1.Value = 80;
            string newextractpath = extractfolderPath + "\\" + op_temp.Text + "-" + lver + "" + "\\" + op_temp.Text;
            string newextractpathto = finalfolder + "\\" + foldervalue;
            if (SF_DEBUG_CR)
            {
                Console.WriteLine("------------------------------------###");
                Console.WriteLine(newfilefolder);
                Console.WriteLine(newextractpath);
                Console.WriteLine(newextractpathto);
            }
            // Move all files (and overwrite) from newextractpath to newextractpathto
            // Ensure destination folder exists
            if (!Directory.Exists(newextractpathto))
            {
                Directory.CreateDirectory(newextractpathto);
            }

            // Move all files from newextractpath to newextractpathto (overwrite if exists)
            foreach (var filePath in Directory.GetFiles(newextractpath))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(newextractpathto, fileName);

                // If destination file exists, delete it first to allow overwrite
                if (System.IO.File.Exists(destFilePath))
                {
                    System.IO.File.Delete(destFilePath);
                }

                System.IO.File.Move(filePath, destFilePath);
            }

            System.IO.File.Delete(downloadfolderPath + "\\" + op_temp.Text + "-" + lver + ".zip");
            Directory.Delete(newextractpath, true);
            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = false;
            op_folder = "";
            progressBar1.Value = 100;
            labelTask2.Text = translateKey("update_finished");
            labelTask2.ForeColor = Color.Lime;
            reload_local_library();

            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            ///
            timer_library_update = new System.Windows.Forms.Timer();
            timer_library_update.Interval = 10000; // 2 seconds
            timer_library_update.Tick += Timer_Tick2;
            timer_library_update.Start();

        }


        private void buttonSelectionNoUp_Click(object sender, EventArgs e)
        {
            panelSelection.SendToBack();
            panelSelection.Visible = false;
            buttonSelectionNo.Visible = false;
            buttonSelectionYes.Visible = false;
            buttonSelectionNoUp.Visible = false;
            buttonSelectionYesUp.Visible = false;
            Panel_Library.BringToFront();
        }

        private async void lib_sw_delete(ListViewItem item)
        {
            string foldervalue = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";
            folderName = "sf-apps";
            string finalfolder = Path.Combine(exeDirectory, folderName);
            string newfilefolder = finalfolder + "\\" + foldervalue;
            if (!IsFolderLocked(newfilefolder))
            {
                panelSelection.BringToFront();
                panelSelection.Visible = true;
                buttonSelectionNo.Visible = true;
                buttonSelectionYes.Visible = true;
                buttonSelectionNoUp.Visible = false;
                buttonSelectionYesUp.Visible = false;
                labelSelection.ForeColor = Color.FromArgb(222, 222, 222);
                labelSelection.Text = translateKey("text_wanna_delete");
                op_temp = item;

            } else
            {
                panelSelection.BringToFront();
                panelSelection.Visible = true;
                buttonSelectionNo.Visible = false;
                buttonSelectionYes.Visible = false;
                buttonSelectionNoUp.Visible = false;
                buttonSelectionYesUp.Visible = false;
                labelSelection.ForeColor = Color.FromArgb(255, 0, 0);
                labelSelection.Text = translateKey("files_in_use_product");
            }
        }

        public static bool IsFolderLocked(string folderPath)
        {
            try
            {
                foreach (var filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    if (IsFileLocked(filePath))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static bool IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return false; 
            }
            catch (IOException)
            {
                return true;  
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private void buttonSelectionNo_Click(object sender, EventArgs e)
        {
            panelSelection.SendToBack();
            panelSelection.Visible = false;
            buttonSelectionNo.Visible = false;
            buttonSelectionYes.Visible = false;
            buttonSelectionNoUp.Visible = false;
            buttonSelectionYesUp.Visible = false;
            Panel_Library.BringToFront();
        }

        private void buttonSelectionYes_Click(object sender, EventArgs e)
        {
            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            progressBar1.Visible = true;
            labelTask2.Visible = true;
            string foldervalue = op_temp.SubItems.Count > 5 ? op_temp.SubItems[5].Text : string.Empty;
            labelTask2.Text = translateKey("delete") + ": " + foldervalue;
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            panelSelection.SendToBack();
            panelSelection.Visible = false;
            buttonSelectionNo.Visible = false;
            buttonSelectionYes.Visible = false;
            buttonSelectionNoUp.Visible = false;
            buttonSelectionYesUp.Visible = false;
            Panel_Library.BringToFront();
            labelSelection.Text = translateKey("please_wait");
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 1;
            progressBar1.Value = 50;

            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = true;
            op_folder = foldervalue;


            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";

            ////////////////////////////////////////////////////////////
            // Create Temp folder for Updater and Downloads
            ////////////////////////////////////////////////////////////
            folderName = "sf-apps";
            string downloadfolderPath = Path.Combine(exeDirectory, folderName);
            folderName = folderName + "\\" + foldervalue;
            Directory.Delete(folderName, true);

            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = false;
            op_folder = "";
            progressBar1.Value = 100;
            labelTask2.Text = translateKey("deletion_finished");
            labelTask2.ForeColor = Color.Lime;
            reload_local_library();

            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            ///
            timer_library_update = new System.Windows.Forms.Timer();
            timer_library_update.Interval = 10000; // 2 seconds
            timer_library_update.Tick += Timer_Tick2;
            timer_library_update.Start();
        }

        private async void store_download_package(ListViewItem item)
        {

            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = true;

            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            progressBar1.Visible = true;
            labelTask2.Visible = true;
            labelTask2.Text = translateKey("download") + ": " + item.Text;
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);

            ////////////////////////////////////////////////////////////
            // Folder Creation Preparation
            ////////////////////////////////////////////////////////////
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";

            ////////////////////////////////////////////////////////////
            // Create Temp folder for Updater and Downloads
            ////////////////////////////////////////////////////////////
            folderName = "sf-cache/downloading";
            string downloadfolderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(downloadfolderPath);
            folderName = "sf-cache/extracting";
            string extractfolderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(extractfolderPath);
            folderName = "sf-apps";
            string finalfolder = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(finalfolder);

            // The text of the third column, etc. (second subitem)
            string versionvalue = item.SubItems.Count > 3 ? item.SubItems[3].Text : string.Empty;

            // Download the File
            await SF_DownloadFile(setting_server_store + "/_store/module/" + item.Text + "-" + versionvalue + ".zip", downloadfolderPath + "\\" + item.Text + "-" + versionvalue + ".zip");

            // Extract the Folder
            SF_Extract(downloadfolderPath + "\\" + item.Text + "-" + versionvalue + ".zip", extractfolderPath + "\\" + item.Text + "-" + versionvalue + "");

            int counterfolder = 100000;
            while (Directory.Exists(finalfolder + "\\" + counterfolder))
            {
                counterfolder = counterfolder + 1;
            }

            // Move Folder
            Directory.Move(extractfolderPath + "\\" + item.Text + "-" + versionvalue + "\\" + item.Text, finalfolder + "\\" + counterfolder);

            ////////////////////////////////////////////////////////////
            // OP Running = True
            ////////////////////////////////////////////////////////////
            op_running = false;

            ////////////////////////////////////////////////////////////
            // Progress Display
            ////////////////////////////////////////////////////////////
            ///
            timer_library_update = new System.Windows.Forms.Timer();
            timer_library_update.Interval = 10000; // 2 seconds
            timer_library_update.Tick += Timer_Tick2;
            timer_library_update.Start();
        }

        ////////////////////////////////////////////////////////////
        // Init
        ////////////////////////////////////////////////////////////
        private void Timer_Tick2(object sender, EventArgs e)
        {
            timer_library_update.Stop();
            if (!op_running) { 
                progressBar1.Visible = false;
                labelTask2.Visible = false;
                labelTask2.Text = "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Language Functions
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private string translateKey(string key_to_translate)
        {
            switch (setting_language)
            {
                case "en":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Online software store server is online and available.";
                        case "llupdate_loading": return "Contacting online software store server...";
                        case "lupdate_out_lower": return "You have the latest version of the Suitefish Windows client.";
                        case "lupdate_out_higher": return "A new Suitefish client update is available!";
                        case "lupdate_out_equal": return "You have the latest version of the Suitefish Windows client.";
                        case "lupdate_loading": return "Contacting client update server...";
                        case "lupdate_server_error": return "Error reaching Suitefish client update server!";
                        case "llupdate_server_error": return "Error reaching Suitefish software store server!";
                        case "lupdate_server_ok": return "Suitefish client update server reached, checking for updates...";
                        case "llupdate_server_ok": return "Suitefish software store server reached, refreshing list...";
                        case "maximize": return "Maximize";
                        case "executable": return "Executable";
                        case "please_wait": return "Please wait";
                        case "store": return "Store";
                        case "unknown": return "Unknown";
                        case "minimize": return "Minimize";
                        case "download": return "Download";
                        case "refresh": return "Refresh";
                        case "close": return "Close";
                        case "start": return "Start";
                        case "delete": return "Delete";
                        case "open_folder": return "Open Folder";
                        case "exit": return "Exit";
                        case "error": return "Error";
                        case "server_error": return "Server Error";
                        case "server_ok": return "Online server is available.";
                        case "server_maintenance": return "Online server under maintenance.";
                        case "open": return "Open";
                        case "version": return "Version";
                        case "settings": return "Settings";
                        case "library": return "Library";
                        case "update": return "Update";
                        case "library_server": return "Software Store Server";
                        case "update_server": return "Client Update Server";
                        case "about": return "About";
                        case "website": return "Website";
                        case "github": return "GitHub";
                        case "installed": return "Installed";
                        case "on": return "On";
                        case "off": return "Off";
                        case "save": return "Save";
                        case "cancel": return "Cancel";
                        case "status": return "Status";
                        case "op_download_start": return "Download starting...";
                        case "op_download_ok": return "Download finished.";
                        case "op_extract_start": return "Decompressing started...";
                        case "op_extract_ok": return "Decompressing finished.";
                        case "name": return "Name";
                        case "yes": return "Yes";
                        case "no": return "No";
                        case "description": return "Description";
                        case "size": return "Size (GB)";
                        case "latest_version": return "Latest Version";
                        case "author": return "Author";
                        case "license": return "License";
                        case "revert": return "Revert";
                        case "restore": return "Restore";
                        case "current": return "Current";
                        case "language": return "Language";
                        case "op_move_start": return "Moving folder...";
                        case "op_move_ok": return "Folder move completed.";
                        case "folder": return "Folder";
                        case "information": return "Information";
                        case "settings_saved": return "Settings have been applied.";
                        case "settings_restored_to_default": return "Restored to Default";
                        case "identifier": return "Identifier";
                        case "documentation": return "Documentation";
                        case "windows_client": return "Windows Client";
                        case "text_about_license": return "GPLv3 Open Source License";
                        case "text_about_creator": return "Made by Bugfish™";
                        case "text_settings_tray_set": return "Close to Tray";
                        case "text_about_updatet": return "Click 'Update' to close the software and start the update process.";
                        case "text_update_available": return "A new update for the Suitefish Windows client is available!";
                        case "list_update_complete": return "List update complete.";
                        case "cannot_download": return "Cannot download while a task is running!";
                        case "cannot_delete": return "Cannot delete while a task is running!";
                        case "cannot_start": return "Cannot start while a task is running!";
                        case "cannot_update": return "Cannot update while a task is running!";
                        case "text_wanna_delete": return "Delete selected product?";
                        case "deletion_finished": return "Deletion successful.";
                        case "text_wanna_update": return "Update selected product?";
                        case "update_finished": return "Update successful.";
                        case "clearing": return "Clearing...";
                        case "extract": return "Éxtract";
                        case "move": return "Move";
                        case "files_in_use_product": return "Product is running or files are locked.";
                        case "task_running": return "A task is running. Do you really want to close the software?";
                        default: return "ERROR:NOKEY";
                    }
                case "de":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Der Online-Softwarestore-Server ist online und verfügbar.";
                        case "llupdate_loading": return "Kontakt zum Online-Softwarestore-Server wird hergestellt...";
                        case "lupdate_out_lower": return "Sie haben die aktuellste Version des Suitefish Windows-Clients.";
                        case "lupdate_out_higher": return "Ein neues Suitefish-Client-Update ist verfügbar!";
                        case "lupdate_out_equal": return "Sie haben die aktuellste Version des Suitefish Windows-Clients.";
                        case "lupdate_loading": return "Kontakt zum Client-Update-Server wird hergestellt...";
                        case "lupdate_server_error": return "Fehler beim Erreichen des Suitefish Client-Update-Servers!";
                        case "llupdate_server_error": return "Fehler beim Erreichen des Suitefish Softwarestore-Servers!";
                        case "lupdate_server_ok": return "Suitefish Client-Update-Server erreicht, überprüfe auf Updates...";
                        case "llupdate_server_ok": return "Suitefish Softwarestore-Server erreicht, aktualisiere Liste...";
                        case "maximize": return "Maximieren";
                        case "executable": return "Ausführbare Datei";
                        case "please_wait": return "Bitte warten";
                        case "store": return "Store";
                        case "unknown": return "Unbekannt";
                        case "minimize": return "Minimieren";
                        case "download": return "Herunterladen";
                        case "refresh": return "Aktualisieren";
                        case "close": return "Schließen";
                        case "start": return "Starten";
                        case "delete": return "Löschen";
                        case "open_folder": return "Ordner öffnen";
                        case "exit": return "Beenden";
                        case "error": return "Fehler";
                        case "server_error": return "Serverfehler";
                        case "server_ok": return "Online-Server ist verfügbar.";
                        case "server_maintenance": return "Online-Server in Wartung.";
                        case "open": return "Öffnen";
                        case "version": return "Version";
                        case "settings": return "Einstellungen";
                        case "library": return "Bibliothek";
                        case "update": return "Aktualisieren";
                        case "library_server": return "Softwarestore-Server";
                        case "update_server": return "Client-Update-Server";
                        case "about": return "Über";
                        case "website": return "Website";
                        case "github": return "GitHub";
                        case "installed": return "Installiert";
                        case "on": return "An";
                        case "off": return "Aus";
                        case "save": return "Speichern";
                        case "cancel": return "Abbrechen";
                        case "status": return "Status";
                        case "op_download_start": return "Download wird gestartet...";
                        case "op_download_ok": return "Download abgeschlossen.";
                        case "op_extract_start": return "Entpacken gestartet...";
                        case "op_extract_ok": return "Entpacken abgeschlossen.";
                        case "name": return "Name";
                        case "yes": return "Ja";
                        case "no": return "Nein";
                        case "description": return "Beschreibung";
                        case "size": return "Größe (GB)";
                        case "latest_version": return "Neueste Version";
                        case "author": return "Autor";
                        case "license": return "Lizenz";
                        case "revert": return "Zurücksetzen";
                        case "restore": return "Wiederherstellen";
                        case "current": return "Aktuell";
                        case "language": return "Sprache";
                        case "op_move_start": return "Ordner wird verschoben...";
                        case "op_move_ok": return "Ordner verschoben.";
                        case "folder": return "Ordner";
                        case "information": return "Information";
                        case "settings_saved": return "Einstellungen wurden übernommen.";
                        case "settings_restored_to_default": return "Auf Standard zurückgesetzt";
                        case "identifier": return "Kennung";
                        case "documentation": return "Dokumentation";
                        case "windows_client": return "Windows-Client";
                        case "text_about_license": return "GPLv3 Open-Source-Lizenz";
                        case "text_about_creator": return "Erstellt von Bugfish™";
                        case "text_settings_tray_set": return "Beim Schließen in den Tray";
                        case "text_about_updatet": return "Klicken Sie auf 'Aktualisieren', um die Software zu schließen und den Updatevorgang zu starten.";
                        case "text_update_available": return "Ein neues Update für den Suitefish Windows-Client ist verfügbar!";
                        case "list_update_complete": return "Listenaktualisierung abgeschlossen.";
                        case "cannot_download": return "Download während eines laufenden Vorgangs nicht möglich!";
                        case "cannot_delete": return "Löschen während eines laufenden Vorgangs nicht möglich!";
                        case "cannot_start": return "Start während eines laufenden Vorgangs nicht möglich!";
                        case "cannot_update": return "Update während eines laufenden Vorgangs nicht möglich!";
                        case "text_wanna_delete": return "Ausgewählte Produkt löschen?";
                        case "deletion_finished": return "Löschen erfolgreich.";
                        case "text_wanna_update": return "Ausgewählte Produkt aktualisieren?";
                        case "update_finished": return "Update erfolgreich.";
                        case "clearing": return "Wird gelöscht...";
                        case "extract": return "Entpacken";
                        case "move": return "Verschieben";
                        case "files_in_use_product": return "Produkt läuft oder Dateien sind gesperrt.";
                        case "task_running": return "Es läuft eine Aufgabe. Möchten Sie das Programm wirklich schließen?";
                        default: return "ERROR:NOLANG";
                    }
                case "fr":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Serveur store en ligne disponible.";
                        case "llupdate_loading": return "Connexion au store...";
                        case "lupdate_out_lower": return "Version la plus récente.";
                        case "lupdate_out_higher": return "Nouvelle mise à jour dispo!";
                        case "lupdate_out_equal": return "Version la plus récente.";
                        case "lupdate_loading": return "Connexion au serveur de mise à jour...";
                        case "lupdate_server_error": return "Serveur de mise à jour inaccessible!";
                        case "llupdate_server_error": return "Serveur store inaccessible!";
                        case "lupdate_server_ok": return "Serveur mise à jour atteint...";
                        case "llupdate_server_ok": return "Serveur store atteint...";
                        case "maximize": return "Maximiser";
                        case "executable": return "Exécutable";
                        case "please_wait": return "Patientez";
                        case "store": return "Store";
                        case "unknown": return "Inconnu";
                        case "minimize": return "Minimiser";
                        case "download": return "Télécharger";
                        case "refresh": return "Actualiser";
                        case "close": return "Fermer";
                        case "start": return "Démarrer";
                        case "delete": return "Supprimer";
                        case "open_folder": return "Ouvrir dossier";
                        case "exit": return "Quitter";
                        case "error": return "Erreur";
                        case "server_error": return "Erreur serveur";
                        case "server_ok": return "Serveur disponible.";
                        case "server_maintenance": return "Serveur en maintenance.";
                        case "open": return "Ouvrir";
                        case "version": return "Version";
                        case "settings": return "Paramètres";
                        case "library": return "Bibliothèque";
                        case "update": return "Mise à jour";
                        case "library_server": return "Serveur Store";
                        case "update_server": return "Serveur MàJ";
                        case "about": return "À propos";
                        case "website": return "Site";
                        case "github": return "GitHub";
                        case "installed": return "Installé";
                        case "on": return "Activé";
                        case "off": return "Désactivé";
                        case "save": return "Sauvegarder";
                        case "cancel": return "Annuler";
                        case "status": return "Statut";
                        case "op_download_start": return "Téléchargement en cours...";
                        case "op_download_ok": return "Téléchargement fini.";
                        case "op_extract_start": return "Décompression en cours...";
                        case "op_extract_ok": return "Décompression finie.";
                        case "name": return "Nom";
                        case "yes": return "Oui";
                        case "no": return "Non";
                        case "description": return "Description";
                        case "size": return "Taille (Go)";
                        case "latest_version": return "Dernière version";
                        case "author": return "Auteur";
                        case "license": return "Licence";
                        case "revert": return "Réinitialiser";
                        case "restore": return "Restaurer";
                        case "current": return "Actuel";
                        case "language": return "Langue";
                        case "op_move_start": return "Déplacement dossier...";
                        case "op_move_ok": return "Dossier déplacé.";
                        case "folder": return "Dossier";
                        case "information": return "Info";
                        case "settings_saved": return "Paramètres appliqués.";
                        case "settings_restored_to_default": return "Restauré par défaut";
                        case "identifier": return "ID";
                        case "documentation": return "Documentation";
                        case "windows_client": return "Client Windows";
                        case "text_about_license": return "Licence GPLv3";
                        case "text_about_creator": return "Par Bugfish™";
                        case "text_settings_tray_set": return "Fermer dans la barre";
                        case "text_about_updatet": return "Cliquez 'MàJ' pour démarrer.";
                        case "text_update_available": return "Nouvel update dispo!";
                        case "list_update_complete": return "Liste mise à jour.";
                        case "cannot_download": return "Téléchargement impossible (tâche active)!";
                        case "cannot_delete": return "Suppression impossible (tâche active)!";
                        case "cannot_start": return "Démarrage impossible (tâche active)!";
                        case "cannot_update": return "Update impossible (tâche active)!";
                        case "text_wanna_delete": return "Supprimer ce produit?";
                        case "deletion_finished": return "Suppression réussie.";
                        case "text_wanna_update": return "Mettre à jour ce produit?";
                        case "update_finished": return "Update réussi.";
                        case "clearing": return "Nettoyage...";
                        case "extract": return "Extraire";
                        case "move": return "Déplacer";
                        case "files_in_use_product": return "Produit actif ou fichiers verrouillés.";
                        case "task_running": return "Une tâche est en cours. Voulez-vous vraiment fermer le logiciel ?";
                        default: return "ERROR:NOLANG";
                    }
                case "it":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Server store online disponibile.";
                        case "llupdate_loading": return "Connessione allo store...";
                        case "lupdate_out_lower": return "Hai l’ultima versione.";
                        case "lupdate_out_higher": return "Nuovo aggiornamento disponibile!";
                        case "lupdate_out_equal": return "Hai l’ultima versione.";
                        case "lupdate_loading": return "Connessione server aggiornamenti...";
                        case "lupdate_server_error": return "Errore collegamento update server!";
                        case "llupdate_server_error": return "Errore collegamento store server!";
                        case "lupdate_server_ok": return "Server update raggiunto...";
                        case "llupdate_server_ok": return "Server store raggiunto...";
                        case "maximize": return "Ingrandisci";
                        case "executable": return "Eseguibile";
                        case "please_wait": return "Attendere";
                        case "store": return "Store";
                        case "unknown": return "Sconosciuto";
                        case "minimize": return "Riduci";
                        case "download": return "Scarica";
                        case "refresh": return "Aggiorna";
                        case "close": return "Chiudi";
                        case "start": return "Avvia";
                        case "delete": return "Elimina";
                        case "open_folder": return "Apri cartella";
                        case "exit": return "Esci";
                        case "error": return "Errore";
                        case "server_error": return "Errore server";
                        case "server_ok": return "Server disponibile.";
                        case "server_maintenance": return "Server in manutenzione.";
                        case "open": return "Apri";
                        case "version": return "Versione";
                        case "settings": return "Impostazioni";
                        case "library": return "Libreria";
                        case "update": return "Aggiorna";
                        case "library_server": return "Server store";
                        case "update_server": return "Server update";
                        case "about": return "Info";
                        case "website": return "Sito";
                        case "github": return "GitHub";
                        case "installed": return "Installato";
                        case "on": return "Attivo";
                        case "off": return "Disattivo";
                        case "save": return "Salva";
                        case "cancel": return "Annulla";
                        case "status": return "Stato";
                        case "op_download_start": return "Download avviato...";
                        case "op_download_ok": return "Download completato.";
                        case "op_extract_start": return "Decompressione in corso...";
                        case "op_extract_ok": return "Decompressione completata.";
                        case "name": return "Nome";
                        case "yes": return "Sì";
                        case "no": return "No";
                        case "description": return "Descrizione";
                        case "size": return "Dimensione (GB)";
                        case "latest_version": return "Ultima versione";
                        case "author": return "Autore";
                        case "license": return "Licenza";
                        case "revert": return "Ripristina";
                        case "restore": return "Ripristina";
                        case "current": return "Corrente";
                        case "language": return "Lingua";
                        case "op_move_start": return "Spostamento cartella...";
                        case "op_move_ok": return "Cartella spostata.";
                        case "folder": return "Cartella";
                        case "information": return "Info";
                        case "settings_saved": return "Impostazioni applicate.";
                        case "settings_restored_to_default": return "Ripristinato default";
                        case "identifier": return "ID";
                        case "documentation": return "Documentazione";
                        case "windows_client": return "Client Windows";
                        case "text_about_license": return "Licenza GPLv3";
                        case "text_about_creator": return "Creato da Bugfish™";
                        case "text_settings_tray_set": return "Chiudi nel tray";
                        case "text_about_updatet": return "Premi 'Aggiorna' per iniziare.";
                        case "text_update_available": return "Nuovo aggiornamento disponibile!";
                        case "list_update_complete": return "Lista aggiornata.";
                        case "cannot_download": return "Download impossibile (operazione in corso)!";
                        case "cannot_delete": return "Eliminazione impossibile (operazione in corso)!";
                        case "cannot_start": return "Avvio impossibile (operazione in corso)!";
                        case "cannot_update": return "Aggiornamento impossibile (operazione in corso)!";
                        case "text_wanna_delete": return "Eliminare prodotto selezionato?";
                        case "deletion_finished": return "Eliminazione completata.";
                        case "text_wanna_update": return "Aggiornare prodotto selezionato?";
                        case "update_finished": return "Aggiornamento completato.";
                        case "clearing": return "Pulizia in corso...";
                        case "extract": return "Estrai";
                        case "move": return "Sposta";
                        case "files_in_use_product": return "Prodotto avviato o file bloccati.";
                        case "task_running": return "Un'attività è in corso. Vuoi davvero chiudere il software?";
                        default: return "ERROR:NOLANG";
                    }
                case "ja":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "オンラインストアサーバーは利用可能です。";
                        case "llupdate_loading": return "ストアサーバーに接続中...";
                        case "lupdate_out_lower": return "最新のSuitefishクライアントを使用中です。";
                        case "lupdate_out_higher": return "新しい更新があります！";
                        case "lupdate_out_equal": return "最新のSuitefishクライアントを使用中です。";
                        case "lupdate_loading": return "更新サーバーに接続中...";
                        case "lupdate_server_error": return "更新サーバーに接続できません！";
                        case "llupdate_server_error": return "ストアサーバーに接続できません！";
                        case "lupdate_server_ok": return "更新サーバー接続済み、更新チェック中...";
                        case "llupdate_server_ok": return "ストアサーバー接続済み、リスト更新中...";
                        case "maximize": return "最大化";
                        case "executable": return "実行ファイル";
                        case "please_wait": return "お待ちください";
                        case "store": return "ストア";
                        case "unknown": return "不明";
                        case "minimize": return "最小化";
                        case "download": return "ダウンロード";
                        case "refresh": return "更新";
                        case "close": return "閉じる";
                        case "start": return "開始";
                        case "delete": return "削除";
                        case "open_folder": return "フォルダーを開く";
                        case "exit": return "終了";
                        case "error": return "エラー";
                        case "server_error": return "サーバーエラー";
                        case "server_ok": return "サーバーは利用可能です。";
                        case "server_maintenance": return "サーバーはメンテ中です。";
                        case "open": return "開く";
                        case "version": return "バージョン";
                        case "settings": return "設定";
                        case "library": return "ライブラリ";
                        case "update": return "更新";
                        case "library_server": return "ストアサーバー";
                        case "update_server": return "更新サーバー";
                        case "about": return "情報";
                        case "website": return "ウェブサイト";
                        case "github": return "GitHub";
                        case "installed": return "インストール済み";
                        case "on": return "オン";
                        case "off": return "オフ";
                        case "save": return "保存";
                        case "cancel": return "キャンセル";
                        case "status": return "状態";
                        case "op_download_start": return "ダウンロード開始...";
                        case "op_download_ok": return "ダウンロード完了。";
                        case "op_extract_start": return "解凍開始...";
                        case "op_extract_ok": return "解凍完了。";
                        case "name": return "名前";
                        case "yes": return "はい";
                        case "no": return "いいえ";
                        case "description": return "説明";
                        case "size": return "サイズ(GB)";
                        case "latest_version": return "最新版";
                        case "author": return "作成者";
                        case "license": return "ライセンス";
                        case "revert": return "元に戻す";
                        case "restore": return "復元";
                        case "current": return "現在";
                        case "language": return "言語";
                        case "op_move_start": return "フォルダー移動中...";
                        case "op_move_ok": return "移動完了。";
                        case "folder": return "フォルダー";
                        case "information": return "情報";
                        case "settings_saved": return "設定を適用しました。";
                        case "settings_restored_to_default": return "デフォルトに戻しました。";
                        case "identifier": return "ID";
                        case "documentation": return "ドキュメント";
                        case "windows_client": return "Windowsクライアント";
                        case "text_about_license": return "GPLv3 オープンソース";
                        case "text_about_creator": return "Bugfish™製";
                        case "text_settings_tray_set": return "閉じてトレイへ";
                        case "text_about_updatet": return "'更新'をクリックして更新を開始。";
                        case "text_update_available": return "新しい更新があります！";
                        case "list_update_complete": return "リスト更新完了。";
                        case "cannot_download": return "作業中はダウンロード不可！";
                        case "cannot_delete": return "作業中は削除不可！";
                        case "cannot_start": return "作業中は開始不可！";
                        case "cannot_update": return "作業中は更新不可！";
                        case "text_wanna_delete": return "選択した製品を削除しますか？";
                        case "deletion_finished": return "削除成功。";
                        case "text_wanna_update": return "選択した製品を更新しますか？";
                        case "update_finished": return "更新成功。";
                        case "clearing": return "クリア中...";
                        case "extract": return "解凍";
                        case "move": return "移動";
                        case "files_in_use_product": return "製品が実行中、またはファイルがロックされています。";
                        case "task_running": return "タスクが実行中です。本当にソフトを終了しますか？";
                        default: return "ERROR:NOLANG";
                    }
                case "zh":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "在线软件商店服务器在线。";
                        case "llupdate_loading": return "连接软件商店服务器...";
                        case "lupdate_out_lower": return "您使用的是最新版本客户端。";
                        case "lupdate_out_higher": return "有新客户端更新！";
                        case "lupdate_out_equal": return "您使用的是最新版本客户端。";
                        case "lupdate_loading": return "连接更新服务器...";
                        case "lupdate_server_error": return "无法连接更新服务器！";
                        case "llupdate_server_error": return "无法连接软件商店服务器！";
                        case "lupdate_server_ok": return "已连接更新服务器，检查更新...";
                        case "llupdate_server_ok": return "已连接软件商店服务器，刷新列表...";
                        case "maximize": return "最大化";
                        case "executable": return "可执行文件";
                        case "please_wait": return "请稍候";
                        case "store": return "商店";
                        case "unknown": return "未知";
                        case "minimize": return "最小化";
                        case "download": return "下载";
                        case "refresh": return "刷新";
                        case "close": return "关闭";
                        case "start": return "启动";
                        case "delete": return "删除";
                        case "open_folder": return "打开文件夹";
                        case "exit": return "退出";
                        case "error": return "错误";
                        case "server_error": return "服务器错误";
                        case "server_ok": return "服务器可用。";
                        case "server_maintenance": return "服务器维护中。";
                        case "open": return "打开";
                        case "version": return "版本";
                        case "settings": return "设置";
                        case "library": return "库";
                        case "update": return "更新";
                        case "library_server": return "软件商店服务器";
                        case "update_server": return "客户端更新服务器";
                        case "about": return "关于";
                        case "website": return "网站";
                        case "github": return "GitHub";
                        case "installed": return "已安装";
                        case "on": return "开";
                        case "off": return "关";
                        case "save": return "保存";
                        case "cancel": return "取消";
                        case "status": return "状态";
                        case "op_download_start": return "开始下载...";
                        case "op_download_ok": return "下载完成。";
                        case "op_extract_start": return "开始解压...";
                        case "op_extract_ok": return "解压完成。";
                        case "name": return "名称";
                        case "yes": return "是";
                        case "no": return "否";
                        case "description": return "描述";
                        case "size": return "大小(GB)";
                        case "latest_version": return "最新版本";
                        case "author": return "作者";
                        case "license": return "许可";
                        case "revert": return "还原";
                        case "restore": return "恢复";
                        case "current": return "当前";
                        case "language": return "语言";
                        case "op_move_start": return "移动文件夹...";
                        case "op_move_ok": return "文件夹移动完成。";
                        case "folder": return "文件夹";
                        case "information": return "信息";
                        case "settings_saved": return "设置已应用。";
                        case "settings_restored_to_default": return "恢复默认设置。";
                        case "identifier": return "标识符";
                        case "documentation": return "文档";
                        case "windows_client": return "Windows客户端";
                        case "text_about_license": return "GPLv3开源许可";
                        case "text_about_creator": return "由Bugfish™制作";
                        case "text_settings_tray_set": return "关闭到托盘";
                        case "text_about_updatet": return "点击“更新”关闭软件开始更新。";
                        case "text_update_available": return "有新版本客户端更新！";
                        case "list_update_complete": return "列表更新完成。";
                        case "cannot_download": return "任务运行中无法下载！";
                        case "cannot_delete": return "任务运行中无法删除！";
                        case "cannot_start": return "任务运行中无法启动！";
                        case "cannot_update": return "任务运行中无法更新！";
                        case "text_wanna_delete": return "删除选中的产品？";
                        case "deletion_finished": return "删除成功。";
                        case "text_wanna_update": return "更新选中的产品？";
                        case "update_finished": return "更新成功。";
                        case "clearing": return "清理中...";
                        case "extract": return "解压";
                        case "move": return "移动";
                        case "files_in_use_product": return "产品正在运行或文件被锁。";
                        case "task_running": return "任务正在运行。确定要关闭软件吗？";
                        default: return "ERROR:NOLANG";
                    }
                case "tr":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Online mağaza sunucusu açık.";
                        case "llupdate_loading": return "Mağaza sunucusuna bağlanılıyor...";
                        case "lupdate_out_lower": return "Suitefish son sürüm kullanılıyor.";
                        case "lupdate_out_higher": return "Yeni Suitefish güncellemesi var!";
                        case "lupdate_out_equal": return "Suitefish son sürüm kullanılıyor.";
                        case "lupdate_loading": return "Güncelleme sunucusuna bağlanılıyor...";
                        case "lupdate_server_error": return "Güncelleme sunucusuna ulaşılamıyor!";
                        case "llupdate_server_error": return "Mağaza sunucusuna ulaşılamıyor!";
                        case "lupdate_server_ok": return "Güncelleme sunucusuna bağlanıldı, güncellemeler kontrol ediliyor...";
                        case "llupdate_server_ok": return "Mağaza sunucusuna bağlanıldı, liste yenileniyor...";
                        case "maximize": return "Büyüt";
                        case "executable": return "Çalıştırılabilir";
                        case "please_wait": return "Lütfen bekleyin";
                        case "store": return "Mağaza";
                        case "unknown": return "Bilinmiyor";
                        case "minimize": return "Küçült";
                        case "download": return "İndir";
                        case "refresh": return "Yenile";
                        case "close": return "Kapat";
                        case "start": return "Başlat";
                        case "delete": return "Sil";
                        case "open_folder": return "Klasörü aç";
                        case "exit": return "Çıkış";
                        case "error": return "Hata";
                        case "server_error": return "Sunucu hatası";
                        case "server_ok": return "Sunucu kullanıma açık.";
                        case "server_maintenance": return "Sunucu bakımda.";
                        case "open": return "Aç";
                        case "version": return "Sürüm";
                        case "settings": return "Ayarlar";
                        case "library": return "Kütüphane";
                        case "update": return "Güncelle";
                        case "library_server": return "Mağaza Sunucusu";
                        case "update_server": return "Güncelleme Sunucusu";
                        case "about": return "Hakkında";
                        case "website": return "Web sitesi";
                        case "github": return "GitHub";
                        case "installed": return "Yüklendi";
                        case "on": return "Açık";
                        case "off": return "Kapalı";
                        case "save": return "Kaydet";
                        case "cancel": return "İptal";
                        case "status": return "Durum";
                        case "op_download_start": return "İndirme başlıyor...";
                        case "op_download_ok": return "İndirme tamamlandı.";
                        case "op_extract_start": return "Çıkarma başlıyor...";
                        case "op_extract_ok": return "Çıkarma tamamlandı.";
                        case "name": return "Ad";
                        case "yes": return "Evet";
                        case "no": return "Hayır";
                        case "description": return "Açıklama";
                        case "size": return "Boyut (GB)";
                        case "latest_version": return "Son sürüm";
                        case "author": return "Yazar";
                        case "license": return "Lisans";
                        case "revert": return "Geri al";
                        case "restore": return "Geri yükle";
                        case "current": return "Geçerli";
                        case "language": return "Dil";
                        case "op_move_start": return "Klasör taşınıyor...";
                        case "op_move_ok": return "Taşıma tamamlandı.";
                        case "folder": return "Klasör";
                        case "information": return "Bilgi";
                        case "settings_saved": return "Ayarlar uygulandı.";
                        case "settings_restored_to_default": return "Varsayılanlar yükleniyor.";
                        case "identifier": return "Kimlik";
                        case "documentation": return "Dokümantasyon";
                        case "windows_client": return "Windows İstemcisi";
                        case "text_about_license": return "GPLv3 Açık Kaynak";
                        case "text_about_creator": return "Bugfish™ tarafından yapıldı";
                        case "text_settings_tray_set": return "Kapatıldığında simge durumuna küçült";
                        case "text_about_updatet": return "'Güncelle'ye tıklayın ve güncelleme başlasın.";
                        case "text_update_available": return "Yeni güncelleme mevcut!";
                        case "list_update_complete": return "Liste güncellendi.";
                        case "cannot_download": return "Görev çalışırken indirilemez!";
                        case "cannot_delete": return "Görev çalışırken silinemez!";
                        case "cannot_start": return "Görev çalışırken başlatılamaz!";
                        case "cannot_update": return "Görev çalışırken güncellenemez!";
                        case "text_wanna_delete": return "Seçilen ürünü sil?";
                        case "deletion_finished": return "Silme başarılı.";
                        case "text_wanna_update": return "Seçilen ürünü güncelle?";
                        case "update_finished": return "Güncelleme başarılı.";
                        case "clearing": return "Temizleniyor...";
                        case "extract": return "Çıkart";
                        case "move": return "Taşı";
                        case "files_in_use_product": return "Ürün çalışıyor veya dosyalar kilitli.";
                        case "task_running": return "Bir görev çalışıyor. Programı kapatmak istiyor musunuz?";
                        default: return "ERROR:NOLANG";
                    }
                case "ru":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Сервер магазина онлайн доступен.";
                        case "llupdate_loading": return "Соединение с сервером магазина...";
                        case "lupdate_out_lower": return "У вас последняя версия клиента.";
                        case "lupdate_out_higher": return "Доступно новое обновление!";
                        case "lupdate_out_equal": return "У вас последняя версия клиента.";
                        case "lupdate_loading": return "Соединение с сервером обновлений...";
                        case "lupdate_server_error": return "Ошибка подключения к серверу обновлений!";
                        case "llupdate_server_error": return "Ошибка подключения к серверу магазина!";
                        case "lupdate_server_ok": return "Сервер обновлений доступен, проверка обновлений...";
                        case "llupdate_server_ok": return "Сервер магазина доступен, обновление списка...";
                        case "maximize": return "Развернуть";
                        case "executable": return "Исполняемый файл";
                        case "please_wait": return "Пожалуйста, подождите";
                        case "store": return "Магазин";
                        case "unknown": return "Неизвестно";
                        case "minimize": return "Свернуть";
                        case "download": return "Загрузка";
                        case "refresh": return "Обновить";
                        case "close": return "Закрыть";
                        case "start": return "Запустить";
                        case "delete": return "Удалить";
                        case "open_folder": return "Открыть папку";
                        case "exit": return "Выход";
                        case "error": return "Ошибка";
                        case "server_error": return "Ошибка сервера";
                        case "server_ok": return "Сервер доступен.";
                        case "server_maintenance": return "Сервер на обслуживании.";
                        case "open": return "Открыть";
                        case "version": return "Версия";
                        case "settings": return "Настройки";
                        case "library": return "Библиотека";
                        case "update": return "Обновить";
                        case "library_server": return "Сервер магазина";
                        case "update_server": return "Сервер обновлений";
                        case "about": return "О программе";
                        case "website": return "Сайт";
                        case "github": return "GitHub";
                        case "installed": return "Установлено";
                        case "on": return "Вкл";
                        case "off": return "Выкл";
                        case "save": return "Сохранить";
                        case "cancel": return "Отмена";
                        case "status": return "Статус";
                        case "op_download_start": return "Начинается загрузка...";
                        case "op_download_ok": return "Загрузка завершена.";
                        case "op_extract_start": return "Начинается распаковка...";
                        case "op_extract_ok": return "Распаковка завершена.";
                        case "name": return "Имя";
                        case "yes": return "Да";
                        case "no": return "Нет";
                        case "description": return "Описание";
                        case "size": return "Размер (ГБ)";
                        case "latest_version": return "Последняя версия";
                        case "author": return "Автор";
                        case "license": return "Лицензия";
                        case "revert": return "Отменить";
                        case "restore": return "Восстановить";
                        case "current": return "Текущий";
                        case "language": return "Язык";
                        case "op_move_start": return "Перемещение папки...";
                        case "op_move_ok": return "Папка перемещена.";
                        case "folder": return "Папка";
                        case "information": return "Информация";
                        case "settings_saved": return "Настройки применены.";
                        case "settings_restored_to_default": return "Сброшено к умолчанию";
                        case "identifier": return "Идентификатор";
                        case "documentation": return "Документация";
                        case "windows_client": return "Клиент Windows";
                        case "text_about_license": return "Лицензия GPLv3";
                        case "text_about_creator": return "Разработано Bugfish™";
                        case "text_settings_tray_set": return "Закрыть в трей";
                        case "text_about_updatet": return "Нажмите 'Обновить' для запуска процесса.";
                        case "text_update_available": return "Доступно новое обновление!";
                        case "list_update_complete": return "Обновление списка завершено.";
                        case "cannot_download": return "Нельзя скачать, пока идет задача!";
                        case "cannot_delete": return "Нельзя удалить, пока идет задача!";
                        case "cannot_start": return "Нельзя запустить, пока идет задача!";
                        case "cannot_update": return "Нельзя обновить, пока идет задача!";
                        case "text_wanna_delete": return "Удалить выбранный продукт?";
                        case "deletion_finished": return "Удаление успешно.";
                        case "text_wanna_update": return "Обновить выбранный продукт?";
                        case "update_finished": return "Обновление успешно.";
                        case "clearing": return "Очистка...";
                        case "extract": return "Распаковать";
                        case "move": return "Переместить";
                        case "files_in_use_product": return "Продукт запущен или файлы заблокированы.";
                        case "task_running": return "Выполняется задача. Вы действительно хотите закрыть программу?";
                        default: return "ERROR:NOLANG";
                    }
                case "pt":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Servidor da loja online está disponível.";
                        case "llupdate_loading": return "Conectando à loja online...";
                        case "lupdate_out_lower": return "Você tem a versão mais recente.";
                        case "lupdate_out_higher": return "Nova atualização disponível!";
                        case "lupdate_out_equal": return "Você tem a versão mais recente.";
                        case "lupdate_loading": return "Conectando ao servidor de atualização...";
                        case "lupdate_server_error": return "Erro ao alcançar o servidor de atualização!";
                        case "llupdate_server_error": return "Erro ao alcançar o servidor da loja!";
                        case "lupdate_server_ok": return "Servidor de atualização alcançado, verificando atualizações...";
                        case "llupdate_server_ok": return "Servidor da loja alcançado, atualizando lista...";
                        case "maximize": return "Maximizar";
                        case "executable": return "Executável";
                        case "please_wait": return "Por favor, aguarde";
                        case "store": return "Loja";
                        case "unknown": return "Desconhecido";
                        case "minimize": return "Minimizar";
                        case "download": return "Baixar";
                        case "refresh": return "Atualizar";
                        case "close": return "Fechar";
                        case "start": return "Iniciar";
                        case "delete": return "Excluir";
                        case "open_folder": return "Abrir pasta";
                        case "exit": return "Sair";
                        case "error": return "Erro";
                        case "server_error": return "Erro no servidor";
                        case "server_ok": return "Servidor disponível.";
                        case "server_maintenance": return "Servidor em manutenção.";
                        case "open": return "Abrir";
                        case "version": return "Versão";
                        case "settings": return "Configurações";
                        case "library": return "Biblioteca";
                        case "update": return "Atualizar";
                        case "library_server": return "Servidor da loja";
                        case "update_server": return "Servidor de atualização";
                        case "about": return "Sobre";
                        case "website": return "Site";
                        case "github": return "GitHub";
                        case "installed": return "Instalado";
                        case "on": return "Ligado";
                        case "off": return "Desligado";
                        case "save": return "Salvar";
                        case "cancel": return "Cancelar";
                        case "status": return "Status";
                        case "op_download_start": return "Iniciando download...";
                        case "op_download_ok": return "Download concluído.";
                        case "op_extract_start": return "Iniciando extração...";
                        case "op_extract_ok": return "Extração concluída.";
                        case "name": return "Nome";
                        case "yes": return "Sim";
                        case "no": return "Não";
                        case "description": return "Descrição";
                        case "size": return "Tamanho (GB)";
                        case "latest_version": return "Última versão";
                        case "author": return "Autor";
                        case "license": return "Licença";
                        case "revert": return "Reverter";
                        case "restore": return "Restaurar";
                        case "current": return "Atual";
                        case "language": return "Idioma";
                        case "op_move_start": return "Movendo pasta...";
                        case "op_move_ok": return "Pasta movida.";
                        case "folder": return "Pasta";
                        case "information": return "Informação";
                        case "settings_saved": return "Configurações aplicadas.";
                        case "settings_restored_to_default": return "Restaurado para padrão";
                        case "identifier": return "Identificador";
                        case "documentation": return "Documentação";
                        case "windows_client": return "Cliente Windows";
                        case "text_about_license": return "Licença GPLv3";
                        case "text_about_creator": return "Feito por Bugfish™";
                        case "text_settings_tray_set": return "Fechar para bandeja";
                        case "text_about_updatet": return "Clique em 'Atualizar' para iniciar.";
                        case "text_update_available": return "Nova atualização disponível!";
                        case "list_update_complete": return "Lista atualizada.";
                        case "cannot_download": return "Não pode baixar durante tarefa!";
                        case "cannot_delete": return "Não pode excluir durante tarefa!";
                        case "cannot_start": return "Não pode iniciar durante tarefa!";
                        case "cannot_update": return "Não pode atualizar durante tarefa!";
                        case "text_wanna_delete": return "Excluir produto selecionado?";
                        case "deletion_finished": return "Exclusão concluída.";
                        case "text_wanna_update": return "Atualizar produto selecionado?";
                        case "update_finished": return "Atualização concluída.";
                        case "clearing": return "Limpando...";
                        case "extract": return "Extrair";
                        case "move": return "Mover";
                        case "files_in_use_product": return "Produto em uso ou arquivos bloqueados.";
                        case "task_running": return "Há uma tarefa em andamento. Deseja realmente fechar o software?";
                        default: return "ERROR:NOLANG";
                    }
                case "kr":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "온라인 스토어 서버가 연결되었습니다.";
                        case "llupdate_loading": return "스토어 서버에 연결 중...";
                        case "lupdate_out_lower": return "최신 클라이언트를 사용 중입니다.";
                        case "lupdate_out_higher": return "새 업데이트가 있습니다!";
                        case "lupdate_out_equal": return "최신 클라이언트를 사용 중입니다.";
                        case "lupdate_loading": return "업데이트 서버에 연결 중...";
                        case "lupdate_server_error": return "업데이트 서버에 연결할 수 없습니다!";
                        case "llupdate_server_error": return "스토어 서버에 연결할 수 없습니다!";
                        case "lupdate_server_ok": return "업데이트 서버 연결됨, 업데이트 확인 중...";
                        case "llupdate_server_ok": return "스토어 서버 연결됨, 목록 갱신 중...";
                        case "maximize": return "최대화";
                        case "executable": return "실행 파일";
                        case "please_wait": return "잠시 기다려 주세요";
                        case "store": return "스토어";
                        case "unknown": return "알 수 없음";
                        case "minimize": return "최소화";
                        case "download": return "다운로드";
                        case "refresh": return "새로 고침";
                        case "close": return "닫기";
                        case "start": return "시작";
                        case "delete": return "삭제";
                        case "open_folder": return "폴더 열기";
                        case "exit": return "종료";
                        case "error": return "오류";
                        case "server_error": return "서버 오류";
                        case "server_ok": return "서버 이용 가능.";
                        case "server_maintenance": return "서버 점검 중.";
                        case "open": return "열기";
                        case "version": return "버전";
                        case "settings": return "설정";
                        case "library": return "라이브러리";
                        case "update": return "업데이트";
                        case "library_server": return "스토어 서버";
                        case "update_server": return "업데이트 서버";
                        case "about": return "정보";
                        case "website": return "웹사이트";
                        case "github": return "GitHub";
                        case "installed": return "설치됨";
                        case "on": return "켜짐";
                        case "off": return "꺼짐";
                        case "save": return "저장";
                        case "cancel": return "취소";
                        case "status": return "상태";
                        case "op_download_start": return "다운로드 시작...";
                        case "op_download_ok": return "다운로드 완료.";
                        case "op_extract_start": return "압축 해제 시작...";
                        case "op_extract_ok": return "압축 해제 완료.";
                        case "name": return "이름";
                        case "yes": return "예";
                        case "no": return "아니오";
                        case "description": return "설명";
                        case "size": return "크기 (GB)";
                        case "latest_version": return "최신 버전";
                        case "author": return "제작자";
                        case "license": return "라이선스";
                        case "revert": return "되돌리기";
                        case "restore": return "복원";
                        case "current": return "현재";
                        case "language": return "언어";
                        case "op_move_start": return "폴더 이동 중...";
                        case "op_move_ok": return "폴더 이동 완료.";
                        case "folder": return "폴더";
                        case "information": return "정보";
                        case "settings_saved": return "설정 저장됨.";
                        case "settings_restored_to_default": return "기본값으로 복원됨";
                        case "identifier": return "식별자";
                        case "documentation": return "문서";
                        case "windows_client": return "윈도우 클라이언트";
                        case "text_about_license": return "GPLv3 오픈소스";
                        case "text_about_creator": return "Bugfish™ 제작";
                        case "text_settings_tray_set": return "닫을 때 트레이로";
                        case "text_about_updatet": return "'업데이트'를 눌러 업데이트를 시작하세요.";
                        case "text_update_available": return "새 업데이트가 있습니다!";
                        case "list_update_complete": return "목록 업데이트 완료.";
                        case "cannot_download": return "작업 중에는 다운로드 불가!";
                        case "cannot_delete": return "작업 중에는 삭제 불가!";
                        case "cannot_start": return "작업 중에는 시작 불가!";
                        case "cannot_update": return "작업 중에는 업데이트 불가!";
                        case "text_wanna_delete": return "선택한 제품을 삭제하시겠습니까?";
                        case "deletion_finished": return "삭제 완료.";
                        case "text_wanna_update": return "선택한 제품을 업데이트하시겠습니까?";
                        case "update_finished": return "업데이트 완료.";
                        case "clearing": return "정리 중...";
                        case "extract": return "압축 해제";
                        case "move": return "이동";
                        case "files_in_use_product": return "제품 실행 중이거나 파일이 잠겨 있습니다.";
                        case "task_running": return "작업이 진행 중입니다. 정말로 종료하시겠습니까?";
                        default: return "ERROR:NOLANG";
                    }
                case "in":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "ऑनलाइन स्टोर सर्वर उपलब्ध है।";
                        case "llupdate_loading": return "स्टोर सर्वर से कनेक्ट हो रहा है...";
                        case "lupdate_out_lower": return "आपके पास नवीनतम संस्करण है।";
                        case "lupdate_out_higher": return "नया अपडेट उपलब्ध है!";
                        case "lupdate_out_equal": return "आपके पास नवीनतम संस्करण है।";
                        case "lupdate_loading": return "अपडेट सर्वर से कनेक्ट हो रहा है...";
                        case "lupdate_server_error": return "अपडेट सर्वर से संपर्क त्रुटि!";
                        case "llupdate_server_error": return "स्टोर सर्वर से संपर्क त्रुटि!";
                        case "lupdate_server_ok": return "अपडेट सर्वर कनेक्ट, अपडेट जांच रहा है...";
                        case "llupdate_server_ok": return "स्टोर सर्वर कनेक्ट, सूची अपडेट कर रहा है...";
                        case "maximize": return "मैक्सिमाइज़";
                        case "executable": return "कार्यकारी फ़ाइल";
                        case "please_wait": return "कृपया प्रतीक्षा करें";
                        case "store": return "स्टोर";
                        case "unknown": return "अज्ञात";
                        case "minimize": return "मिनिमाइज़";
                        case "download": return "डाउनलोड";
                        case "refresh": return "रिफ्रेश";
                        case "close": return "बंद करें";
                        case "start": return "शुरू करें";
                        case "delete": return "हटाएँ";
                        case "open_folder": return "फ़ोल्डर खोलें";
                        case "exit": return "बाहर जाएं";
                        case "error": return "त्रुटि";
                        case "server_error": return "सर्वर त्रुटि";
                        case "server_ok": return "सर्वर उपलब्ध है।";
                        case "server_maintenance": return "सर्वर मेंटेनेंस में है।";
                        case "open": return "खोलें";
                        case "version": return "संस्करण";
                        case "settings": return "सेटिंग्स";
                        case "library": return "लाइब्रेरी";
                        case "update": return "अपडेट";
                        case "library_server": return "स्टोर सर्वर";
                        case "update_server": return "अपडेट सर्वर";
                        case "about": return "जानकारी";
                        case "website": return "वेबसाइट";
                        case "github": return "GitHub";
                        case "installed": return "इंस्टॉल्ड";
                        case "on": return "चालू";
                        case "off": return "बंद";
                        case "save": return "सेव करें";
                        case "cancel": return "रद्द करें";
                        case "status": return "स्थिति";
                        case "op_download_start": return "डाउनलोड शुरू हो रहा है...";
                        case "op_download_ok": return "डाउनलोड पूरा।";
                        case "op_extract_start": return "एक्सट्रैक्शन शुरू...";
                        case "op_extract_ok": return "एक्सट्रैक्शन पूरा।";
                        case "name": return "नाम";
                        case "yes": return "हाँ";
                        case "no": return "नहीं";
                        case "description": return "विवरण";
                        case "size": return "आकार (GB)";
                        case "latest_version": return "नवीनतम संस्करण";
                        case "author": return "लेखक";
                        case "license": return "लाइसेंस";
                        case "revert": return "पूर्ववत";
                        case "restore": return "पुनर्स्थापित करें";
                        case "current": return "वर्तमान";
                        case "language": return "भाषा";
                        case "op_move_start": return "फ़ोल्डर स्थानांतरित कर रहा है...";
                        case "op_move_ok": return "फ़ोल्डर स्थानांतरित हो गया।";
                        case "folder": return "फ़ोल्डर";
                        case "information": return "जानकारी";
                        case "settings_saved": return "सेटिंग्स लागू की गईं।";
                        case "settings_restored_to_default": return "डिफ़ॉल्ट पर पुनर्स्थापित।";
                        case "identifier": return "पहचान";
                        case "documentation": return "डॉक्यूमेंटेशन";
                        case "windows_client": return "विंडोज़ क्लाइंट";
                        case "text_about_license": return "GPLv3 ओपन सोर्स लाइसेंस";
                        case "text_about_creator": return "Bugfish™ द्वारा निर्मित";
                        case "text_settings_tray_set": return "बंद करते समय ट्रे में भेजें";
                        case "text_about_updatet": return "'अपडेट' पर क्लिक करें और प्रक्रिया शुरू करें।";
                        case "text_update_available": return "नया अपडेट उपलब्ध है!";
                        case "list_update_complete": return "सूची अपडेट पूरी!";
                        case "cannot_download": return "कार्य चल रहा है, डाउनलोड नहीं कर सकते!";
                        case "cannot_delete": return "कार्य चल रहा है, हटाया नहीं जा सकता!";
                        case "cannot_start": return "कार्य चल रहा है, शुरू नहीं किया जा सकता!";
                        case "cannot_update": return "कार्य चल रहा है, अपडेट नहीं कर सकते!";
                        case "text_wanna_delete": return "चयनित उत्पाद हटाएं?";
                        case "deletion_finished": return "हटाना सफल।";
                        case "text_wanna_update": return "चयनित उत्पाद अपडेट करें?";
                        case "update_finished": return "अपडेट सफल।";
                        case "clearing": return "साफ़ कर रहा हूँ...";
                        case "extract": return "निकालें";
                        case "move": return "स्थानांतरित करें";
                        case "files_in_use_product": return "उत्पाद चल रहा है या फ़ाइलें लॉक हैं।";
                        case "task_running": return "एक कार्य चल रहा है। क्या आप सच में सॉफ्टवेयर बंद करना चाहते हैं?";
                        default: return "ERROR:NOLANG";
                    }
                case "es":
                    switch (key_to_translate)
                    {
                        case "llupdate_ok": return "Servidor de tienda en línea disponible.";
                        case "llupdate_loading": return "Conectando al servidor de tienda...";
                        case "lupdate_out_lower": return "Tienes la versión más reciente.";
                        case "lupdate_out_higher": return "¡Nueva actualización disponible!";
                        case "lupdate_out_equal": return "Tienes la versión más reciente.";
                        case "lupdate_loading": return "Conectando al servidor de actualización...";
                        case "lupdate_server_error": return "Error al acceder al servidor de actualización!";
                        case "llupdate_server_error": return "Error al acceder al servidor de tienda!";
                        case "lupdate_server_ok": return "Servidor de actualización alcanzado, verificando...";
                        case "llupdate_server_ok": return "Servidor de tienda alcanzado, actualizando lista...";
                        case "maximize": return "Maximizar";
                        case "executable": return "Ejecutable";
                        case "please_wait": return "Por favor espera";
                        case "store": return "Tienda";
                        case "unknown": return "Desconocido";
                        case "minimize": return "Minimizar";
                        case "download": return "Descargar";
                        case "refresh": return "Actualizar";
                        case "close": return "Cerrar";
                        case "start": return "Iniciar";
                        case "delete": return "Eliminar";
                        case "open_folder": return "Abrir carpeta";
                        case "exit": return "Salir";
                        case "error": return "Error";
                        case "server_error": return "Error del servidor";
                        case "server_ok": return "Servidor disponible.";
                        case "server_maintenance": return "Servidor en mantenimiento.";
                        case "open": return "Abrir";
                        case "version": return "Versión";
                        case "settings": return "Configuración";
                        case "library": return "Biblioteca";
                        case "update": return "Actualizar";
                        case "library_server": return "Servidor de tienda";
                        case "update_server": return "Servidor de actualización";
                        case "about": return "Acerca de";
                        case "website": return "Sitio web";
                        case "github": return "GitHub";
                        case "installed": return "Instalado";
                        case "on": return "Encendido";
                        case "off": return "Apagado";
                        case "save": return "Guardar";
                        case "cancel": return "Cancelar";
                        case "status": return "Estado";
                        case "op_download_start": return "Descarga iniciando...";
                        case "op_download_ok": return "Descarga terminada.";
                        case "op_extract_start": return "Descomprimiendo...";
                        case "op_extract_ok": return "Descompresión terminada.";
                        case "name": return "Nombre";
                        case "yes": return "Sí";
                        case "no": return "No";
                        case "description": return "Descripción";
                        case "size": return "Tamaño (GB)";
                        case "latest_version": return "Última versión";
                        case "author": return "Autor";
                        case "license": return "Licencia";
                        case "revert": return "Revertir";
                        case "restore": return "Restaurar";
                        case "current": return "Actual";
                        case "language": return "Idioma";
                        case "op_move_start": return "Moviendo carpeta...";
                        case "op_move_ok": return "Carpeta movida.";
                        case "folder": return "Carpeta";
                        case "information": return "Información";
                        case "settings_saved": return "Configuración aplicada.";
                        case "settings_restored_to_default": return "Restaurado por defecto.";
                        case "identifier": return "Identificador";
                        case "documentation": return "Documentación";
                        case "windows_client": return "Cliente Windows";
                        case "text_about_license": return "Licencia GPLv3";
                        case "text_about_creator": return "Hecho por Bugfish™";
                        case "text_settings_tray_set": return "Cerrar en bandeja";
                        case "text_about_updatet": return "Haz clic en 'Actualizar' para comenzar.";
                        case "text_update_available": return "Nueva actualización disponible!";
                        case "list_update_complete": return "Lista actualizada.";
                        case "cannot_download": return "No se puede descargar durante una tarea!";
                        case "cannot_delete": return "No se puede eliminar durante una tarea!";
                        case "cannot_start": return "No se puede iniciar durante una tarea!";
                        case "cannot_update": return "No se puede actualizar durante una tarea!";
                        case "text_wanna_delete": return "¿Eliminar producto seleccionado?";
                        case "deletion_finished": return "Eliminación exitosa.";
                        case "text_wanna_update": return "¿Actualizar producto seleccionado?";
                        case "update_finished": return "Actualización exitosa.";
                        case "clearing": return "Limpiando...";
                        case "extract": return "Extraer";
                        case "move": return "Mover";
                        case "files_in_use_product": return "Producto en uso o archivos bloqueados.";
                        case "task_running": return "Hay una tarea en ejecución. ¿Realmente deseas cerrar el software?";
                        default: return "ERROR:NOLANG";
                    }
                default: return "ERROR:NOLANG";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Start a Software Button
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async void lib_sw_start(ListViewItem item)
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "";
            string folderPath = "";

            string foldervalue = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
            string xecvalue = item.SubItems.Count > 8 ? item.SubItems[8].Text : string.Empty;
            folderName = "sf-apps\\" + foldervalue;
            string finalfolder = Path.Combine(exeDirectory, folderName);
            string finalized = finalfolder + "\\" + xecvalue;

            var procInfo = new ProcessStartInfo();
            procInfo.FileName = finalized;
            procInfo.WorkingDirectory = finalfolder;
            procInfo.UseShellExecute = true;      // if you need elevation, set Verb = "runas" here
                                                  // procInfo.Verb = "runas"; // if needed for admin rights

            try
            {
                Process.Start(procInfo);
            }
            catch (Exception ex)
            {
                if (SF_DEBUG_CR) { 
                    Console.WriteLine("Error starting process: " + ex.Message);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Library List Functionalities
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void listLibrary_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var info = listLibrary.HitTest(e.Location);
                if (info.Item != null)
                {
                    // Select the clicked item
                    listLibrary.SelectedItems.Clear();
                    info.Item.Selected = true;
                    // Store clicked item for menu handlers
                    listLibrary.Tag = info.Item;
                    if (listLibrary.Tag is ListViewItem itemxc)
                    {
                        ////////////////////////////////////////////////////////////
                        // Folder Creation Preparation
                        ////////////////////////////////////////////////////////////
                        string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        string folderName = "";
                        string folderPath = "";

                        ////////////////////////////////////////////////////////////
                        // Create Temp folder for Updater and Downloads
                        ////////////////////////////////////////////////////////////
                        folderName = "sf-apps";
                        folderPath = Path.Combine(exeDirectory, folderName);
                        string ccurfolder = itemxc.SubItems.Count > 5 ? itemxc.SubItems[5].Text : string.Empty;
                        folderPath = Path.Combine(folderPath, ccurfolder);
                        if (!Directory.Exists(folderPath))
                        {
                            if (SF_DEBUG_CR)
                            {
                                Console.WriteLine(folderPath);
                            }
                            reload_local_library();
                            return;
                        }
                    }
                    if (!op_running)
                    {
                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();
                        ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("start"));
                        action1.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_start(item);
                        };
                        cms.Items.Add(action1);

                        bool use_value_now = true;
                        string tmpvalue;
                        string current_rname = "";
                        string current_version = "";
                        string current_lversion = "";
                        if (listLibrary.Tag is ListViewItem itemxcg)
                        {
                            use_value_now = true;
                            tmpvalue = "";
                            current_rname = itemxcg.Text;
                            current_version = itemxcg.SubItems.Count > 3 ? itemxcg.SubItems[3].Text : string.Empty;
                            current_lversion = itemxcg.SubItems.Count > 4 ? itemxcg.SubItems[4].Text : string.Empty;
                        }
                        string yourVersionString = current_version;  // The version string you have to compare
                        Version yourVersion;
                        bool yourVersionParsed = Version.TryParse(yourVersionString, out yourVersion);
                        string modVersionString = current_lversion ?? "";

                        if (Version.TryParse(modVersionString, out Version modVersion))
                        {
                            if (yourVersionParsed)
                            {
                                int comparison = modVersion.CompareTo(yourVersion);
                                if (comparison == 0)
                                {
                                }
                                else if (comparison < 0)
                                {
                                }
                                else // comparison > 0
                                {
                                    use_value_now = false;
                                }
                            }
                        }

                        ToolStripMenuItem action2 = new ToolStripMenuItem(translateKey("update"));
                        action2.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_update(item);
                        };
                        if (use_value_now) { action2.Enabled = false; }
                        cms.Items.Add(action2);

                        ToolStripMenuItem action3 = new ToolStripMenuItem(translateKey("open_folder"));
                        action3.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        cms.Items.Add(action3);

                        ToolStripMenuItem action4 = new ToolStripMenuItem(translateKey("delete"));
                        action4.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_delete(item);
                        };
                        cms.Items.Add(action4);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listLibrary, e.Location);
                    }
                    else
                    {
                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();

                        string curfolder = "";
                        string start_text = translateKey("start");
                        if (listLibrary.Tag is ListViewItem item)
                        {
                            curfolder = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
                        }
                        if (op_folder.Equals(curfolder))
                        {
                            start_text = translateKey("cannot_start");
                        }
                        ToolStripMenuItem action1 = new ToolStripMenuItem(start_text);
                        action1.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_start(item);
                        };
                        if (op_folder.Equals(curfolder)) { action1.Enabled = false; }
                        cms.Items.Add(action1);

                        ToolStripMenuItem action2 = new ToolStripMenuItem(translateKey("cannot_update"));
                        action2.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        action2.Enabled = false;
                        cms.Items.Add(action2);

                        ToolStripMenuItem action3 = new ToolStripMenuItem(translateKey("open_folder"));
                        action3.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        cms.Items.Add(action3);

                        ToolStripMenuItem action4 = new ToolStripMenuItem(translateKey("cannot_delete"));
                        action4.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_delete(item);
                        };
                        action4.Enabled = false;
                        cms.Items.Add(action4);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listLibrary, e.Location);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////
        // List Library Context
        ////////////////////////////////////////////////////////////
        private void listLibrary_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var info = listLibrary.HitTest(e.Location);
                if (info.Item != null)
                {
                    // Select the clicked item
                    listLibrary.SelectedItems.Clear();
                    info.Item.Selected = true;
                    // Store clicked item for menu handlers
                    listLibrary.Tag = info.Item;
                    if (listLibrary.Tag is ListViewItem itemxc)
                    {
                        ////////////////////////////////////////////////////////////
                        // Folder Creation Preparation
                        ////////////////////////////////////////////////////////////
                        string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        string folderName = "";
                        string folderPath = "";

                        ////////////////////////////////////////////////////////////
                        // Create Temp folder for Updater and Downloads
                        ////////////////////////////////////////////////////////////
                        folderName = "sf-apps";
                        folderPath = Path.Combine(exeDirectory, folderName);
                        string ccurfolder = itemxc.SubItems.Count > 5 ? itemxc.SubItems[5].Text : string.Empty;
                        folderPath = Path.Combine(folderPath, ccurfolder);
                        if (!Directory.Exists(folderPath))
                        {
                            if (SF_DEBUG_CR)
                            {
                                Console.WriteLine(folderPath);
                            }
                            reload_local_library();
                            return;
                        }
                    }
                    if (!op_running)
                    {
                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();
                        ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("start"));
                        action1.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_start(item);
                        };
                        cms.Items.Add(action1);

                        bool use_value_now = true;
                        string tmpvalue;
                        string current_rname = "";
                        string current_version = "";
                        string current_lversion = "";
                        if (listLibrary.Tag is ListViewItem itemxcg)
                        {
                            use_value_now = true;
                            tmpvalue = "";
                            current_rname = itemxcg.Text;
                            current_version = itemxcg.SubItems.Count > 3 ? itemxcg.SubItems[3].Text : string.Empty;
                            current_lversion = itemxcg.SubItems.Count > 4 ? itemxcg.SubItems[4].Text : string.Empty;
                        }
                        string yourVersionString = current_version;  // The version string you have to compare
                        Version yourVersion;
                        bool yourVersionParsed = Version.TryParse(yourVersionString, out yourVersion);
                        string modVersionString = current_lversion ?? "";

                        if (Version.TryParse(modVersionString, out Version modVersion))
                        {
                            if (yourVersionParsed)
                            {
                                int comparison = modVersion.CompareTo(yourVersion);
                                if (comparison == 0)
                                {
                                }
                                else if (comparison < 0)
                                {
                                }
                                else // comparison > 0
                                {
                                    use_value_now = false;
                                }
                            }
                        }

                        ToolStripMenuItem action2 = new ToolStripMenuItem(translateKey("update"));
                        action2.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_update(item);
                        };
                        if (use_value_now) { action2.Enabled = false; }
                        cms.Items.Add(action2);

                        ToolStripMenuItem action3 = new ToolStripMenuItem(translateKey("open_folder"));
                        action3.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        cms.Items.Add(action3);

                        ToolStripMenuItem action4 = new ToolStripMenuItem(translateKey("delete"));
                        action4.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_delete(item);
                        };
                        cms.Items.Add(action4);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listLibrary, e.Location);
                    }
                    else
                    {
                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();

                        string curfolder = "";
                        string start_text = translateKey("start");
                        if (listLibrary.Tag is ListViewItem item)
                        {
                            curfolder = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
                        }
                        if (op_folder.Equals(curfolder))
                        {
                            start_text = translateKey("cannot_start");
                        }
                        ToolStripMenuItem action1 = new ToolStripMenuItem(start_text);
                        action1.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_start(item);
                        };
                        if (op_folder.Equals(curfolder)) { action1.Enabled = false; }
                        cms.Items.Add(action1);

                        ToolStripMenuItem action2 = new ToolStripMenuItem(translateKey("cannot_update"));
                        action2.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        action2.Enabled = false;
                        cms.Items.Add(action2);

                        ToolStripMenuItem action3 = new ToolStripMenuItem(translateKey("open_folder"));
                        action3.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_open_folder(item);
                        };
                        cms.Items.Add(action3);

                        ToolStripMenuItem action4 = new ToolStripMenuItem(translateKey("cannot_delete"));
                        action4.Click += (s, ev) =>
                        {
                            if (listLibrary.Tag is ListViewItem item)
                                lib_sw_delete(item);
                        };
                        action4.Enabled = false;
                        cms.Items.Add(action4);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listLibrary, e.Location);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Open Folder for Page
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void lib_open_folder(ListViewItem item)
        {
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "sf-apps\\";
            string versionvalue = item.SubItems.Count > 5 ? item.SubItems[5].Text : string.Empty;
            string finalfolder = Path.Combine(exeDirectory, folderName + versionvalue);

            if (SF_DEBUG_CR)
            {
                Console.WriteLine("finalfolder for open folder is: " + finalfolder);
            }

            System.Diagnostics.Process.Start("explorer.exe", @finalfolder);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Mouse Double Click Event on Store Item
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void listStore_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var info = listStore.HitTest(e.Location);
                if (info.Item != null)
                {
                    if (!op_running)
                    {
                        // Select the clicked item
                        listStore.SelectedItems.Clear();
                        info.Item.Selected = true;

                        // Store clicked item for menu handlers
                        listStore.Tag = info.Item;

                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();

                        ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("download"));
                        action1.Click += (s, ev) =>
                        {
                            if (listStore.Tag is ListViewItem item)
                                store_download_package(item);
                        };
                        cms.Items.Add(action1);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listStore, e.Location);
                    }
                    else
                    {
                        {
                            // Select the clicked item
                            listStore.SelectedItems.Clear();
                            info.Item.Selected = true;

                            // Store clicked item for menu handlers
                            listStore.Tag = info.Item;

                            // Create context menu dynamically
                            ContextMenuStrip cms = new ContextMenuStrip();

                            ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("cannot_download"));
                            action1.Click += (s, ev) =>
                            {
                                if (listStore.Tag is ListViewItem item)
                                    store_download_package(item);
                            };
                            action1.Enabled = false;
                            cms.Items.Add(action1);

                            // Show the context menu at mouse location relative to listStore
                            cms.Show(listStore, e.Location);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Progress Bar Value Updater
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateProgress(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int>(UpdateProgress), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Move a File
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SF_Move(string sourcePath, string destFolder)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.Text = translateKey("op_move_start");
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                string destPath = Path.Combine(destFolder, Path.GetFileName(sourcePath));

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(destPath))
                    {
                        progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                        labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                        labelTask2.Text = translateKey("error");
                        UpdateProgress(100);
                        return false;
                    }
                    Directory.Move(sourcePath, destPath);
                }
                else
                {
                    progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                    labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                    labelTask2.Text = translateKey("error");
                    UpdateProgress(100);
                    return false;
                }
                progressBar1.ForeColor = Color.Lime;
                labelTask2.ForeColor = Color.Lime;
                UpdateProgress(100);
                labelTask2.Text = translateKey("op_move_ok");
                return true;
            }
            catch (Exception ex)
            {
                progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.Text = translateKey("error") + ": " + ex.Message;
                UpdateProgress(100);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Decompress a File
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SF_Extract(string zipPath, string extractPath)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 101;
            progressBar1.Value = 0;
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.Text = translateKey("op_extract_start");
            try
            {
                if (!System.IO.File.Exists(zipPath))
                {
                    progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                    labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                    labelServer.Text = translateKey("error");
                    return false;
                }
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }
                else
                {
                    Directory.Delete(extractPath, true);
                }
                // Get total number of entries for progress
                int totalEntries;
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    totalEntries = archive.Entries.Count;
                }

                progressBar1.Minimum = 0;
                progressBar1.Maximum = totalEntries;
                progressBar1.Value = 0;

                // Extract each entry and update progress
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        // Skip directories
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        string destPath = Path.Combine(extractPath, entry.FullName);
                        string destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir))
                            Directory.CreateDirectory(destDir);

                        entry.ExtractToFile(destPath, overwrite: true);

                        // Update progress (thread-safe)
                        UpdateProgress(progressBar1.Value + 1);
                    }
                }
                progressBar1.ForeColor = Color.Lime;
                labelTask2.ForeColor = Color.Lime;
                UpdateProgress(totalEntries);
                labelTask2.Text = translateKey("op_extract_ok");
                return true;
            }
            catch (Exception ex)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.Text = translateKey("error") + ": " + ex.Message;
                UpdateProgress(100);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Download A File
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> SF_DownloadFile(string fileUrl, string localSavePath)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
            labelTask2.Text = translateKey("op_download_start");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                            labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                            labelTask2.Text = translateKey("error") + ": " + $"{response.StatusCode}";
                            UpdateProgress(100);
                            return false;
                        }
                        long? totalBytes = response.Content.Headers.ContentLength;
                        long totalRead = 0;
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(localSavePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalRead += bytesRead;

                                if (totalBytes.HasValue)
                                {
                                    int progress = (int)((double)totalRead / totalBytes.Value * 100);
                                    UpdateProgress(progress);
                                }
                            }
                        }
                        progressBar1.ForeColor = Color.Lime;
                        labelTask2.ForeColor = Color.Lime;
                        labelTask2.Text = translateKey("op_download_ok");
                        UpdateProgress(100);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                progressBar1.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.ForeColor = Color.FromArgb(255, 128, 0);
                labelTask2.Text = translateKey("error") + ": " + ex.Message;
                UpdateProgress(100);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////
        // Store Mouseup Event
        ////////////////////////////////////////////////////////////
        private void listStore_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var info = listStore.HitTest(e.Location);
                if (info.Item != null)
                {
                    if (!op_running)
                    {
                        // Select the clicked item
                        listStore.SelectedItems.Clear();
                        info.Item.Selected = true;

                        // Store clicked item for menu handlers
                        listStore.Tag = info.Item;

                        // Create context menu dynamically
                        ContextMenuStrip cms = new ContextMenuStrip();

                        ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("download"));
                        action1.Click += (s, ev) =>
                        {
                            if (listStore.Tag is ListViewItem item)
                                store_download_package(item);
                        };
                        cms.Items.Add(action1);

                        // Show the context menu at mouse location relative to listStore
                        cms.Show(listStore, e.Location);
                    }
                    else
                    {
                        {
                            // Select the clicked item
                            listStore.SelectedItems.Clear();
                            info.Item.Selected = true;

                            // Store clicked item for menu handlers
                            listStore.Tag = info.Item;

                            // Create context menu dynamically
                            ContextMenuStrip cms = new ContextMenuStrip();

                            ToolStripMenuItem action1 = new ToolStripMenuItem(translateKey("cannot_download"));
                            action1.Click += (s, ev) =>
                            {
                                if (listStore.Tag is ListViewItem item)
                                    store_download_package(item);
                            };
                            action1.Enabled = false;
                            cms.Items.Add(action1);

                            // Show the context menu at mouse location relative to listStore
                            cms.Show(listStore, e.Location);
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////
        // Timer Function Store and Software Updates and every 600 Seconds
        ////////////////////////////////////////////////////////////
        private async void reload_external_library()
        {
            listStore.Items.Clear();
            // Get Current Software Version and Update UI if Update is available!
            bool updateAvailable = false;

            labelUpdate.Text = "";
            labelUpdate.Visible = true;
            labelUpdate.ForeColor = Color.LightBlue;
            labelUpdate.Text = translateKey("lupdate_loading");
            JsonElement[] json_software = await SF_GetJsonUpdate(setting_server_update + "/_api/v1.php?api_action=store_software_latest");

            if (SF_DEBUG_CR)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Suitefish Client Update Fetched Information!");
                Console.WriteLine("-----------------------------------------------");
                foreach (var item in json_software)
                {
                    // Serialize each JsonElement to a JSON string
                    string jsonString = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(jsonString);
                }
            }

            string yourVersionString = SF_VERSION_CR;  // The version string you have to compare
            Version yourVersion;
            bool yourVersionParsed = Version.TryParse(yourVersionString, out yourVersion);

            foreach (var jsonElement in json_software)
            {
                if (jsonElement.TryGetProperty("mod_version", out JsonElement modVersionElement))
                {
                    string modVersionString = modVersionElement.GetString() ?? "";
                    if (Version.TryParse(modVersionString, out Version modVersion))
                    {
                        if (!yourVersionParsed)
                        {
                            continue;
                        }

                        int comparison = modVersion.CompareTo(yourVersion);
                        if (comparison == 0)
                        {
                            labelUpdate.ForeColor = Color.Gray;
                            labelUpdate.Text = translateKey("lupdate_out_equal");
                            updateAvailable = true;
                        }
                        else if (comparison < 0)
                        {
                            labelUpdate.ForeColor = Color.Gray;
                            labelUpdate.Text = translateKey("lupdate_out_lower");
                            updateAvailable = true;
                        }
                        else // comparison > 0
                        {
                            labelUpdate.ForeColor = Color.Yellow;
                            labelUpdate.Text = translateKey("lupdate_out_higher");
                            updateAvailable = true;
                        }
                    }
                    else
                    {
                        updateAvailable = false;
                    }
                }
                else
                {
                    updateAvailable = false;
                }
            }
            if (!updateAvailable)
            {
                labelUpdate.ForeColor = Color.Lime;
                labelUpdate.Text = translateKey("lupdate_out_lower");
            }

            // Update Online Software List

            labelServer.Text = "";
            labelServer.Visible = true;
            labelServer.ForeColor = Color.LightBlue;
            labelServer.Text = translateKey("llupdate_loading");
            json_software = await SF_GetJson(setting_server_store + "/_api/v1.php?api_action=store_mod_list_sw");

            if (SF_DEBUG_CR)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Suitefish Client Update Fetched Information!");
                Console.WriteLine("-----------------------------------------------");
                foreach (var item in json_software)
                {
                    // Serialize each JsonElement to a JSON string
                    string jsonString = JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(jsonString);
                }
            }

            foreach (var item in json_software)
            {
                labelServer.ForeColor = Color.Lime;
                labelServer.Text = translateKey("llupdate_ok");
                break;
            }

            current_store_list = json_software;

            foreach (var element in json_software)
            {
                if (element.ValueKind == JsonValueKind.Object)
                {
                    ListViewItem item;
                    string modVersion;
                    if (element.TryGetProperty("mod_rname", out JsonElement modVersionElement44))
                    {
                        modVersion = modVersionElement44.GetString() ?? "";
                    }
                    else
                    {
                        modVersion = "xxx";
                    }

                    item = new ListViewItem(modVersion);

                    if (element.TryGetProperty("mod_name", out JsonElement modVersionElement))
                    {
                        modVersion = modVersionElement.GetString() ?? "";
                        item.SubItems.Add(modVersion);
                    }
                    else
                    {
                        item.SubItems.Add(translateKey("unknown"));
                    }
                    if (element.TryGetProperty("mod_description", out JsonElement modVersionElement1))
                    {
                        modVersion = modVersionElement1.GetString() ?? "";
                        item.SubItems.Add(modVersion);
                    }
                    else
                    {
                        item.SubItems.Add(translateKey("unknown"));
                    }

                    if (element.TryGetProperty("mod_version", out JsonElement modVersionElement2))
                    {
                        modVersion = modVersionElement2.GetString() ?? "";
                        item.SubItems.Add(modVersion);
                    }
                    else
                    {
                        item.SubItems.Add(translateKey("unknown"));
                    }

                    if (element.TryGetProperty("mod_author", out JsonElement modVersionElement244))
                    {
                        modVersion = modVersionElement244.GetString() ?? "";
                        item.SubItems.Add(modVersion);
                    }
                    else
                    {
                        item.SubItems.Add(translateKey("unknown"));
                    }

                    if (element.TryGetProperty("mod_license", out JsonElement modVersionElement2432))
                    {
                        modVersion = modVersionElement2432.GetString() ?? "";
                        item.SubItems.Add(modVersion);
                    }
                    else
                    {
                        item.SubItems.Add(translateKey("unknown"));
                    }

                    listStore.Items.Add(item);
                }
            }
            reload_local_library();
        }

        ////////////////////////////////////////////////////////////
        /// Button to view Store
        ////////////////////////////////////////////////////////////
        private void buttonStore_Click(object sender, EventArgs e)
        {
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(255, 128, 0);
            panelWait.BringToFront();
            panelWait.Visible = true;
            panelSelection.Visible = false;
            Panel_About.Visible = false;
            panelStore.Visible = false;
            Panel_Library.Visible = false;
            Panel_Settings.Visible = false;
            // Rename List Column
            listStore.Columns[0].Text = translateKey("identifier");
            listStore.Columns[1].Text = translateKey("name");
            listStore.Columns[2].Text = translateKey("description");
            listStore.Columns[3].Text = translateKey("version");
            listStore.Columns[4].Text = translateKey("author");
            listStore.Columns[5].Text = translateKey("license");
            // Update Library
            listStore.Items.Clear();
            AdjustLastColumnWidth(listStore);
            reload_external_library();
            panelWait.Visible = false;
            panelStore.Visible = true;
            panelStore.BringToFront();
        }

        ////////////////////////////////////////////////////////////
        /// Library Section Button
        ////////////////////////////////////////////////////////////
        private void buttonLibrary_Click(object sender, EventArgs e)
        {
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonLibrary.BackColor = Color.FromArgb(255, 128, 0);
            panelSelection.Visible = false;
            panelWait.Visible = true;
            Panel_About.Visible = false;
            panelStore.Visible = false;
            Panel_Settings.Visible = false;
            Panel_Library.Visible = false;
            panelWait.BringToFront();
            // Rename List Column
            listLibrary.Columns[0].Text = translateKey("identifier");
            listLibrary.Columns[1].Text = translateKey("name");
            listLibrary.Columns[2].Text = translateKey("description");
            listLibrary.Columns[3].Text = translateKey("version");
            listLibrary.Columns[4].Text = translateKey("latest_version");
            listLibrary.Columns[5].Text = translateKey("folder");
            listLibrary.Columns[6].Text = translateKey("author");
            listLibrary.Columns[7].Text = translateKey("license");
            listLibrary.Columns[8].Text = translateKey("executable");
            // Update Library
            listLibrary.Items.Clear();
            AdjustLastColumnWidth(listLibrary);
            reload_local_library();
            panelWait.Visible = false;
            Panel_Library.Visible = true;
            Panel_Library.BringToFront();
        }

        ////////////////////////////////////////////////////////////
        // Timer JFunction Library Update every 60 Seconds
        ////////////////////////////////////////////////////////////
        private void reload_local_library()
        {
            listLibrary.Items.Clear();
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "sf-apps";
            string folderPath = Path.Combine(exeDirectory, folderName);
            string[] folders = Directory.GetDirectories(folderPath);


            if (SF_DEBUG_CR)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Found Local Folders on 60 Second Check!");
                Console.WriteLine("-----------------------------------------------");
                foreach (string folder in folders)
                {
                    Console.WriteLine(folder);
                }
            }

            List<Dictionary<string, string>> modules = new List<Dictionary<string, string>>();

            // Regex to match simple assignments: $x["key"] = "value";
            Regex simpleAssignRegex = new Regex(@"\$x\[\s*""(?<key>[^""]+)""\s*\]\s*=\s*(""|')(?<value>.*?)(""|');", RegexOptions.Compiled);

            // Regex to match concatenation assignments: $x["key"] = "string".$x["otherKey"];
            Regex concatAssignRegex = new Regex(@"\$x\[\s*""(?<key>[^""]+)""\s*\]\s*=\s*(""|')(?<prefix>.*?)(""|')\s*\.\s*\$x\[\s*""(?<refKey>[^""]+)""\s*\];", RegexOptions.Compiled);

            foreach (string folder in folders)
            {
                string versionFile = Path.Combine(folder, "version.php");
                if (System.IO.File.Exists(versionFile))
                {
                    string content = System.IO.File.ReadAllText(versionFile);
                    var moduleInfo = new Dictionary<string, string>();

                    // First, parse all simple assignments
                    foreach (Match m in simpleAssignRegex.Matches(content))
                    {
                        string key = m.Groups["key"].Value;
                        string value = m.Groups["value"].Value;
                        moduleInfo[key] = value;
                    }

                    // Then, parse concatenation assignments and resolve references
                    foreach (Match m in concatAssignRegex.Matches(content))
                    {
                        string key = m.Groups["key"].Value;
                        string prefix = m.Groups["prefix"].Value;
                        string refKey = m.Groups["refKey"].Value;

                        string refValue = moduleInfo.ContainsKey(refKey) ? moduleInfo[refKey] : "";
                        moduleInfo[key] = prefix + refValue;
                    }
                    moduleInfo["folder"] = folder;
                    if (moduleInfo.Count > 0)
                        modules.Add(moduleInfo);
                }
            }

            // Example: Print each module's name and version
            foreach (var module in modules)
            {
                ListViewItem item = new ListViewItem(module.GetValueOrDefault("rname", translateKey("unknown")));
                item.SubItems.Add(module.GetValueOrDefault("name", translateKey("unknown")));
                item.SubItems.Add(module.GetValueOrDefault("description", translateKey("unknown")));
                item.SubItems.Add(module.GetValueOrDefault("version", translateKey("unknown")));
                string use_value_now = translateKey("unknown");
                string someRnameValue = module.GetValueOrDefault("rname", translateKey("unknown"));
                foreach (JsonElement element in current_store_list)
                {
                    if (element.TryGetProperty("mod_rname", out JsonElement rnameProperty))
                    {
                        if (rnameProperty.GetString() == someRnameValue)
                        {
                            if (element.TryGetProperty("mod_version", out JsonElement versionProperty))
                            {
                                use_value_now = versionProperty.GetString();
                                break; // Found the match, exit loop
                            }
                        }
                    }
                }
                item.SubItems.Add(use_value_now);
                item.SubItems.Add(Path.GetFileName(module.GetValueOrDefault("folder", translateKey("unknown"))));
                item.SubItems.Add(Path.GetFileName(module.GetValueOrDefault("author", translateKey("unknown"))));
                item.SubItems.Add(Path.GetFileName(module.GetValueOrDefault("license", translateKey("unknown"))));
                item.SubItems.Add(Path.GetFileName(module.GetValueOrDefault("software_executable", translateKey("unknown")))); 

                string yourVersionString = module.GetValueOrDefault("version", translateKey("unknown"));  // The version string you have to compare
                Version yourVersion;
                bool yourVersionParsed = Version.TryParse(yourVersionString, out yourVersion);
                string modVersionString = use_value_now ?? "";
                bool nowcegafs = true;
                if (Version.TryParse(modVersionString, out Version modVersion))
                {
                    if (yourVersionParsed)
                    {
                        int comparison = modVersion.CompareTo(yourVersion);
                        if (comparison == 0)
                        {
                        }
                        else if (comparison < 0)
                        {
                        }
                        else // comparison > 0
                        {
                            nowcegafs = false;
                        }
                    }
                }
                if (!nowcegafs) { item.BackColor = Color.FromArgb(42,42,42);  }
                listLibrary.Items.Add(item);
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Fix Column Width
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void AdjustLastColumnWidth(System.Windows.Forms.ListView listView)
        {
            if (listView.View != View.Details || listView.Columns.Count == 0)
                return;

            int totalWidth = listView.ClientSize.Width; // total width inside the ListView control

            // Sum of all columns except the last one
            int otherColumnsWidth = 0;
            for (int i = 0; i < listView.Columns.Count - 1; i++)
            {
                otherColumnsWidth += listView.Columns[i].Width;
            }

            int lastColumnWidth = totalWidth - otherColumnsWidth;

            if (lastColumnWidth > 0)
            {
                listView.Columns[listView.Columns.Count - 1].Width = lastColumnWidth;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// JSON Fetch Async Function
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task<JsonElement[]> SF_GetJsonUpdate(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                labelUpdate.ForeColor = Color.Yellow;
                labelUpdate.Text = translateKey("lupdate_server_error") + " [url-format-error]";
                return Array.Empty<JsonElement>();
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 1. Fetch JSON from URL
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        labelUpdate.ForeColor = Color.Yellow;
                        labelUpdate.Text = translateKey("lupdate_server_error") + " [fetch-error]";
                        return Array.Empty<JsonElement>();
                    }

                    string jsonString = await response.Content.ReadAsStringAsync();

                    // 2. Parse JSON
                    JsonDocument jsonDoc;
                    try
                    {
                        jsonDoc = JsonDocument.Parse(jsonString);
                    }
                    catch (JsonException)
                    {
                        labelUpdate.ForeColor = Color.Yellow;
                        labelUpdate.Text = translateKey("lupdate_server_error") + " [json-exception]";
                        return Array.Empty<JsonElement>();
                    }

                    // 3. Handle JSON array or single JSON object
                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        var list = new List<JsonElement>();
                        foreach (var element in jsonDoc.RootElement.EnumerateArray())
                        {
                            list.Add(element);
                        }
                        labelUpdate.ForeColor = Color.Lime;
                        labelUpdate.Text = translateKey("lupdate_server_ok");
                        return list.ToArray();
                    }
                    else if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        // Wrap single object into array
                        labelUpdate.ForeColor = Color.Lime;
                        labelUpdate.Text = translateKey("lupdate_server_ok");
                        return new JsonElement[] { jsonDoc.RootElement };
                    }
                    else
                    {
                        labelUpdate.ForeColor = Color.Yellow;
                        labelUpdate.Text = translateKey("lupdate_server_error") + " [json-error]";
                        return Array.Empty<JsonElement>();
                    }
                }
            }
            catch (Exception ex) when (ex is UriFormatException || ex is HttpRequestException)
            {
                labelUpdate.ForeColor = Color.Yellow;
                labelUpdate.Text = translateKey("lupdate_server_error") + " [url-error]";
                return Array.Empty<JsonElement>();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// JSON Fetch Async Function
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task<JsonElement[]> SF_GetJson(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                labelServer.ForeColor = Color.Yellow;
                labelServer.Text = translateKey("llupdate_server_error") + " [url-format-error]";
                return Array.Empty<JsonElement>();
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 1. Fetch JSON from URL
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        labelServer.ForeColor = Color.Yellow;
                        labelServer.Text = translateKey("llupdate_server_error") + " [fetch-error]";
                        return Array.Empty<JsonElement>();
                    }

                    string jsonString = await response.Content.ReadAsStringAsync();

                    // 2. Parse JSON
                    JsonDocument jsonDoc;
                    try
                    {
                        jsonDoc = JsonDocument.Parse(jsonString);
                    }
                    catch (JsonException)
                    {
                        labelServer.ForeColor = Color.Yellow;
                        labelServer.Text = translateKey("llupdate_server_error") + " [json-exception]";
                        return Array.Empty<JsonElement>();
                    }

                    // 3. Handle JSON array or single JSON object
                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        var list = new List<JsonElement>();
                        foreach (var element in jsonDoc.RootElement.EnumerateArray())
                        {
                            list.Add(element);
                        }
                        labelServer.ForeColor = Color.Lime;
                        labelServer.Text = translateKey("llupdate_server_ok");
                        return list.ToArray();
                    }
                    else if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        // Wrap single object into array
                        labelServer.ForeColor = Color.Lime;
                        labelServer.Text = translateKey("llupdate_server_ok");
                        return new JsonElement[] { jsonDoc.RootElement };
                    }
                    else
                    {
                        labelServer.ForeColor = Color.Yellow;
                        labelServer.Text = translateKey("llupdate_server_error") + " [json-error]";
                        return Array.Empty<JsonElement>();
                    }
                }
            }
            catch (Exception ex) when (ex is UriFormatException || ex is HttpRequestException)
            {
                labelServer.ForeColor = Color.Yellow;
                labelServer.Text = translateKey("llupdate_server_error") + " [url-error]";
                return Array.Empty<JsonElement>();
            }
        }

        ////////////////////////////////////////////////////////////
        /// Settings Section Button
        ////////////////////////////////////////////////////////////
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(255, 128, 0);
            panelWait.BringToFront();
            panelWait.Visible = true;
            panelSelection.Visible = false;
            Panel_About.Visible = false;
            Panel_Settings.Visible = false;
            panelStore.Visible = false;
            Panel_Library.Visible = false;
            labelLang.Text = translateKey("language");
            labelTrayH.Text = translateKey("text_settings_tray_set");
            btnSettingsRevert.Text = translateKey("revert");
            labelPleaseWait.Text = translateKey("please_wait");
            labelUpdateServerH.Text = translateKey("update_server");
            labelLibraryServerH.Text = translateKey("library_server");
            btnSettingsRestore.Text = translateKey("settings_restored_to_default");
            btnSettingsSave.Text = translateKey("save");
            labelLanguage.Text = translateKey("current") + ": " + setting_language;
            labelLibraryServer.Text = translateKey("current") + ": " + setting_server_store;
            labelUpdateServer.Text = translateKey("current") + ": " + setting_server_update;
            radioTrayOn.Text = translateKey("on");
            buttonStore.Text = translateKey("store");
            radioTrayOff.Text = translateKey("off");
            buttonSelectionNo.Text = translateKey("no");
            buttonSelectionNoUp.Text = translateKey("no");
            buttonSelectionYes.Text = translateKey("yes");
            buttonSelectionYesUp.Text = translateKey("yes");
            if (setting_use_tray_on_exit.Equals("1"))
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("on");
                radioTrayOn.Checked = true;
            }
            else
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("off");
                radioTrayOff.Checked = true;
            }
            if (setting_language.Equals("en"))
            {
                radioEnglish.Checked = true;
            }
            if (setting_language.Equals("de"))
            {
                radioGerman.Checked = true;
            }
            if (setting_language.Equals("fr"))
            {
                radioFrench.Checked = true;
            }
            if (setting_language.Equals("es"))
            {
                radioSpain.Checked = true;
            }
            if (setting_language.Equals("it"))
            {
                radioItaly.Checked = true;
            }
            if (setting_language.Equals("in"))
            {
                radioIndi.Checked = true;
            }
            if (setting_language.Equals("ja"))
            {
                radioJap.Checked = true;
            }
            if (setting_language.Equals("kr"))
            {
                radioKorean.Checked = true;
            }
            if (setting_language.Equals("pt"))
            {
                radioPortugese.Checked = true;
            }
            if (setting_language.Equals("ru"))
            {
                radioRuss.Checked = true;
            }
            if (setting_language.Equals("tr"))
            {
                radioTurke.Checked = true;
            }
            if (setting_language.Equals("zh"))
            {
                radioChine.Checked = true;
            }
            inputLibraryServer.Text = setting_server_store;
            inputUpdateServer.Text = setting_server_update;
            panelWait.SendToBack();
            panelWait.Visible = false;
            Panel_Settings.Visible = true;
            Panel_Settings.BringToFront();
        }

        ////////////////////////////////////////////////////////////
        /// About Section Button
        ////////////////////////////////////////////////////////////
        private void buttonAbout_Click(object sender, EventArgs e)
        {
            buttonLibrary.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonStore.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(28, 28, 28);
            buttonSettings.BackColor = Color.FromArgb(28, 28, 28);
            buttonAbout.BackColor = Color.FromArgb(255, 128, 0);
            panelWait.BringToFront();
            panelSelection.Visible = false;
            panelWait.Visible = true;
            Panel_About.Visible = false;
            Panel_Settings.Visible = false;
            panelStore.Visible = false;
            Panel_Library.Visible = false;
            label1.Text = "Suitefish " + translateKey("windows_client");
            label2.Text = translateKey("text_about_license");
            label3.Text = translateKey("text_about_creator");
            label4.Text = translateKey("version") + ": " + SF_VERSION_CR;
            linkLabel1.Text = translateKey("website") + ": " + "https://suitefish.com";
            linkLabel2.Text = translateKey("github") + ": " + "https://github.com/bugfishtm/suitefish-cms";
            linkLabel3.Text = translateKey("documentation") + ": " + "https://bugfishtm.github.io/suitefish-cms/";
            linkLabel1.Cursor = Cursors.Hand;
            linkLabel2.Cursor = Cursors.Hand;
            linkLabel3.Cursor = Cursors.Hand;
            panelWait.SendToBack();
            panelWait.Visible = false;
            Panel_About.Visible = true;
            Panel_About.BringToFront();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Settings Area Work
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Settings Save
        ////////////////////////////////////////////////////////////
        private void btnSettingsSave_Click(object sender, EventArgs e)
        {
            labelSelection.Text = translateKey("settings_saved");
            labelSelection.ForeColor = Color.Lime;
            panelSelection.Visible = true;
            panelSelection.BringToFront();
            Panel_Settings.Visible = false;
            buttonSelectionNo.Visible = false;
            buttonSelectionYes.Visible = false;
            buttonSelectionNoUp.Visible = false;
            buttonSelectionYesUp.Visible = false;

            // Set new Settings for URLS
            SettingSet("setting_server_store", inputLibraryServer.Text);
            setting_server_store = SettingGet("setting_server_store");
            SettingSet("setting_server_update", inputUpdateServer.Text);
            setting_server_update = SettingGet("setting_server_update");
            // Set new Settings for Tray Icon
            if (radioTrayOn.Checked)
            {
                SettingSet("setting_use_tray_on_exit", "1");
                setting_use_tray_on_exit = "1";
            }
            else
            {
                SettingSet("setting_use_tray_on_exit", "0");
                setting_use_tray_on_exit = "0";
            }
            // Set Language Settings

            if (radioEnglish.Checked)
            {
                SettingSet("setting_language", "en");
            }
            if (radioGerman.Checked)
            {
                SettingSet("setting_language", "de");
            }
            if (radioFrench.Checked)
            {
                SettingSet("setting_language", "fr");
            }
            if (radioSpain.Checked)
            {
                SettingSet("setting_language", "es");
            }
            if (radioItaly.Checked)
            {
                SettingSet("setting_language", "it");
            }
            if (radioIndi.Checked)
            {
                SettingSet("setting_language", "in");
            }
            if (radioJap.Checked)
            {
                SettingSet("setting_language", "ja");
            }
            if (radioKorean.Checked)
            {
                SettingSet("setting_language", "kr");
            }
            if (radioPortugese.Checked)
            {
                SettingSet("setting_language", "pt");
            }
            if (radioRuss.Checked)
            {
                SettingSet("setting_language", "ru");
            }
            if (radioTurke.Checked)
            {
                SettingSet("setting_language", "tr");
            }
            if (radioChine.Checked)
            {
                SettingSet("setting_language", "zh");
            }
            setting_language = SettingGet("setting_language");
            // Initial Label ReSetup
            labelUpdate.Text = translateKey("text_update_available");
            //Header_Version.Text = translateKey("version") + ": " + SF_VERSION_CR;
            // Change Button on Top Translation
            buttonAbout.Text = translateKey("about");
            buttonLibrary.Text = translateKey("library");
            buttonSettings.Text = translateKey("settings");
            labelUpdateServerH.Text = translateKey("update_server");
            labelLibraryServerH.Text = translateKey("library_server");
            buttonSelectionNo.Text = translateKey("no");
            buttonSelectionNoUp.Text = translateKey("no");
            buttonSelectionYes.Text = translateKey("yes");
            buttonSelectionYesUp.Text = translateKey("yes");
            /// ReDraw Header Buttons
            btnMaximizeText = translateKey("maximize");
            btnMinimizeText = translateKey("minimize");
            btnCloseText = translateKey("close");
            tooltip_frame.SetToolTip(btnMaximize, btnMaximizeText);
            tooltip_frame.SetToolTip(btnMinimize, btnMinimizeText);
            tooltip_frame.SetToolTip(btnClose, btnCloseText);
            buttonStore.Text = translateKey("store");
            // Recreate Try Strip for language Change
            contextMenuStrip.Items.Clear();
            contextMenuStrip.Items.Add(translateKey("open"), null, (s, e) => { NotifyIcon_EntryShow(); });
            contextMenuStrip.Items.Add(translateKey("maximize"), null, (s, e) => { NotifyIcon_EntryMaximize(); });
            contextMenuStrip.Items.Add(translateKey("exit"), null, (s, e) => { NotifyIcon_EntryExit(s,e); });
            // Revert and Set new Languages
            labelLang.Text = translateKey("language");
            labelPleaseWait.Text = translateKey("please_wait");
            labelTrayH.Text = translateKey("text_settings_tray_set");
            btnSettingsRestore.Text = translateKey("settings_restored_to_default");
            btnSettingsRevert.Text = translateKey("revert");
            btnSettingsSave.Text = translateKey("save");
            labelLanguage.Text = translateKey("current") + ": " + setting_language;
            labelLibraryServer.Text = translateKey("current") + ": " + setting_server_store;
            labelUpdateServer.Text = translateKey("current") + ": " + setting_server_update;
            radioTrayOn.Text = translateKey("on");
            radioTrayOff.Text = translateKey("off");
            if (setting_use_tray_on_exit.Equals("1"))
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("on");
                radioTrayOn.Checked = true;
            }
            else
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("off");
                radioTrayOff.Checked = true;
            }
            if (setting_language.Equals("en"))
            {
                radioEnglish.Checked = true;
            }
            if (setting_language.Equals("de"))
            {
                radioGerman.Checked = true;
            }
            if (setting_language.Equals("fr"))
            {
                radioFrench.Checked = true;
            }
            if (setting_language.Equals("es"))
            {
                radioSpain.Checked = true;
            }
            if (setting_language.Equals("it"))
            {
                radioItaly.Checked = true;
            }
            if (setting_language.Equals("in"))
            {
                radioIndi.Checked = true;
            }
            if (setting_language.Equals("ja"))
            {
                radioJap.Checked = true;
            }
            if (setting_language.Equals("kr"))
            {
                radioKorean.Checked = true;
            }
            if (setting_language.Equals("pt"))
            {
                radioPortugese.Checked = true;
            }
            if (setting_language.Equals("ru"))
            {
                radioRuss.Checked = true;
            }
            if (setting_language.Equals("tr"))
            {
                radioTurke.Checked = true;
            }
            if (setting_language.Equals("zh"))
            {
                radioChine.Checked = true;
            }
            inputLibraryServer.Text = setting_server_store;
            inputUpdateServer.Text = setting_server_update;
        }

        ////////////////////////////////////////////////////////////
        /// Settings Restore
        ////////////////////////////////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            labelRadio.Text = translateKey("current") + ": " + translateKey("off");
            radioTrayOff.Checked = true;
            radioEnglish.Checked = true;
            inputLibraryServer.Text = constant_default_server;
            inputUpdateServer.Text = constant_default_server;
        }

        ////////////////////////////////////////////////////////////
        /// Settings Revert
        ////////////////////////////////////////////////////////////
        private void btnSettingsRevert_Click(object sender, EventArgs e)
        {
            if (setting_use_tray_on_exit.Equals("1"))
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("on");
                radioTrayOn.Checked = true;
            }
            else
            {
                labelRadio.Text = translateKey("current") + ": " + translateKey("off");
                radioTrayOff.Checked = true;
            }
            if (setting_language.Equals("en"))
            {
                radioEnglish.Checked = true;
            }
            if (setting_language.Equals("de"))
            {
                radioGerman.Checked = true;
            }
            if (setting_language.Equals("fr"))
            {
                radioFrench.Checked = true;
            }
            if (setting_language.Equals("es"))
            {
                radioSpain.Checked = true;
            }
            if (setting_language.Equals("it"))
            {
                radioItaly.Checked = true;
            }
            if (setting_language.Equals("in"))
            {
                radioIndi.Checked = true;
            }
            if (setting_language.Equals("ja"))
            {
                radioJap.Checked = true;
            }
            if (setting_language.Equals("kr"))
            {
                radioKorean.Checked = true;
            }
            if (setting_language.Equals("pt"))
            {
                radioPortugese.Checked = true;
            }
            if (setting_language.Equals("ru"))
            {
                radioRuss.Checked = true;
            }
            if (setting_language.Equals("tr"))
            {
                radioTurke.Checked = true;
            }
            if (setting_language.Equals("zh"))
            {
                radioChine.Checked = true;
            }
            inputLibraryServer.Text = setting_server_store;
            inputUpdateServer.Text = setting_server_update;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CustomUI: Closing Overrides
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Close Window to Tray or Close Completely
        ////////////////////////////////////////////////////////////
        private void CustomUI_FormClosing(object sender, FormClosingEventArgs e)
        {  }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Exit Functions
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CustomUI_app_window_exit(object sender, EventArgs e)
        {
            if (setting_use_tray_on_exit.ToString().Equals("1"))
            {
                notifyIcon.Visible = true;
                this.Hide();
            }
            else
            {
                if (!op_running)
                {
                    Application.Exit();
                    return;
                }
                
                string msg = translateKey("task_running");
                string yesText = translateKey("yes");
                string noText = translateKey("no");

                using (var confirm = new ConfirmCloseForm(msg, yesText, noText))
                {
                    var result = confirm.ShowDialog();
                    if (confirm.Proceed)
                    {
                        Application.Exit();
                    }
                }
            }
        }
        private void CustomUI_app_notify_exit(object sender, EventArgs e)
        {
            if (!op_running)
            {
                Application.Exit();
                return;
            }
            string msg = translateKey("task_running");
            string yesText = translateKey("yes");
            string noText = translateKey("no");

            using (var confirm = new ConfirmCloseForm(msg, yesText, noText))
            {
                var result = confirm.ShowDialog();
                if (confirm.Proceed)
                {
                    Application.Exit();
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Async Deletion Functions
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        // Secure Delete Async File Function
        ////////////////////////////////////////////////////////////
        public static Task SecureDeleteFileAsync(string filePath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteFile(filePath, passes));
        }

        ////////////////////////////////////////////////////////////
        // Secure Delete Async Dir Function
        ////////////////////////////////////////////////////////////
        public static Task SecureDeleteDirectoryAsync(string dirPath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteDirectory(dirPath, passes));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Notification Icon (Tray)
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Double Click on Notify Icon
        ////////////////////////////////////////////////////////////
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        ////////////////////////////////////////////////////////////
        /// Exit Functions on tray icon rightclick - Exit
        ////////////////////////////////////////////////////////////
        private void NotifyIcon_EntryExit(object sender, EventArgs e)
        {
            CustomUI_app_notify_exit(sender,e);
        }

        ////////////////////////////////////////////////////////////
        /// Exit Functions on tray icon rightclick - Show
        ////////////////////////////////////////////////////////////
        private void NotifyIcon_EntryShow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        ////////////////////////////////////////////////////////////
        /// Exit Functions on tray icon rightclick - Maximized
        ////////////////////////////////////////////////////////////
        private void NotifyIcon_EntryMaximize()
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            notifyIcon.Visible = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Settings Get/Set Functions
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        // Set or update a setting by shortcode
        ////////////////////////////////////////////////////////////
        private void SettingSet(string shortcode, string value)
        {
            string query = @"
                  INSERT INTO Settings (Shortcode, Value)
                    VALUES (@shortcode, @value)
                    ON CONFLICT(Shortcode) DO UPDATE SET Value = excluded.Value;
                ";

            var parameters = new[]
            {
                      new SQLiteParameter("@shortcode", shortcode),
                      new SQLiteParameter("@value", value)
                 };

            sqlite.InsertData(query, parameters);
        }

        ////////////////////////////////////////////////////////////
        // Get a setting by shortcode
        ////////////////////////////////////////////////////////////
        private string SettingGet(string shortcode)
        {
            string query = "SELECT Value FROM Settings WHERE Shortcode = @shortcode;";
            var parameters = new[]
            {
                 new SQLiteParameter("@shortcode", shortcode)
            };

            var dt = sqlite.GetDataTable(query, parameters);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Value"].ToString();
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CustomUI: Interface Overrides
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Drag Window by Holding on Header
        ////////////////////////////////////////////////////////////
        private void CustomUI_Header_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        ////////////////////////////////////////////////////////////
        /// Interface Paint Functionality
        ////////////////////////////////////////////////////////////
        private void CustomUI_Interface_Paint(object sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }

        ////////////////////////////////////////////////////////////
        /// Update button locations on resize
        ////////////////////////////////////////////////////////////
        private void CustomUI_Interface_Resize(object sender, EventArgs e)
        {
            btnMinimize.Location = new Point(this.Width - 100, 0);
            btnMaximize.Location = new Point(this.Width - 70, 0);
            btnClose.Location = new Point(this.Width - 40, 0);
        }

        ////////////////////////////////////////////////////////////
        /// ReInitialize Border and Buttons
        ////////////////////////////////////////////////////////////
        private void CustomUI_interface_reinit()
        {
            btnClose.Location = new Point(this.Width - 40, 0);
            btnMaximize.Location = new Point(this.Width - 70, 0);
            btnMinimize.Location = new Point(this.Width - 100, 0);
            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();
        }

        ////////////////////////////////////////////////////////////
        /// Struct for Minimum Resizing in Width and Height
        ////////////////////////////////////////////////////////////
        [StructLayout(LayoutKind.Sequential)]
        public struct CustomUI_MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        ////////////////////////////////////////////////////////////
        /// Allow for resizing by overriding WndProc
        ////////////////////////////////////////////////////////////
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_GETMINMAXINFO = 0x24;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    if (this.WindowState == FormWindowState.Maximized) { return; }
                    base.WndProc(ref m);

                    Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                    if (pos.X < borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOPRIGHT;
                    }
                    else if (pos.X < borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMLEFT;
                    }
                    else if (pos.X > Width - borderWidth && pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                    }
                    else if (pos.X < borderWidth)
                    {
                        m.Result = (IntPtr)HTLEFT;
                    }
                    else if (pos.X > Width - borderWidth)
                    {
                        m.Result = (IntPtr)HTRIGHT;
                    }
                    else if (pos.Y < borderWidth)
                    {
                        m.Result = (IntPtr)HTTOP;
                    }
                    else if (pos.Y > Height - borderWidth)
                    {
                        m.Result = (IntPtr)HTBOTTOM;
                    }
                    else
                    {
                        m.Result = (IntPtr)HTCLIENT;
                    }
                    CustomUI_interface_reinit();
                    return;

                case WM_GETMINMAXINFO:
                    if (this.WindowState == FormWindowState.Maximized) { return; }
                    CustomUI_MINMAXINFO minMaxInfo = (CustomUI_MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(CustomUI_MINMAXINFO));
                    minMaxInfo.ptMinTrackSize.X = SF_MINIMUM_WIDTH; // Minimum width
                    minMaxInfo.ptMinTrackSize.Y = SF_MINIMUM_HEIGHT; // Minimum height
                    Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
                    CustomUI_interface_reinit();
                    this.Invalidate();
                    break;
            }
            base.WndProc(ref m);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CustomUI: Buttons Draw
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Draw the CustomUI Buttons
        ////////////////////////////////////////////////////////////
        private void CustomUI_Buttons_Draw()
        {
            btnMinimize = new System.Windows.Forms.Button
            {
                Text = "_",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 100, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += CustomUI_BtnMinimize_Click;
            btnMinimize.ForeColor = buttonTextColor;
            btnMinimize.BringToFront();
            tooltip_frame.SetToolTip(btnMinimize, btnMinimizeText);
            btnMaximize = new System.Windows.Forms.Button
            {
                Text = "O",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 70, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += CustomUI_BtnMaximize_Click;
            btnMaximize.ForeColor = buttonTextColor;
            btnMaximize.BringToFront();
            tooltip_frame.SetToolTip(btnMaximize, btnMaximizeText);
            btnClose = new System.Windows.Forms.Button
            {
                Text = "X",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 40, 0),
                BackColor = buttonColor,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += CustomUI_BtnClose_Click;
            btnClose.ForeColor = buttonTextColor;
            btnClose.BringToFront();
            tooltip_frame.SetToolTip(btnClose, btnCloseText);
            Header_Panel.Controls.Add(btnMaximize);
            Header_Panel.Controls.Add(btnMinimize);
            Header_Panel.Controls.Add(btnClose);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CustomUI: Buttons Events
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        /// Close Window to Tray or Close Completely
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnClose_Click(object sender, EventArgs e)
        {
            CustomUI_app_window_exit(sender, e);
        }

        ////////////////////////////////////////////////////////////
        /// Minimize Button Click to Minimize Current Form
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            CustomUI_interface_reinit();
        }

        ////////////////////////////////////////////////////////////
        /// Maximize Button Click Functionality
        ////////////////////////////////////////////////////////////
        private void CustomUI_BtnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Height = previousheight;
            }
            else
            {
                previousheight = this.Height;
                this.WindowState = FormWindowState.Maximized;
            }
        }
    }
}
