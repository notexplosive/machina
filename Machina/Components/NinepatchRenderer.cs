using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class NinepatchRenderer : BaseComponent
    {
        protected readonly BoundingRect boundingRect;
        private readonly NinepatchSheet.GenerationDirection generationDirection;

        public NinepatchRenderer(Actor actor, NinepatchSheet spriteSheet,
            NinepatchSheet.GenerationDirection gen = NinepatchSheet.GenerationDirection.Inner) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.generationDirection = gen;
            Sheet = spriteSheet;
        }

        public NinepatchSheet Sheet { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sheet.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, this.generationDirection,
                this.actor.transform.Depth);
        }
    }
}
