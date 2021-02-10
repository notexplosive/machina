using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    /// <summary>
    /// Base class for all components. This lives in Machina.Components even though it's in the engine folder
    /// because it's referenced by more Components than it is Engine things. It lives in the Engine folder to
    /// make it clear this is not a normal component.
    /// </summary>
    public abstract class BaseComponent : NonIteratingCrane
    {
        public readonly Actor actor;
        public BaseComponent(Actor actor)
        {
            this.actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            this.actor.AddComponent(this);
        }

        /// <summary>
        /// Called when an actor is Destroyed
        /// </summary>
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
