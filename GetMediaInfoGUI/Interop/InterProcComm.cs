using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace GetMediaInfoGUI.Interop
{
    /// <summary>
    /// Inter process communication utilities
    /// </summary>
    class InterComm
    {
        #region Singleton
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static InterComm instance;

        /// <summary>
        /// The inter communication singleton.
        /// </summary>
        public static InterComm Instance {
            get {
                if (instance == null)
                {
                    instance = new InterComm();                    
                }

                return instance;
            }
        }
        #endregion

        #region WndProc event management
        /// <summary>
        /// The window message event.
        /// </summary>
        public event WindowMessageEventHandler OnWindowMessage;

        /// <summary>
        /// The OnWindowMessage event delegate.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public delegate void WindowMessageEventHandler(WindowMessageEventArgs e);

        /// <summary>
        /// The method to fire OnWindowMessage event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnWindowMessageReceived(WindowMessageEventArgs e)
        {
            WindowMessageEventHandler handler = OnWindowMessage;
            handler?.Invoke(e);
        }

        /// <summary>
        /// The window message event arguments.
        /// </summary>
        public class WindowMessageEventArgs : EventArgs
        {
            /// <summary>
            /// The sender window handle.
            /// </summary>
            public IntPtr SenderHandle { get; }
            /// <summary>
            /// The message.
            /// </summary>
            public string Message { get; }

            /// <summary>
            /// The default constructor.
            /// </summary>
            /// <param name="hWnd">The window handle</param>
            /// <param name="message">The message</param>
            public WindowMessageEventArgs(IntPtr hWnd, string message)
            {
                SenderHandle = hWnd;
                Message = message;
            }
        }

        /// <summary>
        /// Adds this instance to the HwndSource hooks to be able to intercept messages.
        /// </summary>
        /// <param name="hwndSource">The window handle source</param>
        public void SubscribeWindowMessageHook(HwndSource hwndSource)
        {
            hwndSource.AddHook(WndProc);
        }

        /// <summary>
        /// Unmarshal the received message
        /// </summary>
        /// <param name="lParam">The handle to the message data</param>
        /// <returns>The unmarshalled data from the handle</returns>
        private string UnmarshalMessage(IntPtr lParam)
        {
            COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
            string data = Marshal.PtrToStringUni(cds.lpData);
            return string.IsNullOrEmpty(data) ? null : data;
        }

        #endregion

        #region Interop

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
        /// <param name="hWnd">Handle to the window whose window procedure will receive the message.</param>
        /// <param name="Msg">Specifies the message to be sent.</param>
        /// <param name="wParam">Specifies additional message-specific information.</param>
        /// <param name="lParam">Specifies additional message-specific information.</param>
        /// <returns>The pointer </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,                // Handle to destination window
            Int32 Msg,                  // Message ID 
            IntPtr wParam,              // Handle to window (same as hWnd)
            ref COPYDATASTRUCT lParam   // Pointer to a COPYDATASTRUCT structure
            );

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
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// An application-defined function that processes messages sent to a window. The WNDPROC type defines a pointer to this callback function.
        /// </summary>
        /// <param name="hwnd">The handle of the window.</param>
        /// <param name="msg">The message.</param>
        /// <param name="wParam">Additional message information. The contents of this parameter depend on the value of the uMsg parameter.</param>
        /// <param name="lParam">Additional message information. The contents of this parameter depend on the value of the uMsg parameter.</param>
        /// <param name="handled">The return value is the result of the message processing and depends on the message sent.</param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == InterComm.WM_COPYDATA)
            {
                string data = UnmarshalMessage(lParam);
                if (!string.IsNullOrEmpty(data))
                {
                    OnWindowMessageReceived(new WindowMessageEventArgs(hwnd, data));
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Retrieves the window rectangle.
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <param name="lpRect">The rectangle</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// The COPYDATASTRUCT structure contains data to be passed to another 
        /// application by the WM_COPYDATA message. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // Specifies data to be passed
            public int cbData;          // Specifies the data size in bytes
            public IntPtr lpData;       // Pointer to data to be passed
        }

        /// <summary>
        /// The rectangle structure that defined the window box.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Sends a message to a window.
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public static bool SendMessageToWindow(IntPtr hWnd, int messageId, string message)
        {
            var cds = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                cbData = Encoding.Unicode.GetByteCount(message) + sizeof(char), //length in byte
                lpData = Marshal.StringToHGlobalUni(message) //pointer to string
            };

            return !SendMessage(hWnd, messageId, IntPtr.Zero, ref cds).Equals(IntPtr.Zero);
        }

        /// <summary>
        /// Gets a window that matches the class name or the title.
        /// </summary>
        /// <param name="windowClass">The window class name (null allowed)</param>
        /// <param name="windowTitle">The window title (null allowed)</param>
        /// <returns>The handle of the window; otherwise null</returns>
        public static IntPtr? GetWindow(string windowClass, string windowTitle)
        {
            var ptr = FindWindow(windowClass, windowTitle);
            if (!ptr.Equals(IntPtr.Zero))
            {
                return ptr;
            }

            return null;
        }

        /// <summary>
        /// Gets the rectangle that defines the dimension and position of the window.
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <returns>The rectangle that defines the window; otherwise null</returns>
        public static Rectangle? GetWindowRectangle(IntPtr hWnd)
        {
            var result = GetWindowRect(hWnd, out RECT lpRect);

            if (result)
            {
                return new Rectangle(lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
            }

            return null;
        }
        #endregion
    }
}



