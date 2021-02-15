﻿using Machina.Data;
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
        protected BoundingRect boundingRect;
        protected NinepatchSheet spriteSheet;

        public NinepatchRenderer(Actor actor, NinepatchSheet spriteSheet) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.spriteSheet = spriteSheet;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteSheet.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
        }
    }
}
