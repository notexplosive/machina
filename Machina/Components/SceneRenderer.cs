using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SceneRenderer : UpdateOnlyComponent
    {
        private Canvas canvas;
        private Scene targetScene;

        public SceneRenderer(Actor actor, Scene targetScene) : base(actor)
        {
            this.canvas = RequireComponent<Canvas>();
            //this.canvas.DrawAdditionalContent += this.targetScene.Draw;
            this.canvas.DrawAdditionalContent += (SpriteBatch spriteBatch) => { this.targetScene.Draw(spriteBatch); };
            this.targetScene = targetScene;
        }

        public override void OnRemove()
        {
            //this.canvas.DrawAdditionalContent -= this.targetScene.Draw;
        }

        public override void Update(float dt)
        {
            this.targetScene.Update(dt);
        }
    }
}
