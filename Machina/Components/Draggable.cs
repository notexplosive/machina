using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Components
{
    public class Draggable : BaseComponent
    {
        // TODO: Turn these into EventHandler<DragEventArgs>
        // DragEventArgs should hold: mousePosition and accumulatedDelta
        public delegate void DragAction(Vector2 mousePos, Vector2 accumulatedDelta);

        private readonly Hoverable hoverable;
        private Vector2 accumulatedDragDelta;

        public Draggable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
            IsDragging = false;
        }

        public bool IsDragging { private set; get; }

        /// <summary>
        ///     Passes the accumulated delta of the drag
        /// </summary>
        public event DragAction Drag;

        /// <summary>
        ///     Passes the cursor world position on drag start
        /// </summary>
        public event DragAction DragStart;

        public event DragAction DragEnd;

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
                        DragStart?.Invoke(currentPosition, this.accumulatedDragDelta);
                    }
                }
                else
                {
                    if (IsDragging)
                    {
                        DragEnd?.Invoke(currentPosition, this.accumulatedDragDelta);
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
                Drag?.Invoke(currentPosition, this.accumulatedDragDelta);
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            if (IsDragging)
            {
                spriteBatch.DrawLine(this.actor.transform.Position,
                    this.actor.transform.Position + this.accumulatedDragDelta, Color.Orange, 4, 0.2f);
            }
        }
    }
}