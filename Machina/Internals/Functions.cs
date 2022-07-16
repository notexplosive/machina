using System;
using System.Collections.Generic;

namespace Machina.Internals
{
    public static class Functions
    {
        /// <summary>
        ///     Equivalent to foreach(var item in items) { forEachAction(item); }
        ///     With a key difference: The collection CAN be modified during the loop.
        ///     If an element is added during the loop after the current index, it will be iterated upon.
        ///     If an element is added during the loop before the current index, it will not be iterated upon.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="forEachAction"></param>
        public static void ResilientForEach<T>(IList<T> items, Action<T> forEachAction)
        {
            foreach (var item in items)
            {
                forEachAction(item);
            }
        }

        public class TooManyIterationsException : Exception
        {
        }
    }
}