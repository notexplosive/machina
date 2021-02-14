using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public interface ICrane
    {
        /// <summary>
        /// Called every frame with the delta time since last frame. Might call Start() if the
        /// iterable is being added from the temporary buffer
        /// </summary>
        /// <param name="dt">Delta time since last frame</param>
        public void Update(float dt);
        /// <summary>
        /// Runs after all updates have completed.
        /// </summary>
        public void PostUpdate();
        /// <summary>
        /// Called just before Updates() are issued on any iterables that were just added.
        /// </summary>
        public void Start();

        /// <summary>
        /// Called on every scene BEFORE any real drawing happens, does not imply spriteBatch.Start
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void PreDraw(SpriteBatch spriteBatch);
        /// <summary>
        /// Called every visual frame, spriteBatch.Start has already been called.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch);
        /// <summary>
        /// Happens after Draw if the DebugLevel is higher than Passive
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DebugDraw(SpriteBatch spriteBatch);
        /// <summary>
        /// Called every time the scroll wheel is incremented. Also subsequently calls MouseMove with zero delta.
        /// </summary>
        /// <param name="scrollDelta"></param>
        public void OnScroll(int scrollDelta);
        /// <summary>
        /// Called when the object is removed from its iterable set
        /// </summary>
        public void OnDelete();
        /// <summary>
        /// Called when user presses or releases a key
        /// </summary>
        /// <param name="key">Key pressed or released</param>
        /// <param name="state">Enum for if the button was pressed or released</param>
        /// <param name="modifiers">Modifiers that are currently pressed (ctrl, alt, shift)</param>
        public void OnKey(Keys key, ButtonState state, ModifierKeys modifiers);
        /// <summary>
        /// Called every time mouse moves.
        /// </summary>
        /// <param name="currentPosition">Mouse position transformed into your context</param>
        /// <param name="positionDelta">Mouse movement transformed to your context</param>
        /// <param name="rawDelta">Mouse movement delta in real-world screen pixels</param>
        public void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta);
        /// <summary>
        /// Called when user presses or releases the mouse
        /// </summary>
        /// <param name="button">Mouse button pressed or released</param>
        /// <param name="currentPosition">Mouse position transformed to your context</param>
        /// <param name="state">Button state reflecting if the mouse was pressed or released</param>
        public void OnMouseButton(MouseButton button, Point currentPosition, ButtonState state);
    }

    /// <summary>
    /// Parent class for Scene and Actor that describes how each entry-point method iterates over the iterables and runs
    /// the same entry-point method.
    /// 
    /// Why is it called a Crane? Couldn't tell you, sometimes you just need a name for things.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Crane<T> : ICrane where T : ICrane
    {
        private readonly List<T> iterablesCreatedThisFrame = new List<T>();
        private readonly List<T> iterablesDeletedThisFrame = new List<T>();
        private readonly List<T> iterablesGentlyRemovedThisFrame = new List<T>();
        protected readonly List<T> iterables = new List<T>();

        protected void AddIterable(T newIterable)
        {
            iterablesCreatedThisFrame.Add(newIterable);
        }

        /// <summary>
        /// Remove this iterable, assume it's being deleted (ie: dispose any resources)
        /// </summary>
        /// <param name="removedIterable"></param>
        protected void DeleteIterable(T removedIterable)
        {
            iterablesDeletedThisFrame.Add(removedIterable);
        }

        /// <summary>
        /// Remove this iterable, but don't assume it's being deleted
        /// </summary>
        protected void GentlyRemoveIterable(T removedIterable)
        {
            iterablesGentlyRemovedThisFrame.Add(removedIterable);
        }

        public virtual void Update(float dt)
        {
            foreach (var iterable in iterablesCreatedThisFrame)
            {
                this.iterables.Add(iterable);
                iterable.Start();
            }

            iterablesCreatedThisFrame.Clear();

            foreach (var iterable in iterables)
            {
                iterable.Update(dt);
            }

            foreach (var iterable in iterablesDeletedThisFrame)
            {
                if (iterables.Remove(iterable))
                {
                    iterable.OnDelete();
                }
            }

            iterablesDeletedThisFrame.Clear();

            foreach (var iterable in iterablesGentlyRemovedThisFrame)
            {
                iterables.Remove(iterable);
            }

            iterablesGentlyRemovedThisFrame.Clear();
        }

        public virtual void PostUpdate()
        {
            foreach (var iterable in iterables)
            {
                iterable.PostUpdate();
            }
        }

        public virtual void Start()
        {
            foreach (var iterable in iterables)
            {
                iterable.Start();
            }
        }

        public virtual void PreDraw(SpriteBatch spriteBatch)
        {
            foreach (var iterable in iterables)
            {
                iterable.PreDraw(spriteBatch);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var iterable in iterables)
            {
                iterable.Draw(spriteBatch);
            }
        }
        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
            foreach (var iterable in iterables)
            {
                iterable.DebugDraw(spriteBatch);
            }
        }
        public virtual void OnScroll(int scrollDelta)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnScroll(scrollDelta);
            }
        }

        public virtual void OnDelete()
        {
            foreach (var iterable in iterables)
            {
                iterable.OnDelete();
            }
        }

        public virtual void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnKey(key, state, modifiers);
            }
        }
        public virtual void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnMouseUpdate(currentPosition, positionDelta, rawDelta);
            }
        }
        public virtual void OnMouseButton(MouseButton button, Point currentPosition, ButtonState state)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnMouseButton(button, currentPosition, state);
            }
        }

        internal void OnMouseUpdate(object p, Vector2 vector2, Vector2 rawDelta)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class NonIteratingCrane : ICrane
    {
        public virtual void Start()
        {
        }
        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
        }

        public virtual void OnMouseButton(MouseButton button, Point currentPosition, ButtonState buttonState)
        {
        }

        public virtual void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
        }

        public virtual void OnDelete()
        {
        }

        public virtual void OnScroll(int scrollDelta)
        {
        }

        public virtual void PreDraw(SpriteBatch spriteBatch)
        {
        }
        public virtual void Update(float dt)
        {
        }
        public virtual void PostUpdate()
        {
        }
    }
}
