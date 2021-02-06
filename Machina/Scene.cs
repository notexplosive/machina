using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Machina.Data;
using Microsoft.Xna.Framework;

namespace Machina
{
    class Scene
    {
        public readonly Camera camera = new Camera();
        private readonly List<Actor> actors = new List<Actor>();
        private int previousScroll; // TODO: move this to its own Game-wide Input object... or something?

        public void Update(float dt)
        {
            foreach (var actor in actors)
            {
                actor.Update(dt);
            }

            var currentScroll = Mouse.GetState().ScrollWheelValue;
            var scrollDelta = currentScroll - this.previousScroll;
            this.previousScroll = currentScroll;

            if (scrollDelta != 0)
            {
                OnScroll(scrollDelta / 120);
            }
        }

        public void PreDraw(SpriteBatch spriteBatch)
        {
            // Does NOT supply a spriteBatch.Begin(), you will need to supply your own when you need it
            foreach (var actor in actors)
            {
                actor.EarlyDraw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, DepthStencilState.Default, null, null, camera.TranslationMatrix);

            foreach (var actor in actors)
            {
                actor.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void OnScroll(int scrollDelta)
        {
            foreach (var actor in actors)
            {
                actor.OnScroll(scrollDelta);
            }
        }

        public Actor AddActor(Vector2 position = new Vector2())
        {
            var actor = new Actor(this);
            actor.position = position;
            return actor;
        }

        public Actor AddActor(Actor actor)
        {
            this.actors.Add(actor);
            return actor;
        }

        public void RemoveActor(Actor actor)
        {
            actor.OnRemove();
            actors.Remove(actor);
        }
    }
}
