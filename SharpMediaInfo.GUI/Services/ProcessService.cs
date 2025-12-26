using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SysException = System.Exception;

namespace SharpMediaInfo.GUI.Services;

/// <summary>
/// Cross-platform process service for starting external processes safely
/// </summary>
public class ProcessService : IProcessService
{
    public string GetCurrentExecutablePath()
    {
        var process = Process.GetCurrentProcess();
        return Environment.ProcessPath ?? process.MainModule?.FileName ?? string.Empty;
    }

    public Task<bool> StartNewInstanceAsync(string arguments)
    {
        try
        {
            var exePath = GetCurrentExecutablePath();
            
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
            {
                Debug.WriteLine("Cannot start new instance: executable path not found");
                return Task.FromResult(false);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            var process = Process.Start(startInfo);
            
            if (process != null)
            {
                Debug.WriteLine($"Started new instance with PID {process.Id}");
                return Task.FromResult(true);
            }

            Debug.WriteLine("Failed to start new instance: Process.Start returned null");
            return Task.FromResult(false);
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"Error starting new instance: {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> OpenUrlAsync(string url)
    {
        try
        {
            // Validate URL
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine("Cannot open URL: URL is null or empty");
                return Task.FromResult(false);
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                Debug.WriteLine($"Cannot open URL: Invalid format '{url}'");
                return Task.FromResult(false);
            }

            // Only allow http and https schemes for security
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                Debug.WriteLine($"Cannot open URL: Unsupported scheme '{uri.Scheme}'");
                return Task.FromResult(false);
            }

            // Platform-specific URL opening
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: Use cmd /c start
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start {url}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux: Use xdg-open
                Process.Start(new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = url,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS: Use open
                Process.Start(new ProcessStartInfo
                {
                    FileName = "open",
                    Arguments = url,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            else
            {
                // Fallback: Use UseShellExecute
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }

            Debug.WriteLine($"Opened URL: {url}");
            return Task.FromResult(true);
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"Error opening URL '{url}': {ex.Message}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> OpenFileWithDefaultAppAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Debug.WriteLine("Cannot open file: path is null or empty");
                return Task.FromResult(false);
            }

            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"Cannot open file: file not found '{filePath}'");
                return Task.FromResult(false);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true,
                Verb = "open"
            };

            var process = Process.Start(startInfo);
            
            if (process != null)
            {
                Debug.WriteLine($"Opened file with default app: {Path.GetFileName(filePath)}");
                return Task.FromResult(true);
            }

            Debug.WriteLine($"Failed to open file: Process.Start returned null");
            return Task.FromResult(false);
        }
        catch (SysException ex)
        {
            Debug.WriteLine($"Error opening file '{filePath}': {ex.Message}");
            return Task.FromResult(false);
        }
    }
}
