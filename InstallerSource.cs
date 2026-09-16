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
[assembly: AssemblyVersion("3.9.0.0")]
[assembly: AssemblyFileVersion("3.9.0.0")]

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
            this.Text = "IT Support Ultimate Toolkit v3.9.0 Setup";
            this.ClientSize = new Size(620, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            // =========================
            // TOP HEADER BANNER (Fixed Height: 76px)
            // =========================
            pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(620, 76),
                BackColor = Color.FromArgb(67, 56, 202)
            };
            pnlHeader.Paint += (s, pe) =>
            {
                using (var brush = new LinearGradientBrush(pnlHeader.ClientRectangle, Color.FromArgb(49, 46, 129), Color.FromArgb(79, 70, 229), LinearGradientMode.Horizontal))
                {
                    pe.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
                using (var pen = new Pen(Color.FromArgb(99, 102, 241), 1))
                {
                    pe.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
                }
            };

            lblHeaderTitle = new Label
            {
                Text = "IT Support Ultimate Toolkit 3.9 Enterprise",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(22, 14),
                AutoSize = true,
                UseMnemonic = false
            };

            lblHeaderSubtitle = new Label
            {
                Text = "Created & Developed by MEUK THAREACH \u2022 Setup Wizard",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(224, 231, 255),
                BackColor = Color.Transparent,
                Location = new Point(23, 42),
                AutoSize = true,
                UseMnemonic = false
            };

            picHeaderIcon = new PictureBox
            {
                Size = new Size(48, 48),
                Location = new Point(545, 14),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

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

            // =========================
            // BOTTOM FOOTER BAR (Fixed Height: 58px)
            // =========================
            pnlFooter = new Panel
            {
                Location = new Point(0, 362),
                Size = new Size(620, 58),
                BackColor = Color.FromArgb(241, 245, 249)
            };
            pnlFooter.Paint += (s, pe) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    pe.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
                }
            };

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

            // =========================
            // CENTER CONTENT CONTAINER (Strictly 0, 76 to 620, 286 - NO OVERLAP)
            // =========================
            pnlContainer = new Panel
            {
                Location = new Point(0, 76),
                Size = new Size(620, 286),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            this.Controls.Add(pnlContainer);

            // ----------------------------------------------------
            // Page 1: Welcome Page
            // ----------------------------------------------------
            pnlWelcome = new Panel { Dock = DockStyle.Fill };
            var lblW1 = new Label
            {
                Text = "Welcome to IT Support Ultimate Toolkit Setup",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(32, 20),
                AutoSize = true,
                UseMnemonic = false
            };
            var lblW2 = new Label
            {
                Text = "This wizard will install the official IT Support Ultimate Toolkit v3.9.0 on your computer.\r\n\r\n" +
                       "Key Capabilities Included:\r\n" +
                       "  \u2022  130 Enterprise IT, Network, Sysadmin & CCTV Utilities\r\n" +
                       "  \u2022  Smart Multi-Vendor CLI Generators (Cisco, MikroTik, FortiGate, Ruijie)\r\n" +
                       "  \u2022  50+ Hardware Brands Master Default Credentials & IP Database\r\n" +
                       "  \u2022  25 In-depth IT Troubleshooting SOP Playbooks & Decision Trees\r\n" +
                       "  \u2022  100% Offline Operation & Cloud Auto-Updater\r\n" +
                       "  \u2022  Created & Developed by MEUK THAREACH\r\n\r\n" +
                       "Click Next to continue with the installation.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(34, 54),
                Size = new Size(554, 218),
                UseMnemonic = false
            };
            pnlWelcome.Controls.Add(lblW1);
            pnlWelcome.Controls.Add(lblW2);

            // ----------------------------------------------------
            // Page 2: License Agreement Page (Clean Spacing & Scroll)
            // ----------------------------------------------------
            pnlLicense = new Panel { Dock = DockStyle.Fill };
            var lblL1 = new Label
            {
                Text = "End-User License Agreement",
                Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(32, 12),
                AutoSize = true,
                UseMnemonic = false
            };
            var lblL2 = new Label
            {
                Text = "Please read the following important information before continuing.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(33, 34),
                AutoSize = true,
                UseMnemonic = false
            };
            var txtLic = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(32, 56),
                Size = new Size(556, 172),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtLic.Text = "IT SUPPORT ULTIMATE TOOLKIT - END USER LICENSE AGREEMENT\r\n" +
                          "Creator, Architect & Lead Developer: MEUK THAREACH\r\n" +
                          "Release Version: 3.9.0 Enterprise Desktop Edition\r\n\r\n" +
                          "1. GRANT OF LICENSE\r\n" +
                          "This software is licensed free of charge for professional and commercial use by IT Support Technicians, System Administrators, Network Engineers, and CCTV Specialists.\r\n\r\n" +
                          "2. OFFLINE OPERATION & DATA PRIVACY\r\n" +
                          "All core diagnostic engines and configuration generators operate 100% locally and offline without transmitting any sensitive credentials, configurations, or personal data.\r\n\r\n" +
                          "3. COPYRIGHT & ATTRIBUTION\r\n" +
                          "All intellectual property, architecture, user interface designs, and utility suites are created and owned by MEUK THAREACH.";
            
            // Unfocus text box so it doesn't stay highlighted or scrolled weirdly
            txtLic.Select(0, 0);

            rbAccept = new RadioButton
            {
                Text = "I accept the agreement",
                Location = new Point(34, 240),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Checked = true // Checked by default so Next button is active!
            };
            rbAccept.CheckedChanged += (s, e) => btnNext.Enabled = rbAccept.Checked;

            rbDecline = new RadioButton
            {
                Text = "I do not accept the agreement",
                Location = new Point(230, 240),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Checked = false
            };

            pnlLicense.Controls.Add(lblL1);
            pnlLicense.Controls.Add(lblL2);
            pnlLicense.Controls.Add(txtLic);
            pnlLicense.Controls.Add(rbAccept);
            pnlLicense.Controls.Add(rbDecline);

            // ----------------------------------------------------
            // Page 3: Installation Options
            // ----------------------------------------------------
            pnlOptions = new Panel { Dock = DockStyle.Fill };
            var lblO1 = new Label { Text = "Select Destination Location", Font = new Font("Segoe UI", 11.5F, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Location = new Point(32, 16), AutoSize = true, UseMnemonic = false };
            var lblO2 = new Label { Text = "Where should IT Support Ultimate Toolkit be installed?", Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(33, 38), AutoSize = true, UseMnemonic = false };
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\IT Support Ultimate Toolkit");
            txtInstallPath = new TextBox { Text = defaultPath, Location = new Point(34, 66), Size = new Size(458, 25), ReadOnly = true, Font = new Font("Segoe UI", 9F) };
            var btnBrowse = new Button { Text = "Browse...", Location = new Point(498, 64), Size = new Size(90, 28), FlatStyle = FlatStyle.System };
            btnBrowse.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select Destination Folder for IT Support Ultimate Toolkit:";
                    if (fbd.ShowDialog() == DialogResult.OK) txtInstallPath.Text = Path.Combine(fbd.SelectedPath, "IT Support Ultimate Toolkit");
                }
            };
            chkDesktop = new CheckBox { Text = "Create a Desktop shortcut (Recommended)", Checked = true, Location = new Point(36, 118), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59) };
            chkStartMenu = new CheckBox { Text = "Create Start Menu shortcuts", Checked = true, Location = new Point(36, 148), AutoSize = true, Font = new Font("Segoe UI", 9F), ForeColor = Color.FromArgb(51, 65, 85) };
            pnlOptions.Controls.Add(lblO1);
            pnlOptions.Controls.Add(lblO2);
            pnlOptions.Controls.Add(txtInstallPath);
            pnlOptions.Controls.Add(btnBrowse);
            pnlOptions.Controls.Add(chkDesktop);
            pnlOptions.Controls.Add(chkStartMenu);

            // ----------------------------------------------------
            // Page 4: Install Progress
            // ----------------------------------------------------
            pnlInstall = new Panel { Dock = DockStyle.Fill };
            var lblI1 = new Label { Text = "Installing IT Support Ultimate Toolkit...", Font = new Font("Segoe UI", 12F, FontStyle.Bold), Location = new Point(32, 28), AutoSize = true };
            lblInstallStatus = new Label { Text = "Extracting components...", Location = new Point(34, 65), Size = new Size(550, 20), ForeColor = Color.FromArgb(100, 116, 139) };
            prgInstall = new ProgressBar { Location = new Point(34, 95), Size = new Size(554, 24), Minimum = 0, Maximum = 100, Value = 15 };
            pnlInstall.Controls.Add(lblI1);
            pnlInstall.Controls.Add(lblInstallStatus);
            pnlInstall.Controls.Add(prgInstall);

            // ----------------------------------------------------
            // Page 5: Finished Page
            // ----------------------------------------------------
            pnlFinished = new Panel { Dock = DockStyle.Fill };
            var lblF1 = new Label { Text = "Completing the IT Support Ultimate Toolkit Setup", Font = new Font("Segoe UI", 12.5F, FontStyle.Bold), Location = new Point(32, 28), Size = new Size(550, 40) };
            var lblF2 = new Label { Text = "Setup has finished installing IT Support Ultimate Toolkit on your computer.\r\n\r\nThe application may be launched by selecting the installed shortcut.\r\n\r\nClick Finish to exit Setup.", Location = new Point(34, 75), Size = new Size(550, 70), Font = new Font("Segoe UI", 9.5F), ForeColor = Color.FromArgb(51, 65, 85) };
            chkLaunchNow = new CheckBox { Text = "Launch IT Support Ultimate Toolkit now", Checked = true, Location = new Point(36, 160), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59) };
            pnlFinished.Controls.Add(lblF1);
            pnlFinished.Controls.Add(lblF2);
            pnlFinished.Controls.Add(chkLaunchNow);

            // Add all pages to container
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

            if (page == 1)
            {
                btnBack.Enabled = false;
                btnNext.Text = "Next >";
                btnNext.Enabled = true;
            }
            else if (page == 2)
            {
                btnBack.Enabled = true;
                btnNext.Text = "Next >";
                btnNext.Enabled = (rbAccept != null && rbAccept.Checked);
            }
            else if (page == 3)
            {
                btnBack.Enabled = true;
                btnNext.Text = "Install";
                btnNext.Enabled = true;
            }
            else if (page == 4)
            {
                btnBack.Enabled = false;
                btnNext.Enabled = false;
                btnCancel.Enabled = false;
            }
            else if (page == 5)
            {
                btnBack.Enabled = false;
                btnNext.Text = "Finish";
                btnNext.Enabled = true;
                btnCancel.Enabled = false;
            }
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
            else if (currentPage == 3)
            {
                ShowPage(4);
                StartInstallation(txtInstallPath.Text.Trim());
            }
            else if (currentPage == 5)
            {
                if (chkLaunchNow.Checked && File.Exists(finalInstalledHtml))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "msedge.exe",
                            Arguments = "--app=\"file:///" + finalInstalledHtml.Replace('\\', '/') + "\" --start-maximized",
                            WindowStyle = ProcessWindowStyle.Maximized,
                            UseShellExecute = true
                        });
                    }
                    catch { }
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
                        worker.ReportProgress(92, "Creating Start Menu shortcut...");
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
                                key.SetValue("DisplayVersion", "3.9.0");
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
