using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// These are extension functions that act as Getters and Setters for tweenable values
    /// </summary>
    public static class TweenDataFunctions
    {
        public static Vector2 GetPosition(this Actor actor)
        {
            return actor.Position;
        }

        public static void SetPosition(this Actor actor, Vector2 value)
        {
            actor.Position = value;
        }

        public static TweenAccessors<Vector2> PositionTweenAccessors(this Actor actor)
        {
            return new TweenAccessors<Vector2>(actor.GetPosition, actor.SetPosition);
        }
    }

    public struct TweenAccessors<T> where T : struct
    {
        public readonly Func<T> getter;
        public readonly Action<T> setter;

        public TweenAccessors(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
    }
}
