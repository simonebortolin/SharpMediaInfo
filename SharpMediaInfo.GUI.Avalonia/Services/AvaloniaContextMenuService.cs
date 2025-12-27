using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Services;

/// <summary>
/// Avalonia context menu service - No-op implementation
/// Context menu integration is platform-specific and not supported in cross-platform Avalonia apps
/// </summary>
public class AvaloniaContextMenuService : IContextMenuService
{
    public string GetExecutablePath()
    {
        var process = Process.GetCurrentProcess();
        return Environment.ProcessPath ?? process.MainModule?.FileName ?? string.Empty;
    }

    public Task<bool> InstallAsync(string exePath)
    {
        // Not supported in cross-platform Avalonia
        return Task.FromResult(false);
    }

    public Task<bool> IsInstalledAsync()
    {
        // Not supported in cross-platform Avalonia
        return Task.FromResult(false);
    }

    public Task ShowContextMenuAsync(object? dataContext, IEnumerable<object> menuItems)
    {
        // Context menu will be handled directly in XAML for Avalonia
        return Task.CompletedTask;
    }

    public Task<bool> UninstallAsync()
    {
        // Not supported in cross-platform Avalonia
        return Task.FromResult(false);
    }
}
