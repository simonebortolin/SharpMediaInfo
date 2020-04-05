using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetMediaInfoGUI.Utils
{
    class StringUtils
    {

        /// <summary>
        /// Returns whether at least one input string is not null or empty or whitespace.
        /// </summary>
        /// <param name="args">The input strings to check.</param>
        /// <returns>True, if there is at least one input string that it's not null or empty or whitespace; otherwise, false.</returns>
        public static bool AnyNonEmpty(params string[] args)
        {
            return args.Any(i => !string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i));
        }
    }
}
