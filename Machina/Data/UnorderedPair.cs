using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class UnorderedPair<T> : IEquatable<UnorderedPair<T>> where T : struct
    {
        public readonly HashSet<T> set = new HashSet<T>();

        public UnorderedPair(T a, T b)
        {
            this.set.Add(a);
            this.set.Add(b);
        }

        public bool Equals(UnorderedPair<T> other)
        {
            return this.set.SetEquals(other.set);
        }

        public bool Contains(T c)
        {
            return this.set.Contains(c);
        }

        public bool IsEmpty()
        {
            return this.set.Count == 0;
        }

        public override int GetHashCode()
        {
            return this.set.GetHashCode();
        }

        public static bool operator ==(UnorderedPair<T> a, UnorderedPair<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(UnorderedPair<T> a, UnorderedPair<T> b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            var casted = obj as UnorderedPair<T>;
            if (casted != null)
            {
                return Equals(casted);
            }

            return false;
        }

        /// <summary>
        ///     Pass in one element that is in the pair, return the other one
        /// </summary>
        /// <param name="me">Element that exists in the pair</param>
        /// <returns></returns>
        public T Other(T me)
        {
            foreach (var s in this.set)
            {
                if (!s.Equals(me))
                {
                    return s;
                }
            }

            throw new ArgumentException("Could not find other");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var s in this.set)
            {
                sb.Append(s);
                sb.Append(", ");
            }

            return sb.ToString();
        }
    }
}