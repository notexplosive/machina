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

        public MoveOnDrag(Actor actor) : base(actor)
        {
            this.draggable = RequireComponent<Draggable>();
            this.draggable.onDrag += OnDrag;
            this.draggable.onDragStart += OnDragStart;
        }

        public override void OnDelete()
        {
            this.draggable.onDrag -= OnDrag;
            this.draggable.onDragStart -= OnDragStart;
        }

        private void OnDragStart(Vector2 point)
        {
            this.positionAtStartOfDrag = this.actor.progeny.Position; //point.ToVector2();
        }

        private void OnDrag(Vector2 delta)
        {
            this.actor.progeny.Position = this.positionAtStartOfDrag + delta;
        }
    }
}
