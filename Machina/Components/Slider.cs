using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class Slider : BaseComponent, UIState<int>
    {
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;
        private float percentImpl;
        private float Percent
        {
            get => percentImpl;
            set
            {
                this.percentImpl = value;
                if (this.percentImpl < 0)
                {
                    this.percentImpl = 0;
                }

                if (this.percentImpl > 1f)
                {
                    this.percentImpl = 1f;
                }

                this.percentImpl = MathF.Round(this.percentImpl * numberOfIncrements) / numberOfIncrements;
            }
        }

        private readonly SpriteSheet thumbSprites;
        private readonly NinepatchSheet sliderSheet;
        private readonly IFrameAnimation thumbAnimation;
        private int numberOfIncrements;
        private bool isDragging;
        private Vector2 dragStartPos;
        private float percentAtStartOfDrag;
        private readonly int thumbWidth = 16;
        private int TotalWidth => this.boundingRect.Width - thumbWidth;
        private Rectangle ThumbRect => new Rectangle(this.boundingRect.Location + new Vector2(TotalWidth * Percent, 0).ToPoint(),
                                                    new Point(thumbWidth, this.boundingRect.Height));

        public Slider(Actor actor, NinepatchSheet sliderSheet, SpriteSheet thumbSprites, IFrameAnimation thumbSpriteAnimation, int numberOfIncrements = 100, int startingIncrement = 50) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.numberOfIncrements = numberOfIncrements;
            Percent = (float) startingIncrement / numberOfIncrements;
            this.thumbSprites = thumbSprites;
            this.sliderSheet = sliderSheet;
            this.thumbAnimation = thumbSpriteAnimation;
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (button == MouseButton.Left && this.hoverable.IsHovered && buttonState == ButtonState.Pressed)
            {
                var localPos = currentPosition - this.boundingRect.TopLeft - new Vector2(thumbWidth / 2, 0);
                Percent = localPos.X / this.TotalWidth;


                this.isDragging = true;
                this.dragStartPos = currentPosition;
                this.percentAtStartOfDrag = Percent;
            }

            if (buttonState == ButtonState.Released)
            {
                this.isDragging = false;
            }
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (isDragging)
            {
                var deltaPercent = (currentPosition - this.dragStartPos).X / TotalWidth;
                Percent = this.percentAtStartOfDrag + deltaPercent;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.sliderSheet.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, NinepatchSheet.GenerationDirection.Inner, transform.Depth);
            this.thumbSprites.DrawFrame(spriteBatch, this.thumbAnimation.GetFrame(0), ThumbRect.Center.ToVector2(), transform.Depth - 1);
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(ThumbRect, Color.Red, (transform.Depth - 5).AsFloat);
        }

        public int State
        {
            get => (int) (Percent * this.numberOfIncrements);
        }
    }
}
