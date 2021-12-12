using System.Diagnostics;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public interface ILayoutSibling
    {
        public Orientation ParentOrientation { get; }
    }
    public class LayoutSiblingWithCachedOrientation : BaseComponent, ILayoutSibling
    {
        public LayoutSiblingWithCachedOrientation(Actor actor, Orientation orientation) : base(actor)
        {
            ParentOrientation = orientation;
        }

        public Orientation ParentOrientation { get; }
    }

    public class LayoutElement : BaseComponent, ILayoutSibling
    {
        public readonly BoundingRect boundingRect;
        private readonly LayoutGroup parentGroup;
        private bool stretchHorizontally;
        private bool stretchVertically;

        public LayoutElement(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.parentGroup = this.actor.GetComponentInImmediateParent<LayoutGroup>();
            Debug.Assert(this.parentGroup != null, "LayoutElement does not have a LayoutGroup parent");
        }

        public Rectangle Rect => this.boundingRect.Rect;

        public Orientation ParentOrientation => this.parentGroup.orientation;

        public LayoutElement StretchVertically()
        {
            this.stretchVertically = true;
            this.parentGroup.ExecuteLayout();
            return this;
        }

        public LayoutElement StretchHorizontally()
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
    }
}