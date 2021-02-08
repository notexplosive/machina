using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina
{
    /// <summary>
    /// Base class for all components. Child components are expected to implement their own Update and Draw functions.
    /// </summary>
    public abstract class BaseComponent
    {
        protected Actor actor;
        public abstract void Update(float dt);
        public abstract void Draw(SpriteBatch spriteBatch);
        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
        }
        public virtual void OnScroll(int scrollDelta)
        {
        }
        public virtual void OnActorDestroy()
        {
        }
        public virtual void OnRemove()
        {
        }
        public virtual void EarlyDraw(SpriteBatch spriteBatch)
        {
        }

        protected T RequireComponent<T>() where T : BaseComponent
        {
            var component = this.actor.GetComponent<T>();
            Debug.Assert(component != null, "Missing component " + typeof(T).FullName);
            return component;
        }

        public BaseComponent(Actor actor)
        {
            this.actor = actor;
            // THIS IS THE ONE TIME IT'S OKAY TO CALL ADD COMPONENT, ALL OTHER TIMES ARE FORBIDDEN
            this.actor.AddComponent(this);
        }

        public virtual void OnKey(Keys key, ButtonState pressType, ModifierKeys modifiers)
        {
        }

        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta)
        {
        }

        public virtual void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
        }
    }

    /// <summary>
    /// Component that does not need to implement any entrypoint functions. Essentially it's a component that just holds data
    /// </summary>
    abstract class DataComponent : BaseComponent
    {
        public DataComponent(Actor actor) : base(actor) { }

        public override void Update(float dt)
        {
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }

    /// <summary>
    /// Component that does not update, but does draw
    /// </summary>
    abstract class DrawOnlyComponent : BaseComponent
    {
        public DrawOnlyComponent(Actor actor) : base(actor) { }

        public override void Update(float dt)
        {
        }
    }

    /// <summary>
    /// Component that does not draw, but does update
    /// </summary>
    abstract class UpdateOnlyComponent : BaseComponent
    {
        public UpdateOnlyComponent(Actor actor) : base(actor) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
