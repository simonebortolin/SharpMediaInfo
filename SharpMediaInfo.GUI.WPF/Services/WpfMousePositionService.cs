using SharpMediaInfo.GUI.Services;
using SharpMediaInfo.GUI.WPF.Extensions;
using System.Runtime.InteropServices;

namespace SharpMediaInfo.GUI.WPF.Services;

/// <summary>
/// WPF implementation of mouse position service using Win32 API
/// </summary>
public class WpfMousePositionService : IMousePositionService
{
    public (int X, int Y) GetScreenPosition()
    {
        return MouseExtensions.GetScreenPosition();
    }

}
