﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class TextureRenderer : BaseComponent
    {
        public Texture2D texture;

        public TextureRenderer(Actor actor, Texture2D texture) : base(actor)
        {
            this.texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, this.actor.position, null, Color.White, this.actor.angle, new Vector2(texture.Width / 2, texture.Height / 2), Vector2.One, SpriteEffects.None, this.actor.depth);
        }

        public override void Update(float dt)
        {

        }
    }
}
