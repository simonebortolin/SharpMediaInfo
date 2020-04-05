using System;
using System.Runtime.InteropServices;

namespace GetMediaInfoGUI
{
    class InterComm
    {
        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another 
        /// application
        /// </summary>
        public const Int32 WM_COPYDATA = 0x004A;

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage 
        /// function calls the window procedure for the specified window and does 
        /// not return until the window procedure has processed the message. 
        /// </summary>
        /// <param name="hWnd">
        /// Handle to the window whose window procedure will receive the message.
        /// </param>
        /// <param name="Msg">Specifies the message to be sent.</param>
        /// <param name="wParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <param name="lParam">
        /// Specifies additional message-specific information.
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,                // Handle to destination window
            Int32 Msg,                  // Message ID 
            IntPtr wParam,              // Handle to window (same as hWnd)
            ref COPYDATASTRUCT lParam   // Pointer to a COPYDATASTRUCT structure
            );

        //public static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, StringBuilder lParam);

        /// <summary>
        /// The FindWindow function retrieves a handle to the top-level window 
        /// whose class name and window name match the specified strings. This 
        /// function does not search child windows. This function does not 
        /// perform a case-sensitive search.
        /// </summary>
        /// <param name="lpClassName">Class name</param>
        /// <param name="lpWindowName">Window caption</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(
            string lpClassName,         // Class name 
            string lpWindowName         // Window caption
            );
    }

    /// <summary>
    /// The COPYDATASTRUCT structure contains data to be passed to another 
    /// application by the WM_COPYDATA message. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;       // Specifies data to be passed
        public int cbData;          // Specifies the data size in bytes
        public IntPtr lpData;       // Pointer to data to be passed
    }
}



