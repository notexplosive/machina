﻿using System.Diagnostics;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class LayoutElement : BaseComponent
    {
        public readonly BoundingRect boundingRect;
        private readonly LayoutGroup parentGroup;
        private bool stretchHorizontally;
        private bool stretchVertically;

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.parentGroup = this.actor.GetComponentInImmediateParent<LayoutGroup>();
            Debug.Assert(this.parentGroup != null, "LayoutElement does not have a LayoutGroup parent");
        }

        public Rectangle Rect => this.boundingRect.Rect;

        public Orientation GroupOrientation => this.parentGroup.orientation;

        public LayoutElement StretchVertically()
        {
            this.stretchVertically = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public LayoutElement StretchHorizontally()
        {
            this.stretchHorizontally = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public override void Start()
        {
            this.parentGroup.ExecuteLayout();
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(MachinaGame.Assets.GetSpriteFont("TinyFont"), this.actor.name,
                this.boundingRect.TopLeft, Color.Orange);
        }

        public bool IsStretchedAlong(Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return this.stretchVertically;
            }

            return this.stretchHorizontally;
        }

        public bool IsStretchPerpendicular(Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return this.stretchHorizontally;
            }

            return this.stretchVertically;
        }
    }
}