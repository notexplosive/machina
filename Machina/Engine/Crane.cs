using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    // TODO: maybe we can use an interface ICrane and make `class Crane<T> where T : ICrane`
    public abstract class Crane
    {
        protected List<Crane> iterables;
        protected bool doNotUseCraneIterator = false;

        public virtual void Update(float dt)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

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
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.PreDraw(spriteBatch);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.Draw(spriteBatch);
            }
        }
        public virtual void DebugDraw(SpriteBatch spriteBatch)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.DebugDraw(spriteBatch);
            }
        }
        public virtual void OnScroll(int scrollDelta)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.OnScroll(scrollDelta);
            }
        }

        public virtual void OnRemove()
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.OnRemove();
            }
        }

        public virtual void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.OnKey(key, buttonState, modifiers);
            }
        }
        public virtual void OnMouseMove(Point currentPosition, Vector2 positionDelta)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.OnMouseMove(currentPosition, positionDelta);
            }
        }
        public virtual void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            if (doNotUseCraneIterator)
            {
                return;
            }

            foreach (var iterable in iterables)
            {
                iterable.OnMouseButton(mouseButton, currentPosition, buttonState);
            }
        }

    }
}
