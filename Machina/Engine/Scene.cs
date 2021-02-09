using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Machina.Engine
{
    public class Scene : Crane<Actor>
    {
        public readonly Camera camera;

        public Scene(GameCanvas resizer = null)
        {
            this.iterables = new List<Actor>();
            this.camera = new Camera(resizer);
        }

        public Actor AddActor(string name, Vector2 position = new Vector2())
        {
            var actor = new Actor(name, this);
            actor.Position = position;
            return actor;
        }

        public Actor AddActor(Actor actor)
        {
            this.iterables.Add(actor);
            return actor;
        }

        public void RemoveActor(Actor actor)
        {
            actor.OnRemove();
            iterables.Remove(actor);
        }

        public override void Update(float dt)
        {
            foreach (var actor in iterables)
            {
                actor.Update(dt);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null, camera.TranslationMatrix);

            foreach (var actor in iterables)
            {
                actor.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, DepthStencilState.Default, null, null, camera.TranslationMatrix);

            foreach (var actor in iterables)
            {
                actor.DebugDraw(spriteBatch);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Same as Scene.OnRemove
        /// </summary>
        public void RemoveAllActors()
        {
            foreach (var actor in iterables)
            {
                actor.OnRemove();
            }
        }
    }
}
