using Machina.Engine;
using Microsoft.Xna.Framework;

namespace Machina.Components
{
    public class MoveOnDrag : BaseComponent
    {
        private readonly Draggable draggable;
        private readonly Transform targetTransform;
        private Vector2 positionAtStartOfDrag;

        public MoveOnDrag(Actor actor, Transform targetTransform = null) : base(actor)
        {
            this.draggable = RequireComponent<Draggable>();
            this.draggable.Drag += OnDrag;
            this.draggable.DragStart += OnDragStart;

            if (targetTransform == null)
            {
                this.targetTransform = transform;
            }
            else
            {
                this.targetTransform = targetTransform;
            }
        }

        public override void OnDeleteFinished()
        {
            this.draggable.Drag -= OnDrag;
            this.draggable.DragStart -= OnDragStart;
        }

        private void OnDragStart(Vector2 point, Vector2 delta)
        {
            this.positionAtStartOfDrag = this.targetTransform.Position; //point.ToVector2();
        }

        private void OnDrag(Vector2 mousePos, Vector2 delta)
        {
            this.targetTransform.Position = this.positionAtStartOfDrag + delta;
        }
    }
}
