using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ITToolkit
{
    public class SetupForm : Form
    {
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private PictureBox picHeaderIcon;

        private Panel pnlFooter;
        private Button btnBack;
        private Button btnNext;
        private Button btnCancel;

        // Container Panel to isolate wizard pages from header & footer docking
        private Panel pnlContainer;

        private Panel pnlWelcome;
        private Panel pnlLicense;
        private Panel pnlOptions;
        private Panel pnlInstall;
        private Panel pnlFinished;

        // License Page Controls
        private RadioButton rbAccept;
        private RadioButton rbDecline;

        // Options Page Controls
        private TextBox txtInstallPath;
        private Button btnBrowse;
        private CheckBox chkDesktop;
        private CheckBox chkStartMenu;
        private CheckBox chkRegister;

        // Install Page Controls
        private ProgressBar prgInstall;
        private Label lblInstallStatus;

        // Finish Page Controls
        private CheckBox chkLaunchNow;

        private int currentPage = 1;
        private string finalInstalledExe = "";

        public SetupForm()
        {
            InitializeComponent();
            ShowPage(1);
        }

        private void InitializeComponent()
        {
            this.Text = "IT Support Ultimate Toolkit Setup";
            this.Size = new Size(620, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // =========================
            // LOAD EMBEDDED ICONS / ASSETS
            // =========================
            Image logoImg = null;
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                using (var iconStream = asm.GetManifestResourceStream("app.ico"))
                {
                    if (iconStream != null)
                    {
                        this.Icon = new Icon(iconStream);
                    }
                }

                using (var pngStream = asm.GetManifestResourceStream("app-icon.png"))
                {
                    if (pngStream != null)
                    {
                        logoImg = Image.FromStream(pngStream);
                    }
                }
            }
            catch { }

            if (logoImg == null && this.Icon != null)
            {
                try { logoImg = this.Icon.ToBitmap(); } catch { }
            }

            // =========================
            // TOP HEADER BANNER (Dock: Top)
            // =========================
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 76;
            pnlHeader.BackColor = Color.FromArgb(67, 56, 202); // Modern Vibrant Indigo

            pnlHeader.Paint += (s, pe) =>
            {
                using (var brush = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(49, 46, 129),   // Deep Indigo
                    Color.FromArgb(79, 70, 229),   // Primary Indigo
                    LinearGradientMode.Horizontal))
                {
                    pe.Graphics.FillRectangle(brush, pnlHeader.ClientRectangle);
                }
                // Subtle bottom accent border
                using (var pen = new Pen(Color.FromArgb(99, 102, 241), 1))
                {
                    pe.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
                }
            };

            lblHeaderTitle = new Label();
            lblHeaderTitle.Text = "IT Support Ultimate Toolkit 3.5 Enterprise";
            lblHeaderTitle.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold, GraphicsUnit.Point);
            lblHeaderTitle.ForeColor = Color.White;
            lblHeaderTitle.BackColor = Color.Transparent;
            lblHeaderTitle.Location = new Point(22, 14);
            lblHeaderTitle.AutoSize = true;
            lblHeaderTitle.UseMnemonic = false;

            lblHeaderSubtitle = new Label();
            lblHeaderSubtitle.Text = "Created & Developed by MEUK THAREACH • Setup Wizard";
            lblHeaderSubtitle.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblHeaderSubtitle.ForeColor = Color.FromArgb(224, 231, 255);
            lblHeaderSubtitle.BackColor = Color.Transparent;
            lblHeaderSubtitle.Location = new Point(23, 42);
            lblHeaderSubtitle.AutoSize = true;
            lblHeaderSubtitle.UseMnemonic = false;

            picHeaderIcon = new PictureBox();
            picHeaderIcon.Size = new Size(50, 50);
            picHeaderIcon.Location = new Point(542, 13);
            picHeaderIcon.SizeMode = PictureBoxSizeMode.Zoom;
            picHeaderIcon.BackColor = Color.Transparent;
            if (logoImg != null) picHeaderIcon.Image = logoImg;

            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblHeaderSubtitle);
            pnlHeader.Controls.Add(picHeaderIcon);

            // =========================
            // BOTTOM FOOTER (Dock: Bottom)
            // =========================
            pnlFooter = new Panel();
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Height = 60;
            pnlFooter.BackColor = Color.FromArgb(248, 250, 252);

            Panel line = new Panel();
            line.Dock = DockStyle.Top;
            line.Height = 1;
            line.BackColor = Color.FromArgb(226, 232, 240);
            pnlFooter.Controls.Add(line);

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(90, 32);
            btnCancel.Location = new Point(502, 14);
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.Font = new Font("Segoe UI", 9F);
            btnCancel.Click += (s, e) => this.Close();

            btnNext = new Button();
            btnNext.Text = "Next >";
            btnNext.Size = new Size(92, 32);
            btnNext.Location = new Point(400, 14);
            btnNext.BackColor = Color.FromArgb(79, 70, 229);
            btnNext.ForeColor = Color.White;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnNext.Click += BtnNext_Click;

            btnBack = new Button();
            btnBack.Text = "< Back";
            btnBack.Size = new Size(90, 32);
            btnBack.Location = new Point(300, 14);
            btnBack.FlatStyle = FlatStyle.System;
            btnBack.Font = new Font("Segoe UI", 9F);
            btnBack.Click += BtnBack_Click;

            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Controls.Add(btnNext);
            pnlFooter.Controls.Add(btnBack);

            // ===============================================
            // CENTRAL CONTAINER PANEL (Isolated Middle Area)
            // ===============================================
            pnlContainer = new Panel();
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.BackColor = Color.FromArgb(248, 250, 252);

            // =========================
            // PAGE 1: WELCOME
            // =========================
            pnlWelcome = new Panel();
            pnlWelcome.Dock = DockStyle.Fill;
            pnlWelcome.Padding = new Padding(28, 16, 28, 16);

            Label lblWelcomeTitle = new Label();
            lblWelcomeTitle.Text = "Welcome to the IT Support Ultimate Toolkit Setup Wizard";
            lblWelcomeTitle.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            lblWelcomeTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblWelcomeTitle.Location = new Point(26, 14);
            lblWelcomeTitle.Size = new Size(550, 26);
            lblWelcomeTitle.UseMnemonic = false;

            Label lblAuthor = new Label();
            lblAuthor.Text = "★ Created & Developed by: MEUK THAREACH  •  Enterprise Desktop Edition";
            lblAuthor.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            lblAuthor.ForeColor = Color.FromArgb(79, 70, 229);
            lblAuthor.Location = new Point(26, 42);
            lblAuthor.Size = new Size(550, 22);
            lblAuthor.UseMnemonic = false;

            Label lblWelcomeBody = new Label();
            lblWelcomeBody.Text =
                "This wizard will guide you through installing IT Support Ultimate Toolkit on your PC.\n\n" +
                "Included in this release (74 Pro Utilities across 7 Categories):\n" +
                "  ▸  Smart CLI Generators (VLAN/Trunk, Dual IPsec VPN Matcher, Hairpin NAT)\n" +
                "  ▸  Enterprise Configs & SOPs (Cisco, MikroTik, FortiGate, UniFi, 12 Playbooks)\n" +
                "  ▸  Master Credential DB & Error Decoders (50+ Brands, 80+ BSOD/FortiGate Codes)\n" +
                "  ▸  CCTV & Security Surveillance Pro (RTSP, Storage, Diagnostics, PoE, NAT)\n" +
                "  ▸  Network & Infrastructure Suite (MAC, CIDR, Subnet, DNS DoH, Wi-Fi QR)\n" +
                "  ▸  Windows & Enterprise IT (Active Directory, Outlook Repair, RAID, Battery)\n" +
                "  ▸  100% Offline Core — Functions in datacenters and server rooms without internet\n\n" +
                "Click Next to review the License Agreement and continue.";
            lblWelcomeBody.Font = new Font("Segoe UI", 9F);
            lblWelcomeBody.ForeColor = Color.FromArgb(51, 65, 85);
            lblWelcomeBody.Location = new Point(26, 70);
            lblWelcomeBody.Size = new Size(550, 215);
            lblWelcomeBody.UseMnemonic = false;

            pnlWelcome.Controls.Add(lblWelcomeTitle);
            pnlWelcome.Controls.Add(lblAuthor);
            pnlWelcome.Controls.Add(lblWelcomeBody);

            // =========================
            // PAGE 2: LICENSE AGREEMENT (EULA)
            // =========================
            pnlLicense = new Panel();
            pnlLicense.Dock = DockStyle.Fill;
            pnlLicense.Padding = new Padding(28, 12, 28, 12);

            Label lblLicenseTitle = new Label();
            lblLicenseTitle.Text = "License Agreement & Terms of Service";
            lblLicenseTitle.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            lblLicenseTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblLicenseTitle.Location = new Point(26, 12);
            lblLicenseTitle.Size = new Size(550, 24);
            lblLicenseTitle.UseMnemonic = false;

            Label lblLicenseDesc = new Label();
            lblLicenseDesc.Text = "Please read the following license agreement carefully before continuing:";
            lblLicenseDesc.Font = new Font("Segoe UI", 8.75F);
            lblLicenseDesc.ForeColor = Color.FromArgb(71, 85, 105);
            lblLicenseDesc.Location = new Point(26, 36);
            lblLicenseDesc.Size = new Size(550, 18);
            lblLicenseDesc.UseMnemonic = false;

            TextBox txtLicense = new TextBox();
            txtLicense.Multiline = true;
            txtLicense.ReadOnly = true;
            txtLicense.ScrollBars = ScrollBars.Vertical;
            txtLicense.BackColor = Color.White;
            txtLicense.ForeColor = Color.FromArgb(30, 41, 59);
            txtLicense.Font = new Font("Segoe UI", 8.5F);
            txtLicense.Location = new Point(28, 58);
            txtLicense.Size = new Size(548, 162);
            txtLicense.Text =
                "IT SUPPORT ULTIMATE TOOLKIT - END USER LICENSE AGREEMENT (EULA)\r\n" +
                "Version 3.0 Enterprise Desktop Edition\r\n" +
                "Created & Developed by MEUK THAREACH\r\n" +
                "Copyright (c) 2026 MEUK THAREACH. All Rights Reserved.\r\n\r\n" +
                "1. GRANT OF LICENSE:\r\n" +
                "This software is provided free of charge for IT Support Specialists, Helpdesk Technicians, System Administrators, Network Engineers, and CCTV Surveillance Professionals for troubleshooting, administration, and educational purposes.\r\n\r\n" +
                "2. OFFLINE & ONLINE ARCHITECTURE:\r\n" +
                "The core engineering algorithms and utilities in this software are engineered to operate 100% offline without requiring internet access. Network-dependent features (such as latency pings, DoH DNS lookups, and update checks) operate gracefully with status detection.\r\n\r\n" +
                "3. SCRIPTING & TECHNICAL RESPONSIBILITY:\r\n" +
                "Commands and configuration templates generated by this toolkit (Cisco Switch, MikroTik RouterOS, Active Directory PowerShell, Linux shell scripts, CCTV RSTP/NAT rules) are provided for professional technical guidance. Administrators must verify scripts before executing on production infrastructure.\r\n\r\n" +
                "4. INTELLECTUAL PROPERTY & ATTRIBUTION:\r\n" +
                "All original code, graphics, layout, and software design are the intellectual property of MEUK THAREACH. Redistribution in original form is permitted.\r\n\r\n" +
                "By selecting 'I accept the agreement', you agree to be bound by the terms of this license.";

            rbAccept = new RadioButton();
            rbAccept.Text = "I accept the agreement (យល់ព្រមតាមលក្ខខណ្ឌប្រើប្រាស់)";
            rbAccept.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            rbAccept.ForeColor = Color.FromArgb(15, 23, 42);
            rbAccept.Location = new Point(30, 226);
            rbAccept.AutoSize = true;
            rbAccept.UseMnemonic = false;
            rbAccept.CheckedChanged += (s, e) => {
                if (currentPage == 2) btnNext.Enabled = rbAccept.Checked;
            };

            rbDecline = new RadioButton();
            rbDecline.Text = "I do not accept the agreement (មិនយល់ព្រម)";
            rbDecline.Font = new Font("Segoe UI", 9F);
            rbDecline.ForeColor = Color.FromArgb(100, 116, 139);
            rbDecline.Location = new Point(30, 250);
            rbDecline.AutoSize = true;
            rbDecline.Checked = true;
            rbDecline.UseMnemonic = false;
            rbDecline.CheckedChanged += (s, e) => {
                if (currentPage == 2) btnNext.Enabled = rbAccept.Checked;
            };

            pnlLicense.Controls.Add(lblLicenseTitle);
            pnlLicense.Controls.Add(lblLicenseDesc);
            pnlLicense.Controls.Add(txtLicense);
            pnlLicense.Controls.Add(rbAccept);
            pnlLicense.Controls.Add(rbDecline);

            // =========================
            // PAGE 3: OPTIONS & DESTINATION
            // =========================
            pnlOptions = new Panel();
            pnlOptions.Dock = DockStyle.Fill;
            pnlOptions.Padding = new Padding(28, 16, 28, 16);

            Label lblDestTitle = new Label();
            lblDestTitle.Text = "Select Destination Location";
            lblDestTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDestTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblDestTitle.Location = new Point(26, 14);
            lblDestTitle.AutoSize = true;
            lblDestTitle.UseMnemonic = false;

            Label lblDestDesc = new Label();
            lblDestDesc.Text = "Setup will install IT Support Ultimate Toolkit into the following folder:";
            lblDestDesc.ForeColor = Color.FromArgb(71, 85, 105);
            lblDestDesc.Location = new Point(26, 38);
            lblDestDesc.AutoSize = true;
            lblDestDesc.UseMnemonic = false;

            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\IT Support Ultimate Toolkit");
            txtInstallPath = new TextBox();
            txtInstallPath.Text = defaultPath;
            txtInstallPath.Location = new Point(28, 64);
            txtInstallPath.Size = new Size(436, 25);
            txtInstallPath.Font = new Font("Segoe UI", 9F);

            btnBrowse = new Button();
            btnBrowse.Text = "Browse...";
            btnBrowse.Location = new Point(474, 62);
            btnBrowse.Size = new Size(100, 28);
            btnBrowse.FlatStyle = FlatStyle.System;
            btnBrowse.Click += BtnBrowse_Click;

            Label lblShortcutsTitle = new Label();
            lblShortcutsTitle.Text = "Additional Shortcuts & Registration:";
            lblShortcutsTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblShortcutsTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblShortcutsTitle.Location = new Point(26, 112);
            lblShortcutsTitle.AutoSize = true;
            lblShortcutsTitle.UseMnemonic = false;

            chkDesktop = new CheckBox();
            chkDesktop.Text = "Create a Desktop shortcut";
            chkDesktop.Checked = true;
            chkDesktop.Location = new Point(30, 138);
            chkDesktop.AutoSize = true;
            chkDesktop.UseMnemonic = false;

            chkStartMenu = new CheckBox();
            chkStartMenu.Text = "Create a Start Menu folder and shortcut";
            chkStartMenu.Checked = true;
            chkStartMenu.Location = new Point(30, 166);
            chkStartMenu.AutoSize = true;
            chkStartMenu.UseMnemonic = false;

            chkRegister = new CheckBox();
            chkRegister.Text = "Register in Windows Installed Apps (Control Panel / Settings)";
            chkRegister.Checked = true;
            chkRegister.Location = new Point(30, 194);
            chkRegister.AutoSize = true;
            chkRegister.UseMnemonic = false;

            pnlOptions.Controls.Add(lblDestTitle);
            pnlOptions.Controls.Add(lblDestDesc);
            pnlOptions.Controls.Add(txtInstallPath);
            pnlOptions.Controls.Add(btnBrowse);
            pnlOptions.Controls.Add(lblShortcutsTitle);
            pnlOptions.Controls.Add(chkDesktop);
            pnlOptions.Controls.Add(chkStartMenu);
            pnlOptions.Controls.Add(chkRegister);

            // =========================
            // PAGE 4: INSTALLING PROGRESS
            // =========================
            pnlInstall = new Panel();
            pnlInstall.Dock = DockStyle.Fill;
            pnlInstall.Padding = new Padding(28, 16, 28, 16);

            Label lblInstallingTitle = new Label();
            lblInstallingTitle.Text = "Installing IT Support Ultimate Toolkit...";
            lblInstallingTitle.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            lblInstallingTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblInstallingTitle.Location = new Point(26, 26);
            lblInstallingTitle.AutoSize = true;
            lblInstallingTitle.UseMnemonic = false;

            lblInstallStatus = new Label();
            lblInstallStatus.Text = "Preparing installation files...";
            lblInstallStatus.ForeColor = Color.FromArgb(71, 85, 105);
            lblInstallStatus.Location = new Point(26, 64);
            lblInstallStatus.Size = new Size(540, 20);
            lblInstallStatus.UseMnemonic = false;

            prgInstall = new ProgressBar();
            prgInstall.Location = new Point(28, 92);
            prgInstall.Size = new Size(546, 24);
            prgInstall.Style = ProgressBarStyle.Continuous;

            pnlInstall.Controls.Add(lblInstallingTitle);
            pnlInstall.Controls.Add(lblInstallStatus);
            pnlInstall.Controls.Add(prgInstall);

            // =========================
            // PAGE 5: FINISHED
            // =========================
            pnlFinished = new Panel();
            pnlFinished.Dock = DockStyle.Fill;
            pnlFinished.Padding = new Padding(28, 16, 28, 16);

            Label lblFinishTitle = new Label();
            lblFinishTitle.Text = "Completing the IT Support Ultimate Toolkit Setup";
            lblFinishTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblFinishTitle.ForeColor = Color.FromArgb(15, 23, 42);
            lblFinishTitle.Location = new Point(26, 20);
            lblFinishTitle.Size = new Size(550, 48);
            lblFinishTitle.UseMnemonic = false;

            Label lblFinishBody = new Label();
            lblFinishBody.Text =
                "Setup has finished installing IT Support Ultimate Toolkit on your computer.\n\n" +
                "Created & Developed by: MEUK THAREACH\n" +
                "Edition: Version 3.5 Enterprise Pro (74 Pro IT & CCTV Tools & Playbooks)\n" +
                "Core Architecture: 100% Offline Ready with Online Probes\n\n" +
                "Thank you for choosing IT Support Ultimate Toolkit!";
            lblFinishBody.Font = new Font("Segoe UI", 9.25F);
            lblFinishBody.ForeColor = Color.FromArgb(51, 65, 85);
            lblFinishBody.Location = new Point(26, 72);
            lblFinishBody.Size = new Size(550, 105);
            lblFinishBody.UseMnemonic = false;

            chkLaunchNow = new CheckBox();
            chkLaunchNow.Text = "Launch IT Support Ultimate Toolkit now";
            chkLaunchNow.Checked = true;
            chkLaunchNow.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            chkLaunchNow.ForeColor = Color.FromArgb(79, 70, 229);
            chkLaunchNow.Location = new Point(28, 192);
            chkLaunchNow.AutoSize = true;
            chkLaunchNow.UseMnemonic = false;

            pnlFinished.Controls.Add(lblFinishTitle);
            pnlFinished.Controls.Add(lblFinishBody);
            pnlFinished.Controls.Add(chkLaunchNow);

            // Add all pages inside central container
            pnlContainer.Controls.Add(pnlWelcome);
            pnlContainer.Controls.Add(pnlLicense);
            pnlContainer.Controls.Add(pnlOptions);
            pnlContainer.Controls.Add(pnlInstall);
            pnlContainer.Controls.Add(pnlFinished);

            // Add layout controls to Form in correct docking order
            this.Controls.Add(pnlContainer);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);
            pnlContainer.BringToFront();
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
                btnCancel.Enabled = true;
            }
            else if (page == 2)
            {
                btnBack.Enabled = true;
                btnNext.Text = "Next >";
                btnNext.Enabled = rbAccept != null && rbAccept.Checked;
                btnCancel.Enabled = true;
            }
            else if (page == 3)
            {
                btnBack.Enabled = true;
                btnNext.Text = "Install";
                btnNext.Enabled = true;
                btnCancel.Enabled = true;
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
            if (currentPage == 1)
            {
                ShowPage(2);
            }
            else if (currentPage == 2)
            {
                if (rbAccept != null && !rbAccept.Checked)
                {
                    MessageBox.Show("Please accept the license agreement to continue.", "License Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                ShowPage(3);
            }
            else if (currentPage == 3)
            {
                string targetDir = txtInstallPath.Text.Trim();
                if (string.IsNullOrEmpty(targetDir))
                {
                    MessageBox.Show("Please specify a valid installation folder.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ShowPage(4);
                StartInstallation(targetDir);
            }
            else if (currentPage == 5)
            {
                if (chkLaunchNow.Checked && File.Exists(finalInstalledExe))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(finalInstalledExe) { WorkingDirectory = Path.GetDirectoryName(finalInstalledExe) });
                    }
                    catch { }
                }
                this.Close();
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select Destination Folder for IT Support Ultimate Toolkit:";
                fbd.SelectedPath = txtInstallPath.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtInstallPath.Text = Path.Combine(fbd.SelectedPath, "IT Support Ultimate Toolkit");
                }
            }
        }

        private void StartInstallation(string targetDir)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (s, ev) =>
            {
                try
                {
                    // 1. Close any running instances of IT Support Toolkit to release file locks on ITSupportToolkit.exe
                    worker.ReportProgress(2, "Closing running instances of IT Support Toolkit...");
                    string[] procNames = { "ITSupportToolkit", "IT-Support-Toolkit" };
                    foreach (string pn in procNames)
                    {
                        try
                        {
                            foreach (Process p in Process.GetProcessesByName(pn))
                            {
                                try
                                {
                                    p.Kill();
                                    p.WaitForExit(2500);
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    Thread.Sleep(400);

                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }

                    var asm = Assembly.GetExecutingAssembly();
                    string[] resources = {
                        "ITSupportToolkit.exe",
                        "index.html",
                        "manifest.json",
                        "app.ico",
                        "app-icon.png",
                        "Launch_App.bat",
                        "Update-Toolkit.bat",
                        "README.md",
                        "Uninstall.exe"
                    };

                    int step = 0;
                    foreach (var resName in resources)
                    {
                        step++;
                        int progress = (int)((step / (double)(resources.Length + 3)) * 100);
                        worker.ReportProgress(progress, "Extracting " + resName + "...");

                        using (Stream stream = asm.GetManifestResourceStream(resName))
                        {
                            if (stream != null)
                            {
                                string outPath = Path.Combine(targetDir, resName);
                                SafeExtractFile(stream, outPath, procNames);
                            }
                        }
                        Thread.Sleep(50);
                    }

                    string mainExe = Path.Combine(targetDir, "ITSupportToolkit.exe");
                    string appIco = Path.Combine(targetDir, "app.ico");
                    string uninstallExe = Path.Combine(targetDir, "Uninstall.exe");
                    finalInstalledExe = mainExe;

                    // Create Desktop Shortcut
                    if (chkDesktop.Checked)
                    {
                        worker.ReportProgress(80, "Creating Desktop shortcut...");
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        string lnkPath = Path.Combine(desktopPath, "IT Support Ultimate Toolkit.lnk");
                        CreateWindowsShortcut(lnkPath, mainExe, targetDir, appIco, "IT Support Ultimate Toolkit Desktop Edition");
                    }

                    // Create Start Menu Shortcut
                    if (chkStartMenu.Checked)
                    {
                        worker.ReportProgress(90, "Creating Start Menu shortcut...");
                        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                        string folder = Path.Combine(startMenu, "IT Support Ultimate Toolkit");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        string appLnk = Path.Combine(folder, "IT Support Ultimate Toolkit.lnk");
                        CreateWindowsShortcut(appLnk, mainExe, targetDir, appIco, "IT Support Ultimate Toolkit");

                        string uninstLnk = Path.Combine(folder, "Uninstall IT Support Ultimate Toolkit.lnk");
                        CreateWindowsShortcut(uninstLnk, uninstallExe, targetDir, appIco, "Uninstall IT Support Ultimate Toolkit");
                    }

                    // Register in Add/Remove Programs (Registry)
                    if (chkRegister.Checked)
                    {
                        worker.ReportProgress(95, "Registering in Windows Installed Apps...");
                        try
                        {
                            using (RegistryKey parent = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true))
                            {
                                if (parent != null)
                                {
                                    using (RegistryKey appKey = parent.CreateSubKey("ITSupportToolkit"))
                                    {
                                        appKey.SetValue("DisplayName", "IT Support Ultimate Toolkit");
                                        appKey.SetValue("DisplayVersion", "3.5.0");
                                        appKey.SetValue("Publisher", "MEUK THAREACH");
                                        appKey.SetValue("DisplayIcon", appIco);
                                        appKey.SetValue("InstallLocation", targetDir);
                                        appKey.SetValue("UninstallString", "\"" + uninstallExe + "\"");
                                        appKey.SetValue("NoModify", 1, RegistryValueKind.DWord);
                                        appKey.SetValue("NoRepair", 1, RegistryValueKind.DWord);
                                        appKey.SetValue("EstimatedSize", 2800, RegistryValueKind.DWord);
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    worker.ReportProgress(100, "Installation complete!");
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    ev.Result = ex;
                }
            };

            worker.ProgressChanged += (s, ev) =>
            {
                prgInstall.Value = Math.Min(100, Math.Max(0, ev.ProgressPercentage));
                if (ev.UserState != null) lblInstallStatus.Text = ev.UserState.ToString();
            };

            worker.RunWorkerCompleted += (s, ev) =>
            {
                if (ev.Result is Exception)
                {
                    Exception err = (Exception)ev.Result;
                    MessageBox.Show("Installation failed: " + err.Message, "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ShowPage(3);
                }
                else
                {
                    ShowPage(5);
                }
            };

            worker.RunWorkerAsync();
        }

        private static void SafeExtractFile(Stream stream, string outPath, string[] procNames)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                try
                {
                    if (File.Exists(outPath))
                    {
                        try { File.SetAttributes(outPath, FileAttributes.Normal); } catch { }
                    }
                    using (FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        stream.CopyTo(fs);
                    }
                    return;
                }
                catch (IOException)
                {
                    // Attempt to close any newly spawned or lingering process locking this file
                    foreach (string pn in procNames)
                    {
                        try
                        {
                            foreach (Process p in Process.GetProcessesByName(pn))
                            {
                                try { p.Kill(); p.WaitForExit(1500); } catch { }
                            }
                        }
                        catch { }
                    }

                    // Windows NTFS file rename trick: Windows permits renaming an in-use/locked file!
                    try
                    {
                        string tempOld = outPath + ".old." + Guid.NewGuid().ToString("N").Substring(0, 6);
                        if (File.Exists(outPath))
                        {
                            File.Move(outPath, tempOld);
                            using (FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                            {
                                stream.CopyTo(fs);
                            }
                            try { File.Delete(tempOld); } catch { }
                            return;
                        }
                    }
                    catch { }

                    Thread.Sleep(500);
                }
            }

            // Final fallback attempt
            using (FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
            }
        }

        private static void CreateWindowsShortcut(string shortcutPath, string targetPath, string workingDir, string iconPath, string description)
        {
            try
            {
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType != null)
                {
                    object shell = Activator.CreateInstance(shellType);
                    object shortcut = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
                    Type shortcutType = shortcut.GetType();
                    shortcutType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut, new object[] { targetPath });
                    shortcutType.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, shortcut, new object[] { workingDir });
                    if (File.Exists(iconPath))
                    {
                        shortcutType.InvokeMember("IconLocation", BindingFlags.SetProperty, null, shortcut, new object[] { iconPath + ",0" });
                    }
                    if (!string.IsNullOrEmpty(description))
                    {
                        shortcutType.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut, new object[] { description });
                    }
                    shortcutType.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut, null);
                    return;
                }
            }
            catch { }

            // Fallback via PowerShell
            try
            {
                string ps = string.Format(
                    "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('{0}'); $s.TargetPath = '{1}'; $s.WorkingDirectory = '{2}'; $s.IconLocation = '{3},0'; $s.Description = '{4}'; $s.Save()",
                    shortcutPath.Replace("'", "''"), targetPath.Replace("'", "''"), workingDir.Replace("'", "''"), iconPath.Replace("'", "''"), description.Replace("'", "''")
                );
                ProcessStartInfo psi = new ProcessStartInfo("powershell", "-NoProfile -ExecutionPolicy Bypass -Command \"" + ps + "\"");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                using (Process p = Process.Start(psi)) { p.WaitForExit(3000); }
            }
            catch { }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SetupForm());
        }
    }
}
