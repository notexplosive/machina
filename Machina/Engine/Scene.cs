﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Machina.Engine
{
    public class Scene : Crane<Actor>
    {
        public readonly Camera camera;
        public readonly HitTester hitTester;

        public Scene(GameCanvas resizer = null)
        {
            this.camera = new Camera(resizer);
            this.hitTester = new HitTester();
        }

        public Actor AddActor(string name, Vector2 position = new Vector2())
        {
            var actor = new Actor(name, this);
            actor.Position = position;
            return actor;
        }

        public Actor AddActor(Actor actor)
        {
            AddIterable(actor);
            return actor;
        }

        public void RemoveActor(Actor actor)
        {
            DeleteIterable(actor);
        }

        public override void Update(float dt)
        {
            this.hitTester.Clear();
            base.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null, camera.GraphicsTransformMatrix);

            base.Draw(spriteBatch);

            spriteBatch.End();
        }
        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, DepthStencilState.Default, null, null, camera.GraphicsTransformMatrix);

            base.DebugDraw(spriteBatch);

            spriteBatch.End();
        }

        public override void OnMouseButton(MouseButton mouseButton, Point screenPosition, ButtonState buttonState)
        {
            base.OnMouseButton(mouseButton, camera.ScreenToWorld(screenPosition.ToVector2()).ToPoint(), buttonState);
        }

        public override void OnMouseUpdate(Point screenPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            base.OnMouseUpdate(camera.ScreenToWorld(screenPosition.ToVector2()).ToPoint(), Vector2.Transform(positionDelta, Matrix.Invert(camera.MouseDeltaMatrix)), rawDelta);
        }

        public int CountActors()
        {
            return this.iterables.Count;
        }
    }
}
