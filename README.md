# 🛠️ IT Support Ultimate Toolkit (Desktop Edition 🇰🇭 / 🇬🇧)

ឧបករណ៍ជំនួយការងារបច្ចេកវិទ្យាព័ត៌មាន (IT Support, System Administrator & Network Engineer Toolkit)
Created & Developed by **MEUK THAREACH** • Version 3.0 Enterprise Desktop Edition.

---

## 📦 របៀបដំឡើង និងប្រើប្រាស់ (Installation & Distribution)

### ជម្រើសទី ១: Windows Setup Wizard (`IT-Support-Toolkit-Setup.exe`) - **Recommended**
- ដំណើរការឯកសារ `IT-Support-Toolkit-Setup.exe`
- កម្មវិធីនឹងបើកផ្ទាំង Setup Wizard ស្វ័យប្រវត្តិ
- អ្នកអាចជ្រើសរើស Folder ដំឡើង (Default: `%LOCALAPPDATA%\Programs\IT Support Ultimate Toolkit`)
- បង្កើត Shortcut នៅលើ **Desktop** និងក្នុង **Start Menu**
- ចុះឈ្មោះក្នុង Windows Add/Remove Programs (Settings > Installed Apps)
- ភ្ជាប់មកជាមួយ Uninstaller ស្អាត (`Uninstall.exe`) ដើម្បីដកកម្មវិធីចេញវិញនៅពេលត្រូវការ។

### ជម្រើសទី ២: Standalone Native Executable (`IT-Support-Toolkit.exe`)
- ដំណើរការ `IT-Support-Toolkit.exe` ដោយផ្ទាល់
- បើកក្នុងទម្រង់ Native Desktop Window ជាមួយ High-Resolution Icon `app.ico`
- មិនបាច់ដំឡើង (Portable) អាច copy ដាក់ Flash Drive / USB ប្រើប្រាស់បានគ្រប់ទីកន្លែង។

### ជម្រើសទី ៣: Portable ZIP Archive (`IT-Support-Toolkit-Portable.zip`)
- កញ្ចប់ ZIP រួមមាន `.exe`, `Setup.exe`, `index.html`, `app.ico`, `Launch_App.bat`, និង `README.md`។

