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

            if (curKeys.IsKeyDown(Keys.Up))
                this.actor.position.Y -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Down))
                this.actor.position.Y += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Left))
                this.actor.position.X -= 500f * dt;

            if (curKeys.IsKeyDown(Keys.Right))
                this.actor.position.X += 500f * dt;

            if (curKeys.IsKeyDown(Keys.Q))
            {
                this.actor.depth += 0.1f;
            }

            if (curKeys.IsKeyDown(Keys.E))
            {
                this.actor.depth -= 0.1f;
            }

        }
    }
}
