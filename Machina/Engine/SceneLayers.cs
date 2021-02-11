using Microsoft.Xna.Framework;
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

        public void Add(Scene scene)
        {
            this.sceneList.Add(scene);
        }

        public Scene[] AllScenes()
        {
            Scene[] array = new Scene[sceneList.Count + (debugScene != null ? 1 : 0)];
            sceneList.CopyTo(array);
            array[sceneList.Count] = debugScene;
            return array;
        }

        internal void Update(float dt, Point canvasTopLeft, float canvasScaleFactor)
        {
            var scenes = AllScenes();

            // Update happens BEFORE input processing, this way we can set initial state for input processing during Update()
            // and then modify that state in input processing.
            foreach (Scene scene in scenes)
            {
                scene.Update(dt);
            }

            // Input Processing
            var delta = scrollTracker.CalculateDelta();
            keyTracker.Calculate();
            mouseTracker.Calculate(canvasTopLeft, canvasScaleFactor);

            foreach (Scene scene in scenes)
            {
                if (delta != 0)
                {
                    scene.OnScroll(delta);
                }

                foreach (var key in keyTracker.Released)
                {
                    scene.OnKey(key, ButtonState.Released, keyTracker.Modifiers);
                }

                foreach (var mouseButton in mouseTracker.Pressed)
                {
                    scene.OnMouseButton(mouseButton, mouseTracker.CurrentPosition, ButtonState.Pressed);
                }

                foreach (var mouseButton in mouseTracker.Released)
                {
                    scene.OnMouseButton(mouseButton, mouseTracker.CurrentPosition, ButtonState.Released);
                }

                foreach (var key in keyTracker.Pressed)
                {
                    scene.OnKey(key, ButtonState.Pressed, keyTracker.Modifiers);
                }

                // At this point the raw and processed deltas are equal, downstream (Scene and below) they will differ
                scene.OnMouseUpdate(mouseTracker.CurrentPosition, mouseTracker.PositionDelta, mouseTracker.PositionDelta);
            }

            var willApproveCandidate = true;
            // Traverse scenes in reverse draw order (top to bottom)
            for (int i = scenes.Length - 1; i >= 0; i--)
            {
                var scene = scenes[i];
                var candidate = scene.hitTester.Candidate;
                if (!candidate.IsEmpty())
                {
                    candidate.approvalCallback?.Invoke(willApproveCandidate);
                    willApproveCandidate = false;
                }
            }
        }
    }
}
