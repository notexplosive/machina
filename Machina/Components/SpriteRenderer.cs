﻿using Machina.Data;
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
        private float scale = 1f;
        public Color color;
        public bool IsPaused
        {
            get; set;
        }
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
            this.spriteSheet.DrawFrame(spriteBatch, CurrentFrame, this.actor.transform.Position, this.scale, this.actor.transform.Angle, new PointBool(FlipX, FlipY), this.actor.transform.Depth, color);
        }

        public override void Update(float dt)
        {
            if (!IsPaused)
            {
                IncrementTime(dt);
            }
        }

        public int CurrentFrame => this.currentAnimation.GetFrame(elapsedTime);

        public void SetFrame(int frame)
        {
            this.elapsedTime = frame;
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
