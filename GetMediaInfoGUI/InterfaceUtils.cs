using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace GetMediaInfoGUI
{
    public static class InterfaceUtils
    {
        public static void Check(this Button Button)
        {
            Button.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 221, 221, 221));
            Button.BorderBrush = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 112, 112, 112));
        }

        public static void Uncheck(this Button Button)
        {
            Button.Background = Brushes.Transparent;
            Button.BorderBrush = Brushes.Transparent;
        }

        public static void SetChecked(this Button button, bool isChecked)
        {
            if (isChecked)
                button.Check();
            else
                button.Uncheck();
        }

        public static char Last(this StringBuilder StringBuilder)
        {
            return StringBuilder[StringBuilder.Length - 1];
        }

        public static char First(this StringBuilder StringBuilder)
        {
            return StringBuilder[0];
        }

        public static StringBuilder TrimEnd(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            if (TrimChars != null && TrimChars.Length == 0)
                TrimChars = new char[1] { ' ' };
            if (StringBuilder.Length > 1)
            {
                while (TrimChars.Any<char>(c => (int)StringBuilder.Last() == (int)c))
                    StringBuilder.Remove(StringBuilder.Length - 1, 1);
            }
            return StringBuilder;
        }

        public static StringBuilder TrimStart(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            if (TrimChars != null && TrimChars.Length == 0)
                TrimChars = new char[1] { ' ' };
            if (StringBuilder.Length > 1)
            {
                while (TrimChars.Any<char>(c => (int)StringBuilder.First() == (int)c))
                    StringBuilder.Remove(0, 1);
            }
            return StringBuilder;
        }

        public static StringBuilder Trim(this StringBuilder StringBuilder, params char[] TrimChars)
        {
            StringBuilder.TrimStart(TrimChars);
            StringBuilder.TrimEnd(TrimChars);
            return StringBuilder;
        }

        public static int FindIndex<T>(this IEnumerable<T> IEnumerable, Func<T, bool> Predicate)
        {
            int num = 0;
            foreach (T obj in IEnumerable)
            {
                if (Predicate(obj))
                    return num;
                ++num;
            }
            return -1;
        }

        public static string OrIfNullOrEmpty(this string theString, string anotherString)
        {
            if (string.IsNullOrEmpty(theString)) return anotherString;
            return theString;
        }
        public static string AddGlueIfNotNullOrEmpty(this string theString, string glueStart, string glueEnd)
        {
            if (!string.IsNullOrEmpty(theString)) return glueStart + theString + glueEnd;
            return theString;
        }
        public static string AppendIfNotNullOrEmpty(this string theString, string anotherString, string glueStart = "", string glueEnd = "")
        {
            if (!string.IsNullOrEmpty(anotherString)) return theString + glueStart + anotherString + glueEnd;
            return theString;
        }
    }
}
