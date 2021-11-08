using Machina.Data;
using Machina.Engine;

namespace Machina.Components
{
    public class SwapNinepatchOnHover : BaseComponent
    {
        private readonly Hoverable hoverable;
        private readonly NinepatchSheet hoverSheet;
        private readonly NinepatchSheet normalSheet;
        private readonly NinepatchRenderer renderer;

        public SwapNinepatchOnHover(Actor actor, NinepatchSheet hoverSheet) : base(actor)
        {
            this.renderer = RequireComponent<NinepatchRenderer>();
            this.hoverable = RequireComponent<Hoverable>();
            this.normalSheet = this.renderer.Sheet;
            this.hoverSheet = hoverSheet;

            this.hoverable.OnHoverStart += OnHoverStart;
            this.hoverable.OnHoverEnd += OnHoverEnd;
        }

        public override void OnDeleteFinished()
        {
            this.hoverable.OnHoverStart -= OnHoverStart;
            this.hoverable.OnHoverEnd -= OnHoverEnd;
        }

        private void OnHoverEnd()
        {
            this.renderer.Sheet = this.normalSheet;
        }

        private void OnHoverStart()
        {
            this.renderer.Sheet = this.hoverSheet;
        }
    }
}