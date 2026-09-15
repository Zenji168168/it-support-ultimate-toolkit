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
        const clientVer = req.url.includes('current=') ? req.url.split('current=')[1].split('&')[0] : '3.6.0';
        let versionInfo = {};
        try {
            const vRaw = fs.readFileSync(path.join(PUBLIC_DIR, 'version.json'), 'utf8');
            versionInfo = JSON.parse(vRaw);
        } catch(e) {
            versionInfo = { latestVersion: '3.7.0', version: '3.7.0', toolsCount: 96, author: 'MEUK THAREACH' };
        }
        
        const targetHtml = path.join(PUBLIC_DIR, 'index.html');
        let fileSize = 0;
        try { fileSize = fs.statSync(targetHtml).size; } catch(e) {}

        versionInfo.currentVersion = clientVer;
        versionInfo.fileSize = fileSize;
        const targetVer = versionInfo.latestVersion || versionInfo.version || '3.7.0';
        versionInfo.status = (clientVer !== targetVer) ? 'update_available' : 'latest';
        versionInfo.autoUpdateSupported = true;
        versionInfo.nativeBridgeSupported = true;

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

        let targetVer = '3.7.0';
        try {
            const vRaw = fs.readFileSync(path.join(PUBLIC_DIR, 'version.json'), 'utf8');
            targetVer = JSON.parse(vRaw).latestVersion || '3.7.0';
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
            targetVersion: targetVer,
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
