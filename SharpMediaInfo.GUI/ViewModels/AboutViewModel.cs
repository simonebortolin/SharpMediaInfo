using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SysException = System.Exception;

namespace SharpMediaInfo.GUI.ViewModels;

/// <summary>One clickable row in the About tab's "Used libraries" list.</summary>
/// <param name="Name">The library/project's display name.</param>
/// <param name="Url">Where <see cref="AboutTabViewModel.OpenUrlCommand"/> sends the user.</param>
/// <param name="Description">One-line blurb of what it's used for.</param>
public record AboutLinkItem(string Name, string Url, string Description);

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

    public string GitHubUrl => "https://github.com/simonebortolin/SharpMediaInfo";

    public string MediaInfoUrl => "https://mediaarea.net/MediaInfo";

    public string Id => this.GetType().FullName!;

    public IReadOnlyList<AboutLinkItem> Libraries { get; } = BuildLibraries();

    /// <summary>
    /// Builds the credited library list. The shared ViewModel backs both the Avalonia and WPF
    /// heads, so the UI-framework entries are picked by which framework is actually loaded.
    /// </summary>
    private static IReadOnlyList<AboutLinkItem> BuildLibraries()
    {
        var loaded = new HashSet<string>(
            AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name ?? string.Empty),
            StringComparer.OrdinalIgnoreCase);

        var libraries = new List<AboutLinkItem>
        {
            new("MediaInfo", "https://mediaarea.net/MediaInfo", "Native library by MediaArea that reads technical and tag data out of media files"),
        };

        if (loaded.Contains("Avalonia.Base"))
        {
            libraries.Add(new("Avalonia", "https://avaloniaui.net/", "Cross-platform UI framework behind this app, with its Fluent theme"));
            libraries.Add(new("MessageBox.Avalonia", "https://github.com/AvaloniaCommunity/MessageBox.Avalonia", "Message and confirmation dialogs"));
        }
        else if (loaded.Contains("PresentationFramework"))
        {
            libraries.Add(new("WPF", "https://github.com/dotnet/wpf", "Windows UI framework behind this app, with its Fluent theme"));
            libraries.Add(new("Microsoft.Xaml.Behaviors.Wpf", "https://github.com/microsoft/XamlBehaviorsWpf", "XAML behaviors and triggers for wiring view events to commands"));
        }

        libraries.Add(new("CommunityToolkit.Mvvm", "https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/", "MVVM boilerplate - source-generated observable properties and relay commands"));
        libraries.Add(new("Microsoft.Extensions.DependencyInjection", "https://learn.microsoft.com/dotnet/core/extensions/dependency-injection", "Wiring up services and ViewModels"));
        libraries.Add(new("Polly", "https://github.com/App-vNext/Polly", "Retry and resilience policies"));
        libraries.Add(new($".NET {Environment.Version.Major}", "https://dotnet.microsoft.com/", "The runtime everything above runs on"));

        return libraries;
    }

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
