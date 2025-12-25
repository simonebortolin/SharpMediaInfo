using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.ViewModels;

/// <summary>
/// ViewModel for the About tab
/// </summary>
public partial class AboutViewModel : ObservableObject, ITabViewModel
{
    public string Header => "About";

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
    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore errors opening URL
        }
    }

    public Task<bool> OnCloseAsync()
    {
        return Task.FromResult(true);
    }
}
