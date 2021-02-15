using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    class LayoutGroup : BaseComponent
    {
        private readonly List<LayoutElement> elements = new List<LayoutElement>();
        private BoundingRect boundingRect;
        public int VerticalPadding;

        public LayoutGroup(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Update(float dt)
        {
            // ExecuteLayout(); // Avoiding doing this every frame, could get quite costly.
        }

        public void ExecuteLayout()
        {
            // Determine true size of elements
            var groupRect = this.boundingRect.Rect;
            var remainingSize = groupRect.Size;
            var verticalStretchElements = new List<LayoutElement>();
            var horizontalStretchElements = new List<LayoutElement>();
            foreach (var element in elements)
            {
                if (!element.StretchVertically)
                {
                    remainingSize.Y -= element.Rect.Size.Y;
                }
                else
                {
                    verticalStretchElements.Add(element);
                }

                if (element.StretchHorizontally)
                {
                    horizontalStretchElements.Add(element);
                }

                remainingSize.Y -= VerticalPadding;
            }

            // Update size of stretch elements
            if (verticalStretchElements.Count > 0)
            {
                var stretchHeight = remainingSize.Y / verticalStretchElements.Count;

                Debug.Assert(stretchHeight > 0, "Not enough room to lay out stretch elements");

                foreach (var verticalElement in verticalStretchElements)
                {
                    verticalElement.boundingRect.Height = stretchHeight;
                }
            }

            if (horizontalStretchElements.Count > 0)
            {
                foreach (var horizontalElement in horizontalStretchElements)
                {
                    horizontalElement.boundingRect.Width = groupRect.Width;
                }
            }

            // Place elements
            var nextElementLocation = groupRect.Location;
            foreach (var element in elements)
            {
                element.actor.transform.Position = nextElementLocation.ToVector2();
                nextElementLocation += new Point(0, element.Rect.Height) + new Point(0, this.VerticalPadding);
            }
        }

        public LayoutElement CreateElement(Actor actor)
        {
            var element = new LayoutElement(actor);
            elements.Add(element);
            ExecuteLayout();
            return element;
        }
    }
}
