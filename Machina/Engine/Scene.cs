using System;
using System.Collections;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine.Debugging.Data;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine
{
    public class CoroutineWrapper : IEnumerable<ICoroutineAction>, IDisposable
    {
        public readonly IEnumerator<ICoroutineAction> content;
        private bool hasFinished;

        public CoroutineWrapper(IEnumerator<ICoroutineAction> content)
        {
            this.content = content;
        }

        public ICoroutineAction Current => this.content.Current;

        public bool IsDone()
        {
            return this.content.Current == null || this.hasFinished;
        }

        public bool MoveNext()
        {
            var hasNext = this.content.MoveNext();
            this.hasFinished = !hasNext;
            return hasNext;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.content;
        }

        public IEnumerator<ICoroutineAction> GetEnumerator()
        {
            return this.content;
        }

        public void Dispose()
        {
            this.content.Dispose();
        }
    }

    public class Scene : Crane<Actor>
    {
        private readonly List<CoroutineWrapper> coroutines = new List<CoroutineWrapper>();
        public readonly IFrameStep frameStep;
        public readonly HitTester hitTester = new HitTester();
        public readonly SceneLayers sceneLayers;
        public Camera camera;
        private List<Action> deferredActions = new List<Action>();

        public Scene(SceneLayers sceneLayers, IFrameStep frameStep = null)
        {
            if (sceneLayers != null)
            {
                this.camera = new Camera(sceneLayers.gameCanvas);
            }

            this.sceneLayers = sceneLayers;
            this.frameStep = frameStep != null ? frameStep : new EmptyFrameStep();
        }

        public bool IsFrozen { get; private set; }

        public float TimeScale { get; set; } = 1f;

        public void SetGameCanvas(IGameViewport gameCanvas)
        {
            this.camera = new Camera(gameCanvas);
        }

        public Actor AddActor(string name, Vector2 position = new Vector2(), float angle = 0f,
            int depthAsInt = Depth.MaxAsInt / 2)
        {
            var actor = new Actor(name, this);
            actor.transform.Position = position;
            actor.transform.Angle = angle;
            actor.transform.Depth = new Depth(depthAsInt);
            return actor;
        }

        public WaitUntil StartCoroutine(IEnumerator<ICoroutineAction> coroutine)
        {
            var wrapper = new CoroutineWrapper(coroutine);
            this.coroutines.Add(wrapper);
            coroutine.MoveNext();
            return new WaitUntil(wrapper.IsDone);
        }

        public List<Actor> GetRootLevelActors()
        {
            return new List<Actor>(this.iterables);
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

                for (var i = 0; i < actor.transform.ChildCount; i++)
                {
                    var child = actor.transform.ChildAt(i);
                    if (!actor.transform.IsIterablePendingDeletion(child))
                    {
                        extractChild(accumulator, child);
                    }
                }
            }

            var result = new List<Actor>();
            foreach (var actor in this.iterables)
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
            foreach (var action in this.deferredActions)
            {
                action.Invoke();
            }

            this.deferredActions.Clear();

            var coroutinesCopy = new List<CoroutineWrapper>(this.coroutines);
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
                        coroutine.Dispose();
                    }
                }
            }

            base.Update(dt * TimeScale);
        }

        /// <summary>
        ///     Scene stops getting input, stops updating, it just draws
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
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, this.sceneLayers.SamplerState,
                null, null, null, this.camera.GraphicsTransformMatrix);

            base.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, this.sceneLayers.SamplerState, DepthStencilState.Default,
                null, null, this.camera.GraphicsTransformMatrix);

            base.DebugDraw(spriteBatch);

            spriteBatch.End();
        }

        public override void OnMouseButton(MouseButton mouseButton, Vector2 screenPosition, ButtonState buttonState)
        {
            // Convert position to account for camera
            base.OnMouseButton(mouseButton, this.camera.ScreenToWorld(screenPosition), buttonState);
        }

        public override void OnMouseUpdate(Vector2 screenPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            ClearHitTester();
            // Convert position to account for camera
            base.OnMouseUpdate(this.camera.ScreenToWorld(screenPosition),
                Vector2.Transform(positionDelta, Matrix.Invert(this.camera.MouseDeltaMatrix)), rawDelta);
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

        /// <summary>
        /// Queues up an arbitrary function to be run at the start of next Update()
        /// </summary>
        /// <param name="action"></param>
        public void AddDeferredAction(Action action)
        {
            this.deferredActions.Add(action);
        }
    }
}