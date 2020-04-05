using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GetMediaInfoGUI.Utils
{
    class MessageBoxHelper
    {
        /// <summary>
        /// Shows a message box at the center of the parent.
        /// </summary>
        /// <param name="parent">The parent window</param>
        /// <param name="message">The message</param>
        /// <returns>The message box result</returns>
        public static MessageBoxResult Show(Window parent, string message)
        {
            SetNextWindowsChildrenPosition(parent);
            return MessageBox.Show(parent, message);
        }

        /// <summary>
        /// Show a message box with caption at the center of the parent.
        /// </summary>
        /// <param name="parent">The parent window</param>
        /// <param name="message">the message</param>
        /// <param name="caption">The caption</param>
        /// <returns>The message box result</returns>
        public static MessageBoxResult ShowError(Window parent, string message)
        {
            SetNextWindowsChildrenPosition(parent);
            return MessageBox.Show(parent, message, "GetMediaInfoGUI - Error detected", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Show a message box with caption at the center of the parent.
        /// </summary>
        /// <param name="parent">The parent window</param>
        /// <param name="message">the message</param>
        /// <param name="caption">The caption</param>
        /// <param name="buttons">The buttons</param>
        /// <param name="icon">The dialog icon</param>
        /// <returns>The message box result</returns>
        public static MessageBoxResult Show(Window parent, string message, string caption, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Question)
        {
            SetNextWindowsChildrenPosition(parent);
            return MessageBox.Show(parent, message, caption, buttons, icon);
        }

        #region Helpers
        private static void SetNextWindowsChildrenPosition(Window window)
        {
            MessageBoxCenterHelper helper = new MessageBoxCenterHelper();
            helper.Prep(window);
        }
        #endregion

        #region Helper classes
        private class MessageBoxCenterHelper
        {
            private int messageHook;
            private IntPtr parentFormHandle;

            public void Prep(Window window)
            {
                NativeMethods.CenterMessageCallBackDelegate callBackDelegate = new NativeMethods.CenterMessageCallBackDelegate(CenterMessageCallBack);
                GCHandle.Alloc(callBackDelegate);

                parentFormHandle = new WindowInteropHelper(window).Handle;
                messageHook = NativeMethods.SetWindowsHookEx(5, callBackDelegate, new IntPtr(NativeMethods.GetWindowLong(parentFormHandle, -6)), NativeMethods.GetCurrentThreadId()).ToInt32();
            }

            private int CenterMessageCallBack(int message, int wParam, int lParam)
            {
                int xPos;
                int yPos;

                if (message == 5)
                {
                    NativeMethods.GetWindowRect(parentFormHandle, out NativeMethods.RECT formRect);
                    NativeMethods.GetWindowRect(new IntPtr(wParam), out NativeMethods.RECT messageBoxRect);

                    xPos = (int)((formRect.Left + (formRect.Right - formRect.Left) / 2) - ((messageBoxRect.Right - messageBoxRect.Left) / 2));
                    yPos = (int)((formRect.Top + (formRect.Bottom - formRect.Top) / 2) - ((messageBoxRect.Bottom - messageBoxRect.Top) / 2));

                    NativeMethods.SetWindowPos(wParam, 0, xPos, yPos, 0, 0, 0x1 | 0x4 | 0x10);
                    NativeMethods.UnhookWindowsHookEx(messageHook);
                }

                return 0;
            }
        }

        private static class NativeMethods
        {
            internal struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            internal delegate int CenterMessageCallBackDelegate(int message, int wParam, int lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnhookWindowsHookEx(int hhk);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("kernel32.dll")]
            internal static extern int GetCurrentThreadId();

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int hook, CenterMessageCallBackDelegate callback, IntPtr hMod, int dwThreadId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        }
        #endregion
    }
}
