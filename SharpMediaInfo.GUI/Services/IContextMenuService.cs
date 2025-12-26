using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Service for managing Windows Explorer context menu integration
/// </summary>
public interface IContextMenuService
{
    /// <summary>
    /// Check if context menu is currently installed
    /// </summary>
    Task<bool> IsInstalledAsync();
    
    /// <summary>
    /// Install context menu entries
    /// </summary>
    /// <param name="exePath">Path to the executable</param>
    Task<bool> InstallAsync(string exePath);
    
    /// <summary>
    /// Uninstall context menu entries
    /// </summary>
    Task<bool> UninstallAsync();
    
    /// <summary>
    /// Get the current executable path
    /// </summary>
    string GetExecutablePath();
}
