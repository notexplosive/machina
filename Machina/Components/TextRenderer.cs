using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    internal class TextRenderer : BaseComponent
    {
        public TextRenderer(Actor actor, SpriteFont font, string startingText = "") : base(actor)
        {
            Text = startingText;
            Font = font;
            FontColor = Color.White;
        }

        public Color FontColor { get; set; }

        private string Text { get; }

        public SpriteFont Font { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, this.actor.transform.Position, FontColor, this.actor.transform.Angle,
                new Vector2(), 1f, SpriteEffects.None, this.actor.transform.Depth.AsFloat);
        }
    }
}
