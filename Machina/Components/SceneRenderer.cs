using Machina.Engine;
using Machina.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class SceneRenderer : BaseComponent, IGameCanvas
    {
        private readonly Canvas canvas;
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;
        private readonly SceneLayers sceneLayers;
        private Func<bool> shouldAllowKeyboardEvents;
        // Normally we only recieve mouse inputs if we're being hovered, this lambda lets you bypass that.
        private Func<bool> bypassHoverConstraint;
        public readonly Scene primaryScene;

        public SceneRenderer(Actor actor) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.sceneLayers = new SceneLayers(false, this, new EmptyFrameStep());
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.hoverable.OnHoverEnd += ClearHitTesters;

            this.primaryScene = sceneLayers.AddNewScene();

            this.shouldAllowKeyboardEvents = () => false;
            this.bypassHoverConstraint = () => false;
        }

        public SceneRenderer SetShouldAllowKeyboardEventsLambda(Func<bool> shouldAllowKeyboardEvents)
        {
            this.shouldAllowKeyboardEvents = shouldAllowKeyboardEvents;
            return this;
        }

        public SceneRenderer SetBypassHoverConstraintLambda(Func<bool> canBypassHoverConstraint)
        {
            this.bypassHoverConstraint = canBypassHoverConstraint;
            return this;
        }

        private void DrawInnerScene(SpriteBatch spriteBatch)
        {
            this.sceneLayers.DrawOnCanvas(spriteBatch);
        }

        public override void OnDeleteFinished()
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
                Matrix.Invert(camera.GameCanvasMatrix) * Matrix.Invert(MouseTransformMatrix), this.actor.scene.sceneLayers.CurrentInputFrameState,
                this.hoverable.IsHovered || bypassHover, this.shouldAllowKeyboardEvents());
        }

        public override void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            if (this.shouldAllowKeyboardEvents())
            {
                this.sceneLayers.AddPendingTextInput(null, inputEventArgs);
            }
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
