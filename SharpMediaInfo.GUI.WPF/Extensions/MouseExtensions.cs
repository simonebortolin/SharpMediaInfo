using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace SharpMediaInfo.GUI.WPF.Extensions;

/// <summary>
/// Mouse utility extensions using Win32 interop
/// </summary>
public static class MouseExtensions
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(ref Win32Point pt);

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32Point
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Gets the mouse position relative to the screen (not the window)
    /// </summary>
    /// <returns>The screen coordinates of the mouse cursor</returns>
    public static System.Windows.Point GetScreenPosition()
    {
        var w32Point = new Win32Point();
        GetCursorPos(ref w32Point);
        return new System.Windows.Point(w32Point.X, w32Point.Y);
    }

    /// <summary>
    /// Gets the screen bounds of a window
    /// </summary>
    public static Rect GetWindowScreenBounds(Window window)
    {
        var source = System.Windows.Interop.HwndSource.FromVisual(window);
        if (source != null)
        {
            var handle = source.Handle;
            if (GetWindowRect(handle, out RECT rect))
            {
                return new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
        }
        
        // Fallback to WPF coordinates
        return new Rect(window.Left, window.Top, window.ActualWidth, window.ActualHeight);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
