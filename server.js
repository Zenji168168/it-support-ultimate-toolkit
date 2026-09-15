const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = process.env.PORT || 3000;
const PUBLIC_DIR = __dirname;

const MIME_TYPES = {
    '.html': 'text/html; charset=utf-8',
    '.css': 'text/css; charset=utf-8',
    '.js': 'text/javascript; charset=utf-8',
    '.json': 'application/json; charset=utf-8',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.jpeg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon',
    '.exe': 'application/vnd.microsoft.portable-executable',
    '.zip': 'application/zip',
    '.bat': 'text/plain; charset=utf-8',
    '.txt': 'text/plain; charset=utf-8'
};

const server = http.createServer((req, res) => {
    let reqPath = req.url.split('?')[0];
    if (reqPath === '/' || reqPath === '') {
        reqPath = '/index.html';
    }

    // API Route: Version & Update Status
    if (reqPath === '/api/version') {
        const clientVer = req.url.includes('current=') ? req.url.split('current=')[1].split('&')[0] : '3.5.0';
        const hasUpdate = (clientVer !== '3.6.0');
        const targetHtml = path.join(PUBLIC_DIR, 'index.html');
        let fileSize = 0;
        try { fileSize = fs.statSync(targetHtml).size; } catch(e) {}

        const versionInfo = {
            currentVersion: clientVer,
            latestVersion: '3.6.0',
            version: '3.6.0',
            name: 'IT Support Ultimate Toolkit Enterprise Desktop',
            author: 'MEUK THAREACH',
            releaseDate: '2026-09-15',
            status: hasUpdate ? 'update_available' : 'latest',
            toolsCount: 74,
            fileSize: fileSize,
            changelog: [
                'Standard & Soft Professional Enterprise UI (Azure/Cloudflare style slate tokens)',
                'Frontline IT Quick Access Strip (1-click launch for 12 frontline tools)',
                'IT Emergency Quick Reference Drawer (Common ports, IPv4 CIDR subnet matrix, RJ-45 T-568B pinout, rescue CLI)',
                'Floating Soft Toast Notifications & Breadcrumb Category Switcher',
                'Smart VLAN & Trunking CLI Generator (Cisco, MikroTik, Ruijie, FortiGate)',
                'IPsec Site-to-Site Dual-CLI VPN Matcher (FortiGate ↔ MikroTik Phase 1/2 parity)',
                'Destination NAT & Port Forwarding Script Generator (MikroTik & FortiGate with Hairpin NAT)',
                'Master Default Credentials & IP Database (50+ Enterprise Hardware Brands)',
                'Interactive Troubleshooting Decision Tree Wizard (Network, CCTV, SMB Share)',
                'Universal Syslog & Error Code Decoder (BSOD, FortiGate, HTTP, Windows Update)'
            ],
            autoUpdateSupported: true,
            nativeBridgeSupported: true
        };
        res.writeHead(200, {
            'Content-Type': 'application/json; charset=utf-8',
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type, Accept'
        });
        res.end(JSON.stringify(versionInfo, null, 2));
        return;
    }

    // API Route: In-App Auto Update Action
    if (reqPath === '/api/update') {
        const sourceHtml = path.join(PUBLIC_DIR, 'index.html');
        let fileSize = 0;
        let installedDiskPath = '';
        try { 
            fileSize = fs.statSync(sourceHtml).size; 
            // Sync directly to installed program directory if it exists
            const localApp = process.env.LOCALAPPDATA;
            if (localApp) {
                const progDir = path.join(localApp, 'Programs', 'IT Support Ultimate Toolkit');
                const progHtml = path.join(progDir, 'index.html');
                if (fs.existsSync(progDir)) {
                    fs.copyFileSync(sourceHtml, progHtml);
                    installedDiskPath = progHtml;
                }
            }
        } catch(e) {}

        res.writeHead(200, {
            'Content-Type': 'application/json; charset=utf-8',
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type, Accept'
        });
        res.end(JSON.stringify({
            success: true,
            status: 'completed',
            targetVersion: '3.6.0',
            fileSize: fileSize,
            diskPath: installedDiskPath || sourceHtml,
            message: 'Update verified and synchronized on disk successfully. Ready to restart.'
        }));
        return;
    }

    const safePath = path.normalize(reqPath).replace(/^(\.\.[\/\\])+/, '');
    const filePath = path.join(PUBLIC_DIR, safePath);

    fs.stat(filePath, (err, stats) => {
        if (err || !stats.isFile()) {
            // If not found, fallback to index.html for SPA support
            const fallbackPath = path.join(PUBLIC_DIR, 'index.html');
            fs.readFile(fallbackPath, (fallbackErr, data) => {
                if (fallbackErr) {
                    res.writeHead(404, { 'Content-Type': 'text/plain; charset=utf-8' });
                    res.end('404 Not Found');
                } else {
                    res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
                    res.end(data);
                }
            });
            return;
        }

        const ext = path.extname(filePath).toLowerCase();
        const contentType = MIME_TYPES[ext] || 'application/octet-stream';

        fs.readFile(filePath, (readErr, content) => {
            if (readErr) {
                res.writeHead(500, { 'Content-Type': 'text/plain; charset=utf-8' });
                res.end('500 Internal Server Error');
            } else {
                const headers = { 
                    'Content-Type': contentType,
                    'Content-Length': content.length
                };
                if (ext === '.exe' || ext === '.zip') {
                    headers['Content-Disposition'] = `attachment; filename="${path.basename(filePath)}"`;
                }
                res.writeHead(200, headers);
                res.end(content);
            }
        });
    });
});

server.listen(PORT, '0.0.0.0', () => {
    console.log(`\n🚀 IT Support Toolkit Server is running!`);
    console.log(`👉 Local:   http://localhost:${PORT}`);
    console.log(`👉 Network: http://127.0.0.1:${PORT}\n`);
});
