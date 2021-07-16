using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Internals
{
    public static class ForEach
    {
        public static void Each<T>(List<T> items, Action<T> forEachAction)
        {
            int whileLoopIterationCount = 0;
            HashSet<T> visited = new HashSet<T>();
            while (true)
            {
                whileLoopIterationCount++;
                if (whileLoopIterationCount > 256)
                {
                    throw new TooManyIterationsException();
                }

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (!visited.Contains(item))
                    {
                        forEachAction(item);
                        visited.Add(item);
                    }
                }

                var visitedValidItemCount = 0;
                foreach (var item in items)
                {
                    if (visited.Contains(item))
                    {
                        visitedValidItemCount++;
                    }
                }

                if (visitedValidItemCount == items.Count)
                {
                    return;
                }
            }
        }

        public class TooManyIterationsException : Exception
        {
        }
    }
}
