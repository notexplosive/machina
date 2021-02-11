using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class Hoverable : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly bool blockSubsequentHovers;
        public bool IsHovered
        {
            get;
            private set;
        }

        /// <summary>
        /// The cursor is within the BoundingRect, but it might be blocked
        /// </summary>
        public bool IsSoftHovered
        {
            get;
            private set;
        }

        public Action OnHoverStart;
        public Action OnHoverEnd;
        private bool wasHovered;
        private Point mousePos;

        public Hoverable(Actor actor, bool blockSubsequentHovers = true) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.blockSubsequentHovers = blockSubsequentHovers;
        }

        public override void Update(float dt)
        {
            this.IsSoftHovered = false;
            HitTest(this.mousePos);

            if (IsHovered && !this.wasHovered)
            {
                OnHoverStart?.Invoke();
            }

            if (!IsHovered && this.wasHovered)
            {
                OnHoverEnd?.Invoke();
            }

            this.wasHovered = IsHovered;
            IsHovered = false;
        }

        public override void OnMouseUpdate(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            HitTest(currentPosition);
            this.mousePos = currentPosition;
        }

        private void OnHitTestApproval(bool approval)
        {
            IsHovered = approval;
        }

        private void HitTest(Point mousePos)
        {
            if (this.boundingRect.Rect.Contains(mousePos))
            {
                this.IsSoftHovered = true;
                this.actor.scene.hitTester.AddCandidate(new HitTestResult(this.actor, OnHitTestApproval));
            }
        }
    }
}
