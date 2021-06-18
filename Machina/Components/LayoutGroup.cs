using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Components
{
    public class LayoutGroup : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        [Obsolete]
        public int PaddingBetweenElements
        {
            set
            {
                this.padding = value;
                ExecuteLayout();
            }
        }
        private int padding;
        private int margin;

        public readonly Orientation orientation;

        public LayoutGroup(Actor actor, Orientation orientation) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.orientation = orientation;

            this.boundingRect.onSizeChange += size => ExecuteLayout();
        }

        public override void Start()
        {
            ExecuteLayout();
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
                    remainingAlongSize -= this.padding;
                }

                index++;
            }

            // Update size of stretch elements
            if (stretchAlong.Count > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlong.Count;

                if (totalAlongSize != 0)
                {
                    //Debug.Assert(alongSizeOfEachStretchedElement > 0, "Not enough room to lay out stretch elements");
                }

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
                element.actor.transform.Position = nextLocation.ToVector2() + element.boundingRect.Offset;
                if (isVertical)
                {
                    nextLocation += new Point(0, element.Rect.Height + this.padding);
                }
                else
                {
                    nextLocation += new Point(element.Rect.Width + this.padding, 0);
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

        public LayoutGroup SetMargin(int margin)
        {
            this.margin = margin;
            ExecuteLayout();
            return this;
        }

        public LayoutGroup SetPaddingBetweenElements(int padding)
        {
            this.padding = padding;
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

        public LayoutGroup HorizontallyStretchedSpacer(int size = 0)
        {
            var spacer = transform.AddActorAsChild("horizontal-spacer");
            new BoundingRect(spacer, new Point(size));
            new LayoutElement(spacer).StretchHorizontally();

            return this;
        }

        public LayoutGroup VerticallyStretchedSpacer(int size = 0)
        {
            var spacer = transform.AddActorAsChild("vertical-spacer");
            new BoundingRect(spacer, new Point(size));
            new LayoutElement(spacer).StretchVertically();

            return this;
        }

        public LayoutGroup PixelSpacer(int size)
        {
            var spacer = transform.AddActorAsChild("spacer");
            new BoundingRect(spacer, new Point(size, size));
            new LayoutElement(spacer);

            return this;
        }

        public LayoutGroup PixelSpacer(int width, int height)
        {
            var spacer = transform.AddActorAsChild("spacer");
            new BoundingRect(spacer, new Point(width, height));
            new LayoutElement(spacer);

            return this;
        }

        public LayoutElement AddElement(string name, Point size, Action<Actor> onPostCreate)
        {
            var elementActor = transform.AddActorAsChild(name);
            elementActor.transform.LocalDepth = -1;
            new BoundingRect(elementActor, size);
            var element = new LayoutElement(elementActor);
            onPostCreate?.Invoke(elementActor);

            return element;
        }

        public LayoutGroup AddVerticallyStretchedElement(string name, int size, Action<Actor> onPostCreate)
        {
            AddElement(name, new Point(size, size), onPostCreate).StretchVertically();
            return this;
        }

        public LayoutGroup AddHorizontallyStretchedElement(string name, int size, Action<Actor> onPostCreate)
        {
            AddElement(name, new Point(size, size), onPostCreate).StretchHorizontally();
            return this;
        }
        public LayoutGroup AddBothStretchedElement(string name, Action<Actor> onPostCreate)
        {
            AddElement(name, Point.Zero, onPostCreate).StretchHorizontally().StretchVertically();
            return this;
        }
    }
}
