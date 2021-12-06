using System.Diagnostics;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class LayoutElementComponent : BaseComponent, Layout.IElement
    {
        public readonly BoundingRect boundingRect;
        private readonly LayoutGroupComponent parentGroup;
        private bool stretchHorizontally;
        private bool stretchVertically;

        public LayoutElementComponent(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.parentGroup = this.actor.GetComponentInImmediateParent<LayoutGroupComponent>();
            Debug.Assert(this.parentGroup != null, "LayoutElement does not have a LayoutGroup parent");
        }

        public Rectangle Rect => this.boundingRect.Rect;

        public Orientation GroupOrientation => this.parentGroup.Orientation;

        public Point Size => this.boundingRect.Size;

        public Point Position { get => transform.Position.ToPoint(); set => transform.Position = value.ToVector2(); }

        public Point Offset => this.boundingRect.Offset.ToPoint();

        public Layout.IElement StretchVertically()
        {
            this.stretchVertically = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public Layout.IElement StretchHorizontally()
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
            spriteBatch.DrawString(MachinaClient.Assets.GetSpriteFont("TinyFont"), this.actor.name,
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

        public Layout.IElement SetHeight(int height)
        {
            this.boundingRect.Height = height;
            return this;
        }

        public Layout.IElement SetWidth(int width)
        {
            this.boundingRect.Width = width;
            return this;
        }
    }
}