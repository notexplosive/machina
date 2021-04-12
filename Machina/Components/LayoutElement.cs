using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    public class LayoutElement : BaseComponent
    {
        public readonly BoundingRect boundingRect;
        private LayoutGroup group => this.actor.transform.Parent?.actor.GetComponent<LayoutGroup>();
        private bool stretchHorizontally;
        private bool stretchVertically;

        public Rectangle Rect => this.boundingRect.Rect;

        public LayoutElement StretchVertically()
        {
            stretchVertically = true;
            this.group?.ExecuteLayout();
            return this;
        }

        public LayoutElement StretchHorizontally()
        {
            stretchHorizontally = true;
            this.group?.ExecuteLayout();
            return this;
        }

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.group?.ExecuteLayout();

            Debug.Assert(this.actor.GetComponentInImmediateParent<LayoutGroup>() != null, "LayoutElement does not have a LayoutGroup parent");
        }

        public Orientation GroupOrientation => group.orientation;

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
