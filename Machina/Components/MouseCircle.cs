using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class MouseCircle : BaseComponent
    {
        private int radius;
        private Color color;

        public MouseCircle(Actor actor, int radius, Color color) : base(actor)
        {
            this.radius = radius;
            this.color = color;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle(new CircleF(this.actor.Position, this.radius), 20, this.color, 1, this.actor.Depth - .0000001f);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.actor.Position = currentPosition;
        }

        public override void OnMouseButton(MouseButton mouseButton, Vector2 currentPosition, ButtonState buttonState)
        {
            if (mouseButton == MouseButton.Left && buttonState == ButtonState.Pressed && MachinaGame.DebugLevel >= DebugLevel.Active)
            {
                var spawnedActor = this.actor.scene.AddActor("Spawned circle", currentPosition);
                new DestroyTimer(spawnedActor, 1);
            }
        }
    }
}
