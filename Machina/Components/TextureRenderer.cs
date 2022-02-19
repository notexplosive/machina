using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class TextureRenderer : BaseComponent
    {
        public readonly Texture2D texture;
        private Vector2 offset;

        public float Opacity { get; set; } = 1f;

        public TextureRenderer(Actor actor, Texture2D texture) : base(actor)
        {
            this.texture = texture;
            this.offset = Vector2.Zero;
        }

        public TextureRenderer CenterOffset()
        {
            this.offset = new Vector2(this.texture.Width / 2, this.texture.Height / 2);
            return this;
        }

        public TextureRenderer SetOffset(Vector2 offset)
        {
            this.offset = offset;
            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.actor.transform.Position, null, Color.White.WithMultipliedOpacity(Opacity), this.actor.transform.Angle,
                this.offset, Vector2.One, SpriteEffects.None, this.actor.transform.Depth.AsFloat);
        }

        public void SetupBoundingRect()
        {
            RequireComponent<BoundingRect>().SetSize(new Point(this.texture.Width, this.texture.Height));
        }
    }
}