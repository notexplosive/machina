using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machina.Engine
{
    public class SceneLayers
    {
        public readonly Scene debugScene;
        private readonly List<Scene> sceneList = new List<Scene>();
        private readonly ScrollTracker scrollTracker = new ScrollTracker();
        private readonly KeyTracker keyTracker = new KeyTracker();
        private readonly MouseTracker mouseTracker = new MouseTracker();
        private readonly IFrameStep frameStep;

        public SceneLayers(Scene debugScene, IFrameStep frameStep)
        {
            this.debugScene = debugScene;
            this.frameStep = frameStep;
        }

        internal Scene AddNewScene(GameCanvas canvas)
        {
            var scene = new Scene(canvas, this.frameStep);
            Add(scene);
            return scene;
        }

        public void Add(Scene scene)
        {
            this.sceneList.Add(scene);
        }

        public Scene[] AllScenes()
        {
            Scene[] array = new Scene[sceneList.Count + (debugScene != null ? 1 : 0)];
            sceneList.CopyTo(array);
            if (debugScene != null)
            {
                array[sceneList.Count] = debugScene;
            }
            return array;
        }

        public void UpdateWithNoInput(float dt)
        {
            Update(dt, Matrix.Identity, InputState.Empty);
        }

        public void Update(float dt, InputState input)
        {
            Update(dt, Matrix.Identity, input);
        }

        public void Update(float dt, Matrix mouseTransformMatrix, InputState inputState, bool allowMouseUpdate = true, bool allowKeyboardEvents = true)
        {
            var scenes = AllScenes();

            scrollTracker.Calculate(inputState.mouseState);
            keyTracker.Calculate(inputState.keyboardState);
            mouseTracker.Calculate(inputState.mouseState);

            var rawMousePos = Vector2.Transform(mouseTracker.RawWindowPosition.ToVector2(), mouseTransformMatrix);

            if (keyTracker.Pressed.Contains(Keys.Space) && keyTracker.Modifiers.control)
            {
                this.frameStep.IsPaused = !this.frameStep.IsPaused;
            }

            if (this.frameStep.IsPaused && scrollTracker.ScrollDelta < 0)
            {
                this.frameStep.Step(scenes);
            }

            foreach (Scene scene in scenes)
            {
                scene.FlushBuffers();

                if (allowKeyboardEvents)
                {
                    foreach (var key in keyTracker.Released)
                    {
                        scene.OnKey(key, ButtonState.Released, keyTracker.Modifiers);
                    }

                    foreach (var key in keyTracker.Pressed)
                    {
                        scene.OnKey(key, ButtonState.Pressed, keyTracker.Modifiers);
                    }
                }

                if (allowMouseUpdate)
                {
                    if (scrollTracker.ScrollDelta != 0)
                    {
                        scene.OnScroll(scrollTracker.ScrollDelta);
                    }

                    foreach (var mouseButton in mouseTracker.ButtonsPressedThisFrame)
                    {
                        scene.OnMouseButton(mouseButton, rawMousePos, ButtonState.Pressed);
                    }

                    foreach (var mouseButton in mouseTracker.ButtonsReleasedThisFrame)
                    {
                        scene.OnMouseButton(mouseButton, rawMousePos, ButtonState.Released);
                    }

                    // At this point the raw and processed deltas are equal, downstream (Scene and below) they will differ
                    scene.OnMouseUpdate(rawMousePos, mouseTracker.PositionDelta, mouseTracker.PositionDelta);
                }
            }

            foreach (Scene scene in scenes)
            {
                if (!scene.frameStep.IsPaused)
                {
                    scene.Update(dt);
                }
            }

            HitTestResult.ApproveTopCandidate(scenes);
        }

        public void PreDraw(SpriteBatch spriteBatch)
        {
            var scenes = AllScenes();
            foreach (var scene in scenes)
            {
                scene.PreDraw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var scenes = AllScenes();

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
            }

            if (MachinaGame.DebugLevel > DebugLevel.Passive)
            {
                foreach (var scene in scenes)
                {
                    scene.DebugDraw(spriteBatch);
                }
            }

            if (this.frameStep.IsPaused)
            {
                spriteBatch.Begin();
                this.frameStep.Draw(spriteBatch);
                spriteBatch.End();
            }
        }
    }
}
