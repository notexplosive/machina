using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Machina
{
    class Scene
    {
        private readonly List<Actor> actors = new List<Actor>();
        private int previousScroll;

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
                OnScroll(scrollDelta);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var actor in actors)
            {
                actor.Draw(spriteBatch);
            }
        }
        public void OnScroll(int scrollDelta)
        {
            foreach (var actor in actors)
            {
                actor.OnScroll(scrollDelta);
            }
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        public void RemoveActor(Actor actor)
        {
            actors.Remove(actor);
        }
    }
}
