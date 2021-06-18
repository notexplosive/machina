using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    public class ResizeEventArgs : EventArgs
    {
        public Vector2 PositionOffset
        {
            get; set;
        }
        public Vector2 NewSize
        {
            get; set;
        }
    }

    public class BoundingRectResizer : BaseComponent
    {
        private enum RectEdge
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner
        }

        private readonly Hoverable hoverable;
        private readonly BoundingRect boundingRect;
        private readonly XYPair<int> grabHandleThickness;
        private GrabState grabState;
        private Vector2 currentMousePosition;
        private readonly Point? minSize;
        private readonly Point? maxSize;
        public event EventHandler<ResizeEventArgs> Resized;

        public BoundingRectResizer(Actor actor, XYPair<int> grabHandleThickness, Point? minSize, Point? maxSize) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();

            this.minSize = minSize;
            this.maxSize = maxSize;
            this.grabHandleThickness = grabHandleThickness;

            if (minSize.HasValue && maxSize.HasValue)
                ClampParentBoundingRectAndUpdateSelf();

            Resized += OnResizeDefault;
        }

        private void OnResizeDefault(object sender, ResizeEventArgs e)
        {
            this.boundingRect.transform.Position += e.PositionOffset;
            this.boundingRect.SetSize(e.NewSize.ToPoint());
        }

        private void ClampParentBoundingRectAndUpdateSelf()
        {
            this.boundingRect.Width = Math.Clamp(this.boundingRect.Width, this.minSize.Value.X, this.maxSize.Value.X);
            this.boundingRect.Height = Math.Clamp(this.boundingRect.Height, this.minSize.Value.Y, this.maxSize.Value.Y);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.grabState.edge == RectEdge.None)
            {
                var edge = GetEdgeAtPoint(currentPosition);
                if (edge != RectEdge.None)
                {
                    SetCursorBasedOnEdge(edge);
                }
            }
            else
            {
                SetCursorBasedOnEdge(this.grabState.edge);
                this.currentMousePosition = currentPosition;
            }

            if (minSize.HasValue && maxSize.HasValue)
                ClampParentBoundingRectAndUpdateSelf();
        }

        private void SetCursorBasedOnEdge(RectEdge edge)
        {
            if (edge == RectEdge.BottomLeftCorner || edge == RectEdge.TopRightCorner)
                Mouse.SetCursor(MouseCursor.SizeNESW);

            if (edge == RectEdge.TopLeftCorner || edge == RectEdge.BottomRightCorner)
                Mouse.SetCursor(MouseCursor.SizeNWSE);

            if (edge == RectEdge.Right || edge == RectEdge.Left)
                Mouse.SetCursor(MouseCursor.SizeWE);

            if (edge == RectEdge.Bottom || edge == RectEdge.Top)
                Mouse.SetCursor(MouseCursor.SizeNS);
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (button == MouseButton.Left)
            {
                if (state == ButtonState.Pressed)
                {
                    this.grabState = new GrabState(GetEdgeAtPoint(currentPosition), currentPosition, this.boundingRect.Rect, this.minSize, this.maxSize);
                }
                else
                {
                    if (this.grabState.edge != RectEdge.None)
                    {
                        Resized?.Invoke(this, new ResizeEventArgs
                        {
                            PositionOffset = this.grabState.GetPositionDelta(this.currentMousePosition),
                            NewSize = this.boundingRect.Size.ToVector2() + this.grabState.GetSizeDelta(this.currentMousePosition),
                        });
                    }
                    this.grabState = new GrabState(RectEdge.None, Vector2.Zero, Rectangle.Empty, null, null);
                }
            }
        }

        private RectEdge GetEdgeAtPoint(Vector2 currentPosition)
        {
            if (!this.hoverable.IsHovered)
            {
                return RectEdge.None;
            }

            var topLeft = this.boundingRect.TopLeft;
            var bottomRight = this.boundingRect.TopLeft + this.boundingRect.Size.ToVector2();

            var isAlongTop = currentPosition.Y < topLeft.Y + this.grabHandleThickness.Y;
            var isAlongLeft = currentPosition.X < topLeft.X + this.grabHandleThickness.X;
            var isAlongRight = currentPosition.X > bottomRight.X - this.grabHandleThickness.X;
            var isAlongBottom = currentPosition.Y > bottomRight.Y - this.grabHandleThickness.Y;

            if (isAlongTop)
            {
                if (isAlongLeft)
                {
                    return RectEdge.TopLeftCorner;
                }

                if (isAlongRight)
                {
                    return RectEdge.TopRightCorner;
                }

                return RectEdge.Top;
            }
            else if (isAlongBottom)
            {
                if (isAlongLeft)
                {
                    return RectEdge.BottomLeftCorner;
                }

                if (isAlongRight)
                {
                    return RectEdge.BottomRightCorner;
                }

                return RectEdge.Bottom;
            }
            else if (isAlongLeft)
            {
                return RectEdge.Left;
            }
            else if (isAlongRight)
            {
                return RectEdge.Right;
            }

            return RectEdge.None;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.grabState.edge != RectEdge.None)
            {
                var sizeDelta = this.grabState.GetSizeDelta(this.currentMousePosition);
                var posDelta = this.grabState.GetPositionDelta(this.currentMousePosition);

                var rect = new Rectangle((posDelta + this.transform.Position).ToPoint(), this.boundingRect.Rect.Size + sizeDelta.ToPoint());
                spriteBatch.DrawRectangle(rect, Color.White, 1f, transform.Depth - 10);
            }
        }

        private struct GrabState
        {
            public readonly RectEdge edge;
            private readonly Vector2 positionOfGrab;
            private readonly Rectangle currentRect;
            private readonly Point? minSize;
            private readonly Point? maxSize;

            public GrabState(RectEdge edge, Vector2 positionOfGrab, Rectangle currentRect, Point? minSize, Point? maxSize)
            {
                this.edge = edge;
                this.positionOfGrab = positionOfGrab;
                this.currentRect = currentRect;
                this.minSize = minSize;
                this.maxSize = maxSize;
            }

            private Vector2 GetTotalDelta(Vector2 currentPosition)
            {
                return currentPosition - positionOfGrab;
            }

            public Vector2 GetPositionDelta(Vector2 currentPosition)
            {
                if (this.edge == RectEdge.Right || this.edge == RectEdge.Bottom || this.edge == RectEdge.BottomRightCorner)
                    return Vector2.Zero;

                var delta = -GetSizeDelta(currentPosition);

                if (this.edge == RectEdge.BottomLeftCorner || this.edge == RectEdge.Left)
                {
                    delta.Y = 0;
                }

                if (this.edge == RectEdge.TopRightCorner)
                {
                    delta.X = 0;
                }

                return delta;
            }

            public Vector2 GetSizeDelta(Vector2 currentPosition)
            {
                var sizeDelta = GetTotalDelta(currentPosition);

                if (IsAlongLeft)
                {
                    sizeDelta.X = -sizeDelta.X;
                }

                if (IsLeftOrRight)
                {
                    sizeDelta.Y = 0;
                }

                if (this.edge == RectEdge.Bottom || this.edge == RectEdge.Top)
                {
                    sizeDelta.X = 0;
                }

                if (IsAlongTop)
                {
                    sizeDelta.Y = -sizeDelta.Y;
                }

                if (this.minSize.HasValue && this.maxSize.HasValue)
                {
                    var currentSize = currentRect.Size;
                    var deltaX = Math.Clamp(currentSize.X + sizeDelta.X, this.minSize.Value.X, this.maxSize.Value.X) - currentSize.X;
                    var deltaY = Math.Clamp(currentSize.Y + sizeDelta.Y, this.minSize.Value.Y, this.maxSize.Value.Y) - currentSize.Y;
                    return new Vector2(deltaX, deltaY);
                }
                else
                {
                    return sizeDelta;
                }

            }

            public bool IsAlongTop => this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.TopRightCorner || this.edge == RectEdge.Top;
            public bool IsLeftOrRight => this.edge == RectEdge.Left || this.edge == RectEdge.Right;
            public bool IsAlongLeft => this.edge == RectEdge.Left || this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.BottomLeftCorner;
        }
    }
}
