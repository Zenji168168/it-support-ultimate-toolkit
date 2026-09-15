using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;

[assembly: AssemblyTitle("IT Support Ultimate Toolkit Setup")]
[assembly: AssemblyDescription("Official Enterprise Setup Wizard for IT Support Ultimate Toolkit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("MEUK THAREACH")]
[assembly: AssemblyProduct("IT Support Ultimate Toolkit")]
[assembly: AssemblyCopyright("Copyright (c) 2026 MEUK THAREACH. All rights reserved.")]
[assembly: AssemblyTrademark("MEUK THAREACH")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("7b51e042-4f32-4467-872e-336c84c17290")]
[assembly: AssemblyVersion("3.6.0.0")]
[assembly: AssemblyFileVersion("3.6.0.0")]

namespace ITSupportToolkitInstaller
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CleanSetupForm());
        }
    }

    public class CleanSetupForm : Form
    {
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private PictureBox picHeaderIcon;

        private Panel pnlFooter;
        private Button btnBack;
        private Button btnNext;
        private Button btnCancel;

        private Panel pnlContainer;
        private Panel pnlWelcome;
        private Panel pnlLicense;
        private Panel pnlOptions;
        private Panel pnlInstall;
        private Panel pnlFinished;

        private RadioButton rbAccept;
        private RadioButton rbDecline;
        private TextBox txtInstallPath;
        private CheckBox chkDesktop;
        private CheckBox chkStartMenu;
        private ProgressBar prgInstall;
        private Label lblInstallStatus;
        private CheckBox chkLaunchNow;

        private int currentPage = 1;
        private string finalInstalledHtml = "";

        public CleanSetupForm()
        {
            InitializeComponent();
            ShowPage(1);
        }

        private void InitializeComponent()
        {
            this.Text = "IT Support Ultimate Toolkit v3.6.0 Setup";
            this.ClientSize = new Size(620, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            // Header
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.FromArgb(67, 56, 202) };
            pnlHeader.Paint += (s, pe) =>
            {
                using (var brush = new LinearGradientBrush(pnlHeader.ClientRectangle, Color.FromArgb(49, 46, 129), Color.FromArgb(79, 70, 229), LinearGradientMode.Horizontal))
                {
                    pe.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
            };

            lblHeaderTitle = new Label { Text = "IT Support Ultimate Toolkit 3.6 Enterprise", Font = new Font("Segoe UI", 12.5F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, Location = new Point(22, 14), AutoSize = true };
            lblHeaderSubtitle = new Label { Text = "Created & Developed by MEUK THAREACH * Setup Wizard", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(224, 231, 255), BackColor = Color.Transparent, Location = new Point(23, 42), AutoSize = true };
            picHeaderIcon = new PictureBox { Size = new Size(48, 48), Location = new Point(545, 14), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };

            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using (var icoStream = asm.GetManifestResourceStream("app.ico"))
                {
                    if (icoStream != null) this.Icon = new Icon(icoStream);
                }
                using (var pngStream = asm.GetManifestResourceStream("app-icon.png"))
                {
                    if (pngStream != null) picHeaderIcon.Image = Image.FromStream(pngStream);
                }
            }
            catch { }

            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Controls.Add(picHeaderIcon);
            this.Controls.Add(pnlHeader);

            // Footer
            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 58, BackColor = Color.FromArgb(241, 245, 249) };
            pnlFooter.Paint += (s, pe) => { using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1)) { pe.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0); } };

            btnCancel = new Button { Text = "Cancel", Size = new Size(88, 30), Location = new Point(512, 14), FlatStyle = FlatStyle.System };
            btnCancel.Click += (s, e) => this.Close();

            btnNext = new Button { Text = "Next >", Size = new Size(88, 30), Location = new Point(416, 14), FlatStyle = FlatStyle.System };
            btnNext.Click += BtnNext_Click;

            btnBack = new Button { Text = "< Back", Size = new Size(88, 30), Location = new Point(320, 14), FlatStyle = FlatStyle.System, Enabled = false };
            btnBack.Click += BtnBack_Click;

            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Controls.Add(btnNext);
            pnlFooter.Controls.Add(btnBack);
            this.Controls.Add(pnlFooter);

            // Container
            pnlContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 250, 252) };
            this.Controls.Add(pnlContainer);

            // Page 1: Welcome
            pnlWelcome = new Panel { Dock = DockStyle.Fill };
            var lblW1 = new Label { Text = "Welcome to IT Support Ultimate Toolkit Setup", Font = new Font("Segoe UI", 12.5F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(30, 24), AutoSize = true };
            var lblW2 = new Label { Text = "This wizard will install the official IT Support Ultimate Toolkit v3.6.0 on your computer.\r\n\r\nFeatures Included:\r\n  * 74 Enterprise IT, Network, Sysadmin & CCTV Utilities\r\n  * Smart CLI Generators (VLAN/Trunk, Dual IPsec VPN, Hairpin NAT)\r\n  * 50+ Hardware Vendor Default Passwords & IPs\r\n  * 12 In-depth IT Troubleshooting Decision Tree SOPs\r\n  * 100% Offline Core Capabilities\r\n  * Created & Developed by MEUK THAREACH\r\n\r\nClick Next to continue with the installation.", Font = new Font("Segoe UI", 9.5F), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(32, 60), Size = new Size(550, 200) };
            pnlWelcome.Controls.Add(lblW1);
            pnlWelcome.Controls.Add(lblW2);

            // Page 2: License
            pnlLicense = new Panel { Dock = DockStyle.Fill };
            var lblL1 = new Label { Text = "End-User License Agreement (EULA)", Font = new Font("Segoe UI", 11.5F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(30, 16), AutoSize = true };
            var txtLic = new TextBox { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Location = new Point(32, 45), Size = new Size(556, 170), Font = new Font("Segoe UI", 8.5F), BackColor = Color.White };
            txtLic.Text = "IT SUPPORT ULTIMATE TOOLKIT - END USER LICENSE AGREEMENT\r\nCreator: MEUK THAREACH\r\nVersion: 3.6.0 Enterprise Desktop\r\n\r\n1. GRANT OF LICENSE\r\nThis software is licensed free of charge for professional and commercial use by IT Support Technicians, System Administrators, Network Engineers, and CCTV Specialists.\r\n\r\n2. OFFLINE OPERATION & PRIVACY\r\nThe core utilities operate 100% offline without transmitting any sensitive device credentials, configurations, or personal data to third parties.\r\n\r\n3. COPYRIGHT & ATTRIBUTION\r\nAll intellectual property, user interface designs, and utility suites are created and owned by MEUK THAREACH.";
            rbAccept = new RadioButton { Text = "I accept the agreement", Location = new Point(34, 222), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            rbAccept.CheckedChanged += (s, e) => btnNext.Enabled = rbAccept.Checked;
            rbDecline = new RadioButton { Text = "I do not accept the agreement", Location = new Point(220, 222), AutoSize = true, Checked = true };
            pnlLicense.Controls.Add(lblL1);
            pnlLicense.Controls.Add(txtLic);
            pnlLicense.Controls.Add(rbAccept);
            pnlLicense.Controls.Add(rbDecline);

            // Page 3: Options
            pnlOptions = new Panel { Dock = DockStyle.Fill };
            var lblO1 = new Label { Text = "Installation Options", Font = new Font("Segoe UI", 11.5F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(30, 16), AutoSize = true };
            var lblO2 = new Label { Text = "Destination Folder:", Location = new Point(30, 48), AutoSize = true };
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\IT Support Ultimate Toolkit");
            txtInstallPath = new TextBox { Text = defaultPath, Location = new Point(32, 70), Size = new Size(460, 24), ReadOnly = true };
            var btnBrowse = new Button { Text = "Browse...", Location = new Point(500, 68), Size = new Size(88, 26), FlatStyle = FlatStyle.System };
            btnBrowse.Click += (s, e) => {
                using (var fbd = new FolderBrowserDialog()) {
                    if (fbd.ShowDialog() == DialogResult.OK) txtInstallPath.Text = Path.Combine(fbd.SelectedPath, "IT Support Ultimate Toolkit");
                }
            };
            chkDesktop = new CheckBox { Text = "Create a Desktop shortcut (Recommended)", Checked = true, Location = new Point(34, 116), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            chkStartMenu = new CheckBox { Text = "Create Start Menu shortcuts", Checked = true, Location = new Point(34, 146), AutoSize = true };
            pnlOptions.Controls.Add(lblO1);
            pnlOptions.Controls.Add(lblO2);
            pnlOptions.Controls.Add(txtInstallPath);
            pnlOptions.Controls.Add(btnBrowse);
            pnlOptions.Controls.Add(chkDesktop);
            pnlOptions.Controls.Add(chkStartMenu);

            // Page 4: Install Progress
            pnlInstall = new Panel { Dock = DockStyle.Fill };
            var lblI1 = new Label { Text = "Installing IT Support Ultimate Toolkit...", Font = new Font("Segoe UI", 11.5F, FontStyle.Bold), Location = new Point(30, 30), AutoSize = true };
            lblInstallStatus = new Label { Text = "Extracting components...", Location = new Point(32, 65), Size = new Size(550, 20), ForeColor = Color.FromArgb(100, 116, 139) };
            prgInstall = new ProgressBar { Location = new Point(32, 95), Size = new Size(556, 24), Minimum = 0, Maximum = 100, Value = 10 };
            pnlInstall.Controls.Add(lblI1);
            pnlInstall.Controls.Add(lblInstallStatus);
            pnlInstall.Controls.Add(prgInstall);

            // Page 5: Finished
            pnlFinished = new Panel { Dock = DockStyle.Fill };
            var lblF1 = new Label { Text = "Completing the IT Support Ultimate Toolkit Setup", Font = new Font("Segoe UI", 12.5F, FontStyle.Bold), Location = new Point(30, 30), Size = new Size(550, 60) };
            var lblF2 = new Label { Text = "Setup has finished installing IT Support Ultimate Toolkit v3.6.0 on your computer.\r\n\r\nThe application may be launched by selecting the installed shortcuts.", Location = new Point(32, 100), Size = new Size(550, 60) };
            chkLaunchNow = new CheckBox { Text = "Launch IT Support Ultimate Toolkit now", Checked = true, Location = new Point(34, 175), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            pnlFinished.Controls.Add(lblF1);
            pnlFinished.Controls.Add(lblF2);
            pnlFinished.Controls.Add(chkLaunchNow);

            pnlContainer.Controls.Add(pnlWelcome);
            pnlContainer.Controls.Add(pnlLicense);
            pnlContainer.Controls.Add(pnlOptions);
            pnlContainer.Controls.Add(pnlInstall);
            pnlContainer.Controls.Add(pnlFinished);
        }

        private void ShowPage(int page)
        {
            currentPage = page;
            pnlWelcome.Visible = (page == 1);
            pnlLicense.Visible = (page == 2);
            pnlOptions.Visible = (page == 3);
            pnlInstall.Visible = (page == 4);
            pnlFinished.Visible = (page == 5);

            if (page == 1) { btnBack.Enabled = false; btnNext.Text = "Next >"; btnNext.Enabled = true; }
            else if (page == 2) { btnBack.Enabled = true; btnNext.Text = "Next >"; btnNext.Enabled = rbAccept != null && rbAccept.Checked; }
            else if (page == 3) { btnBack.Enabled = true; btnNext.Text = "Install"; btnNext.Enabled = true; }
            else if (page == 4) { btnBack.Enabled = false; btnNext.Enabled = false; btnCancel.Enabled = false; }
            else if (page == 5) { btnBack.Enabled = false; btnNext.Text = "Finish"; btnNext.Enabled = true; btnCancel.Enabled = false; }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (currentPage == 2) ShowPage(1);
            else if (currentPage == 3) ShowPage(2);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentPage == 1) ShowPage(2);
            else if (currentPage == 2) ShowPage(3);
            else if (currentPage == 3) { ShowPage(4); StartInstallation(txtInstallPath.Text.Trim()); }
            else if (currentPage == 5)
            {
                if (chkLaunchNow.Checked && File.Exists(finalInstalledHtml))
                {
                    try { Process.Start(new ProcessStartInfo { FileName = "msedge.exe", Arguments = "--app=\"file:///" + finalInstalledHtml.Replace('\\', '/') + "\" --start-maximized", WindowStyle = ProcessWindowStyle.Maximized, UseShellExecute = true }); } catch { }
                }
                this.Close();
            }
        }

        private void StartInstallation(string targetDir)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, ev) =>
            {
                try
                {
                    if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

                    var asm = Assembly.GetExecutingAssembly();
                    string[] resources = { "index.html", "app-icon.png", "app.ico", "version.json", "README.md" };

                    int step = 0;
                    foreach (var res in resources)
                    {
                        step++;
                        worker.ReportProgress((int)((step / (double)resources.Length) * 70), "Extracting " + res + "...");
                        using (var stm = asm.GetManifestResourceStream(res))
                        {
                            if (stm != null)
                            {
                                string outPath = Path.Combine(targetDir, res);
                                using (var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                                {
                                    stm.CopyTo(fs);
                                }
                            }
                        }
                    }

                    // Create clean Launch-Toolkit.bat in target folder
                    string batPath = Path.Combine(targetDir, "Launch-Toolkit.bat");
                    File.WriteAllText(batPath, "@echo off\r\nstart \"\" /max msedge --app=\"file:///%~dp0index.html\" --start-maximized\r\nexit\r\n");

                    string mainHtml = Path.Combine(targetDir, "index.html");
                    string appIco = Path.Combine(targetDir, "app.ico");
                    finalInstalledHtml = mainHtml;

                    // Desktop Shortcut
                    if (chkDesktop.Checked)
                    {
                        worker.ReportProgress(85, "Creating Desktop shortcut...");
                        string deskLnk = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "IT Support Ultimate Toolkit.lnk");
                        CreateShortcut(deskLnk, "msedge.exe", "--app=\"file:///" + mainHtml.Replace('\\', '/') + "\" --start-maximized", targetDir, appIco);
                    }

                    // Start Menu Shortcut
                    if (chkStartMenu.Checked)
                    {
                        worker.ReportProgress(95, "Creating Start Menu shortcut...");
                        string smDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "IT Support Ultimate Toolkit");
                        if (!Directory.Exists(smDir)) Directory.CreateDirectory(smDir);
                        string smLnk = Path.Combine(smDir, "IT Support Ultimate Toolkit.lnk");
                        CreateShortcut(smLnk, "msedge.exe", "--app=\"file:///" + mainHtml.Replace('\\', '/') + "\" --start-maximized", targetDir, appIco);
                    }

                    // Create Uninstall.bat
                    string uninstBat = Path.Combine(targetDir, "Uninstall.bat");
                    string deskLnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "IT Support Ultimate Toolkit.lnk");
                    string smDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "IT Support Ultimate Toolkit");
                    string uninstScript = "@echo off\r\ntitle Uninstall IT Support Ultimate Toolkit\r\necho Removing shortcuts...\r\ndel \"" + deskLnkPath + "\" 2>nul\r\nrd /s /q \"" + smDirPath + "\" 2>nul\r\necho Removing installation files...\r\ntimeout /t 1 >nul\r\nstart /b \"\" cmd /c rd /s /q \"" + targetDir + "\"\r\nexit\r\n";
                    File.WriteAllText(uninstBat, uninstScript);

                    // Register in Windows Settings -> Apps & Features
                    try
                    {
                        using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\ITSupportUltimateToolkit"))
                        {
                            if (key != null)
                            {
                                key.SetValue("DisplayName", "IT Support Ultimate Toolkit");
                                key.SetValue("DisplayVersion", "3.6.0");
                                key.SetValue("Publisher", "MEUK THAREACH");
                                key.SetValue("DisplayIcon", appIco);
                                key.SetValue("InstallLocation", targetDir);
                                key.SetValue("UninstallString", "\"" + uninstBat + "\"");
                                key.SetValue("HelpLink", "https://github.com/Zenji168168/it-support-ultimate-toolkit");
                            }
                        }
                    }
                    catch { }

                    worker.ReportProgress(100, "Installation complete!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Installation notice: " + ex.Message, "Setup Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                if (prgInstall != null) prgInstall.Value = Math.Min(100, Math.Max(0, ev.ProgressPercentage));
                if (lblInstallStatus != null) lblInstallStatus.Text = ev.UserState as string;
            };

            worker.RunWorkerCompleted += (s, ev) => ShowPage(5);
            worker.RunWorkerAsync();
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string arguments, string workingDir, string iconLocation)
        {
            try
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null) return;
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = workingDir;
                shortcut.WindowStyle = 3; // Maximized window mode
                shortcut.IconLocation = iconLocation + ", 0";
                shortcut.Description = "IT Support Ultimate Toolkit Desktop Edition by MEUK THAREACH";
                shortcut.Save();
            }
            catch { }
        }
    }
}
