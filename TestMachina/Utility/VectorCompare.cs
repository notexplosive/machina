using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestMachina.Utility
{
    public static class VectorCompare
    {
        public static bool ApproximateEqual(this Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).Length() < 0.0001f;
        }
    }
}
