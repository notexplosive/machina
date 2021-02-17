using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class LayoutElement : BaseComponent
    {
        public readonly BoundingRect boundingRect;
        private LayoutGroup group => this.actor.Parent?.GetComponent<LayoutGroup>();
        private bool stretchHorizontally;
        private bool stretchVertically;

        public Rectangle Rect => this.boundingRect.Rect;

        public bool StretchVertically
        {
            get => this.stretchVertically;
            set
            {
                stretchVertically = value;
                this.group?.ExecuteLayout();
            }
        }

        public bool StretchHorizontally
        {
            get => this.stretchHorizontally;
            set
            {
                stretchHorizontally = value;
                this.group?.ExecuteLayout();
            }
        }

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.group?.ExecuteLayout();
        }
    }
}
