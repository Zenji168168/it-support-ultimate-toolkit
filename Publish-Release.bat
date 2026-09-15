@echo off
title IT Support Ultimate Toolkit - Cloud Publisher
color 0a

echo ========================================================
echo     IT Support Ultimate Toolkit - 1-Click Cloud Publisher
echo               Created by MEUK THAREACH
echo ========================================================
echo.

echo [1/4] Checking Git and GitHub CLI status...
git status
if %ERRORLEVEL% neq 0 (
    echo Error: Git not initialized. Initializing now...
    git init
    git branch -M main
)

echo.
echo [2/4] Staging changes and committing to local repository...
git add .
git commit -m "Release v3.6.0 Enterprise Soft UI - 74 Tools and Cloud Auto-Updater"

echo.
echo [3/4] Pushing to GitHub Cloud (Zenji168168/it-support-ultimate-toolkit)...
git push -u origin main

echo.
echo [4/4] Creating or Updating GitHub Cloud Release with setup binaries...
gh release create v3.6.0 IT-Support-Toolkit-Setup.exe ITSupportToolkit.exe IT-Support-Toolkit-Portable.zip --title "IT Support Ultimate Toolkit v3.6.0 Enterprise" --notes "Official Enterprise Soft UI Edition with 74 IT & CCTV Utilities by MEUK THAREACH" --clobber

echo.
echo ========================================================
echo  SUCCESS! Toolkit published to GitHub Cloud!
echo  Any PC worldwide can now check and receive this update!
echo ========================================================
pause
