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
        private SceneLayers sceneLayers = new SceneLayers(null);

        public SceneRenderer(Actor actor, Scene targetScene) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            this.canvas.DrawAdditionalContent += DrawInnerScene;
            this.sceneLayers.Add(targetScene);
        }

        private void DrawInnerScene(SpriteBatch spriteBatch)
        {
            this.sceneLayers.Draw(spriteBatch);
        }

        public override void OnDelete()
        {
            this.canvas.DrawAdditionalContent -= DrawInnerScene;
        }

        public override void Update(float dt)
        {
            var camera = this.actor.scene.camera;
            this.sceneLayers.Update(dt, Matrix.Invert(camera.GameCanvasMatrix) * Matrix.Invert(MouseTransformMatrix));
        }

        /*
        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.targetScene.OnMouseUpdate(GetTransformedMousePosition(currentPosition), positionDelta, rawDelta);
        }

        public override void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            this.targetScene.OnMouseButton(mouseButton, GetTransformedMousePosition(currentPosition), buttonState);
        }

        public override void OnScroll(int scrollDelta)
        {
            this.targetScene.OnScroll(scrollDelta);
            // this.targetScene.OnMouseMove(GetTransformedMousePosition(this.cachedMousePosition), Vector2.Zero, Vector2.Zero);
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            this.targetScene.OnKey(key, buttonState, modifiers);
        }
        */

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
