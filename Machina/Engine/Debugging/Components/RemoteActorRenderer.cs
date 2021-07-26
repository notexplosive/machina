using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Components
{
    public class RemoteActorRenderer : BaseComponent
    {
        private readonly Actor remoteActor;
        private Vector2 cameraOffset;
        private bool isDragging;

        public RemoteActorRenderer(Actor actor, Actor remoteActor) : base(actor)
        {
            this.remoteActor = remoteActor;
            this.cameraOffset = Vector2.Zero;

        }

        public override void Update(float dt)
        {
            this.actor.scene.camera.UnscaledPosition = this.remoteActor.transform.Position - cameraOffset;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.isDragging)
            {
                this.cameraOffset += positionDelta;
            }
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Right)
            {
                this.isDragging = state == ButtonState.Pressed;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.remoteActor.Draw(spriteBatch);
        }
    }
}
