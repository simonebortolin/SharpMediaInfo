using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Interface Helpers Methods Class
    /// </summary>
    static public class InterfaceUtils
    {
        #region ExtensionMethods

        /// <summary>
        /// Extension method: Changes the input button style to checked.
        /// </summary>
        /// <param name="Button">The button that needs to be changed.</param>
        static public void Check(this Button Button)
        {
            Button.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            Button.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 112, 112, 112));
        }

        /// <summary>
        /// Extension method: Changes the input button style to unchecked.
        /// </summary>
        /// <param name="Button">The button that needs to be changed.</param>
        static public void Uncheck(this Button Button)
        {
            Button.Background = Brushes.Transparent;
            Button.BorderBrush = Brushes.Transparent;
        }

        /// <summary>
        /// Extension method: Sets the check state of the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="isChecked">if set to <c>true</c> change the button style to check; othewise removed the checked style.</param>
        static public void SetChecked(this Button button, bool isChecked)
        {
            if (isChecked)
            {
                button.Check();
            } else
            {
                button.Uncheck();
            }
        }

        /// <summary>
        /// Extension method: gets the last char from the input StringBuilder.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <returns>The last char inside the input StringBuilder.</returns>
        static public char Last(this StringBuilder StringBuilder)
        {
            return StringBuilder[StringBuilder.Length - 1];
        }

        /// <summary>
        /// Extension method: gets the first char from the input StringBuilder.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <returns>The first char inside the input StringBuilder.</returns>
        static public char First(this StringBuilder StringBuilder)
        {
            return StringBuilder[0];
        }

        /// <summary>
        /// Extension method: it removes from the input StringBuilder all final occurrences of a character sequence, if the array is empty removes trailing spaces.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <param name="TrimChars">Array of unicode characters that needs to be removed.</param>
        /// <returns>The StringBuilder with the trimmed end.</returns>
        static public StringBuilder TrimEnd(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            if (TrimChars?.Length == 0) TrimChars = new char[] { ' ' };
            if (StringBuilder.Length > 1)
                while (TrimChars.Any(c => StringBuilder.Last() == c))
                {
                    StringBuilder.Remove(StringBuilder.Length - 1, 1);
                }
            return StringBuilder;
        }

        /// <summary>
        /// Extension method: it removes from the input StringBuilder all initial occurrences of a character sequence, if the array is empty removes leading spaces.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <param name="TrimChars">Array of unicode characters that needs to be removed.</param>
        /// <returns>The StringBuilder with the trimmed start.</returns>
        static public StringBuilder TrimStart(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            if (TrimChars?.Length == 0) TrimChars = new char[] { ' ' };
            if (StringBuilder.Length > 1)
                while (TrimChars.Any(c => StringBuilder.First() == c))
                {
                    StringBuilder.Remove(0, 1);
                }
            return StringBuilder;
        }

        /// <summary>
        /// Extension method: it removes from the input StringBuilder all final and initial occurrences of a character sequence, if the array is empty removes trailing and leading spaces.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <param name="TrimChars">Array of unicode characters that needs to be removed.</param>
        /// <returns>The trimmed StringBuilder.</returns>
        static public StringBuilder Trim(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            StringBuilder.TrimStart(TrimChars);
            StringBuilder.TrimEnd(TrimChars);
            return StringBuilder;
        }

        /// <summary> Extension method: Finds the index of the first item matching the Predicate in an enumerable input object.</summary>
        /// <param name="IEnumerable">The enumerable object.</param>
        /// <param name="Predicate">The predicate to determine the matching.</param>
        /// <returns>The index of the first matching item, or -1 if no items matches.</returns>
        public static int FindIndex<T>(this IEnumerable<T> IEnumerable, Func<T, bool> Predicate)
        {
            int index = 0;
            foreach (T item in IEnumerable)
            {
                if (Predicate(item)) return index;
                index++;
            }
            return -1;
        }
        #endregion
    }
}
