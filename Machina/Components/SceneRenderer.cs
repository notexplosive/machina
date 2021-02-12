using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SceneRenderer : BaseComponent
    {
        private readonly Canvas canvas;
        private readonly Hoverable hoverable;
        private readonly SceneLayers sceneLayers = new SceneLayers(null);
        private readonly Func<bool> shouldAllowKeyboardEvents;
        // Normally we only recieve mouse inputs if we're being hovered, this lambda lets you bypass that.
        public Func<bool> bypassHoverConstraint;

        public SceneRenderer(Actor actor, Scene targetScene, Func<bool> shouldAllowKeyboardEvents) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.hoverable = RequireComponent<Hoverable>();
            this.hoverable.OnHoverEnd += ClearHitTesters;

            this.sceneLayers.Add(targetScene);
            this.shouldAllowKeyboardEvents = shouldAllowKeyboardEvents;
            this.bypassHoverConstraint = () => { return false; };
        }

        private void DrawInnerScene(SpriteBatch spriteBatch)
        {
            this.sceneLayers.Draw(spriteBatch);
        }

        public override void OnDelete()
        {
            this.canvas.DrawAdditionalContent -= DrawInnerScene;
            this.hoverable.OnHoverEnd -= ClearHitTesters;
        }

        private void ClearHitTesters()
        {
            foreach (var scene in this.sceneLayers.AllScenes())
            {
                scene.ClearHitTester();
            }
        }

        public override void Update(float dt)
        {
            var camera = this.actor.scene.camera;
            var bypassHover = this.bypassHoverConstraint.Invoke();
            this.sceneLayers.Update(
                dt,
                Matrix.Invert(camera.GameCanvasMatrix) * Matrix.Invert(MouseTransformMatrix),
                this.hoverable.IsHovered || bypassHover, this.shouldAllowKeyboardEvents());
        }

        /// <summary>
        /// Gets the position of the mouse within the scene, assuming the scene is not rotated.
        /// </summary>
        private Matrix MouseTransformMatrix
        {
            get
            {
                var topLeft = this.canvas.TopLeftCorner;
                return Matrix.CreateTranslation(topLeft.X, topLeft.Y, 0);
            }
        }
    }
}
