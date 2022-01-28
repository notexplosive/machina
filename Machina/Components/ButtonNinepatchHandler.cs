using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace Machina.Components
{
    public class ButtonNinepatchHandler : BaseComponent
    {
        private readonly Clickable clickable;
        private readonly NinepatchSheet defaultSheet;
        private readonly NinepatchSheet hoverSheet;
        private readonly NinepatchSheet pressedSheet;
        private readonly NinepatchRenderer renderer;

        public ButtonNinepatchHandler(Actor actor, NinepatchSheet hoverSheet, NinepatchSheet pressedSheet) : base(actor)
        {
            this.hoverSheet = hoverSheet;
            this.pressedSheet = pressedSheet;
            this.clickable = RequireComponent<Clickable>();
            this.renderer = RequireComponent<NinepatchRenderer>();
            this.defaultSheet = this.renderer.Sheet;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.clickable.IsHovered)
            {
                this.renderer.Sheet = this.hoverSheet;

                if (this.clickable.IsPrimedForLeftMouseButton)
                {
                    this.renderer.Sheet = this.pressedSheet;
                }
            }
            else
            {
                this.renderer.Sheet = this.defaultSheet;
            }
        }
    }
}