using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Components
{
    public class RemoteActorRenderer : BaseComponent
    {
        private readonly Actor remoteActor;

        public RemoteActorRenderer(Actor actor, Actor remoteActor, Canvas windowCanvas) : base(actor)
        {
            this.remoteActor = remoteActor;

        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.actor.scene.camera.UnscaledPosition = this.remoteActor.transform.Position;
            this.remoteActor.Draw(spriteBatch);
        }
    }
}
