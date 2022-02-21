using System;
using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public static class VectorExtensions
    {
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
    }
}
