using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace Machina
{
    class Scene
    {
        private readonly List<Actor> actors = new List<Actor>();

        public void Update(float dt)
        {
            foreach (var actor in actors)
            {
                actor.Update(dt);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var actor in actors)
            {
                actor.Draw(spriteBatch);
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
