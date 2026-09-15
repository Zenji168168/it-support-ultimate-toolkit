using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ITToolkit
{
    static class Uninstaller
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to completely remove IT Support Ultimate Toolkit from your computer?",
                "Uninstall IT Support Ultimate Toolkit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string installDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');

                // 1. Remove Desktop Shortcut
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string desktopShortcut = Path.Combine(desktopPath, "IT Support Ultimate Toolkit.lnk");
                if (File.Exists(desktopShortcut))
                {
                    try { File.Delete(desktopShortcut); } catch { }
                }

                // 2. Remove Start Menu Shortcut
                string startMenuPrograms = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                string appStartMenuFolder = Path.Combine(startMenuPrograms, "IT Support Ultimate Toolkit");
                if (Directory.Exists(appStartMenuFolder))
                {
                    try { Directory.Delete(appStartMenuFolder, true); } catch { }
                }

                // 3. Remove Registry Entry
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true))
                    {
                        if (key != null)
                        {
                            key.DeleteSubKeyTree("ITSupportToolkit", false);
                        }
                    }
                }
                catch { }

                // 4. Remove extracted cache in LocalAppData\ITSupportToolkit
                try
                {
                    string localAppCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ITSupportToolkit");
                    if (Directory.Exists(localAppCache) && !localAppCache.Equals(installDir, StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.Delete(localAppCache, true);
                    }
                }
                catch { }

                // 5. Schedule self-deletion of install directory
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = string.Format("/c timeout /t 2 /nobreak > NUL & rmdir /s /q \"{0}\"", installDir);
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
                Process.Start(psi);

                MessageBox.Show(
                    "IT Support Ultimate Toolkit was successfully removed from your computer.",
                    "Uninstall Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred during uninstall: " + ex.Message,
                    "Uninstall Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
