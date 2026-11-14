using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>
/// No recent files: a browser File handle can't be reopened after the page is closed, so there
/// is nothing meaningful to remember.
/// </summary>
public sealed class BrowserRecentFilesService : IRecentFilesService
{
    public Task<ObservableCollection<string>> LoadAsync(int maxItems) => Task.FromResult(new ObservableCollection<string>());

    public Task AddAsync(string filePath, ObservableCollection<string> current, int maxItems) => Task.CompletedTask;

    public Task ClearAsync(ObservableCollection<string> current)
    {
        current.Clear();
        return Task.CompletedTask;
    }
}

/// <summary>Opens links in a new browser tab; there are no processes to start.</summary>
public sealed class BrowserProcessService : IProcessService
{
    public async Task<bool> OpenUrlAsync(string url)
    {
        var launcher = App.GetTopLevel()?.Launcher;
        return launcher != null
            && Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && await launcher.LaunchUriAsync(uri);
    }

    public Task<bool> StartNewInstanceAsync(string arguments) => Task.FromResult(false);

    public Task<bool> OpenFileWithDefaultAppAsync(string filePath) => Task.FromResult(false);

    public string GetCurrentExecutablePath() => string.Empty;
}

public sealed class BrowserMousePositionService : IMousePositionService
{
    public (int X, int Y) GetScreenPosition() => (0, 0);
}
