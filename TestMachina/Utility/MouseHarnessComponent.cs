using System;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace TestMachina.Utility
{
    public class MouseHarnessComponent : BaseComponent
    {
        private readonly Action<Vector2, Vector2, Vector2> onMouseUpdate;

        public MouseHarnessComponent(Actor actor, Action<Vector2, Vector2, Vector2> onMouseUpdate) : base(actor)
        {
            this.onMouseUpdate = onMouseUpdate;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.onMouseUpdate(currentPosition, positionDelta, rawDelta);
        }
    }
}