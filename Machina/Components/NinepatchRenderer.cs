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
        public NinepatchSheet SpriteSheet
        {
            get; set;
        }

        public NinepatchRenderer(Actor actor, NinepatchSheet spriteSheet) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            SpriteSheet = spriteSheet;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteSheet.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
        }
    }
}
