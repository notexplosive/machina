using System.Diagnostics;
using Machina.Engine;

namespace Machina.Components
{
    public interface IComponent : ICrane
    {
        /// <summary>
        ///     Called when an actor is Destroyed
        /// </summary>
        public void OnActorDestroy();
    }

    /// <summary>
    ///     Base class for all components. This lives in Machina.Components even though it's in the engine folder
    ///     because it's referenced by more Components than it is Engine things. It lives in the Engine folder to
    ///     make it clear this is not a normal component.
    /// </summary>
    public abstract class BaseComponent : NonIteratingCrane, IComponent
    {
        public readonly Actor actor;

        protected BaseComponent(Actor actor)
        {
            this.actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            this.actor.AddComponent(this);
        }

        public Transform transform => this.actor.transform;

        public virtual void OnActorDestroy()
        {
        }

        protected T RequireComponent<T>() where T : BaseComponent
        {
            var component = this.actor.GetComponent<T>();
            Debug.Assert(component != null, "Missing component " + typeof(T).FullName);
            return component;
        }

        public override string ToString()
        {
            return this.actor + "." + GetType().Name;
        }
    }
}
