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
    class Scrollbar : BaseComponent
    {
        private readonly BoundingRect myBoundingRect;
        private readonly Hoverable hoverable;
        private readonly BoundingRect containerBoundingRect;
        private readonly Camera targetCamera;
        private readonly int scrollIncrement;
        public readonly MinMax<int> worldBounds;
        private bool isGrabbed;
        private float mouseYOnGrab;
        private float scrollPercentOnGrab;

        public Scrollbar(Actor actor, BoundingRect containerBoundingRect, Camera targetCamera, MinMax<int> scrollRange, int scrollIncrement = 64) : base(actor)
        {
            this.myBoundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.containerBoundingRect = containerBoundingRect;
            this.targetCamera = targetCamera;

            this.myBoundingRect.SetOffsetToTopLeft();
            this.actor.SetParent(containerBoundingRect.actor);

            this.worldBounds = scrollRange;
            this.scrollIncrement = scrollIncrement;
            CurrentScrollUnits = 0;

            this.targetCamera.OnChangeZoom += UpdateScrollReflexive;
        }

        public override void OnDelete()
        {
            this.targetCamera.OnChangeZoom -= UpdateScrollReflexive;
        }

        public override void Update(float dt)
        {
            this.myBoundingRect.Height = this.containerBoundingRect.Height;
            this.actor.progeny.LocalPosition = new Vector2(this.containerBoundingRect.Width - this.containerBoundingRect.Offset.X, -this.containerBoundingRect.Offset.Y);
            this.targetCamera.Position = new Vector2(this.targetCamera.Position.X, CurrentScrollUnits);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsScrollbarNeeded)
            {
                spriteBatch.FillRectangle(ThumbRect, Color.Orange, this.actor.progeny.Depth);
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            Point containerLocation = this.containerBoundingRect.Location;
            spriteBatch.DrawRectangle(new Rectangle(
                containerLocation,
                new Point(this.containerBoundingRect.Width, (int) TotalWorldUnits)), Color.Orange, 2);
            spriteBatch.DrawRectangle(new Rectangle(
                new Point(containerLocation.X, containerLocation.Y + (int) CurrentScrollUnits),
                new Point(this.containerBoundingRect.Width, (int) OnScreenUnits)), Color.Blue, 2);
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (button == MouseButton.Left)
            {
                var wasPressed = buttonState == ButtonState.Pressed;
                var thumbRect = ThumbRect;
                var isCursorWithinThumb = thumbRect.Contains(currentPosition) && this.hoverable.IsHovered;

                // Grab the thumb and drag (or release)
                if ((wasPressed && isCursorWithinThumb))
                {
                    this.isGrabbed = true;
                    this.mouseYOnGrab = currentPosition.Y;
                    this.scrollPercentOnGrab = CurrentScrollPercent;
                }
                // Click along the bar
                else if (wasPressed && !isCursorWithinThumb && this.hoverable.IsHovered)
                {
                    this.isGrabbed = true;
                    this.mouseYOnGrab = currentPosition.Y;


                    var thumbCenterY = thumbRect.Y + thumbRect.Height / 2;
                    var deltaFromThumb = currentPosition.Y - thumbCenterY;
                    var scrollDeltaPercent = CalculateDeltaPercent(deltaFromThumb);

                    CurrentScrollPercent += scrollDeltaPercent;

                    this.scrollPercentOnGrab = CurrentScrollPercent;
                }
                else if (!wasPressed)
                {
                    this.isGrabbed = false;
                }
            }
        }

        public override void OnScroll(int scrollDelta)
        {
            if (this.hoverable.IsHovered)
            {
                ApplyScrollDelta(scrollDelta);
            }
        }

        public void ApplyScrollDelta(int scrollDelta)
        {
            CurrentScrollUnits -= (int) (scrollDelta * this.scrollIncrement / this.targetCamera.Zoom);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            var totalDelta = currentPosition.Y - this.mouseYOnGrab;
            var totalScrollDeltaPercent = CalculateDeltaPercent(totalDelta);
            if (this.isGrabbed)
            {
                this.CurrentScrollPercent = totalScrollDeltaPercent + scrollPercentOnGrab;
            }
        }

        private void UpdateScrollReflexive(float oldZoom, float newZoom)
        {
            SetClampedScrollUnits(CurrentScrollUnits);
        }

        private float CalculateDeltaPercent(float deltaWorldUnits)
        {
            return deltaWorldUnits / (this.containerBoundingRect.Height - ThumbHeight);
        }

        private bool IsScrollbarNeeded => OnScreenPercent < 1f;
        private float TotalWorldUnits => (this.worldBounds.max - OnScreenUnits) - this.worldBounds.min;
        private float OnScreenUnits => this.containerBoundingRect.Height / this.targetCamera.Zoom;
        private float OnScreenPercent => OnScreenUnits / (this.worldBounds.max - this.worldBounds.min);
        private int ThumbHeight => (int) (this.containerBoundingRect.Height * OnScreenPercent);
        private Rectangle ThumbRect
        {
            get
            {
                float scrollPercent = CurrentScrollPercent;
                int thumbYPosition = (int) ((this.containerBoundingRect.Height - ThumbHeight) * scrollPercent);
                return new Rectangle(this.myBoundingRect.Rect.Location + new Point(0, thumbYPosition),
                            new Point(this.myBoundingRect.Width, ThumbHeight));
            }
        }

        private float currentScrollUnits;
        public float CurrentScrollUnits
        {
            get => this.currentScrollUnits;
            set
            {
                SetClampedScrollUnits(value);
            }
        }

        private void SetClampedScrollUnits(float value)
        {
            if (this.IsScrollbarNeeded)
            {
                this.currentScrollUnits = Math.Clamp(value, this.worldBounds.min, this.worldBounds.max - OnScreenUnits);
            }
            else
            {
                this.currentScrollUnits = 0;
            }
        }

        public float CurrentScrollPercent
        {
            get => (this.currentScrollUnits - this.worldBounds.min) / this.TotalWorldUnits;
            set
            {
                CurrentScrollUnits = value * this.TotalWorldUnits;
            }
        }
    }
}
