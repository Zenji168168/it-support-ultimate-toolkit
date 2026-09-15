const { app, BrowserWindow, Menu } = require('electron');
const path = require('path');

function createWindow() {
    const win = new BrowserWindow({
        width: 1240,
        height: 880,
        minWidth: 900,
        minHeight: 650,
        title: "IT Support Ultimate Toolkit",
        backgroundColor: '#0f172a',
        webPreferences: {
            nodeIntegration: false,
            contextIsolation: true
        }
    });

    // Remove default menu bar for modern desktop app appearance
    Menu.setApplicationMenu(null);

    // Load the application
    win.loadFile(path.join(__dirname, 'index.html'));

    // Handle external links safely
    win.webContents.setWindowOpenHandler(({ url }) => {
        require('electron').shell.openExternal(url);
        return { action: 'deny' };
    });
}

app.whenReady().then(() => {
    createWindow();

    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) {
            createWindow();
        }
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});
