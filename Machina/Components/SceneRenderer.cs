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
        private Canvas canvas;
        private Scene targetScene;

        public SceneRenderer(Actor actor, Scene targetScene) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.targetScene = targetScene;
        }

        private void DrawInnerScene(SpriteBatch spriteBatch)
        {
            this.targetScene.Draw(spriteBatch);
        }

        public override void OnRemove()
        {
            this.canvas.DrawAdditionalContent -= DrawInnerScene;
        }

        public override void Update(float dt)
        {
            this.targetScene.Update(dt);
        }


        public override void OnMouseMove(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.targetScene.OnMouseMove(GetTransformedMousePosition(currentPosition), positionDelta, rawDelta);
        }

        public override void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            this.targetScene.OnMouseButton(mouseButton, GetTransformedMousePosition(currentPosition), buttonState);
        }

        public override void OnScroll(int scrollDelta)
        {
            this.targetScene.OnScroll(scrollDelta);
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            this.targetScene.OnKey(key, buttonState, modifiers);
        }

        /// <summary>
        /// Gets the position of the mouse within the scene, assuming the scene is not rotated.
        /// </summary>
        private Point GetTransformedMousePosition(Point currentPosition)
        {
            var topLeft = this.canvas.TopLeftCorner;
            var transform = Matrix.CreateTranslation(topLeft.X, topLeft.Y, 0);
            var transformedPosition = Vector2.Transform(currentPosition.ToVector2(), Matrix.Invert(transform));
            return transformedPosition.ToPoint();
        }
    }
}
