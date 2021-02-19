using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class NinepatchRenderer : BaseComponent
    {
        protected readonly BoundingRect boundingRect;
        private readonly NinepatchSheet.GenerationDirection generationDirection;

        public NinepatchSheet SpriteSheet
        {
            get; set;
        }

        public NinepatchRenderer(Actor actor, NinepatchSheet spriteSheet, NinepatchSheet.GenerationDirection gen = NinepatchSheet.GenerationDirection.Inner) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.generationDirection = gen;
            SpriteSheet = spriteSheet;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteSheet.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, this.generationDirection, this.actor.transform.Depth);
        }
    }
}
