@echo off
setlocal enabledelayedexpansion
title IT Support Ultimate Toolkit - Automated Cloud Publisher
color 0b

echo ========================================================
echo     IT Support Ultimate Toolkit - 1-Click Cloud Publisher
echo               Created by MEUK THAREACH
echo ========================================================
echo.

:: 1. Read Current Version from version.json
for /f "tokens=2 delims=:, " %%a in ('findstr "latestVersion" "%~dp0version.json"') do (
    set "CURRENT_VER=%%~a"
)
set "CURRENT_VER=%CURRENT_VER:"=%"
set "CURRENT_VER=%CURRENT_VER: =%"

if "%CURRENT_VER%"=="" set "CURRENT_VER=3.6.0"

echo Current Toolkit Version: v%CURRENT_VER%
set /p "NEW_VER=Enter Version for this Release (Press Enter to keep %CURRENT_VER%): "
if "%NEW_VER%"=="" set "NEW_VER=%CURRENT_VER%"

echo.
echo ========================================================
echo Publishing IT Support Ultimate Toolkit v%NEW_VER%...
echo ========================================================
echo.

:: 2. Sync CleanPackage folder
echo [1/6] Synchronizing CleanPackage files...
if not exist "%~dp0CleanPackage" mkdir "%~dp0CleanPackage"
copy /y "%~dp0index.html" "%~dp0CleanPackage\index.html" >nul
copy /y "%~dp0app.ico" "%~dp0CleanPackage\app.ico" >nul
copy /y "%~dp0app-icon.png" "%~dp0CleanPackage\app-icon.png" >nul
copy /y "%~dp0version.json" "%~dp0CleanPackage\version.json" >nul
copy /y "%~dp0README.md" "%~dp0CleanPackage\README.md" >nul

:: 3. Recompile Setup Wizard
echo.
echo [2/6] Compiling IT-Support-Toolkit-Setup.exe with Microsoft .NET C#...
set "CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

"%CSC%" /nologo /target:winexe /win32icon:"%~dp0app.ico" /resource:"%~dp0index.html,index.html" /resource:"%~dp0app-icon.png,app-icon.png" /resource:"%~dp0app.ico,app.ico" /resource:"%~dp0version.json,version.json" /resource:"%~dp0README.md,README.md" /out:"%~dp0IT-Support-Toolkit-Setup.exe" "%~dp0InstallerSource.cs"

if %ERRORLEVEL% neq 0 (
    color 0c
    echo Error: Compilation failed!
    pause
    exit /b %ERRORLEVEL%
)
echo Compiled IT-Support-Toolkit-Setup.exe successfully!

:: 4. Verify with Windows Defender
echo.
echo [3/6] Verifying installer with Windows Defender...
if exist "C:\Program Files\Windows Defender\MpCmdRun.exe" (
    "C:\Program Files\Windows Defender\MpCmdRun.exe" -Scan -ScanType 3 -File "%~dp0IT-Support-Toolkit-Setup.exe"
)

:: 5. Create Portable ZIP & Copy to Downloads
echo.
echo [4/6] Building portable zip & syncing to Downloads folder...
powershell -ExecutionPolicy Bypass -NoProfile -Command "Compress-Archive -Path '%~dp0CleanPackage\*' -DestinationPath '%~dp0IT-Support-Toolkit-Portable.zip' -Force; Copy-Item '%~dp0IT-Support-Toolkit-Portable.zip' '%~dp0IT-Support-Toolkit-v%NEW_VER%.zip' -Force; $dl = Join-Path ([Environment]::GetFolderPath('UserProfile')) 'Downloads\IT-Support-Toolkit-Setup.exe'; Copy-Item '%~dp0IT-Support-Toolkit-Setup.exe' $dl -Force; Write-Host 'Copied to Downloads folder!' -ForegroundColor Green"

:: 6. Git Commit & Push
echo.
echo [5/6] Committing changes to Git repository...
git add -A
git commit -m "Release v%NEW_VER% Enterprise Toolkit - Auto Published by MEUK THAREACH"
echo Pushing to GitHub (Zenji168168/it-support-ultimate-toolkit)...
git push origin main

:: 7. Upload / Update GitHub Release
echo.
echo [6/6] Uploading setup binaries to GitHub Cloud Release (v%NEW_VER%)...
gh release view v%NEW_VER% >nul 2>nul
if %ERRORLEVEL% equ 0 (
    echo Updating existing Release v%NEW_VER%...
    gh release upload v%NEW_VER% "%~dp0IT-Support-Toolkit-Setup.exe" "%~dp0IT-Support-Toolkit-Portable.zip" "%~dp0IT-Support-Toolkit-v%NEW_VER%.zip" --clobber
) else (
    echo Creating brand-new Release v%NEW_VER%...
    gh release create v%NEW_VER% "%~dp0IT-Support-Toolkit-Setup.exe" "%~dp0IT-Support-Toolkit-Portable.zip" "%~dp0IT-Support-Toolkit-v%NEW_VER%.zip" --title "IT Support Ultimate Toolkit v%NEW_VER% Enterprise" --notes "Official Enterprise Release with 110 IT, Network, Sysadmin & CCTV Utilities by MEUK THAREACH"
)

echo.
echo ========================================================
echo  SUCCESS! Toolkit v%NEW_VER% published to GitHub Cloud!
echo  All computers worldwide will now detect this update!
echo ========================================================
pause
