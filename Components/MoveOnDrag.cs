using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class MoveOnDrag : BaseComponent
    {
        private readonly Draggable draggable;
        private Vector2 positionAtStartOfDrag;
        private Transform targetTransform;

        public MoveOnDrag(Actor actor, Transform targetTransform = null) : base(actor)
        {
            this.draggable = RequireComponent<Draggable>();
            this.draggable.onDrag += OnDrag;
            this.draggable.onDragStart += OnDragStart;

            if (targetTransform == null)
            {
                this.targetTransform = transform;
            }
            else
            {
                this.targetTransform = targetTransform;
            }
        }

        public override void OnDelete()
        {
            this.draggable.onDrag -= OnDrag;
            this.draggable.onDragStart -= OnDragStart;
        }

        private void OnDragStart(Vector2 point)
        {
            this.positionAtStartOfDrag = this.targetTransform.Position; //point.ToVector2();
        }

        private void OnDrag(Vector2 delta)
        {
            this.targetTransform.Position = this.positionAtStartOfDrag + delta;
        }
    }
}
