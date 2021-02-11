using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
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
        private float scale = 3f;
        public Color color;
        public bool FlipX
        {
            get; set;
        }
        public bool FlipY
        {
            get; set;
        }

        public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
        {
            this.spriteSheet = spriteSheet;
            currentAnimation = spriteSheet.DefaultAnimation;
            color = Color.White;
        }

        public SpriteRenderer SetupBoundingRect()
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

            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.spriteSheet.DrawFrame(CurrentFrame, spriteBatch, this.actor.Position, this.scale, this.actor.Angle, FlipX, FlipY, this.actor.depth, color);
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

        public SpriteRenderer SetAnimation(IFrameAnimation animation)
        {
            if (!this.currentAnimation.Equals(animation))
            {
                this.elapsedTime = 0;
                this.currentAnimation = animation;
            }

            return this;
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
