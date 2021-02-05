using Machina.Data;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SpriteRenderer : BaseComponent
    {
        public readonly SpriteSheet spriteSheet;
        private LinearFrameAnimation currentAnimation;
        private float elapsedTime;
        private int framesPerSecond = 15;

        public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
        {
            this.spriteSheet = spriteSheet;
            currentAnimation = spriteSheet.DefaultAnimation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.spriteSheet.DrawFrame(CurrentFrame, spriteBatch, this.actor.position, 6f);
        }

        public override void Update(float dt)
        {
            IncrementTime(dt);
        }

        public int CurrentFrame
        {
            get
            {
                return this.currentAnimation.GetFrame(elapsedTime);
            }
        }

        public void SetAnimation(LinearFrameAnimation animation)
        {
            if (!this.currentAnimation.Equals(animation))
            {
                this.elapsedTime = 0;
                this.currentAnimation = animation;
            }
        }

        private void IncrementTime(float dt)
        {
            SetElapsedTime(this.elapsedTime + dt * this.framesPerSecond);
        }

        private void SetElapsedTime(float newTime)
        {
            this.elapsedTime = newTime;
        }
    }
}
