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
            var localPos = this.actor.transform.LocalPosition;

            if (curKeys.IsKeyDown(Keys.Up))
                localPos.Y -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                localPos.Y += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                localPos.X -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                localPos.X += 500f * dt;

            this.actor.transform.LocalPosition = localPos;
            if (curKeys.IsKeyDown(Keys.Q))
            {
                this.actor.transform.Depth += 1;
            }

            if (curKeys.IsKeyDown(Keys.Z))
            {
                this.actor.transform.Angle += (float) Math.PI / 60;
            }

            if (curKeys.IsKeyDown(Keys.X))
            {
                this.actor.transform.Angle -= (float) Math.PI / 60;
            }
        }
    }
}
