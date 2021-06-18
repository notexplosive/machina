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
    public class Draggable : BaseComponent
    {
        // TODO: Turn these into EventHandler<DragEventArgs>
        // DragEventArgs should hold: mousePosition and accumulatedDelta

        /// <summary>
        /// Passes the accumulated delta of the drag
        /// </summary>
        public event Action<Vector2> Drag;
        /// <summary>
        /// Passes the cursor world position on drag start
        /// </summary>
        public event Action<Vector2> DragStart;
        public event Action<Vector2> DragEnd;

        public bool IsDragging
        {
            private set; get;
        }
        private Vector2 accumulatedDragDelta;
        private readonly Hoverable hoverable;

        public Draggable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
            IsDragging = false;
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (button == MouseButton.Left)
            {
                if (buttonState == ButtonState.Pressed)
                {
                    if (this.hoverable.IsHovered && !IsDragging)
                    {
                        IsDragging = true;
                        this.accumulatedDragDelta = Vector2.Zero;
                        DragStart?.Invoke(currentPosition);
                    }
                }
                else
                {
                    if (IsDragging)
                    {
                        DragEnd?.Invoke(currentPosition);
                    }
                    IsDragging = false;
                }
            }
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (IsDragging)
            {
                this.accumulatedDragDelta += positionDelta;
                Drag?.Invoke(this.accumulatedDragDelta);
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            if (IsDragging)
            {
                spriteBatch.DrawLine(this.actor.transform.Position, this.actor.transform.Position + accumulatedDragDelta, Color.Orange, 4, 0.2f);
            }
        }
    }
}
