using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ITToolkit
{
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        const uint WM_SETICON = 0x0080;
        const int ICON_SMALL = 0;
        const int ICON_BIG = 1;

        [STAThread]
        static void Main()
        {
            try
            {
                // Target folder in AppData to extract standalone files
                string localAppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ITSupportToolkit");
                if (!Directory.Exists(localAppDir))
                {
                    Directory.CreateDirectory(localAppDir);
                }

                string targetHtml = Path.Combine(localAppDir, "index.html");

                // Extract embedded resources
                var assembly = Assembly.GetExecutingAssembly();
                bool shouldExtract = !File.Exists(targetHtml);
                if (!shouldExtract)
                {
                    try
                    {
                        using (Stream stream = assembly.GetManifestResourceStream("ITToolkit.index.html"))
                        {
                            if (stream != null && stream.Length > new FileInfo(targetHtml).Length)
                            {
                                shouldExtract = true;
                            }
                        }
                    }
                    catch { }
                }
                if (shouldExtract)
                {
                    ExtractResource(assembly, "ITToolkit.index.html", targetHtml);
                }
                ExtractResource(assembly, "ITToolkit.app-icon.png", Path.Combine(localAppDir, "app-icon.png"));
                ExtractResource(assembly, "ITToolkit.app.ico", Path.Combine(localAppDir, "app.ico"));
                ExtractResource(assembly, "ITToolkit.app.ico", Path.Combine(localAppDir, "favicon.ico"));
                ExtractResource(assembly, "ITToolkit.manifest.json", Path.Combine(localAppDir, "manifest.json"));

                // If local index.html exists in current directory, prefer local file
                string currentDirHtml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");
                string launchHtml = File.Exists(currentDirHtml) ? currentDirHtml : targetHtml;

                // Start Embedded Native HTTP Server & Auto-Updater Bridge in background
                LocalServer server = null;
                int serverPort = 0;
                try
                {
                    server = new LocalServer(localAppDir, launchHtml);
                    if (server.Start())
                    {
                        serverPort = server.Port;
                    }
                }
                catch { }

                // ALWAYS launch Edge with direct native file:/// URL!
                // file:/// is 100% reliable, zero latency, zero port conflicts, zero firewall prompts,
                // and completely immune to browser process delegation or disconnect errors.
                string url = "file:///" + launchHtml.Replace('\\', '/');

                // Load Icon handle for window titlebar injection
                Icon appIcon = null;
                try
                {
                    string icoPath = Path.Combine(localAppDir, "app.ico");
                    if (File.Exists(icoPath))
                    {
                        appIcon = new Icon(icoPath);
                    }
                }
                catch { }

                // Locate Microsoft Edge
                string edgePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe");
                if (!File.Exists(edgePath))
                {
                    edgePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application\msedge.exe");
                }

                if (File.Exists(edgePath))
                {
                    string profileDir = Path.Combine(localAppDir, "Profile");
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = edgePath;
                    // Launch Edge maximized with dedicated app profile
                    psi.Arguments = "--app=\"" + url + "\" --start-maximized --user-data-dir=\"" + profileDir + "\"";
                    psi.UseShellExecute = true;
                    Process edgeProc = Process.Start(psi);

                    // Track window handle to inject icon and maintain LocalServer update bridge
                    IntPtr targetHwnd = IntPtr.Zero;
                    if (appIcon != null)
                    {
                        IntPtr hIcon = appIcon.Handle;
                        for (int i = 0; i < 40; i++)
                        {
                            Thread.Sleep(200);
                            EnumWindows((hWnd, lParam) =>
                            {
                                StringBuilder sb = new StringBuilder(256);
                                GetWindowText(hWnd, sb, 256);
                                string title = sb.ToString();
                                if (title.IndexOf("IT Support Ultimate Toolkit", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_SMALL, hIcon);
                                    SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_BIG, hIcon);
                                    targetHwnd = hWnd;
                                    return false;
                                }
                                return true;
                            }, IntPtr.Zero);

                            if (targetHwnd != IntPtr.Zero) break;
                        }
                    }

                    // Keep launcher alive while application window is open so LocalServer update bridge stays active
                    int consecutiveMisses = 0;
                    while (true)
                    {
                        Thread.Sleep(1200);
                        bool windowFound = false;
                        EnumWindows((hWnd, lParam) =>
                        {
                            StringBuilder sb = new StringBuilder(256);
                            GetWindowText(hWnd, sb, 256);
                            string title = sb.ToString();
                            if (title.IndexOf("IT Support Ultimate Toolkit", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                windowFound = true;
                                if (appIcon != null && targetHwnd == IntPtr.Zero)
                                {
                                    SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_SMALL, appIcon.Handle);
                                    SendMessage(hWnd, WM_SETICON, (IntPtr)ICON_BIG, appIcon.Handle);
                                }
                                targetHwnd = hWnd;
                                return false;
                            }
                            return true;
                        }, IntPtr.Zero);

                        if (windowFound)
                        {
                            consecutiveMisses = 0;
                        }
                        else
                        {
                            consecutiveMisses++;
                            // Only exit if window is closed for 3 consecutive checks (3.6s)
                            if (consecutiveMisses >= 3)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Process.Start(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting IT Support Toolkit: " + ex.Message, "IT Support Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ExtractResource(Assembly assembly, string resourceName, string targetFilePath)
        {
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (FileStream fileStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Lightweight Embedded Local Web Server & Native File Updater
    /// Serves files locally with zero latency (100% offline) and performs physical on-disk file updates!
    /// </summary>
    class LocalServer
    {
        private HttpListener listener;
        private int port = 39281;
        private string webRoot;
        private string launchHtmlPath;
        private Thread serverThread;
        private volatile bool running = false;

        public int Port { get { return port; } }
        public bool IsRunning { get { return running; } }

        public LocalServer(string rootDir, string htmlPath)
        {
            webRoot = rootDir;
            launchHtmlPath = htmlPath;
        }

        public bool Start()
        {
            for (int p = 39281; p <= 39300; p++)
            {
                try
                {
                    listener = new HttpListener();
                    listener.Prefixes.Add("http://127.0.0.1:" + p + "/");
                    listener.Start();
                    port = p;
                    running = true;
                    break;
                }
                catch
                {
                    if (listener != null)
                    {
                        try { listener.Close(); } catch { }
                        listener = null;
                    }
                }
            }

            if (!running) return false;

            serverThread = new Thread(ListenLoop);
            serverThread.IsBackground = true;
            serverThread.Start();
            return true;
        }

        private void ListenLoop()
        {
            while (running && listener != null && listener.IsListening)
            {
                try
                {
                    var ctx = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, ctx);
                }
                catch
                {
                    if (!running) break;
                }
            }
        }

        private void ProcessRequest(object state)
        {
            var ctx = (HttpListenerContext)state;
            var req = ctx.Request;
            var res = ctx.Response;

            try
            {
                // Add CORS headers for unrestricted local frontend fetch
                res.Headers["Access-Control-Allow-Origin"] = "*";
                res.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";
                res.Headers["Access-Control-Allow-Headers"] = "Content-Type, Accept";

                if (req.HttpMethod == "OPTIONS")
                {
                    res.StatusCode = 204;
                    res.Close();
                    return;
                }

                string rawPath = req.Url.AbsolutePath.TrimStart('/');
                if (string.IsNullOrEmpty(rawPath) || rawPath == "index.html")
                {
                    // Serve physical index.html from disk
                    if (File.Exists(launchHtmlPath))
                    {
                        byte[] htmlBytes = File.ReadAllBytes(launchHtmlPath);
                        res.ContentType = "text/html; charset=utf-8";
                        res.ContentLength64 = htmlBytes.Length;
                        res.OutputStream.Write(htmlBytes, 0, htmlBytes.Length);
                        res.Close();
                        return;
                    }
                }

                // API: Version Check & Update Query
                if (rawPath == "api/version")
                {
                    string json = GetVersionJson();
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                    res.ContentType = "application/json; charset=utf-8";
                    res.ContentLength64 = jsonBytes.Length;
                    res.OutputStream.Write(jsonBytes, 0, jsonBytes.Length);
                    res.Close();
                    return;
                }

                // API: Perform Real File Update on Disk
                if (rawPath == "api/update")
                {
                    string resultJson = PerformDiskUpdate(req);
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(resultJson);
                    res.ContentType = "application/json; charset=utf-8";
                    res.ContentLength64 = jsonBytes.Length;
                    res.OutputStream.Write(jsonBytes, 0, jsonBytes.Length);
                    res.Close();
                    return;
                }

                // API: Restart Application
                if (rawPath == "api/restart")
                {
                    byte[] okBytes = Encoding.UTF8.GetBytes("{\"success\":true,\"restarting\":true}");
                    res.ContentType = "application/json; charset=utf-8";
                    res.OutputStream.Write(okBytes, 0, okBytes.Length);
                    res.Close();

                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        Thread.Sleep(600);
                        RestartProcess();
                    });
                    return;
                }

                // Serve static assets from local application folder
                string assetPath = Path.Combine(webRoot, rawPath);
                if (!File.Exists(assetPath))
                {
                    // Try current working dir / exe dir
                    assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rawPath);
                }

                if (File.Exists(assetPath))
                {
                    string ext = Path.GetExtension(assetPath).ToLower();
                    if (ext == ".png") res.ContentType = "image/png";
                    else if (ext == ".ico") res.ContentType = "image/x-icon";
                    else if (ext == ".json") res.ContentType = "application/json";
                    else if (ext == ".css") res.ContentType = "text/css";
                    else if (ext == ".js") res.ContentType = "application/javascript";
                    else res.ContentType = "application/octet-stream";

                    byte[] assetBytes = File.ReadAllBytes(assetPath);
                    res.ContentLength64 = assetBytes.Length;
                    res.OutputStream.Write(assetBytes, 0, assetBytes.Length);
                    res.Close();
                    return;
                }

                // 404 fallback
                res.StatusCode = 404;
                byte[] notFound = Encoding.UTF8.GetBytes("404 Not Found");
                res.OutputStream.Write(notFound, 0, notFound.Length);
                res.Close();
            }
            catch (Exception ex)
            {
                try
                {
                    res.StatusCode = 500;
                    byte[] err = Encoding.UTF8.GetBytes("{\"error\":\"" + ex.Message.Replace("\"", "'") + "\"}");
                    res.OutputStream.Write(err, 0, err.Length);
                    res.Close();
                }
                catch { }
            }
        }

        private string GetVersionJson()
        {
            string currentVer = "3.6.0";
            string latestVer = "3.6.0";
            bool hasUpdate = false;
            long diskBytes = 0;

            try
            {
                if (File.Exists(launchHtmlPath))
                {
                    FileInfo fi = new FileInfo(launchHtmlPath);
                    diskBytes = fi.Length;
                }
            }
            catch { }

            // Enforce modern TLS for GitHub HTTPS connections
            try { ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls; } catch { }

            // Priority 1: Check GitHub Cloud Manifest (accessible anywhere worldwide)
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "ITToolkit-CloudUpdater/3.6");
                    string cloudJson = wc.DownloadString("https://raw.githubusercontent.com/Zenji168168/it-support-ultimate-toolkit/main/version.json?t=" + DateTime.UtcNow.Ticks);
                    if (!string.IsNullOrEmpty(cloudJson) && cloudJson.Contains("latestVersion"))
                    {
                        int idx = cloudJson.IndexOf("\"latestVersion\":") + 16;
                        int s = cloudJson.IndexOf('"', idx) + 1;
                        int e = cloudJson.IndexOf('"', s);
                        if (s > 0 && e > s)
                        {
                            latestVer = cloudJson.Substring(s, e - s);
                            hasUpdate = (latestVer != currentVer);
                        }
                    }
                }
            }
            catch
            {
                // Fallback: Check local dev server on localhost:3000
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("User-Agent", "ITToolkit-Updater/3.6");
                        string remoteJson = wc.DownloadString("http://localhost:3000/api/version?current=" + currentVer);
                        if (!string.IsNullOrEmpty(remoteJson) && remoteJson.Contains("latestVersion"))
                        {
                            int idx = remoteJson.IndexOf("\"latestVersion\":") + 16;
                            int s = remoteJson.IndexOf('"', idx) + 1;
                            int e = remoteJson.IndexOf('"', s);
                            if (s > 0 && e > s)
                            {
                                latestVer = remoteJson.Substring(s, e - s);
                                hasUpdate = (latestVer != currentVer);
                            }
                        }
                    }
                }
                catch { }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"currentVersion\":\"").Append(currentVer).Append("\",");
            sb.Append("\"latestVersion\":\"").Append(latestVer).Append("\",");
            sb.Append("\"isUpdateAvailable\":").Append(hasUpdate ? "true" : "false").Append(",");
            sb.Append("\"toolsCount\":74,");
            sb.Append("\"author\":\"MEUK THAREACH\",");
            sb.Append("\"nativeBridge\":true,");
            sb.Append("\"cloudProvider\":\"GitHub (Zenji168168)\",");
            sb.Append("\"port\":").Append(port).Append(",");
            sb.Append("\"diskBytes\":").Append(diskBytes).Append(",");
            sb.Append("\"installedPath\":\"").Append(launchHtmlPath.Replace("\\", "\\\\")).Append("\"");
            sb.Append("}");
            return sb.ToString();
        }

        private string PerformDiskUpdate(HttpListenerRequest req)
        {
            try
            {
                // Enforce modern TLS for GitHub HTTPS downloads
                try { ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls; } catch { }

                string updateUrl = "https://raw.githubusercontent.com/Zenji168168/it-support-ultimate-toolkit/main/index.html";
                if (!string.IsNullOrEmpty(req.QueryString["url"]))
                {
                    updateUrl = req.QueryString["url"];
                }
                else if (req.HasEntityBody)
                {
                    using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                    {
                        string body = reader.ReadToEnd();
                        if (body.Contains("\"url\":"))
                        {
                            int idx = body.IndexOf("\"url\":") + 6;
                            int start = body.IndexOf('"', idx) + 1;
                            int end = body.IndexOf('"', start);
                            if (start > 0 && end > start)
                            {
                                updateUrl = body.Substring(start, end - start);
                            }
                        }
                    }
                }

                // 1. Download updated file into temporary path
                string tempFile = launchHtmlPath + ".download";
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "ITToolkit-DiskUpdater/3.5");
                    wc.DownloadFile(updateUrl, tempFile);
                }

                // 2. Validate file integrity: > 100 KB and contains signature
                FileInfo fi = new FileInfo(tempFile);
                if (!fi.Exists || fi.Length < 100000)
                {
                    try { if (fi.Exists) fi.Delete(); } catch { }
                    return "{\"success\":false,\"error\":\"Downloaded package size is invalid (" + (fi.Exists ? fi.Length : 0) + " bytes). Expected > 100 KB.\"}";
                }

                string headerPart;
                using (StreamReader sr = new StreamReader(tempFile))
                {
                    char[] buf = new char[8192];
                    int r = sr.Read(buf, 0, buf.Length);
                    headerPart = new string(buf, 0, r);
                }

                if (!headerPart.Contains("IT Support Ultimate Toolkit") && !headerPart.Contains("MEUK THAREACH"))
                {
                    try { File.Delete(tempFile); } catch { }
                    return "{\"success\":false,\"error\":\"Downloaded file does not match IT Support Ultimate Toolkit signature.\"}";
                }

                // 3. Create backup copy of existing file
                string bakFile = launchHtmlPath + ".bak";
                try
                {
                    if (File.Exists(bakFile)) File.Delete(bakFile);
                    if (File.Exists(launchHtmlPath)) File.Copy(launchHtmlPath, bakFile);
                }
                catch { }

                // 4. Overwrite physical file on disk
                File.Copy(tempFile, launchHtmlPath, true);
                try { File.Delete(tempFile); } catch { }

                long newBytes = new FileInfo(launchHtmlPath).Length;
                StringBuilder res = new StringBuilder();
                res.Append("{");
                res.Append("\"success\":true,");
                res.Append("\"message\":\"Successfully updated index.html on disk!\",");
                res.Append("\"updatedBytes\":").Append(newBytes).Append(",");
                res.Append("\"diskPath\":\"").Append(launchHtmlPath.Replace("\\", "\\\\")).Append("\"");
                res.Append("}");
                return res.ToString();
            }
            catch (Exception ex)
            {
                return "{\"success\":false,\"error\":\"Update failed: " + ex.Message.Replace("\"", "'") + "\"}";
            }
        }

        private void RestartProcess()
        {
            try
            {
                string exe = Assembly.GetExecutingAssembly().Location;
                Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
                Environment.Exit(0);
            }
            catch { }
        }
    }
}
