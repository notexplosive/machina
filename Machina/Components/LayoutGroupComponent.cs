using System;
using System.Collections.Generic;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace Machina.Components
{
    public class LayoutGroupComponent : BaseComponent, ILayoutGroup<Actor>
    {
        private readonly BoundingRect boundingRect;

        public Orientation Orientation { get; }
        public Point MarginSize { get; private set; }
        public int Padding { get; set; }
        public Point Size => this.boundingRect.Size;
        public Point Offset => this.boundingRect.Offset.ToPoint();
        public Point Position { get => this.transform.Position.ToPoint(); set => this.transform.Position = value.ToVector2(); }

        public ILayoutElement SetWidth(int width)
        {
            this.boundingRect.Width = width;
            return this.actor.GetComponent<LayoutElementComponent>();
        }
        public ILayoutElement SetHeight(int height)
        {
            this.boundingRect.Height = height;
            return this.actor.GetComponent<LayoutElementComponent>();
        }

        public LayoutGroupComponent(Actor actor, Orientation orientation) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.Orientation = orientation;

            this.boundingRect.onSizeChange += size => ExecuteLayout();
        }

        public override void Start()
        {
            ExecuteLayout();
        }

        public void ExecuteLayout()
        {
            LayoutGroup<Actor>.ExecuteLayout(this);
        }

        public ILayoutGroup<Actor> SetMarginSize(Point margin)
        {
            this.MarginSize = margin;
            ExecuteLayout();
            return this;
        }

        public ILayoutGroup<Actor> SetPaddingBetweenElements(int padding)
        {
            this.Padding = padding;
            ExecuteLayout();
            return this;
        }

        public List<ILayoutElement> GetAllElements()
        {
            var result = new List<ILayoutElement>();
            for (var i = 0; i < transform.ChildCount; i++)
            {
                var element = transform.ChildAt(i).GetComponent<LayoutElementComponent>();
                if (element != null)
                {
                    result.Add(element);
                }
            }

            return result;
        }

        public ILayoutGroup<Actor> AddHorizontallyStretchedSpacer()
        {
            var spacer = transform.AddActorAsChild("h");
            new BoundingRect(spacer, new Point(0));
            new LayoutElementComponent(spacer).StretchHorizontally();

            transform.FlushBuffers();
            return this;
        }

        public ILayoutGroup<Actor> AddVerticallyStretchedSpacer()
        {
            var spacer = transform.AddActorAsChild("v");
            new BoundingRect(spacer, new Point(0));
            new LayoutElementComponent(spacer).StretchVertically();

            transform.FlushBuffers();
            return this;
        }

        public ILayoutGroup<Actor> PixelSpacer(int size)
        {
            var spacer = transform.AddActorAsChild("ps" + size);
            new BoundingRect(spacer, new Point(size, size));
            new LayoutElementComponent(spacer);

            transform.FlushBuffers();
            return this;
        }

        public ILayoutGroup<Actor> PixelSpacer(int width, int height)
        {
            var spacer = transform.AddActorAsChild("ps" + width + "x" + height);
            new BoundingRect(spacer, new Point(width, height));
            new LayoutElementComponent(spacer);

            transform.FlushBuffers();
            return this;
        }

        /// <summary>
        /// Prefer using AddSpecificSizeElement
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="onPostCreate"></param>
        /// <returns></returns>
        public ILayoutElement AddElement(string name, Point size, Action<Actor> onPostCreate)
        {
            var elementActor = transform.AddActorAsChild(name);
            elementActor.transform.LocalDepth = -1;
            new BoundingRect(elementActor, size);
            var element = new LayoutElementComponent(elementActor);
            onPostCreate?.Invoke(elementActor);

            transform.FlushBuffers();
            return element;
        }

        public ILayoutGroup<Actor> AddSpecificSizeElement(string name, Point size, Action<Actor> onPostCreate)
        {
            AddElement(name, size, onPostCreate);
            return this;
        }

        public ILayoutGroup<Actor> AddVerticallyStretchedElement(string name, int size, Action<Actor> onPostCreate)
        {
            AddElement(name, new Point(size, size), onPostCreate).StretchVertically();
            return this;
        }

        public ILayoutGroup<Actor> AddHorizontallyStretchedElement(string name, int size, Action<Actor> onPostCreate)
        {
            AddElement(name, new Point(size, size), onPostCreate).StretchHorizontally();
            return this;
        }

        public ILayoutGroup<Actor> AddBothStretchedElement(string name, Action<Actor> onPostCreate)
        {
            AddElement(name, Point.Zero, onPostCreate).StretchHorizontally().StretchVertically();
            return this;
        }

        public bool IsStretchedAlong(Orientation orientation)
        {
            return this.actor.GetComponent<LayoutElementComponent>().IsStretchedAlong(orientation);
        }

        public bool IsStretchPerpendicular(Orientation orientation)
        {
            return this.actor.GetComponent<LayoutElementComponent>().IsStretchPerpendicular(orientation);
        }

        public ILayoutElement StretchHorizontally()
        {
            return this.actor.GetComponent<LayoutElementComponent>().StretchHorizontally();
        }

        public ILayoutElement StretchVertically()
        {
            return this.actor.GetComponent<LayoutElementComponent>().StretchVertically();
        }
    }
}