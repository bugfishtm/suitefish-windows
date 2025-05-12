using System.Data.SQLite;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Security.Policy;
using System.Media;
using System.Security.Principal;
using System.Diagnostics;
using suitefish.Library;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace suitefish
{
    public partial class Interface : Form
    {
        // Form Styling Related
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private int borderWidth = 5; // Set the width of the border
        private Color borderColor = Color.FromArgb(0x32, 0x32, 0x32); // Set the color of the border
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnMaximize;
        private System.Windows.Forms.Button btnClose;
        private Point offset;
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        // Additional
        private Sqlite sqlite;
        private ContextMenuStrip contextMenuStrip;
        private NotifyIcon notifyIcon;
        // Setting Variables
        private string server_store;
        private bool use_tray_on_exit = true;

        public Interface()
        {
            // Ensure Admin Permission
            EnsureRunAsAdmin();

            // Initialize Component
            InitializeComponent();

            // Set Form Title
            this.Text = "Suitefish";

            // Show only welcome panel
            CWelcome_Panel.Visible = true;
            CStore_Panel.Visible = false;
            CLibrary_Panel.Visible = false;
            CUpdate_Panel.Visible = false;
            CSettings_Panel.Visible = false;

            // Initialize SQLite
            sqlite = new Sqlite("data.db");
            sqlite.CreateTable("CREATE TABLE IF NOT EXISTS Settings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Value TEXT NOT NULL, Shortcode TEXT NOT NULL UNIQUE);");

            // Set Default Settings
            if (SettingGet("server_store") == null)
            {
                SettingSet("server_store", "https://suitefish.com");
            }
            server_store = SettingGet("server_store");
            if (SettingGet("use_tray_on_exit") == null)
            {
                SettingSet("use_tray_on_exit", "0");
            }
            use_tray_on_exit = true; if (SettingGet("use_tray_on_exit").Equals("0")) { use_tray_on_exit = false; }

            // Fix Interface
            interface_init_frame_btn();

            // Create Required Folders
            string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string folderName = "SetFolderName";
            string folderPath = Path.Combine(exeDirectory, folderName);
            folderName = "Cache";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            folderName = "Apps";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            folderName = "Update";
            folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);

            // Setup Window Icon
            this.Icon = new Icon(new MemoryStream(Properties.Resources.favicon));

            // Setup Window Fix Timer
            timer1.Interval = 500;
            timer1.Tick += suitefish_uiupdate_Tick;
            timer1.Start();

            // Subscribe to the Paint event
            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new Padding(borderWidth);
            this.Padding = new Padding(5);
            this.Paint += new PaintEventHandler(Interface_Paint);
            this.Resize += Interface_Resize;

            // Create and configure ContextMenuStrip
            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Show", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; notifyIcon.Visible = true; });
            contextMenuStrip.Items.Add("Minimize", null, (s, e) => { this.Hide(); });
            contextMenuStrip.Items.Add("Exit", null, (s, e) => { app_exit(false); });

            // Create and configure NotifyIcon
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(new MemoryStream(Properties.Resources.favicon)),
                Visible = true,
                ContextMenuStrip = contextMenuStrip
            };
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;

            // Add Navigation List Items
            Navi_List.MouseDoubleClick += Navi_List_MouseDoubleClick;
            Navi_List.Items.Add("Store");
            Navi_List.Items.Add("Library");
            Navi_List.Items.Add("Update");
            Navi_List.Items.Add("Settings");
        }

        // Set or update a setting by shortcode
        private void Navi_List_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = Navi_List.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches && index < Navi_List.Items.Count)
            {
                var item = Navi_List.Items[index];
                if(item.ToString().Equals("Store"))
                {
                    CWelcome_Panel.Visible = false;
                    CStore_Panel.Visible = true;
                    CLibrary_Panel.Visible = false;
                    CUpdate_Panel.Visible = false;
                    CSettings_Panel.Visible = false;
                }
                if (item.ToString().Equals("Library"))
                {
                    CWelcome_Panel.Visible = false;
                    CStore_Panel.Visible = false;
                    CLibrary_Panel.Visible = true;
                    CUpdate_Panel.Visible = false;
                    CSettings_Panel.Visible = false;
                }
                if (item.ToString().Equals("Update"))
                {
                    CWelcome_Panel.Visible = false;
                    CStore_Panel.Visible = false;
                    CLibrary_Panel.Visible = false;
                    CUpdate_Panel.Visible = true;
                    CSettings_Panel.Visible = false;
                }
                if (item.ToString().Equals("Settings"))
                {
                    CWelcome_Panel.Visible = false;
                    CStore_Panel.Visible = false;
                    CLibrary_Panel.Visible = false;
                    CUpdate_Panel.Visible = false;
                    CSettings_Panel.Visible = true;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Settings Functions
        //////////////////////////////////////////////////////////////////////////////////

        // Set or update a setting by shortcode
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

        // Get a setting by shortcode
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

        //////////////////////////////////////////////////////////////////////////////////
        /// Exit Functions
        //////////////////////////////////////////////////////////////////////////////////
        private void app_exit(bool header_press)
        {
            if (header_press)
            {
                if (use_tray_on_exit)
                {
                    this.Hide();
                    notifyIcon.Visible = true;
                }
                else
                {
                    Application.Exit();
                }
            }
            else { Application.Exit(); }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Async Functions
        //////////////////////////////////////////////////////////////////////////////////

        // Secure Delete Async Function
        public static Task SecureDeleteFileAsync(string filePath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteFile(filePath, passes));
        }

        // Secure Delete Async Function
        public static Task SecureDeleteDirectoryAsync(string dirPath, int passes = 1)
        {
            return Task.Run(() => SecureDelete.SecureDeleteDirectory(dirPath, passes));
        }

        // Sleep for Second Amount
        public async Task SleepSecondsAsync(int seconds)
        {
            seconds = seconds * 1000;
            await Task.Delay(seconds);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// UI Elements
        //////////////////////////////////////////////////////////////////////////////////

        // Initialize Border and Buttons
        private void interface_init_frame_btn()
        {
            // Minimize Button
            btnMinimize = new System.Windows.Forms.Button
            {
                Text = "_",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 95, 5),
                BackColor = Color.FromArgb(0xFF, 0xFF, 0xFF),
                FlatStyle = FlatStyle.Flat
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += BtnMinimize_Click;
            btnMinimize.ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            tooltip_frame.SetToolTip(btnMinimize, "Minimize");

            // Maximize Button
            btnMaximize = new System.Windows.Forms.Button
            {
                Text = "O",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 65, 5),
                BackColor = Color.FromArgb(0xFF, 0xFF, 0xFF),
                FlatStyle = FlatStyle.Flat
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += BtnMaximize_Click;
            btnMaximize.ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            tooltip_frame.SetToolTip(btnMaximize, "Maximize");

            // Close Button
            btnClose = new System.Windows.Forms.Button
            {
                Text = "X",
                Size = new Size(30, 30),
                Location = new Point(this.Width - 35, 5),
                BackColor = Color.FromArgb(0xFF, 0xFF, 0xFF),
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += BtnClose_Click;
            btnClose.ForeColor = Color.FromArgb(0x00, 0x00, 0x00);
            tooltip_frame.SetToolTip(btnClose, "Close");

            // Add buttons to the form
            this.Controls.Add(btnMinimize);
            this.Controls.Add(btnMaximize);
            this.Controls.Add(btnClose);

            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();

        }

        // Close Window to Tray or Close Completely
        private void BtnClose_Click(object sender, EventArgs e)
        {
            app_exit(true);
        }

        // ReInitialize Border and Buttons
        private void interface_reinit()
        {
            btnClose.Location = new Point(this.Width - 35, 5);
            btnMaximize.Location = new Point(this.Width - 65, 5);
            btnMinimize.Location = new Point(this.Width - 95, 5);
            btnClose.BringToFront();
            btnMaximize.BringToFront();
            btnMinimize.BringToFront();
        }

        // Allow for resizing by overriding WndProc
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
                    interface_reinit();
                    return;

                case WM_GETMINMAXINFO:
                    MINMAXINFO minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
                    minMaxInfo.ptMinTrackSize.X = 1200; // Minimum width
                    minMaxInfo.ptMinTrackSize.Y = 700; // Minimum height
                    Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
                    interface_reinit();
                    break;
            }
            base.WndProc(ref m);
        }

        // Additional Function for MouseMove
        private void header_frame_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Move the form with the mouse
                Point newLocation = this.PointToScreen(new Point(e.X, e.Y));
                newLocation.Offset(-offset.X, -offset.Y);

                // Ensure the form stays within the screen bounds
                Screen screen = Screen.FromPoint(newLocation);
                Rectangle screenBounds = screen.Bounds;

                // Adjust newLocation if it goes outside screen bounds
                int newX = Math.Max(screenBounds.Left, Math.Min(screenBounds.Right - this.Width, newLocation.X));
                int newY = Math.Max(screenBounds.Top, Math.Min(screenBounds.Bottom - this.Height, newLocation.Y));

                this.Location = new Point(newX, newY);
            }
        }

        // Drag Window by Holding on Header
        private void Header_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // Maximize and Minize Windows with Double Click
        private void Header_Panel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Normal)
                    this.WindowState = FormWindowState.Maximized;
                else if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;
            }
            interface_reinit();
        }

        // Interface Resize Functionality
        private void Interface_Resize(object sender, EventArgs e)
        {
            // Update button locations on resize
            btnMinimize.Location = new Point(this.Width - 95, 5);
            btnMaximize.Location = new Point(this.Width - 65, 5);
            btnClose.Location = new Point(this.Width - 35, 5);
        }

        // NotifyIcon DoubleClick
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show(); this.WindowState = FormWindowState.Normal; notifyIcon.Visible = true;
        }

        // Maximize Button Click Functionality
        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                Rectangle workingArea = Screen.GetWorkingArea(this);
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;
                this.Location = new Point(Math.Max(this.Location.X, workingArea.X),
                          Math.Max(this.Location.Y, workingArea.Y));
            }
        }

        // Interface Paint Functionality
        private void Interface_Paint(object sender, PaintEventArgs e)
        {
            // Draw the custom border
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1));
            }
        }


        // Extra Function for Minimum Resizing in Width and Height to not make the Window Disappear
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        // Function for Mouse Move on Title Bar Selection
        private void header_frame_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Capture the offset from the mouse cursor to the form's location
                offset = new Point(e.X, e.Y);
            }
        }

        // Minimize Button Click to Minimize Current Form
        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            interface_reinit();
        }

        // Tick Timer Fix Window Resize Errors
        private void suitefish_uiupdate_Tick(object sender, EventArgs e)
        {
            interface_reinit();
        }

        //////////////////////////////////////////////////////////////////////////////////
        /// Administrator Permission Request
        //////////////////////////////////////////////////////////////////////////////////

        // Check for Admin Permissions
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // Ensure Admin Permissions
        public static void EnsureRunAsAdmin()
        {
            if (!IsAdministrator())
            {
                // Restart the application with admin rights
                var proc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath,
                    Verb = "runas" // This triggers the UAC prompt
                };

                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    // User refused the elevation
                    MessageBox.Show("Administrator permissions are required to continue.", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Application.Exit(); // Close the current instance
            }
        }
    }
}
