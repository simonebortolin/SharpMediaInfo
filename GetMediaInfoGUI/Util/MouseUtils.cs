using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GetMediaInfoGUI.Util
{
    /// <summary>
    /// Mouse utilities.
    /// </summary>
    class MouseUtils
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        /// <summary>
        /// Gets the mouse position relative to the screen.
        /// </summary>
        /// <returns>The mouse position</returns>
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
