﻿using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class Draggable : BaseComponent
    {
        /// <summary>
        /// Passes the accumulated delta of the drag
        /// </summary>
        public Action<Vector2> onDrag;
        /// <summary>
        /// Passes the cursor world position on drag start
        /// </summary>
        public Action<Point> onDragStart;
        public bool IsDragging
        {
            private set; get;
        }
        private Vector2 accumulatedDragDelta;
        private readonly Hoverable hoverable;

        public Draggable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
            this.IsDragging = false;
        }

        public override void Update(float dt)
        {

        }

        public override void OnMouseButton(MouseButton button, Point currentPosition, ButtonState buttonState)
        {
            if (button == MouseButton.Left)
            {
                if (buttonState == ButtonState.Pressed)
                {
                    if (this.hoverable.IsHovered && !this.IsDragging)
                    {
                        this.IsDragging = true;
                        this.accumulatedDragDelta = Vector2.Zero;
                        this.onDragStart?.Invoke(currentPosition);
                    }
                }
                else
                {
                    this.IsDragging = false;
                }
            }
        }

        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.IsDragging)
            {
                this.accumulatedDragDelta += positionDelta;
                this.onDrag?.Invoke(this.accumulatedDragDelta);
            }
        }
    }
}