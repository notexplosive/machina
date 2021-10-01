using System.Collections.Generic;
using System.Diagnostics;
using Machina.Data;
using Machina.Internals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine
{
    public interface ICrane
    {
        /// <summary>
        ///     Called every frame with the delta time since last frame.
        /// </summary>
        /// <param name="dt">Delta time since last frame</param>
        public void Update(float dt);

        /// <summary>
        ///     Runs after all updates have completed.
        /// </summary>
        public void OnPostUpdate();

        /// <summary>
        ///     Called just before Updates() are issued on any iterables that were just added.
        /// </summary>
        public void Start();

        /// <summary>
        ///     Called on every scene BEFORE any real drawing happens, does not imply spriteBatch.Begin
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void PreDraw(SpriteBatch spriteBatch);

        /// <summary>
        ///     Called every visual frame, spriteBatch.Begin has already been called.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch);

        /// <summary>
        ///     Happens after Draw if the DebugLevel is higher than Passive
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DebugDraw(SpriteBatch spriteBatch);

        /// <summary>
        ///     Called every time the scroll wheel is incremented. Also subsequently calls MouseMove with zero delta.
        /// </summary>
        /// <param name="scrollDelta"></param>
        public void OnScroll(int scrollDelta);

        /// <summary>
        ///     Called when the object is removed from its iterable set
        /// </summary>
        public void OnDeleteFinished();

        /// <summary>
        ///     Called when the object is flagged for removal from its iterable set
        /// </summary>
        public void OnDeleteImmediate();

        /// <summary>
        ///     Called when user presses or releases a key
        /// </summary>
        /// <param name="key">Key pressed or released</param>
        /// <param name="state">Enum for if the button was pressed or released</param>
        /// <param name="modifiers">Modifiers that are currently pressed (ctrl, alt, shift)</param>
        public void OnKey(Keys key, ButtonState state, ModifierKeys modifiers);

        /// <summary>
        ///     Called every time mouse moves.
        /// </summary>
        /// <param name="currentPosition">Mouse position transformed into your context</param>
        /// <param name="positionDelta">Mouse movement transformed to your context</param>
        /// <param name="rawDelta">Mouse movement delta in real-world screen pixels</param>
        public void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta);

        /// <summary>
        ///     Called when user presses or releases the mouse
        /// </summary>
        /// <param name="button">Mouse button pressed or released</param>
        /// <param name="currentPosition">Mouse position transformed to your context</param>
        /// <param name="state">Button state reflecting if the mouse was pressed or released</param>
        public void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state);

        /// <summary>
        ///     TEST ONLY: Effectively this is a zero second update that doesn't call Update.
        ///     It will flush the newIterable and deletedIterable buffers.
        /// </summary>
        public void FlushBuffers();

        /// <summary>
        ///     When a TextInput event is fired
        /// </summary>
        /// <param name="textInputEventArgs"></param>
        void OnTextInput(TextInputEventArgs textInputEventArgs);
    }

    /// <summary>
    ///     Parent class for Scene and Actor that describes how each entry-point method iterates over the iterables and runs
    ///     the same entry-point method.
    ///     Why is it called a Crane? Couldn't tell you, sometimes you just need a name for things.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Crane<T> : ICrane where T : ICrane
    {
        protected readonly List<T> iterables = new List<T>();
        private readonly List<T> iterablesCreatedThisFrame = new List<T>();
        private readonly List<T> iterablesDeletedThisFrame = new List<T>();
        private readonly List<T> iterablesGentlyRemovedThisFrame = new List<T>();

        public virtual void Update(float dt)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.Update(dt); });
            OnPostUpdate();
        }

        public void FlushBuffers()
        {
            FlushCreatedIterables();
            Functions.ResilientForEach(this.iterables, iterable => { iterable.FlushBuffers(); });
            FlushRemovedAndDeletedIterables();
        }

        public virtual void OnPostUpdate()
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnPostUpdate(); });
        }

        public virtual void Start()
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.Start(); });
        }

        public virtual void PreDraw(SpriteBatch spriteBatch)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.PreDraw(spriteBatch); });
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.Draw(spriteBatch); });
        }

        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.DebugDraw(spriteBatch); });
        }

        public virtual void OnScroll(int scrollDelta)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnScroll(scrollDelta); });
        }

        public virtual void OnDeleteFinished()
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnDeleteFinished(); });
        }

        public virtual void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnKey(key, state, modifiers); });
        }

        public virtual void OnTextInput(TextInputEventArgs textInputEventArgs)
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnTextInput(textInputEventArgs); });
        }

        public virtual void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            Functions.ResilientForEach(this.iterables,
                iterable => { iterable.OnMouseUpdate(currentPosition, positionDelta, rawDelta); });
        }

        public virtual void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            Functions.ResilientForEach(this.iterables,
                iterable => { iterable.OnMouseButton(button, currentPosition, state); });
        }

        public void OnDeleteImmediate()
        {
            Functions.ResilientForEach(this.iterables, iterable => { iterable.OnDeleteImmediate(); });
        }

        protected void AddIterable(T newIterable)
        {
            this.iterablesCreatedThisFrame.Add(newIterable);
        }

        /// <summary>
        ///     Is a child iterable pending deletion?
        /// </summary>
        /// <param name="iterable"></param>
        /// <returns></returns>
        public bool IsIterablePendingDeletion(T iterable)
        {
            return this.iterablesDeletedThisFrame.Contains(iterable) ||
                   this.iterablesGentlyRemovedThisFrame.Contains(iterable);
        }

        /// <summary>
        ///     Remove this iterable, assume it's being deleted (ie: dispose any resources)
        /// </summary>
        /// <param name="removedIterable"></param>
        protected void DeleteIterable(T removedIterable)
        {
            this.iterablesDeletedThisFrame.Add(removedIterable);
            removedIterable.OnDeleteImmediate();
        }

        /// <summary>
        ///     Remove this iterable, but don't assume it's being deleted
        /// </summary>
        protected void GentlyRemoveIterable(T removedIterable)
        {
            this.iterablesGentlyRemovedThisFrame.Add(removedIterable);
        }

        private void FlushCreatedIterables()
        {
            while (this.iterablesCreatedThisFrame.Count > 0)
            {
                var iterable = this.iterablesCreatedThisFrame[0];
                this.iterablesCreatedThisFrame.RemoveAt(0);

                this.iterables.Add(iterable);
                iterable.Start();
            }

            Debug.Assert(this.iterablesCreatedThisFrame.Count == 0);
        }

        private void FlushRemovedAndDeletedIterables()
        {
            while (this.iterablesDeletedThisFrame.Count > 0)
            {
                var iterable = this.iterablesDeletedThisFrame[0];
                this.iterablesDeletedThisFrame.RemoveAt(0);

                if (this.iterables.Remove(iterable))
                {
                    iterable.OnDeleteFinished();
                }
            }

            Debug.Assert(this.iterablesDeletedThisFrame.Count == 0);

            while (this.iterablesGentlyRemovedThisFrame.Count > 0)
            {
                var iterable = this.iterablesGentlyRemovedThisFrame[0];
                this.iterablesGentlyRemovedThisFrame.RemoveAt(0);
                this.iterables.Remove(iterable);
            }

            Debug.Assert(this.iterablesGentlyRemovedThisFrame.Count == 0);
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

        public virtual void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
        }

        public virtual void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
        }

        public virtual void OnDeleteFinished()
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

        public virtual void OnPostUpdate()
        {
        }

        public virtual void OnTextInput(TextInputEventArgs inputEventArgs)
        {
        }

        public virtual void OnDeleteImmediate()
        {
        }

        public void FlushBuffers()
        {
        }
    }
}
