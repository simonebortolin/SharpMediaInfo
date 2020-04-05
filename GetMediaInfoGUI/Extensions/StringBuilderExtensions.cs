using System.Linq;
using System.Text;

namespace GetMediaInfoGUI.Extensions
{
    /// <summary>
    /// String builder extension methods
    /// </summary>
    static class StringBuilderExtensions
    {
        /// <summary>
        /// Extension method: gets the last char from the input StringBuilder.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <returns>The last char inside the input StringBuilder.</returns>
        public static char Last(this StringBuilder StringBuilder)
        {
            return StringBuilder[StringBuilder.Length - 1];
        }

        /// <summary>
        /// Extension method: gets the first char from the input StringBuilder.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <returns>The first char inside the input StringBuilder.</returns>
        public static char First(this StringBuilder StringBuilder)
        {
            return StringBuilder[0];
        }

        /// <summary>
        /// Extension method: it removes from the input StringBuilder all final occurrences of a character sequence, if the array is empty removes trailing spaces.
        /// </summary>
        /// <param name="StringBuilder">The StringBuilder.</param>
        /// <param name="TrimChars">Array of unicode characters that needs to be removed.</param>
        /// <returns>The StringBuilder with the trimmed end.</returns>
        public static StringBuilder TrimEnd(this StringBuilder StringBuilder, params char[] TrimChars)
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
        public static StringBuilder TrimStart(this StringBuilder StringBuilder, params char[] TrimChars)
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
        public static StringBuilder Trim(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            StringBuilder.TrimStart(TrimChars);
            StringBuilder.TrimEnd(TrimChars);
            return StringBuilder;
        }
    }
}
