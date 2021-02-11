using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class MouseCircle : BaseComponent
    {
        private int radius;

        public MouseCircle(Actor actor, int radius) : base(actor)
        {
            this.radius = radius;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(new CircleF(this.actor.Position, this.radius), 20, new Color(1, 0, 0, 1f), 1, this.actor.depth - .0000001f);
            spriteBatch.DrawCircle(new CircleF(this.actor.scene.camera.WorldToScreen(new Vector2(500, 500)), this.radius + 2), 20, new Color(1, 1, 0, 1f), 1, this.actor.depth - .0000001f);
        }

        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.actor.Position = currentPosition.ToVector2();
        }
    }
}
