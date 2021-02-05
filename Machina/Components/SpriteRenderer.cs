using Machina.Data;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SpriteRenderer : BaseComponent
    {
        private SpriteSheet spriteSheet;
        private int currentFrame;
        private float elapsedTime;
        private int fps;

        public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
        {
            this.spriteSheet = spriteSheet;
            this.currentFrame = 0;
            this.elapsedTime = 0f;
            this.fps = 15;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.spriteSheet.DrawFrame(currentFrame, spriteBatch, this.actor.position, 6f);
        }

        public override void Update(float dt)
        {
            this.elapsedTime += dt * fps;
            float totalTime = (int) spriteSheet.FrameCount;
            while (elapsedTime > totalTime)
            {
                elapsedTime -= totalTime;
            }

            while (elapsedTime < 0)
            {
                elapsedTime += totalTime;
            }
            this.currentFrame = (int) elapsedTime;
        }
    }
}
