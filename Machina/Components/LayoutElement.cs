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
        public Rectangle Rect => this.boundingRect.Rect;

        public bool StretchVertically
        {
            get; set;
        }

        public bool StretchHorizontally
        {
            get; set;
        }

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }
    }
}
