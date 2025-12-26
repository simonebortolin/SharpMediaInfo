# SharpMediaInfo Context Menu Installer
# Run: powershell -ExecutionPolicy Bypass -File install-context-menu.ps1
# NOTE: NO ADMIN REQUIRED - Installs for current user only

param(
    [string]$ExePath = ""
)

# Auto-detect exe path if not provided
if ([string]::IsNullOrEmpty($ExePath)) {
    $ExePath = Join-Path $PSScriptRoot "..\SharpMediaInfo.GUI.WPF\bin\Debug\net10.0-windows\SharpMediaInfo.GUI.WPF.exe"
    
    if (-not (Test-Path $ExePath)) {
        $ExePath = Join-Path $PSScriptRoot "..\SharpMediaInfo.GUI.WPF\bin\Release\net10.0-windows\SharpMediaInfo.GUI.WPF.exe"
    }
    
    if (-not (Test-Path $ExePath)) {
        Write-Host "ERROR: Could not find SharpMediaInfo.GUI.WPF.exe" -ForegroundColor Red
        Write-Host "Please specify the path manually:" -ForegroundColor Yellow
        Write-Host "  .\install-context-menu.ps1 -ExePath 'C:\Path\To\SharpMediaInfo.GUI.WPF.exe'" -ForegroundColor Cyan
        pause
        exit 1
    }
}

$ExePath = (Resolve-Path $ExePath).Path
Write-Host "Installing context menu for: $ExePath" -ForegroundColor Green
Write-Host "Installation scope: Current user (no admin required)" -ForegroundColor Cyan

# Registry paths (HKEY_CURRENT_USER - no admin needed!)
$fileShellPath = "Registry::HKEY_CURRENT_USER\Software\Classes\*\shell\SharpMediaInfo"
$folderShellPath = "Registry::HKEY_CURRENT_USER\Software\Classes\Directory\shell\SharpMediaInfo"

# Function to create registry entries
function Set-ContextMenu {
    param(
        [string]$Path,
        [string]$DisplayName,
        [string]$ExePath
    )
    
    # Create main key
    if (-not (Test-Path $Path)) {
        New-Item -Path $Path -Force | Out-Null
    }
    Set-ItemProperty -Path $Path -Name "(Default)" -Value $DisplayName
    Set-ItemProperty -Path $Path -Name "Icon" -Value "`"$ExePath`",0"
    
    # Create command subkey
    $commandPath = Join-Path $Path "command"
    if (-not (Test-Path $commandPath)) {
        New-Item -Path $commandPath -Force | Out-Null
    }
    Set-ItemProperty -Path $commandPath -Name "(Default)" -Value "`"$ExePath`" `"%1`""
    
    Write-Host "  ✓ Created: $Path" -ForegroundColor Green
}

try {
    Write-Host "`nInstalling context menu entries..." -ForegroundColor Cyan
    
    # Install for all files
    Set-ContextMenu -Path $fileShellPath -DisplayName "Analyze with SharpMediaInfo" -ExePath $ExePath
    
    # Install for folders
    Set-ContextMenu -Path $folderShellPath -DisplayName "Analyze folder with SharpMediaInfo" -ExePath $ExePath
    
    Write-Host "`n✅ Context menu installed successfully!" -ForegroundColor Green
    Write-Host "`nYou can now right-click any file and select 'Analyze with SharpMediaInfo'" -ForegroundColor Yellow
    Write-Host "`nNote: On Windows 11, look for it under 'Show more options'" -ForegroundColor Cyan
    Write-Host "`nScope: Current user only (no admin required)" -ForegroundColor Gray
    
} catch {
    Write-Host "`n❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
