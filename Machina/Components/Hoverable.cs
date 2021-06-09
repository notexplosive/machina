using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class Hoverable : BaseComponent
    {

        public bool IsHovered
        {
            get;
            private set;
        }

        /// <summary>
        /// The cursor is within the BoundingRect, but it might be blocked or might be invisible
        /// </summary>
        public bool IsSoftHovered
        {
            get;
            private set;
        }

        public Action OnHoverStart;
        public Action OnHoverEnd;
        private bool wasHovered;
        private bool debug_isHoveredFromCallbacks;
        private bool softOnly;
        private readonly BoundingRect boundingRect;

        public Hoverable(Actor actor, bool softOnly = false) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();

            this.OnHoverStart += Debug_IndicateHoverStarted;
            this.OnHoverEnd += Debug_IndicateHoverEnded;

            this.softOnly = softOnly;
        }

        public override void Update(float dt)
        {
            // Order matters. We unhover the previous hovered, THEN hover the new hovered
            if (!IsHovered && this.wasHovered)
            {
                OnHoverEnd?.Invoke();
            }

            if (IsHovered && !this.wasHovered)
            {
                OnHoverStart?.Invoke();
            }

            this.wasHovered = IsHovered;
        }

        public override void OnPostUpdate()
        {
            IsHovered = false;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.IsSoftHovered = false;

            if (this.boundingRect.Rect.Contains(currentPosition))
            {
                this.IsSoftHovered = true;
                if (this.actor.Visible && !this.softOnly)
                {
                    this.actor.scene.hitTester.AddCandidate(new HitTestResult(this.actor, OnHitTestApproval));
                }
            }
            else
            {
                // Only really matters in a Framestep scenario
                IsHovered = false;
            }
        }

        private void OnHitTestApproval(bool approval)
        {
            IsHovered = approval;
        }

        // Debugging //

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            if (this.IsHovered)
            {
                spriteBatch.FillRectangle(this.boundingRect.Rect, new Color(Color.Blue, 0.25f), (this.actor.transform.Depth - 1).AsFloat);
            }

            if (this.debug_isHoveredFromCallbacks)
            {
                spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.Orange, 2f, (this.actor.transform.Depth - 2).AsFloat);
            }
        }
        private void Debug_IndicateHoverStarted()
        {
            this.debug_isHoveredFromCallbacks = true;
        }

        private void Debug_IndicateHoverEnded()
        {
            this.debug_isHoveredFromCallbacks = false;
        }
    }
}
