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
            var localPos = this.actor.progeny.LocalPosition;

            if (curKeys.IsKeyDown(Keys.Up))
                localPos.Y -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                localPos.Y += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                localPos.X -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                localPos.X += 500f * dt;

            this.actor.progeny.LocalPosition = localPos;
            if (curKeys.IsKeyDown(Keys.Q))
            {
                this.actor.progeny.Depth += 0.1f;
            }

            if (curKeys.IsKeyDown(Keys.E))
            {
                this.actor.progeny.Depth -= 0.1f;
            }

            if (curKeys.IsKeyDown(Keys.Z))
            {
                this.actor.progeny.Angle += (float)Math.PI / 60;
            }

            if (curKeys.IsKeyDown(Keys.X))
            {
                this.actor.progeny.Angle -= (float)Math.PI / 60;
            }
        }
    }
}
