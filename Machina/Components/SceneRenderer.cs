using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SceneRenderer : BaseComponent, IGameCanvas
    {
        private readonly Canvas canvas;
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;
        private readonly SceneLayers sceneLayers;
        private readonly Func<bool> shouldAllowKeyboardEvents;
        // Normally we only recieve mouse inputs if we're being hovered, this lambda lets you bypass that.
        public Func<bool> bypassHoverConstraint;
        public readonly Scene primaryScene;

        public SceneRenderer(Actor actor, Func<bool> shouldAllowKeyboardEvents) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.sceneLayers = new SceneLayers(false, this, new EmptyFrameStep());
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.hoverable.OnHoverEnd += ClearHitTesters;

            this.primaryScene = sceneLayers.AddNewScene();

            this.shouldAllowKeyboardEvents = shouldAllowKeyboardEvents;
            this.bypassHoverConstraint = () => { return false; };
        }

        private void DrawInnerScene(SpriteBatch spriteBatch)
        {
            this.sceneLayers.DrawOnCanvas(spriteBatch);
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
                Matrix.Invert(camera.GameCanvasMatrix) * Matrix.Invert(MouseTransformMatrix), InputState.Raw,
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

        public SceneLayers SceneLayers => this.sceneLayers;
        public Point ViewportSize => this.boundingRect.Size;
        public Point WindowSize => this.boundingRect.Size;
        public float ScaleFactor => 1;
        public Rectangle CanvasRect => new Rectangle(new Point(0, 0), ViewportSize);
    }
}
