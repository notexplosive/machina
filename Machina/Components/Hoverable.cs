﻿using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class Hoverable : BaseComponent
    {

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
        private bool debug_isHoveredFromCallbacks;
        private readonly BoundingRect boundingRect;

        public Hoverable(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();

            this.OnHoverStart += Debug_IndicateHoverStarted;
            this.OnHoverEnd += Debug_IndicateHoverEnded;
        }

        public override void Update(float dt)
        {
            this.IsSoftHovered = false;
            // It kind of sucks that we have to call this in two places to make sure it "works"
            // Maybe we can remove this if we provide SceneRenderer with a full SceneLayers
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


        // Debugging //

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            if (this.IsHovered)
            {
                spriteBatch.FillRectangle(this.boundingRect.Rect, new Color(Color.Blue, 0.25f), this.actor.depth - 0.000001f);
            }

            if (this.debug_isHoveredFromCallbacks)
            {
                spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.Orange, 2f, this.actor.depth - 0.000002f);
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