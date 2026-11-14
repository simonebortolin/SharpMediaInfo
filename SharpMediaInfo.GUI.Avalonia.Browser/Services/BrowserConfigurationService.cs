using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>Keeps the settings in the browser's localStorage.</summary>
public sealed class BrowserConfigurationService : IConfigurationService
{
    private const string StorageKey = "SharpMediaInfo.AppConfiguration";
    private bool _attached;

    public Task<AppConfiguration> LoadConfigurationAsync()
    {
        try
        {
            var json = BrowserInterop.GetItem(StorageKey);
            if (!string.IsNullOrEmpty(json))
                return Task.FromResult(JsonSerializer.Deserialize<AppConfiguration>(json) ?? new AppConfiguration());
        }
        catch (System.Exception e)
        {
            // Corrupted or unreadable settings: start from the defaults.
            Debug.WriteLine(e);
        }

        return Task.FromResult(new AppConfiguration());
    }

    public Task SaveConfigurationAsync(AppConfiguration configuration)
    {
        if (!BrowserInterop.SetItem(StorageKey, JsonSerializer.Serialize(configuration)))
            Debug.WriteLine("Settings not saved: localStorage is unavailable");
        return Task.CompletedTask;
    }

    public void Attach(AppConfiguration configuration)
    {
        if (_attached || configuration == null)
            return;

        configuration.PropertyChanged += async (_, _) => await SaveConfigurationAsync(configuration);
        _attached = true;
    }
}
