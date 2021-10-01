using System;
using System.Diagnostics;

namespace Machina.Data
{
    /// <summary>
    ///     Pair of values representing a minimum and maximum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct MinMax<T> where T : struct, IComparable<T>
    {
        public readonly T min;
        public readonly T max;

        public MinMax(T min, T max)
        {
            this.min = min;
            this.max = max;

            Debug.Assert(min.CompareTo(max) <= 0, "Min cannot be > max");
        }

        public bool IsWithin(T value)
        {
            return value.CompareTo(this.min) >= 0 && value.CompareTo(this.max) <= 0;
        }

        public override string ToString()
        {
            return this.min + " - " + this.max;
        }
    }
}
