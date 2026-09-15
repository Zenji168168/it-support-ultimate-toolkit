@echo off
title IT Support Ultimate Toolkit - Installer
color 0a

echo ========================================================
echo     IT Support Ultimate Toolkit - Quick Installer
echo               Created by MEUK THAREACH
echo ========================================================
echo.

set "TARGET=%LOCALAPPDATA%\Programs\IT Support Ultimate Toolkit"
if not exist "%TARGET%" mkdir "%TARGET%"

echo [1/3] Copying files to %TARGET%...
copy /y "%~dp0index.html" "%TARGET%\index.html" >nul
copy /y "%~dp0app.ico" "%TARGET%\app.ico" >nul
copy /y "%~dp0app-icon.png" "%TARGET%\app-icon.png" >nul
copy /y "%~dp0Launch-Toolkit.bat" "%TARGET%\Launch-Toolkit.bat" >nul
copy /y "%~dp0version.json" "%TARGET%\version.json" >nul

echo [2/3] Creating Desktop Shortcut...
powershell -ExecutionPolicy Bypass -NoProfile -Command "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut([Environment]::GetFolderPath('Desktop') + '\IT Support Ultimate Toolkit.lnk'); $s.TargetPath = 'msedge.exe'; $s.Arguments = '--app=\"file:///' + ($env:LOCALAPPDATA -replace '\\', '/') + '/Programs/IT Support Ultimate Toolkit/index.html\" --start-maximized'; $s.WindowStyle = 3; $s.IconLocation = $env:LOCALAPPDATA + '\Programs\IT Support Ultimate Toolkit\app.ico, 0'; $s.Description = 'IT Support Ultimate Toolkit Desktop Edition by MEUK THAREACH'; $s.Save()"

echo [3/3] Launching application...
start "" /max msedge --app="file:///%TARGET:\=/%/index.html" --start-maximized
echo.
echo ========================================================
echo  SUCCESS! Installed to Desktop successfully!
echo ========================================================
timeout /t 2 >nul
exit
