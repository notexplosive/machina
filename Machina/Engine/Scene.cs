using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Machina.Data;
using Microsoft.Xna.Framework;

namespace Machina.Engine
{
    public class Scene
    {
        public readonly Camera camera = new Camera();
        private readonly List<Actor> actors = new List<Actor>();
        private ResizeStatus resizer;

        public Scene(ResizeStatus resizer = null)
        {
            this.resizer = resizer;
        }

        public void Update(float dt)
        {
            foreach (var actor in actors)
            {
                actor.Update(dt);
            }
        }

        public void PreDraw(SpriteBatch spriteBatch)
        {
            // Does NOT supply a spriteBatch.Begin(), you will need to supply your own when you need it
            foreach (var actor in actors)
            {
                actor.EarlyDraw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.resizer != null)
            {
                this.camera.NativeScaleFactor = resizer.ScaleFactor;
            }

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null, camera.TranslationMatrix);

            foreach (var actor in actors)
            {
                actor.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
        public void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, DepthStencilState.Default, null, null, camera.TranslationMatrix);

            foreach (var actor in actors)
            {
                actor.DebugDraw(spriteBatch);
            }

            spriteBatch.End();
        }

        public Actor AddActor(Vector2 position = new Vector2())
        {
            var actor = new Actor(this);
            actor.position = position;
            return actor;
        }

        public Actor AddActor(Actor actor)
        {
            this.actors.Add(actor);
            return actor;
        }

        public void RemoveActor(Actor actor)
        {
            actor.OnRemove();
            actors.Remove(actor);
        }

        public void OnScroll(int scrollDelta)
        {
            foreach (var actor in actors)
            {
                actor.OnScroll(scrollDelta);
            }
        }

        public void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            foreach (var actor in actors)
            {
                actor.OnKey(key, buttonState, modifiers);
            }
        }

        public void OnMouseMove(Point currentPosition, Vector2 positionDelta)
        {
            foreach (var actor in actors)
            {
                actor.OnMouseMove(currentPosition, positionDelta);
            }
        }

        public void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            foreach (var actor in actors)
            {
                actor.OnMouseButton(mouseButton, currentPosition, buttonState);
            }
        }
    }
}
