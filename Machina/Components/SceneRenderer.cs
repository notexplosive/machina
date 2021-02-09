using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
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
    }
}
