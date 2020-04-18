using GetMediaInfoGUI.Theme;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace GetMediaInfoGUI.Themes.Custom
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        private enum DialogButtonType
        {
            Ok = 0,
            Cancel = 1,
            Yes = 2,
            No = 3
        }

        #region Fields
        private MessageBoxResult Result { get; set; } = MessageBoxResult.None;
        private MessageBoxButton DialogButtonsType { get; set; } = MessageBoxButton.OK;
        #endregion

        #region Private
        private MessageDialog(Window owner, string message, string caption, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            InitializeComponent();

            this.Owner = owner;
            this.Title = caption;
            this.MessageContent.Text = message;
            this.DialogButtonsType = buttons;

            switch (this.DialogButtonsType)
            {
                case MessageBoxButton.OK:
                    this.OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    this.OkButton.Visibility = Visibility.Visible;
                    this.CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    this.YesButton.Visibility = Visibility.Visible;
                    this.NoButton.Visibility = Visibility.Visible;
                    this.CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    this.YesButton.Visibility = Visibility.Visible;
                    this.NoButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowHelper.ChangeStyle(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var type = ParseDialogButtonType(button.Tag);
                switch (type)
                {
                    case DialogButtonType.Ok:
                        this.Result = MessageBoxResult.OK;
                        break;
                    case DialogButtonType.Cancel:
                        this.Result = MessageBoxResult.Cancel;
                        break;
                    case DialogButtonType.Yes:
                        this.Result = MessageBoxResult.Yes;
                        break;
                    case DialogButtonType.No:
                        this.Result = MessageBoxResult.No;
                        break;
                }

                this.Close();
            }
        }

        private DialogButtonType ParseDialogButtonType(object value)
        {
            if (value is string str && int.TryParse(str, out int type))
            {
                return (DialogButtonType)Enum.ToObject(typeof(DialogButtonType), type);
            }
            else
            {
                throw new ArgumentException($"'{value}' was not recognized as a valid dialog button type");
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Shows a message dialog with a caption and a message.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns>The message</returns>
        public static void Show(Window owner, string message, string caption)
        {
            var dialog = new MessageDialog(owner, message, caption);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Show a message dialog with a caption, a message and some buttons.
        /// </summary>
        /// <param name="owner">The owner of the dialog</param>
        /// <param name="message">The message to show the dialog</param>
        /// <param name="caption">The caption of the dialog</param>
        /// <param name="buttons">The buttons to show</param>
        /// <returns>The message result</returns>
        public static MessageBoxResult Show(Window owner, string message, string caption, MessageBoxButton buttons)
        {
            var dialog = new MessageDialog(owner, message, caption, buttons);
            dialog.ShowDialog();
            return dialog.Result;
        }
        #endregion

        #region Interop
        /// <summary>
        /// Window helper - Component used to change window style
        /// </summary>
        public static class WindowHelper
        {
            [DllImport("user32.dll")]
            static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

            [DllImport("user32.dll")]
            static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

            [DllImport("user32.dll")]
            static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

            const int GWL_EXSTYLE = -20;
            const int WS_EX_DLGMODALFRAME = 0x0001;
            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;
            const int SWP_NOZORDER = 0x0004;
            const int SWP_FRAMECHANGED = 0x0020;
            const uint WM_SETICON = 0x0080;

            /// <summary>
            /// Changes the style of the target window by removing the icon and minimize/maximise buttons.
            /// </summary>
            /// <param name="window">The target window</param>
            public static void ChangeStyle(Window window)
            {
                // Get this window's handle
                IntPtr hwnd = new WindowInteropHelper(window).Handle;

                // Change the extended window style to not show a window icon
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

                // Update the window's non-client area to reflect the changes
                SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

                SendMessage(hwnd, WM_SETICON, new IntPtr(1), IntPtr.Zero);
                SendMessage(hwnd, WM_SETICON, IntPtr.Zero, IntPtr.Zero);
            }
        }
        #endregion
    }
}
