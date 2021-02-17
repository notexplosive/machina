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
        private BoundingRect boundingRect;
        private Orientation orientation;
        public int PaddingBetweenElements;
        private int margin;
        private int prevChildCount;

        public LayoutGroup(Actor actor, Orientation orientation = Orientation.Vertical) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.orientation = orientation;
        }

        public override void OnDelete()
        {
        }

        public override void Update(float dt)
        {
            // We check if number of children changed in this awkward way during update because
            // if we used a callback when parent is set it would not work because the buffers
            // aren't flushed.
            if (transform.ChildCount != this.prevChildCount)
            {
                OnNumberOfChildrenChanged();
            }

            this.prevChildCount = transform.ChildCount;
        }

        public void ExecuteLayout()
        {
            // Horizontal layout is not yet supported
            // TODO: Generalize this to an "along axis" value, maybe do some callback magic?
            if (orientation == Orientation.Vertical)
            {
                // Determine true size of elements
                var elements = GetAllElements();
                var groupRect = this.boundingRect.Rect;
                var remainingSize = groupRect.Size.Y - this.margin * 2;
                var verticalStretchElements = new List<LayoutElement>();
                var horizontalStretchElements = new List<LayoutElement>();
                foreach (var element in elements)
                {
                    if (!element.StretchVertically)
                    {
                        remainingSize -= element.Rect.Size.Y;
                    }
                    else
                    {
                        verticalStretchElements.Add(element);
                    }

                    if (element.StretchHorizontally)
                    {
                        horizontalStretchElements.Add(element);
                    }

                    remainingSize -= PaddingBetweenElements;
                }

                // Update size of stretch elements
                if (verticalStretchElements.Count > 0)
                {
                    var stretchHeight = remainingSize / verticalStretchElements.Count;

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
                        horizontalElement.boundingRect.Width = groupRect.Width - margin * 2;
                    }
                }

                // Place elements
                var nextLocation = new Point(groupRect.Location.X + margin, groupRect.Location.Y + margin);
                foreach (var element in elements)
                {
                    element.actor.transform.Position = nextLocation.ToVector2();
                    nextLocation += new Point(0, element.Rect.Height + this.PaddingBetweenElements);
                }

                // If we have groups within groups, now we layout the subgroups.
                foreach (var element in elements)
                {
                    var subgroup = element.actor.GetComponent<LayoutGroup>();
                    if (subgroup != null)
                    {
                        subgroup.ExecuteLayout();
                    }
                }
            }

        }

        public void OnNumberOfChildrenChanged()
        {
            ExecuteLayout();
        }

        public LayoutGroup SetMargin(int margin)
        {
            this.margin = margin;
            ExecuteLayout();
            return this;
        }

        public List<LayoutElement> GetAllElements()
        {
            var result = new List<LayoutElement>();
            for (int i = 0; i < this.transform.ChildCount; i++)
            {
                var element = this.transform.ChildAt(i).GetComponent<LayoutElement>();
                if (element != null)
                {
                    result.Add(element);
                }
            }
            return result;
        }
    }
}
