﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public struct Depth
    {
        public const int MaxAsInt = 10569646;
        public static Depth Middle = new Depth(MaxAsInt / 2);
        public static Depth Max = new Depth(MaxAsInt);
        private readonly int val;

        public Depth(int val = MaxAsInt / 2)
        {
            this.val = val;
        }

        public static implicit operator int(Depth d) => d.AsInt;
        public static implicit operator Depth(int i) => new Depth(i);
        public static implicit operator float(Depth d) => d.AsFloat;

        public int AsInt => this.val;
        public float AsFloat
        {
            get
            {
                Debug.Assert(val <= MaxAsInt && val >= 0);
                return (float) this.val / MaxAsInt;
            }
        }

        public static Depth operator +(Depth a, Depth b)
        {
            return new Depth(a.val + b.val);
        }

        public static Depth operator -(Depth a, Depth b)
        {
            return new Depth(a.val - b.val);
        }

        public static Depth operator +(Depth a, int b)
        {
            return new Depth(a.val + b);
        }

        public static Depth operator -(Depth a, int b)
        {
            return new Depth(a.val - b);
        }

        public static bool operator ==(Depth a, Depth b)
        {
            return a.val == b.val;
        }

        public static bool operator !=(Depth a, Depth b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Depth other)
            {
                return other.val == val;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return val;
        }

        public override string ToString()
        {
            return this.val.ToString();
        }
    }
}