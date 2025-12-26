using Avalonia.Controls;
using SharpMediaInfo.GUI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaContextMenuService : IContextMenuService
{
    public string GetExecutablePath()
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> InstallAsync(string exePath)
    {
        throw new System.NotImplementedException();
    }

    public Task<bool> IsInstalledAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task ShowContextMenuAsync(object? dataContext, IEnumerable<object> menuItems)
    {
        // Context menu will be handled directly in XAML for Avalonia
        // This service is a placeholder for future enhancements
        return Task.CompletedTask;
    }

    public Task<bool> UninstallAsync()
    {
        throw new System.NotImplementedException();
    }
}
