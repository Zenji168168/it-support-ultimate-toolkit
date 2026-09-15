@echo off
title IT Support Ultimate Toolkit Launcher
color 0b

echo ===================================================
echo     IT Support Ultimate Toolkit - Desktop Edition
echo               Build by MeukTHAREACH
echo ===================================================
echo Launching Toolkit in Native Desktop App Window...

:: If compiled native launcher is present, run it with embedded updater bridge
if exist "%~dp0ITSupportToolkit.exe" (
    start "" "%~dp0ITSupportToolkit.exe"
    exit
)
if exist "%~dp0IT-Support-Toolkit.exe" (
    start "" "%~dp0IT-Support-Toolkit.exe"
    exit
)

:: Try launching in Edge App Mode (Standalone window, no browser tabs/URL bar)
where msedge >nul 2>nul
if %errorlevel% equ 0 (
    start "" /max msedge --app="%~dp0index.html" --start-maximized
    exit
)

:: Try launching in Chrome App Mode
where chrome >nul 2>nul
if %errorlevel% equ 0 (
    start "" /max chrome --app="%~dp0index.html" --start-maximized
    exit
)

:: Fallback to default browser
start "" "%~dp0index.html"
exit
