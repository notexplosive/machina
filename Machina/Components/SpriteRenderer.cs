using Machina.Data;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    class SpriteRenderer : BaseComponent
    {
        public readonly SpriteSheet spriteSheet;
        private IFrameAnimation currentAnimation;
        private float elapsedTime;
        private int framesPerSecond = 15;
        private float scale = 6f;

        public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
        {
            this.spriteSheet = spriteSheet;
            currentAnimation = spriteSheet.DefaultAnimation;
        }

        public void SetupBoundingRect()
        {
            var boundingRect = this.actor.GetComponent<BoundingRect>();
            var gridBasedSpriteSheet = this.spriteSheet as GridBasedSpriteSheet;

            Debug.Assert(gridBasedSpriteSheet != null, "SpriteSheet is not compatible with SetupBoundingRect");

            if (boundingRect == null)
            {
                boundingRect = new BoundingRect(this.actor, 0, 0);
            }

            boundingRect.Width = (int) (gridBasedSpriteSheet.frameSize.X * this.scale);
            boundingRect.Height = (int) (gridBasedSpriteSheet.frameSize.Y * this.scale);

            boundingRect.SetOffsetToCenter();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.spriteSheet.DrawFrame(CurrentFrame, spriteBatch, this.actor.position, this.scale);
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

        public void SetAnimation(IFrameAnimation animation)
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
