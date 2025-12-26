using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using SharpMediaInfo.GUI.Services;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaClipboardService : IClipboardService
{
    private IClipboard? GetClipboard()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Clipboard;
        }
        return null;
    }

    public void CopyText(string text)
    {
        var clipboard = GetClipboard();
        if (clipboard != null)
        {
            _ = clipboard.SetTextAsync(text); // Fire and forget
        }
    }

    public string? GetText()
    {
        var clipboard = GetClipboard();
        if (clipboard != null)
        {
            // Synchronous API required by interface, but Avalonia is async
            // Return null for now - could use Task.Run but not ideal
            return null;
        }
        return null;
    }
}
