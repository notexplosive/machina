﻿using Machina.Engine;
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
        private readonly PanCameraOnScroll cameraPanner;
        private readonly Camera targetCamera;
        private bool isGrabbed;
        private int mouseYOnGrab;
        private float scrollPercentOnGrab;

        public Scrollbar(Actor actor, BoundingRect containerBoundingRect, PanCameraOnScroll cameraPanner) : base(actor)
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
            spriteBatch.FillRectangle(ThumbRect, Color.Orange, this.actor.depth);
        }

        public override void OnMouseButton(MouseButton button, Point currentPosition, ButtonState buttonState)
        {
            if (button == MouseButton.Left)
            {
                var wasPressed = buttonState == ButtonState.Pressed;
                var isCursorWithinThumb = ThumbRect.Contains(currentPosition) && this.hoverable.IsHovered;

                if ((wasPressed && isCursorWithinThumb) || !wasPressed)
                {
                    this.isGrabbed = wasPressed;
                    this.mouseYOnGrab = currentPosition.Y;
                    this.scrollPercentOnGrab = this.cameraPanner.CurrentScrollPercent;
                }
            }
        }

        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            var totalDelta = currentPosition.Y - this.mouseYOnGrab;
            var totalScrollDeltaPercent = (float) totalDelta / (this.containerBoundingRect.Height - ThumbHeight);

            if (this.isGrabbed)
            {
                this.cameraPanner.CurrentScrollPercent = totalScrollDeltaPercent + scrollPercentOnGrab;
            }
        }

        private float TotalWorldDistance => this.cameraPanner.bounds.max - this.cameraPanner.bounds.min;
        private float TotalScrollbarDistance => this.containerBoundingRect.Height * this.targetCamera.Zoom;
        private float OnScreenPercent => TotalScrollbarDistance / TotalWorldDistance;
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
