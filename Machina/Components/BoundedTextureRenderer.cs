using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class BoundedTextureRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Texture2D texture;

        public Depth DepthOffset { get; set; }
        public Point ViewportOffset { get; set; }

        public BoundedTextureRenderer(Actor actor, Texture2D texture) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.texture = texture;
        }

        public override void Update(float dt)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var viewportRect = this.boundingRect.RectNormalized;
            viewportRect.Location += ViewportOffset;
            spriteBatch.Draw(this.texture, this.boundingRect.TopLeft, viewportRect, Color.White, transform.Angle, Vector2.Zero, 1f, SpriteEffects.None, transform.Depth + DepthOffset);
        }
    }
}
