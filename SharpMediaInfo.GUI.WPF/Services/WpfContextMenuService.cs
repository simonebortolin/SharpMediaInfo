using Microsoft.Win32;
using SharpMediaInfo.GUI.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SysException = System.Exception;

namespace SharpMediaInfo.GUI.WPF.Services;

/// <summary>
/// Windows-specific context menu service using Registry
/// </summary>
public class WpfContextMenuService : IContextMenuService
{
    private const string MenuDisplayName = "Analyze with SharpMediaInfo";
    private const string FolderMenuDisplayName = "Analyze folder with SharpMediaInfo";
    
    // Use HKEY_CURRENT_USER for non-admin installation
    private const string UserFileShellPath = @"Software\Classes\*\shell\SharpMediaInfo";
    private const string UserFolderShellPath = @"Software\Classes\Directory\shell\SharpMediaInfo";
    
    // Admin paths (fallback if user has admin rights)
    private const string SystemFileShellPath = @"*\shell\SharpMediaInfo";
    private const string SystemFolderShellPath = @"Directory\shell\SharpMediaInfo";

    public string GetExecutablePath()
    {
        var process = Process.GetCurrentProcess();
        return Environment.ProcessPath ?? process.MainModule?.FileName ?? string.Empty;
    }

    public Task<bool> IsInstalledAsync()
    {
        try
        {
            // Check user-level first
            using var userKey = Registry.CurrentUser.OpenSubKey(UserFileShellPath);
            if (userKey != null)
                return Task.FromResult(true);

            // Check system-level
            using var systemKey = Registry.ClassesRoot.OpenSubKey(SystemFileShellPath);
            return Task.FromResult(systemKey != null);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<bool> InstallAsync(string exePath)
    {
        if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
        {
            return false;
        }

        try
        {
            // Try user-level installation first (no admin needed)
            await Task.Run(() =>
            {
                CreateUserContextMenuEntry(UserFileShellPath, MenuDisplayName, exePath);
                CreateUserContextMenuEntry(UserFolderShellPath, FolderMenuDisplayName, exePath);
            });

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // Shouldn't happen with HKCU, but just in case
            Debug.WriteLine("❌ Unexpected: UnauthorizedAccessException with HKCU");
            return false;
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"❌ Error installing context menu: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UninstallAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                // Remove user-level entries
                try
                {
                    Registry.CurrentUser.DeleteSubKeyTree(UserFileShellPath, false);
                    Registry.CurrentUser.DeleteSubKeyTree(UserFolderShellPath, false);
                }
                catch
                {
                    // Keys might not exist, ignore
                }

                // Try to remove system-level entries if they exist (will fail without admin)
                try
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(SystemFileShellPath, false);
                    Registry.ClassesRoot.DeleteSubKeyTree(SystemFolderShellPath, false);
                }
                catch
                {
                    // Expected if not admin, ignore
                }
            });

            return true;
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"❌ Error uninstalling context menu: {ex.Message}");
            return false;
        }
    }

    private void CreateUserContextMenuEntry(string registryPath, string displayName, string exePath)
    {
        // Create main menu key in HKEY_CURRENT_USER
        using var menuKey = Registry.CurrentUser.CreateSubKey(registryPath);
        if (menuKey != null)
        {
            menuKey.SetValue("", displayName);
            menuKey.SetValue("Icon", $"\"{exePath}\",0");

            // Create command subkey
            using var commandKey = menuKey.CreateSubKey("command");
            if (commandKey != null)
            {
                commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
            }
        }
    }

    private string GenerateInstallRegContent(string exePath)
    {
        // Escape backslashes for registry
        var escapedPath = exePath.Replace(@"\", @"\\");

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Windows Registry Editor Version 5.00");
        sb.AppendLine();
        // Use HKEY_CURRENT_USER for non-admin installation
        sb.AppendLine(@"[HKEY_CURRENT_USER\Software\Classes\*\shell\SharpMediaInfo]");
        sb.AppendLine($"@=\"{MenuDisplayName}\"");
        sb.AppendLine($"\"Icon\"=\"{escapedPath},0\"");
        sb.AppendLine();
        sb.AppendLine(@"[HKEY_CURRENT_USER\Software\Classes\*\shell\SharpMediaInfo\command]");
        sb.AppendLine($"@=\"\\\"{escapedPath}\\\" \\\"%1\\\"\"");
        sb.AppendLine();
        sb.AppendLine(@"[HKEY_CURRENT_USER\Software\Classes\Directory\shell\SharpMediaInfo]");
        sb.AppendLine($"@=\"{FolderMenuDisplayName}\"");
        sb.AppendLine($"\"Icon\"=\"{escapedPath},0\"");
        sb.AppendLine();
        sb.AppendLine(@"[HKEY_CURRENT_USER\Software\Classes\Directory\shell\SharpMediaInfo\command]");
        sb.AppendLine($"@=\"\\\"{escapedPath}\\\" \\\"%1\\\"\"");
        
        return sb.ToString();
    }

    private string GenerateUninstallRegContent()
    {
        return @"Windows Registry Editor Version 5.00

[-HKEY_CURRENT_USER\Software\Classes\*\shell\SharpMediaInfo]

[-HKEY_CURRENT_USER\Software\Classes\Directory\shell\SharpMediaInfo]
";
    }
}
