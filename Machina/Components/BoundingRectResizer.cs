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
        private readonly Func<Rectangle, Rectangle> adjustRenderRectLambda;
        private GrabState grabState;
        private Vector2 currentMousePosition;
        private bool isDraggingSomething;
        private readonly Point minSize;
        private readonly Point? maxSize;
        public event EventHandler<ResizeEventArgs> Resized;

        public BoundingRectResizer(Actor actor, XYPair<int> grabHandleThickness, Point? minSize, Point? maxSize, Func<Rectangle, Rectangle> adjustRenderRectLambda = null) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();

            this.maxSize = maxSize;
            this.grabHandleThickness = grabHandleThickness;
            this.adjustRenderRectLambda = adjustRenderRectLambda;

            if (minSize.HasValue)
            {
                this.minSize = minSize.Value;
            }
            else
            {
                this.minSize = new Point(0, 0);
            }


            ClampRectMin();
            if (maxSize.HasValue)
            {
                ClampRectMax();
            }


            Resized += OnResizeDefault;
        }

        private void OnResizeDefault(object sender, ResizeEventArgs e)
        {
            this.boundingRect.transform.Position += e.PositionOffset;
            this.boundingRect.SetSize(e.NewSize.ToPoint());
        }

        private void ClampRectMin()
        {
            this.boundingRect.Width = Math.Clamp(this.boundingRect.Width, this.minSize.X, this.boundingRect.Width);
            this.boundingRect.Height = Math.Clamp(this.boundingRect.Height, this.minSize.Y, this.boundingRect.Height);
        }

        private void ClampRectMax()
        {
            this.boundingRect.Width = Math.Clamp(this.boundingRect.Width, this.boundingRect.Width, this.maxSize.Value.X);
            this.boundingRect.Height = Math.Clamp(this.boundingRect.Height, this.boundingRect.Height, this.maxSize.Value.Y);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.grabState.edge == RectEdge.None)
            {
                if (!this.isDraggingSomething)
                {
                    var edge = GetEdgeAtPoint(currentPosition);
                    if (edge != RectEdge.None)
                    {
                        SetCursorBasedOnEdge(edge);
                    }
                }
            }
            else
            {
                SetCursorBasedOnEdge(this.grabState.edge);
                this.currentMousePosition = currentPosition;
            }
        }

        private void SetCursorBasedOnEdge(RectEdge edge)
        {
            if (edge == RectEdge.BottomLeftCorner || edge == RectEdge.TopRightCorner)
                MachinaGame.SetCursor(MouseCursor.SizeNESW);

            if (edge == RectEdge.TopLeftCorner || edge == RectEdge.BottomRightCorner)
                MachinaGame.SetCursor(MouseCursor.SizeNWSE);

            if (edge == RectEdge.Right || edge == RectEdge.Left)
                MachinaGame.SetCursor(MouseCursor.SizeWE);

            if (edge == RectEdge.Bottom || edge == RectEdge.Top)
                MachinaGame.SetCursor(MouseCursor.SizeNS);
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            if (state == ButtonState.Pressed)
            {
                this.isDraggingSomething = true;
            }
            else
            {
                this.isDraggingSomething = false;
            }

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
                    this.grabState = new GrabState(RectEdge.None, Vector2.Zero, Rectangle.Empty, Point.Zero, null);
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

            var isAlongTop2x = currentPosition.Y < topLeft.Y + this.grabHandleThickness.Y * 3;
            var isAlongLeft2x = currentPosition.X < topLeft.X + this.grabHandleThickness.X * 3;
            var isAlongRight2x = currentPosition.X > bottomRight.X - this.grabHandleThickness.X * 3;
            var isAlongBottom2x = currentPosition.Y > bottomRight.Y - this.grabHandleThickness.Y * 3;

            var isAlongTop = currentPosition.Y < topLeft.Y + this.grabHandleThickness.Y;
            var isAlongLeft = currentPosition.X < topLeft.X + this.grabHandleThickness.X;
            var isAlongRight = currentPosition.X > bottomRight.X - this.grabHandleThickness.X;
            var isAlongBottom = currentPosition.Y > bottomRight.Y - this.grabHandleThickness.Y;

            if (isAlongTop2x)
            {
                if (isAlongLeft2x)
                {
                    return RectEdge.TopLeftCorner;
                }

                if (isAlongRight2x)
                {
                    return RectEdge.TopRightCorner;
                }

                if (isAlongTop)
                {
                    return RectEdge.Top;
                }
            }
            else if (isAlongBottom2x)
            {
                if (isAlongLeft2x)
                {
                    return RectEdge.BottomLeftCorner;
                }

                if (isAlongRight2x)
                {
                    return RectEdge.BottomRightCorner;
                }

                if (isAlongBottom)
                {
                    return RectEdge.Bottom;
                }
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
                rect = this.adjustRenderRectLambda(rect);
                spriteBatch.DrawRectangle(rect, Color.White, 1f, transform.Depth - 10);
            }
        }

        private struct GrabState
        {
            public readonly RectEdge edge;
            private readonly Vector2 positionOfGrab;
            private readonly Rectangle currentRect;
            private readonly Point minSize;
            private readonly Point? maxSize;

            public GrabState(RectEdge edge, Vector2 positionOfGrab, Rectangle currentRect, Point minSize, Point? maxSize)
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
                var rawSizeDelta = GetTotalDelta(currentPosition);

                if (IsAlongLeft)
                {
                    rawSizeDelta.X = -rawSizeDelta.X;
                }

                if (IsLeftOrRight)
                {
                    rawSizeDelta.Y = 0;
                }

                if (this.edge == RectEdge.Bottom || this.edge == RectEdge.Top)
                {
                    rawSizeDelta.X = 0;
                }

                if (IsAlongTop)
                {
                    rawSizeDelta.Y = -rawSizeDelta.Y;
                }

                if (this.maxSize.HasValue)
                {
                    var currentSize = currentRect.Size;
                    var deltaX = Math.Clamp(currentSize.X + rawSizeDelta.X, this.minSize.X, this.maxSize.Value.X) - currentSize.X;
                    var deltaY = Math.Clamp(currentSize.Y + rawSizeDelta.Y, this.minSize.Y, this.maxSize.Value.Y) - currentSize.Y;
                    return new Vector2(deltaX, deltaY);
                }
                else
                {
                    var currentSize = currentRect.Size;
                    var deltaX = Math.Max(currentSize.X + rawSizeDelta.X, this.minSize.X) - currentSize.X;
                    var deltaY = Math.Max(currentSize.Y + rawSizeDelta.Y, this.minSize.Y) - currentSize.Y;
                    MachinaGame.Print(deltaX, deltaY);
                    return new Vector2(deltaX, deltaY);
                }
            }

            public bool IsAlongTop => this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.TopRightCorner || this.edge == RectEdge.Top;
            public bool IsLeftOrRight => this.edge == RectEdge.Left || this.edge == RectEdge.Right;
            public bool IsAlongLeft => this.edge == RectEdge.Left || this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.BottomLeftCorner;
        }
    }
}