### ជម្រើសទី ៤: ដំណើរការតាម Local Web Server
```powershell
npm start
# ឬ
node server.js
```
រួចបើក Browser ចូលទៅកាន់: **[http://localhost:3000](http://localhost:3000)**

---

## ⚡ មុខងារពិសេសថ្មីក្នុង Version 3.0 (Pro Desktop Features)
1. **Live System & Hardware Diagnostic Bar**:
   - បង្ហាញព័ត៌មានប្រព័ន្ធប្រតិបត្តិការ (OS & Architecture 64-bit)
   - កម្រិតបង្ហាញអេក្រង់ (Resolution & Color Depth)
   - ចំនួន Logical CPU Cores & RAM
   - ស្ថានភាព Network (Online/Offline) Real-time
   - នាឡិកាឌីជីថល និង Timezone ប្រចាំតំបន់។
2. **Spotlight Command Palette (Ctrl + K / `/`)**:
   - ចុច `Ctrl + K` ឬ `/` នៅលើ Keyboard ដើម្បីបើកផ្ទាំងស្វែងរកឧបករណ៍ទាំង ៤១
   - Filter រហ័សតាម Category (Network, Windows, Linux, Coding, CCTV)
   - ប្រើសញ្ញាព្រួញ ↑ / ↓ និងចុច ↵ Enter ដើម្បីបើកឧបករណ៍ភ្លាមៗ។
3. **Configuration File Exporters**:
   - Export Cisco Switch Config ទៅជា `.cfg` / `.txt`
   - Export MikroTik Script ទៅជា `.rsc`
   - Export Active Directory Script ទៅជា `.ps1`
   - Export SSH Helper ទៅជា `ssh_config`
   - Export RTSP Playlist ទៅជា `.m3u`
   - Export CCTV Storage Calculation Quote ទៅជា `.txt`។
4. **Bilingual Engine (ភាសាខ្មែរ 🇰🇭 / English 🇬🇧)**:
   - ប្តូរភាសាបានភ្លាមៗ 100% គ្រប់ Tools
   - ប្រើប្រាស់ Font **Kantumruy Pro** សម្រាប់ភាសាខ្មែរ និង **Times New Roman** សម្រាប់អក្សរឡាតាំង/English។

---

## 🧰 បញ្ជីមុខងារ និងឧបករណ៍ទាំងអស់ (41 Pro IT & CCTV Tools)

### 🌐 1. Network & Infrastructure (14 Tools)
1. **MAC Format**: បំប្លែង MAC Address ទៅតាម Cisco (`xxxx.xxxx.xxxx`), Windows (`XX-XX-XX-XX-XX-XX`), Linux (`XX:XX:XX:XX:XX:XX`)។
2. **IP Range Calc**: គណនា Network, Broadcast, Usable Range, Host Count តាម CIDR (0-32)។
3. **Cisco Switch Config**: បង្កើត Command កំណត់ VLANs, Access Ports, 802.1Q Trunks, Management SVI, SSH Remote Access, និង Write Memory។
4. **MikroTik RouterOS Config**: បង្កើត Script កំណត់ WAN DHCP Client, LAN Bridge, NAT Masquerade, DHCP Server Pool, និង Port Forwarding។
5. **DNS Record Lookup & DoH**: Query DNS Records (A, AAAA, MX, TXT, NS, CNAME) ផ្ទាល់តាម Cloudflare DoH API និងបង្កើត Command `dig` / `Resolve-DnsName`។
6. **Test-NetConnection & Port Ping**: បង្កើត Command សម្រាប់តេស្ត Port TCP និង Continuous Ping លើ Windows PowerShell និង Linux Netcat (`nc -zv`)។
7. **Web Status Check**: ពិនិត្យមើលថាតើ Website កំពុង Online ឬ Offline និងវាស់កម្រិត Latency (ms)។
8. **Subnet Calc**: គណនា Subnet Mask, Usable Hosts, Total IPs, និង Wildcard Mask។
9. **Wildcard Mask**: គណនា Cisco ACL Wildcard Mask ដោយបញ្ចូល Subnet Mask ឬ CIDR។
10. **IP Binary**: បំប្លែង IPv4 ទៅជា 32-bit Dotted Binary និង Hexadecimal។
11. **IPv6 Format**: បង្រួម (Compress RFC 5952) និងពង្រីក (Expand 8 Hextets) សម្រាប់ IPv6 Address។
12. **Wi-Fi QR Code**: បង្កើត QR Code សម្រាប់ទូរស័ព្ទស្កេនភ្ជាប់ Wi-Fi (WPA/WPA2/WPA3, WEP, Open)។
13. **Bandwidth Calc**: គណនារយៈពេលផ្ទេរទិន្នន័យ (Download/Upload Time) តាមទំហំ File និងល្បឿនអ៊ីនធឺណិត។
14. **Port Lookup (85+ Ports)**: ស្វែងរកលេខ Port ឬឈ្មោះ Service (SSH, RDP, DNS, DHCP, HTTP, HTTPS, SMB, MySQL, MSSQL, Postgres, Proxmox, MongoDB...)។

---

### 🪟 2. Windows & Enterprise IT (5 Modules / 80+ Codes)
1. **Active Directory PowerShell Generator**:
   - ដោះសោគណនី (Unlock-ADAccount)
   - Reset Password & បង្ខំប្តូរនៅពេល Login (Set-ADAccountPassword)
   - ស្វែងរកគណនីដែលកំពុងជាប់សោទាំងអស់ (Search-ADAccount -LockedOut)
   - បង្កើត Corporate User ថ្មី (New-ADUser)
   - បញ្ចូល User ទៅ Security Group (Add-ADGroupMember)
   - បិទគណនី (Disable-ADAccount)
   - ពិនិត្យកាលបរិច្ឆេទ Login ចុងក្រោយ និងថ្ងៃ Expire Password
2. **Outlook & Office 365 Repair Guide**:
   - `outlook.exe /safe` (បើក Safe Mode ដោយគ្មាន Add-ins)
   - `outlook.exe /cleanviews` (Reset Layout / Folders View)
   - `outlook.exe /cleanrules` (លុប Rules ដែលគាំង)
   - `outlook.exe /resetnavpane` (ដោះស្រាយ Error 'Cannot start Microsoft Outlook')
   - កំណត់ Rebuild OST Offline Mailbox Cache ឡើងវិញ
   - Microsoft 365 Click-to-Run Quick & Online Repair
3. **BSOD Stop Code Lookup**:
   - ស្វែងរកកូដ Blue Screen (`0x0000007B`, `CRITICAL_PROCESS_DIED`, `PAGE_FAULT_IN_NONPAGED_AREA`, `KERNEL_DATA_INPAGE_ERROR`, `DPC_WATCHDOG_VIOLATION`, `IRQL_NOT_LESS_OR_EQUAL`) រួមជាមួយមូលហេតុ និងដំណោះស្រាយជាក់ស្តែង។
4. **Windows Commands (CLI / PowerShell)**:
   - បណ្ដុំ Command សំខាន់ៗ: `ipconfig /all`, `flushdns`, `netstat -ano`, `sfc /scannow`, `DISM RestoreHealth`, `powercfg batteryreport`, `slmgr /xpr`, `shutdown /r`។
5. **HTTP Status Codes (60+ RFC Codes)**:
   - ស្វែងរកលេខកូដ និងអត្ថន័យនៃ HTTP Status Codes គ្រប់ប្រភេទ (1xx, 2xx, 3xx, 4xx, 5xx) និង Cloudflare Errors។

---

### 🐧 3. Linux & Sysadmin (5 Tools)
1. **Linux CLI Commands**: បណ្ដុំ Command សម្រាប់ Sysadmin (`ip -c a`, `ss -tulpn`, `df -hT`, `free -h`, `journalctl`, `ufw status`, `ps aux top CPU`, ស្វែងរក File ធំៗ)។
2. **SSH Config & Keygen Helper**: បង្កើតកូដបង្កើត SSH Keypair `ed25519` និងបង្កើត Block `~/.ssh/config` ដើម្បី Login ដោយមិនបាច់វាយ IP រាល់ដង ព្រមទាំងកំណត់ Permission `chmod 600`។
3. **CHMOD Calculator**: គណនាសិទ្ធិ File Permissions តាម Checkbox ឬលេខ Octal (755, 644, 777, 600, 700) និងសញ្ញា Symbolic (`-rwxr-xr-x`)។
4. **Cron Generator**: បង្កើត Cron Schedule 5-part ជាមួយការពន្យល់ជាភាសាខ្មែរ/អង់គ្លេស និង Presets រហ័ស។
5. **Config Diff Compare**: ប្រៀបធៀប File/Config Text ពីរ បង្ហាញបន្ទាត់ខុសគ្នា Line-by-Line Highlight (Added/Removed/Same)។

---

### 💻 4. Coding, Web & Security (11 Tools)
1. **Base64 to Image**: Render Base64 String មកជារូបភាព Preview និង Download។
2. **File to Base64**: បំប្លែង File ឬរូបភាពទៅជា Base64 Data URL ជាមួយទំហំ File Size Info។
3. **Telegram Bot API**: បង្កើត Link និង cURL Command សម្រាប់ផ្ញើសារតាម Telegram Bot ដោយស្វ័យប្រវត្តិ។
4. **JWT Decoder**: បំប្លែង និងពិនិត្យមើល Header & Payload នៃ JWT ព្រមទាំងបង្ហាញកាលបរិច្ឆេទ Expire (ACTIVE / EXPIRED)។
5. **UUID Generator**: បង្កើតលេខកូដ UUID / GUID v4 (Single ឬ 5x Batch)។
6. **Cryptographic Hashes**: បង្កើត SHA-256, SHA-512, និង SHA-1។
7. **Base64 Text (UTF-8 Safe)**: Encode & Decode អត្ថបទទៅ Base64 ដោយ**គាំទ្រភាសាខ្មែរ និង Unicode 100% មិន Error**។
8. **URL Encode / Decode**: Encode/Decode Query String សម្រាប់ Web Development។
9. **Secure Password Generator**: បង្កើត Password ចៃដន្យតាមប្រវែង 8-64 characters រួមជាមួយកម្រិតសុវត្ថិភាព Strength Indicator។
10. **JSON Formatter**: Format Beautify (2 spaces / 4 spaces) និង Minify 1 បន្ទាត់ ជាមួយការចាប់ Error Syntax។
11. **Color Converter**: បំប្លែងរវាង HEX, RGB, HSL ជាមួយប្រអប់ជ្រើសរើស Color Picker និង Live Preview។

---

### 📹 5. CCTV & Security Surveillance Pro (6 Tools / 50+ Brands)
1. **RTSP Stream URL Generator**:
   - បង្កើត Direct RTSP Streaming URLs សម្រាប់ម៉ាកល្បីៗ: Hikvision, Dahua, Uniview, TP-Link VIGI, Imou, EZVIZ, Tiandy, Axis, Hanwha (Samsung), Xiongmai (XM/XMeye), Reolink, និង ONVIF Generic Profile S។
   - គាំទ្រ Main Stream (HD/4K) និង Sub Stream (Multi-view/Mobile)។
   - ភ្ជាប់មកជាមួយ VLC Player និង FFplay low-latency CLI streaming test commands។
   - Export ឯកសារ Playlist `.m3u` សម្រាប់បើកផ្ទាល់ក្នុង VLC ឬ OBS Studio។
2. **CCTV Storage & Bandwidth Calculator**:
   - គណនាទំហំ Hard Disk (TB/GB) និងទំហំ Uplink Bandwidth (Mbps) យ៉ាងសុក្រឹត។
   - គាំទ្រ Resolution ពី 1080p (2MP), 3MP, 4MP 2K Quad HD, 5MP, រហូតដល់ 8MP 4K Ultra HD។
   - គណនាបច្ចេកវិទ្យាបង្រួម H.265+, H.265, H.264 និង Frame Rate (15, 20, 25, 30 FPS)។
   - ណែនាំចំនួន និងទំហំ Hard Drive សម្រាប់ Surveillance (WD Purple, Seagate SkyHawk AI) ព្រមទាំង Switch Network Uplink (100M ឬ Gigabit)។
   - Export របាយការណ៍សម្រង់គម្រោង `.txt` Quote សម្រាប់អតិថិជន។
3. **Camera Default IP, Password & Port Directory**:
   - បញ្ជី IP Address, Default Username, Password និង Port សំខាន់ៗ (HTTP, RTSP, Media, Server) សម្រាប់កាមេរ៉ាជាង 20 ម៉ាកល្បីៗ (Hikvision, Dahua, Uniview, VIGI, EZVIZ, Axis, Hanwha, XMeye, CP PLUS, Bosch, Pelco, Jovision...)។
   - Filter ស្វែងរករហ័សតាមម៉ាក ឬលេខ Port ឬ IP Address។
4. **CCTV Problem Solver & Troubleshooting Matrix**:
   - ការណែនាំដោះស្រាយបញ្ហាបច្ចេកទេសជាក់ស្តែងជាជំហានៗ (Step-by-step) ទាំងភាសាខ្មែរ និងអង់គ្លេស:
     - 🔴 កាមេរ៉ា Offline / No Video (ដាច់តភ្ជាប់)
     - 🌑 យប់ឡើងងងឹតស្លុប / Black Screen / IR LEDs មិនដំណើរការ
     - 🔑 ភ្លេច Password / របៀប Hard Reset កាមេរ៉ា & NVR
     - ⚡ ជាន់ IP Address ឬ Camera នៅខុស Subnet (IP Conflict / Cross-Subnet)
     - ⏳ រូបភាពកន្ត្រាក់, យឺតពេលច្រើនវិនាទី ឬ Frame Drops (Video Lag)
     - ⚠️ NVR បង្ហាញ "No HDD" ឬ "HDD Unformatted" (Power brick amp drops, SATA cable, formatting)។
5. **PoE Power Budget & Cable Distance Calculator**:
   - គណនាបន្ទុកថាមពល PoE Switch (802.3af, 802.3at PoE+, 802.3bt PoE++, Passive 24V)។
   - Real-time Visual Usage Meter បង្ហាញ % Load, ថាមពលសល់, និងការប្រុងប្រយ័ត្ន Overload។
   - គណនាការធ្លាក់ចុះតង់ស្យុង (Voltage Drop) តាមប្រវែងខ្សែ និងគុណភាពខ្សែ (Cat6 Pure Copper, Cat5e, ឬ CCA)។
   - ការព្រមានអំពីដែនកំណត់ប្រវែងខ្សែ 100 ម៉ែត្រ និងតម្រូវការ Extend Mode (10Mbps / 250m)។
6. **Remote Viewing & Port Forwarding Guide**:
   - ការកំណត់មើលពីចម្ងាយតាម P2P Cloud (Hik-Connect, DMSS, EZView, TP-Link VIGI) ដោយមិនចាំបាច់មាន Public IP។
   - បង្កើត Script MikroTik RouterOS DST-NAT Rules ស្វ័យប្រវត្តិ (Web GUI, RTSP Stream, Media Client Port, និង Hairpin NAT Loopback) ព្រមទាំងអាច Export ជា `.rsc` បានភ្លាមៗ។

---

## 👨‍💻 អ្នកអភិវឌ្ឍន៍ (Author)
- **អ្នកបង្កើត (Author)**: MeukTHAREACH
- **កំណែ (Version)**: 3.0 Enterprise Desktop Edition (Bilingual Edition • 41 Pro IT Tools)

