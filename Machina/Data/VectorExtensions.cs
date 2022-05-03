using System;
using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public static class VectorExtensions
    {
        [Obsolete("The implementation is wrong, this is a sentinel to figure out who uses this")]
        public static Vector2 Polar(float length, float angle)
        {
            return new Vector2(length * MathF.Sin(angle), length * MathF.Cos(angle));
        }

        public static Vector2 Normalized(this Vector2 original)
        {
            var copy = original;
            copy.Normalize();
            return copy;
        }

        public static Vector2 Rounded( this Vector2 original)
        {
            return new Vector2(MathF.Round(original.X), MathF.Round(original.Y));
        }
        
        public static bool HasNonzeroLength( this Vector2 original)
        {
            return original.X != 0 || original.Y != 0;
        }
    }
}
