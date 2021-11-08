using System.Diagnostics;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class SpriteRenderer : BaseComponent
    {
        private readonly int framesPerSecond = 15;
        public readonly SpriteSheet spriteSheet;
        public Color color;
        private IFrameAnimation currentAnimation;
        private float elapsedTime;
        private Vector2 offset;
        public float scale = 1f;

        public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
        {
            this.spriteSheet = spriteSheet;
            this.currentAnimation = spriteSheet.DefaultAnimation;
            this.color = Color.White;
        }

        public SpriteRenderer(Actor actor, SpriteFrame spriteFrame) : this(actor, spriteFrame.spriteSheet)
        {
            this.currentAnimation = spriteFrame.animation;
        }

        public bool IsPaused { get; set; }

        public bool FlipX { get; set; }

        public bool FlipY { get; set; }

        public int CurrentFrame => this.currentAnimation.GetFrame(this.elapsedTime);

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
            this.spriteSheet.DrawFrame(spriteBatch, CurrentFrame, this.actor.transform.Position - this.offset,
                this.scale, this.actor.transform.Angle, new PointBool(FlipX, FlipY), this.actor.transform.Depth,
                this.color);
        }

        public override void Update(float dt)
        {
            if (!IsPaused)
            {
                IncrementTime(dt);
            }
        }

        public void DebugSpriteSheet()
        {
            this.spriteSheet.DebugMe = true;
        }

        public void SetFrame(int frame)
        {
            this.elapsedTime = frame;
        }

        public bool IsAnimationFinished()
        {
            return this.elapsedTime > this.currentAnimation.Length;
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

        public SpriteRenderer SetOffset(Vector2 offset)
        {
            this.offset = offset;
            return this;
        }
    }
}