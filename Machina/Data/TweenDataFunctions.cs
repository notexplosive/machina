﻿using Machina.Engine;
using Microsoft.Xna.Framework;

namespace Machina.Data
{
    /// <summary>
    ///     These are extension functions that act as Getters and Setters for tweenable values
    /// </summary>
    public static class TweenDataFunctions
    {
        public static Vector2 GetPosition(this Actor actor)
        {
            return actor.transform.Position;
        }

        public static void SetPosition(this Actor actor, Vector2 value)
        {
            actor.transform.Position = value;
        }

        public static TweenAccessors<Vector2> PositionTweenAccessors(this Actor actor)
        {
            return new TweenAccessors<Vector2>(actor.GetPosition, actor.SetPosition);
        }

        public static TweenAccessors<Vector2> LocalPositionTweenAccessors(this Actor actor)
        {
            return new TweenAccessors<Vector2>(() => actor.transform.LocalPosition,
                val => actor.transform.LocalPosition = val);
        }
    }
}