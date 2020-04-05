using System;
using System.Collections.Generic;

namespace GetMediaInfoGUI.Extensions
{
    /// <summary>
    /// Enumerable extension methods
    /// </summary>
    static class EnumerableExtensions
    {
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
    }
}
