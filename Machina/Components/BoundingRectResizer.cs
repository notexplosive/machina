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
        private readonly BoundingRect myBoundingRect;
        private readonly BoundingRect parentBoundingRect;
        private readonly XYPair<MinMax<int>> sizeRanges;
        private readonly XYPair<int> padding;
        private GrabState grabState;
        private Vector2 currentMousePosition;

        public BoundingRectResizer(Actor actor, Point minSize, Point maxSize) : base(actor)
        {
            this.myBoundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.parentBoundingRect = this.actor.GetComponentInImmediateParent<BoundingRect>();
            Debug.Assert(this.parentBoundingRect != null);

            this.sizeRanges = new XYPair<MinMax<int>>(new MinMax<int>(minSize.X, maxSize.X), new MinMax<int>(minSize.Y, maxSize.Y));
            this.padding = new XYPair<int>(24, 24);

            ClampParentBoundingRectAndUpdateSelf();
        }

        private void ClampParentBoundingRectAndUpdateSelf()
        {
            this.parentBoundingRect.Width = Math.Clamp(this.parentBoundingRect.Width, this.sizeRanges.X.min, this.sizeRanges.X.max);
            this.parentBoundingRect.Height = Math.Clamp(this.parentBoundingRect.Height, this.sizeRanges.Y.min, this.sizeRanges.Y.max);
            this.myBoundingRect.Width = this.parentBoundingRect.Width + this.padding.X;
            this.myBoundingRect.Height = this.parentBoundingRect.Height + this.padding.Y;
            this.myBoundingRect.SetOffset(new Vector2(this.padding.X / 2, this.padding.Y / 2));
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
                    this.grabState = new GrabState(GetEdgeAtPoint(currentPosition), currentPosition);
                }
                else
                {
                    this.grabState = new GrabState(RectEdge.None, Vector2.Zero);
                }
            }
        }

        private RectEdge GetEdgeAtPoint(Vector2 currentPosition)
        {
            if (!this.hoverable.IsHovered)
            {
                return RectEdge.None;
            }

            var topLeft = this.myBoundingRect.TopLeft;
            var bottomRight = this.myBoundingRect.TopLeft + this.myBoundingRect.Size.ToVector2();

            var isAlongTop = currentPosition.Y < topLeft.Y + this.padding.Y;
            var isAlongLeft = currentPosition.X < topLeft.X + this.padding.X;
            var isAlongRight = currentPosition.X > bottomRight.X - this.padding.X;
            var isAlongBottom = currentPosition.Y > bottomRight.Y - this.padding.Y;

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
                var rect = this.parentBoundingRect.Rect;
                var sizeDelta = this.grabState.GetSizeDelta(this.currentMousePosition);
                var offsetDelta = this.grabState.GetOffsetDelta(this.currentMousePosition);
                rect.Inflate(sizeDelta.X, sizeDelta.Y);
                rect.Offset(offsetDelta);
                spriteBatch.DrawRectangle(rect, Color.White, 1f, transform.Depth - 10);
            }
        }

        private struct GrabState
        {
            public readonly RectEdge edge;
            private readonly Vector2 positionOfGrab;

            public GrabState(RectEdge edge, Vector2 positionOfGrab)
            {
                this.edge = edge;
                this.positionOfGrab = positionOfGrab;
            }

            public Vector2 GetDelta(Vector2 currentPosition)
            {
                return currentPosition - positionOfGrab;
            }

            public Vector2 GetOffsetDelta(Vector2 currentPosition)
            {
                var delta = GetDelta(currentPosition);

                if (this.edge == RectEdge.Right || this.edge == RectEdge.Bottom || this.edge == RectEdge.BottomRightCorner)
                {
                    return GetSizeDelta(currentPosition);
                }

                if (this.edge == RectEdge.Top)
                {
                    delta.X = 0;
                }

                if (this.edge == RectEdge.Left)
                {
                    delta.Y = 0;
                }

                if (IsAlongTop || this.edge == RectEdge.BottomLeftCorner)
                {
                    delta.Y /= 2;
                }

                if (this.edge == RectEdge.TopRightCorner || IsAlongLeft)
                {
                    delta.X /= 2;
                }

                return delta;
            }

            public Vector2 GetSizeDelta(Vector2 currentPosition)
            {
                var sizeDelta = GetDelta(currentPosition) / 2;

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

                return sizeDelta;
            }

            public bool IsAlongTop => this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.TopRightCorner || this.edge == RectEdge.Top;
            public bool IsLeftOrRight => this.edge == RectEdge.Left || this.edge == RectEdge.Right;
            public bool IsAlongLeft => this.edge == RectEdge.Left || this.edge == RectEdge.TopLeftCorner || this.edge == RectEdge.BottomLeftCorner;
        }
    }
}
