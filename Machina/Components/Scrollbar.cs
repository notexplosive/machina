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
        private readonly PanCameraFromSceneScrollbar cameraPanner;
        private readonly Camera targetCamera;
        private bool isGrabbed;
        private int mouseYOnGrab;
        private float scrollPercentOnGrab;

        public Scrollbar(Actor actor, BoundingRect containerBoundingRect, PanCameraFromSceneScrollbar cameraPanner) : base(actor)
        {
            this.myBoundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.containerBoundingRect = containerBoundingRect;
            this.cameraPanner = cameraPanner;
            this.targetCamera = cameraPanner.actor.scene.camera;

            this.myBoundingRect.SetOffsetToTopLeft();
            this.actor.parent.Set(containerBoundingRect.actor);
        }

        public override void Update(float dt)
        {
            this.myBoundingRect.Height = this.containerBoundingRect.Height;
            this.actor.LocalPosition = new Vector2(this.containerBoundingRect.Width - this.containerBoundingRect.Offset.X, -this.containerBoundingRect.Offset.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsScrollbarNeeded)
            {
                spriteBatch.FillRectangle(ThumbRect, Color.Orange, this.actor.depth);
            }
        }

        public override void OnMouseButton(MouseButton button, Point currentPosition, ButtonState buttonState)
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
                    this.scrollPercentOnGrab = this.cameraPanner.CurrentScrollPercent;
                }
                // Click along the bar
                else if (wasPressed && !isCursorWithinThumb && this.hoverable.IsHovered)
                {
                    this.isGrabbed = true;
                    this.mouseYOnGrab = currentPosition.Y;


                    var thumbCenterY = thumbRect.Y + thumbRect.Height / 2;
                    var deltaFromThumb = currentPosition.Y - thumbCenterY;
                    var scrollDeltaPercent = CalculateDeltaPercent(deltaFromThumb);

                    this.cameraPanner.CurrentScrollPercent += scrollDeltaPercent;

                    this.scrollPercentOnGrab = this.cameraPanner.CurrentScrollPercent;
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
                this.cameraPanner.OnScroll(scrollDelta);
            }
        }

        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            var totalDelta = currentPosition.Y - this.mouseYOnGrab;
            var totalScrollDeltaPercent = CalculateDeltaPercent(totalDelta);
            if (this.isGrabbed)
            {
                this.cameraPanner.CurrentScrollPercent = totalScrollDeltaPercent + scrollPercentOnGrab;
            }
        }

        private float CalculateDeltaPercent(float deltaWorldUnits)
        {
            return deltaWorldUnits / (this.containerBoundingRect.Height - ThumbHeight);
        }

        private bool IsScrollbarNeeded => OnScreenPercent < 1f;
        private float TotalWorldUnits => this.cameraPanner.worldBounds.max - this.cameraPanner.worldBounds.min /*+ OnScreenUnits*/;
        private float OnScreenUnits => this.containerBoundingRect.Height / this.targetCamera.Zoom;
        private float OnScreenPercent => OnScreenUnits / TotalWorldUnits;
        private int ThumbHeight => (int) (this.containerBoundingRect.Height * OnScreenPercent);
        private Rectangle ThumbRect
        {
            get
            {
                float scrollPercent = this.cameraPanner.CurrentScrollPercent;
                int thumbYPosition = (int) ((this.containerBoundingRect.Height - ThumbHeight) * scrollPercent);
                return new Rectangle(this.myBoundingRect.Rect.Location + new Point(0, thumbYPosition),
                            new Point(this.myBoundingRect.Width, ThumbHeight));
            }
        }
    }
}
