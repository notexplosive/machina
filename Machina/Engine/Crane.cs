﻿using Microsoft.Xna.Framework;
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
        /// Called every frame with the delta time since last frame.
        /// </summary>
        /// <param name="dt">Delta time since last frame</param>
        public void Update(float dt);
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
        public void OnRemove();
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
        /// <param name="transformedPositionDelta">Mouse movement transformed to your context</param>
        /// <param name="rawDelta">Mouse movement delta as a raw value</param>
        public void OnMouseMove(Point currentPosition, Vector2 transformedPositionDelta, Vector2 rawDelta);
        /// <summary>
        /// Called when user presses or releases the mouse
        /// </summary>
        /// <param name="button">Mouse button pressed or released</param>
        /// <param name="currentPosition">Mouse position transformed to your context</param>
        /// <param name="state">Button state reflecting if the mouse was pressed or released</param>
        public void OnMouseButton(MouseButton button, Point currentPosition, ButtonState state);
    }

    public abstract class Crane<T> : ICrane where T : ICrane
    {
        protected List<T> iterables;

        public virtual void Update(float dt)
        {
            foreach (var iterable in iterables)
            {
                iterable.Update(dt);
            }
        }

        /// <summary>
        /// Does NOT supply spriteBatch.Start()
        /// </summary>
        /// <param name="spriteBatch"></param>
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

            this.OnMouseMove(Mouse.GetState().Position, Vector2.Zero, Vector2.Zero);
        }

        public virtual void OnRemove()
        {
            foreach (var iterable in iterables)
            {
                iterable.OnRemove();
            }
        }

        public virtual void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnKey(key, buttonState, modifiers);
            }
        }
        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnMouseMove(currentPosition, positionDelta, rawDelta);
            }
        }
        public virtual void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnMouseButton(mouseButton, currentPosition, buttonState);
            }
        }
    }

    public abstract class NonIteratingCrane : ICrane
    {
        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

        public virtual void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
        }

        public virtual void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
        }

        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
        }

        public virtual void OnRemove()
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
    }
}
