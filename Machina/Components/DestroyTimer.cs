using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class DestroyTimer : BaseComponent
    {
        private float timer;

        public DestroyTimer(Actor actor, float seconds) : base(actor)
        {
            this.timer = seconds;
        }

        public override void Update(float dt)
        {
            this.timer -= dt;

            if (this.timer < 0)
            {
                this.actor.Destroy();
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(new CircleF(this.actor.Position, 5 * this.timer), 15, Color.Red, this.timer);
        }
    }
}
