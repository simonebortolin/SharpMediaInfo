# SharpMediaInfo Context Menu Uninstaller
# Run: powershell -ExecutionPolicy Bypass -File uninstall-context-menu.ps1
# NOTE: NO ADMIN REQUIRED - Removes for current user only

# Registry paths
$paths = @(
    "Registry::HKEY_CURRENT_USER\Software\Classes\*\shell\SharpMediaInfo",
    "Registry::HKEY_CURRENT_USER\Software\Classes\Directory\shell\SharpMediaInfo"
)

Write-Host "Uninstalling SharpMediaInfo context menu..." -ForegroundColor Cyan
Write-Host "Scope: Current user only" -ForegroundColor Gray

$removed = 0
foreach ($path in $paths) {
    if (Test-Path $path) {
        try {
            Remove-Item -Path $path -Recurse -Force
            Write-Host "  ✓ Removed: $path" -ForegroundColor Green
            $removed++
        } catch {
            Write-Host "  ✗ Failed to remove: $path - $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "  ⊘ Not found: $path" -ForegroundColor Gray
    }
}

if ($removed -gt 0) {
    Write-Host "`n✅ Context menu uninstalled successfully! ($removed entries removed)" -ForegroundColor Green
} else {
    Write-Host "`n⚠️ No context menu entries found." -ForegroundColor Yellow
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
