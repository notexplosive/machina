using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class TextureRenderer : BaseComponent
    {
        public readonly Texture2D texture;
        private Vector2 offset;

        public TextureRenderer(Actor actor, Texture2D texture) : base(actor)
        {
            this.texture = texture;
            this.offset = Vector2.Zero;
        }

        public TextureRenderer CenterOffset()
        {
            this.offset = new Vector2(texture.Width / 2, texture.Height / 2);
            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, this.actor.transform.Position, null, Color.White, this.actor.transform.Angle, this.offset, Vector2.One, SpriteEffects.None, this.actor.transform.Depth.AsFloat);
        }

        public void SetupBoundingRect()
        {
            RequireComponent<BoundingRect>().SetSize(new Point(this.texture.Width, this.texture.Height));
        }
    }
}
