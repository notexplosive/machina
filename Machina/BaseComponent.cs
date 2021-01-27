using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina
{
    abstract class BaseComponent
    {
        protected Actor actor;
        public virtual void Update(float dt) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }

        public BaseComponent(Actor actor)
        {
            this.actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            this.actor.AddComponent(this);
        }
    }
}
