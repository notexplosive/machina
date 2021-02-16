using System;
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
        public readonly IFrameStep frameStep = new EmptyFrameStep();
        public readonly HitTester hitTester = new HitTester();

        public Scene(GameCanvas gameCanvas = null, IFrameStep frameStep = null)
        {
            this.camera = new Camera(gameCanvas);
            if (frameStep != null)
            {
                this.frameStep = frameStep;
            }
        }

        public Actor AddActor(string name, Vector2 position = new Vector2(), float angle = 0f, float depth = 0.5f)
        {
            var actor = new Actor(name, this);
            actor.transform.Position = position;
            actor.transform.Angle = angle;
            actor.transform.Depth = depth;
            return actor;
        }

        public List<Actor> GetAllActors()
        {
            void extractChild(List<Actor> accumulator, Actor actor)
            {
                if (IsIterablePendingDeletion(actor))
                {
                    return;
                }
                accumulator.Add(actor);

                for (int i = 0; i < actor.ChildCount; i++)
                {
                    var child = actor.transform.ChildAt(i);
                    extractChild(accumulator, child);
                }
            }

            var result = new List<Actor>();
            foreach (var actor in iterables)
            {
                extractChild(result, actor);
            }

            return result;
        }

        public Actor AddActor(Actor actor)
        {
            AddIterable(actor);
            return actor;
        }

        public void DeleteActor(Actor actor)
        {
            DeleteIterable(actor);
        }

        public void GentlyRemoveActor(Actor actor)
        {
            GentlyRemoveIterable(actor);
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

        public override void OnMouseButton(MouseButton mouseButton, Vector2 screenPosition, ButtonState buttonState)
        {
            // Convert position to account for camera
            base.OnMouseButton(mouseButton, camera.ScreenToWorld(screenPosition), buttonState);
        }

        public override void OnMouseUpdate(Vector2 screenPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            ClearHitTester();
            // Convert position to account for camera
            base.OnMouseUpdate(camera.ScreenToWorld(screenPosition), Vector2.Transform(positionDelta, Matrix.Invert(camera.MouseDeltaMatrix)), rawDelta);
        }

        public void ClearHitTester()
        {
            this.hitTester.Clear();
        }

        public int CountActors()
        {
            return this.iterables.Count;
        }

        public void Step()
        {
            this.frameStep.Step(this);
        }
    }
}
