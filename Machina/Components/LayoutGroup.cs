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
            var isVertical = this.orientation == Orientation.Vertical;
            var groupRect = this.boundingRect.Rect;
            var totalAlongSize = isVertical ? groupRect.Size.Y : groupRect.Size.X;

            var elements = GetAllElements();
            var remainingAlongSize = totalAlongSize - this.margin * 2;
            var stretchAlong = new List<LayoutElement>();
            var stretchPerpendicular = new List<LayoutElement>();

            int last = elements.Count - 1;
            int index = 0;

            // Determine true size of elements, subtract sizes
            foreach (var element in elements)
            {
                if (!element.IsStretchedAlong(this.orientation))
                {
                    if (isVertical)
                    {
                        remainingAlongSize -= element.Rect.Size.Y;
                    }
                    else
                    {
                        remainingAlongSize -= element.Rect.Size.X;
                    }
                }
                else
                {
                    stretchAlong.Add(element);
                }

                if (element.IsStretchPerpendicular(this.orientation))
                {
                    stretchPerpendicular.Add(element);
                }

                if (index != last)
                {
                    remainingAlongSize -= PaddingBetweenElements;
                }

                index++;
            }

            // Update size of stretch elements
            if (stretchAlong.Count > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlong.Count;

                Debug.Assert(alongSizeOfEachStretchedElement > 0, "Not enough room to lay out stretch elements");

                foreach (var alongElement in stretchAlong)
                {
                    if (isVertical)
                    {
                        alongElement.boundingRect.Height = alongSizeOfEachStretchedElement;
                    }
                    else
                    {
                        alongElement.boundingRect.Width = alongSizeOfEachStretchedElement;
                    }
                }
            }

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        perpElement.boundingRect.Width = groupRect.Width - margin * 2;
                    }
                    else
                    {
                        perpElement.boundingRect.Height = groupRect.Height - margin * 2;
                    }
                }
            }

            // Place elements
            var nextLocation = new Point(groupRect.Location.X + margin, groupRect.Location.Y + margin);
            foreach (var element in elements)
            {
                element.actor.transform.Position = nextLocation.ToVector2();
                if (isVertical)
                {
                    nextLocation += new Point(0, element.Rect.Height + this.PaddingBetweenElements);
                }
                else
                {
                    nextLocation += new Point(element.Rect.Width + this.PaddingBetweenElements, 0);
                }
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
