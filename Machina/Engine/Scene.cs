using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Machina.Data;
using Machina.Engine.Debugging.Data;

namespace Machina.Engine
{
    public class Scene : Crane<Actor>
    {
        private readonly List<IEnumerator<ICoroutineAction>> coroutines = new List<IEnumerator<ICoroutineAction>>();
        public readonly IFrameStep frameStep;
        public readonly SceneLayers sceneLayers;
        public readonly HitTester hitTester = new HitTester();
        public Camera camera;
        public bool IsFrozen
        {
            get; private set;
        }
        public float TimeScale
        {
            get;
            set;
        } = 1f;

        public Scene(SceneLayers sceneLayers, IFrameStep frameStep = null)
        {
            if (sceneLayers != null)
            {
                this.camera = new Camera(sceneLayers.gameCanvas);
            }

            this.sceneLayers = sceneLayers;
            this.frameStep = frameStep != null ? frameStep : new EmptyFrameStep();
        }

        public void SetGameCanvas(IGameCanvas gameCanvas)
        {
            this.camera = new Camera(gameCanvas);
        }

        public Actor AddActor(string name, Vector2 position = new Vector2(), float angle = 0f, int depthAsInt = Depth.MaxAsInt / 2)
        {
            var actor = new Actor(name, this);
            actor.transform.Position = position;
            actor.transform.Angle = angle;
            actor.transform.Depth = new Depth(depthAsInt);
            return actor;
        }

        public WaitUntil StartCoroutine(IEnumerator<ICoroutineAction> coroutine)
        {
            this.coroutines.Add(coroutine);
            coroutine.MoveNext();
            return new WaitUntil(() => coroutine.Current == null);
        }

        public List<Actor> GetRootLevelActors()
        {
            return new List<Actor>(iterables);
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

                for (int i = 0; i < actor.transform.ChildCount; i++)
                {
                    var child = actor.transform.ChildAt(i);
                    if (!actor.transform.IsIterablePendingDeletion(child))
                    {
                        extractChild(accumulator, child);
                    }
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
            if (actor.transform.Parent != null)
            {
                actor.transform.Parent.DeleteChild(actor);
            }
            else
            {
                DeleteIterable(actor);
            }
        }

        public void GentlyRemoveActor(Actor actor)
        {
            GentlyRemoveIterable(actor);
        }

        public override void Update(float dt)
        {
            var coroutinesCopy = new List<IEnumerator<ICoroutineAction>>(this.coroutines);
            foreach (var coroutine in coroutinesCopy)
            {
                if (coroutine.Current == null)
                {
                    this.coroutines.Remove(coroutine);
                }
                else if (coroutine.Current.IsComplete(dt * TimeScale))
                {
                    var hasNext = coroutine.MoveNext();
                    if (!hasNext || coroutine.Current == null)
                    {
                        this.coroutines.Remove(coroutine);
                    }
                }
            }
            base.Update(dt * TimeScale);
        }

        /// <summary>
        /// Scene stops getting input, stops updating, it just draws
        /// </summary>
        public void Freeze()
        {
            IsFrozen = true;
        }

        public void Unfreeze()
        {
            IsFrozen = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var blend = new BlendState
            {
                ColorBlendFunction = BlendState.NonPremultiplied.ColorBlendFunction,
                ColorDestinationBlend = BlendState.NonPremultiplied.ColorDestinationBlend,
                ColorSourceBlend = BlendState.NonPremultiplied.ColorSourceBlend,
                AlphaSourceBlend = BlendState.NonPremultiplied.AlphaSourceBlend,
                AlphaDestinationBlend = Blend.DestinationAlpha,
                AlphaBlendFunction = BlendState.NonPremultiplied.AlphaBlendFunction
            };
            spriteBatch.Begin(SpriteSortMode.BackToFront, blend, this.sceneLayers.SamplerState, DepthStencilState.DepthRead, null, null, camera.GraphicsTransformMatrix);

            base.Draw(spriteBatch);

            spriteBatch.End();
        }
        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, this.sceneLayers.SamplerState, DepthStencilState.Default, null, null, camera.GraphicsTransformMatrix);

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

        ~Scene()
        {
            DeleteAllActors();
        }

        public void DeleteAllActors()
        {
            foreach (var actor in GetAllActors())
            {
                actor.Delete();
            }

            FlushBuffers();
        }
    }
}
