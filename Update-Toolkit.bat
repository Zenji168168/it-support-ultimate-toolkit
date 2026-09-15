@echo off
title IT Support Ultimate Toolkit - In-App File Synchronizer
color 0b

echo ========================================================
echo     IT Support Ultimate Toolkit - Automated Updater
echo               Created by MEUK THAREACH
echo ========================================================
echo.
echo [1/3] Connecting to update source & checking local packages...
echo [2/3] Synchronizing latest index.html directly to disk...

powershell -ExecutionPolicy Bypass -Command "$ProgressPreference = 'SilentlyContinue'; $target = '%~dp0index.html'; $tmp = '%~dp0index.html.tmp'; $dl = [Environment]::GetFolderPath('UserProfile') + '\Downloads\index.html'; $updated = $false; if (Test-Path $dl) { $dlItem = Get-Item $dl; if ($dlItem.Length -gt 100000) { Copy-Item $dl $target -Force; Write-Host 'SUCCESS: Synchronized latest index.html from Downloads folder!' -ForegroundColor Green; $updated = $true } }; if (-not $updated) { try { Invoke-WebRequest -Uri 'http://localhost:3000/index.html' -OutFile $tmp -UseBasicParsing; if ((Get-Item $tmp).Length -gt 100000) { Move-Item -Path $tmp -Destination $target -Force; Write-Host 'SUCCESS: Toolkit updated from local server!' -ForegroundColor Green; $updated = $true } else { Write-Host 'Download incomplete.' -ForegroundColor Red } } catch { Write-Host ('Notice: ' + $_.Exception.Message) -ForegroundColor Yellow } }; if ($updated) { Write-Host 'All 74 IT Tools and Playbooks ready!' -ForegroundColor Cyan } else { Write-Host 'Using existing offline version.' -ForegroundColor Gray }"

echo.
echo [3/3] Relaunching IT Support Ultimate Toolkit...
timeout /t 1 /nobreak >nul

if exist "%~dp0ITSupportToolkit.exe" (
    start "" "%~dp0ITSupportToolkit.exe"
    exit
)
if exist "%~dp0IT-Support-Toolkit.exe" (
    start "" "%~dp0IT-Support-Toolkit.exe"
    exit
)
start msedge --app="%~dp0index.html" --start-maximized
exit
