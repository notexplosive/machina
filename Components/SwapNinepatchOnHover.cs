using Machina.Data;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class SwapNinepatchOnHover : BaseComponent
    {
        private readonly NinepatchSheet hoverSheet;
        private readonly NinepatchSheet normalSheet;
        private readonly NinepatchRenderer renderer;
        private readonly Hoverable hoverable;

        public SwapNinepatchOnHover(Actor actor, NinepatchSheet hoverSheet) : base(actor)
        {
            this.renderer = RequireComponent<NinepatchRenderer>();
            this.hoverable = RequireComponent<Hoverable>();
            this.normalSheet = renderer.Sheet;
            this.hoverSheet = hoverSheet;

            this.hoverable.OnHoverStart += OnHoverStart;
            this.hoverable.OnHoverEnd += OnHoverEnd;
        }

        public override void OnDelete()
        {
            this.hoverable.OnHoverStart -= OnHoverStart;
            this.hoverable.OnHoverEnd -= OnHoverEnd;
        }

        private void OnHoverEnd()
        {
            this.renderer.Sheet = normalSheet;
        }

        private void OnHoverStart()
        {
            this.renderer.Sheet = hoverSheet;
        }
    }
}
