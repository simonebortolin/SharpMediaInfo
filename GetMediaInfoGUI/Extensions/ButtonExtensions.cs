using System.Windows.Controls;
using System.Windows.Media;

namespace GetMediaInfoGUI.Extensions
{
    /// <summary>
    /// Button extension methods
    /// </summary>
    static class ButtonExtensions
    {
        /// <summary>
        /// Extension method: Changes the input button style to checked.
        /// </summary>
        /// <param name="Button">The button that needs to be changed.</param>
        public static void Check(this Button Button)
        {
            Button.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            Button.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 112, 112, 112));
        }

        /// <summary>
        /// Extension method: Changes the input button style to unchecked.
        /// </summary>
        /// <param name="Button">The button that needs to be changed.</param>
        public static void Uncheck(this Button Button)
        {
            Button.Background = Brushes.Transparent;
            Button.BorderBrush = Brushes.Transparent;
        }

        /// <summary>
        /// Extension method: Sets the check state of the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="isChecked">if set to <c>true</c> change the button style to check; othewise removed the checked style.</param>
        public static void SetChecked(this Button button, bool isChecked)
        {
            if (isChecked)
            {
                button.Check();
            } else
            {
                button.Uncheck();
            }
        }
    }
}
