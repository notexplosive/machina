﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// Pair of values representing a minimum and maximum
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
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        public override string ToString()
        {
            return this.min + " - " + this.max;
        }
    }
}
