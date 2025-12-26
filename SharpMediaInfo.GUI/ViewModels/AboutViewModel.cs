using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpMediaInfo.GUI.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SysException = System.Exception;

namespace SharpMediaInfo.GUI.ViewModels;

/// <summary>
/// ViewModel for the About tab
/// </summary>
public partial class AboutTabViewModel : ObservableObject, ITabViewModel
{
    private readonly IProcessService? _processService;

    public AboutTabViewModel(IProcessService? processService = null)
    {
        _processService = processService;
    }

    public virtual string Header => "About";

    public string AppName => "SharpMediaInfo GUI";

    public string AppVersion
    {
        get
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            return version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "v1.0.0";
        }
    }

    public string MediaInfoVersion
    {
        get
        {
            try
            {
                return MediaInfo.LibraryVersion().Split(' ').LastOrDefault() ?? "Unknown";
            }
            catch
            {
                return "Not available";
            }
        }
    }

    public string SharpMediaInfoVersion
    {
        get
        {
            var version = typeof(SharpMediaInfo.MediaInfo).Assembly.GetName().Version;
            return version != null ? $"v{version.Major}.{version.Minor}.{version.Build}" : "Unknown";
        }
    }

    public string Description => "A modern MediaInfo viewer built with C# and MVVM architecture.";

    public string Copyright => $"© {DateTime.Now.Year} SharpMediaInfo Contributors";

    public string GitHubUrl => "https://gitlab.com/simonebortolin/get-media";

    public string MediaInfoUrl => "https://mediaarea.net/MediaInfo";

    [RelayCommand]
    private async Task OpenUrlAsync(string url)
    {
        if (_processService != null)
        {
            await _processService.OpenUrlAsync(url);
        }
        else
        {
            // Fallback if service not available
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (SysException ex)
            {
                Debug.WriteLine($"Error opening URL: {ex.Message}");
            }
        }
    }

    public Task<bool> OnCloseAsync()
    {
        return Task.FromResult(true);
    }
}
