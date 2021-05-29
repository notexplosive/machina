using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    public class LayoutElement : BaseComponent
    {
        public readonly BoundingRect boundingRect;
        private bool stretchHorizontally;
        private bool stretchVertically;
        private readonly LayoutGroup parentGroup;

        public Rectangle Rect => this.boundingRect.Rect;

        public LayoutElement StretchVertically()
        {
            stretchVertically = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public LayoutElement StretchHorizontally()
        {
            stretchHorizontally = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.parentGroup = this.actor.GetComponentInImmediateParent<LayoutGroup>();
            Debug.Assert(this.parentGroup != null, "LayoutElement does not have a LayoutGroup parent");
        }

        public override void Start()
        {
            this.parentGroup.ExecuteLayout();
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(MachinaGame.Assets.GetSpriteFont("TinyFont"), this.actor.name, this.boundingRect.TopLeft, Color.Orange);
        }

        public Orientation GroupOrientation => parentGroup.orientation;

        public bool IsStretchedAlong(Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return this.stretchVertically;
            }
            else
            {
                return this.stretchHorizontally;
            }
        }

        public bool IsStretchPerpendicular(Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return this.stretchHorizontally;
            }
            else
            {
                return this.stretchVertically;
            }
        }
    }
}
