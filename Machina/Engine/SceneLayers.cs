using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

        public SceneLayers(Scene debugScene)
        {
            this.debugScene = debugScene;
        }

        /// <summary>
        /// Creates a SceneLayers with provided trackers, only useful for testing.
        /// </summary>
        /// <param name="keyTracker"></param>
        /// <param name="mouseTracker"></param>
        /// <param name="scrollTracker"></param>
        public SceneLayers(KeyTracker keyTracker, MouseTracker mouseTracker, ScrollTracker scrollTracker)
        {
            this.keyTracker = keyTracker;
            this.mouseTracker = mouseTracker;
            this.scrollTracker = scrollTracker;
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

        public void Update(float dt, Matrix mouseTransformMatrix, bool allowMouseUpdate = true, bool allowKeyboardEvents = true)
        {
            var scenes = AllScenes();

            scrollTracker.Calculate();
            keyTracker.Calculate();
            mouseTracker.Calculate();

            var rawMousePos = Vector2.Transform(mouseTracker.RawWindowPosition.ToVector2(), mouseTransformMatrix).ToPoint();

            foreach (Scene scene in scenes)
            {
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
                scene.Update(dt);
            }

            foreach (Scene scene in scenes)
            {
                scene.PostUpdate();
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
        }
    }
}
