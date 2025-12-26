@echo off
setlocal enabledelayedexpansion

:: SharpMediaInfo Context Menu Installer (Batch)
:: This script will call the PowerShell installer
:: NO ADMIN REQUIRED - Installs for current user only

echo ============================================
echo SharpMediaInfo Context Menu Installer
echo ============================================
echo.
echo NOTE: No administrator privileges required
echo Installation scope: Current user only
echo.

echo Launching PowerShell installer...
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0install-context-menu.ps1"

echo.
echo Done!
pause
