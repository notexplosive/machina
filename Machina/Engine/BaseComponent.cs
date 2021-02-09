using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Engine
{
    /// <summary>
    /// Base class for all components
    /// </summary>
    public abstract class BaseComponent : NonIteratingCrane
    {
        protected readonly Actor actor;
        public BaseComponent(Actor actor)
        {
            this.actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            this.actor.AddComponent(this);
        }

        public virtual void OnActorDestroy()
        {
        }

        protected T RequireComponent<T>() where T : BaseComponent
        {
            var component = this.actor.GetComponent<T>();
            Debug.Assert(component != null, "Missing component " + typeof(T).FullName);
            return component;
        }
    }
}
