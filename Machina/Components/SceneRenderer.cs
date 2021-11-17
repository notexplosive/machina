using System;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class SceneRenderer : BaseComponent, IGameCanvas
    {
        private readonly BoundingRect boundingRect;
        private readonly BoundedCanvas canvas;
        private readonly Hoverable hoverable;
        public readonly Scene primaryScene;

        // Normally we only recieve mouse inputs if we're being hovered, this lambda lets you bypass that.
        private Func<bool> bypassHoverConstraint;
        private Func<bool> shouldAllowKeyboardEvents;

        public SceneRenderer(Actor actor) : base(actor)
        {
            this.canvas = RequireComponent<BoundedCanvas>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            SceneLayers = new SceneLayers(false, this);
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.hoverable.OnHoverEnd += ClearHitTesters;

            this.primaryScene = SceneLayers.AddNewScene();

            this.shouldAllowKeyboardEvents = () => false;
            this.bypassHoverConstraint = () => false;
        }

        /// <summary>
        ///     Gets the position of the mouse within the scene, assuming the scene is not rotated.
        /// </summary>
        private Matrix MouseTransformMatrix
        {
            get
            {
                var topLeft = this.canvas.TopLeftCorner;
                return Matrix.CreateTranslation(topLeft.X, topLeft.Y, 0);
            }
        }

        public SceneLayers SceneLayers { get; }

        public Point ViewportSize => this.boundingRect.Size;
        public Point WindowSize => this.boundingRect.Size;
        public float ScaleFactor => 1;
        public Rectangle CanvasRect => new Rectangle(new Point(0, 0), ViewportSize);

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
            SceneLayers.DrawOnCanvas(spriteBatch);
        }

        public override void OnDeleteFinished()
        {
            this.canvas.DrawAdditionalContent -= DrawInnerScene;
            this.hoverable.OnHoverEnd -= ClearHitTesters;
        }

        private void ClearHitTesters()
        {
            foreach (var scene in SceneLayers.AllScenes())
            {
                scene.ClearHitTester();
            }
        }

        public override void Update(float dt)
        {
            var camera = this.actor.scene.camera;
            var bypassHover = this.bypassHoverConstraint.Invoke();
            SceneLayers.Update(
                dt,
                Matrix.Invert(camera.GameCanvasMatrix) * Matrix.Invert(MouseTransformMatrix),
                this.actor.scene.sceneLayers.CurrentInputFrameState,
                this.hoverable.IsHovered || bypassHover, this.shouldAllowKeyboardEvents());
        }

        public override void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            if (this.shouldAllowKeyboardEvents())
            {
                SceneLayers.AddPendingTextInput(null, inputEventArgs);
            }
        }
    }
}