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
        public void Update(float dt);
        public void PreDraw(SpriteBatch spriteBatch);
        public void Draw(SpriteBatch spriteBatch);
        public void DebugDraw(SpriteBatch spriteBatch);
        public void OnScroll(int scrollDelta);
        public void OnRemove();
        public void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers);
        public void OnMouseMove(Point currentPosition, Vector2 positionDelta);
        public void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState);
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

            this.OnMouseMove(Mouse.GetState().Position, new Vector2(0, 0));
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
        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta)
        {
            foreach (var iterable in iterables)
            {
                iterable.OnMouseMove(currentPosition, positionDelta);
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

        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta)
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
