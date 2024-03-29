﻿using Machina.Engine;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Components
{
    internal class MouseCircle : BaseComponent
    {
        private readonly Color color;
        private readonly int radius;

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
            spriteBatch.DrawCircle(new CircleF(this.actor.transform.Position, this.radius), 20, this.color, 1,
                (this.actor.transform.Depth - 1).AsFloat);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.actor.transform.Position = currentPosition;
        }

        public override void OnMouseButton(MouseButton mouseButton, Vector2 currentPosition, ButtonState buttonState)
        {
            if (mouseButton == MouseButton.Left && buttonState == ButtonState.Pressed && Runtime.DebugLevel >= DebugLevel.Active)
            {
                var spawnedActor = this.actor.scene.AddActor("Spawned circle", currentPosition);
                new DestroyTimer(spawnedActor, 1);
            }
        }
    }
}