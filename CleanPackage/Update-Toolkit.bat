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

powershell -ExecutionPolicy Bypass -Command "$ProgressPreference = 'SilentlyContinue'; $target = '%~dp0index.html'; $tmp = '%~dp0index.html.tmp'; $urlCloud = 'https://raw.githubusercontent.com/Zenji168168/it-support-ultimate-toolkit/main/index.html'; $urlLocal = 'http://localhost:3000/index.html'; $updated = $false; try { Invoke-WebRequest -Uri $urlLocal -OutFile $tmp -UseBasicParsing -TimeoutSec 3; if ((Test-Path $tmp) -and (Get-Item $tmp).Length -gt 500000) { Move-Item -Path $tmp -Destination $target -Force; Write-Host 'SUCCESS: Toolkit updated from local master server!' -ForegroundColor Green; $updated = $true } } catch {}; if (-not $updated) { try { Write-Host 'Downloading latest update from GitHub Cloud...' -ForegroundColor Cyan; Invoke-WebRequest -Uri $urlCloud -OutFile $tmp -UseBasicParsing -TimeoutSec 30; if ((Test-Path $tmp) -and (Get-Item $tmp).Length -gt 500000) { Move-Item -Path $tmp -Destination $target -Force; Write-Host 'SUCCESS: Toolkit updated from GitHub Cloud!' -ForegroundColor Green; $updated = $true } } catch { Write-Host ('Cloud sync notice: ' + $_.Exception.Message) -ForegroundColor Yellow } }; if ($updated) { Write-Host 'All 130 IT Tools and Playbooks ready!' -ForegroundColor Green } else { Write-Host 'Using existing offline version.' -ForegroundColor Gray }"

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
