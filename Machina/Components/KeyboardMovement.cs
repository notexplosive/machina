using Machina.Engine;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class KeyboardMovement : BaseComponent
    {
        public KeyboardMovement(Actor actor) : base(actor) { }

        public override void Update(float dt)
        {
            var curKeys = Keyboard.GetState();
            var localPos = this.actor.LocalPosition;

            if (curKeys.IsKeyDown(Keys.Up))
                localPos.Y -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                localPos.Y += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                localPos.X -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                localPos.X += 500f * dt;

            this.actor.LocalPosition = localPos;
            if (curKeys.IsKeyDown(Keys.Q))
            {
                this.actor.depth += 0.1f;
            }

            if (curKeys.IsKeyDown(Keys.E))
            {
                this.actor.depth -= 0.1f;
            }

            if (curKeys.IsKeyDown(Keys.Z))
            {
                this.actor.Angle += (float) Math.PI / 60;
            }

            if (curKeys.IsKeyDown(Keys.X))
            {
                this.actor.Angle -= (float) Math.PI / 60;
            }
        }
    }
}
