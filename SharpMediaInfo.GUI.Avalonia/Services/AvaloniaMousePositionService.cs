using SharpMediaInfo.GUI.Services;

namespace SharpMediaInfo.GUI.Avalonia.Services;

public class AvaloniaMousePositionService : IMousePositionService
{
    public (int X, int Y) GetScreenPosition()
    {
        // Avalonia doesn't provide direct global mouse position API
        // For now, return (0, 0) as fallback
        // In v1, we don't support --open-at-mouse positioning
        // Windows will open at default position or centered
        
        // Future improvement: use platform-specific P/Invoke for Windows
        // For cross-platform, this is acceptable fallback
        
        return (0, 0);
    }
}
