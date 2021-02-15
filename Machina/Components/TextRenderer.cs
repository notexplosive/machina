using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class TextRenderer : BaseComponent
    {
        public Color FontColor
        {
            get; set;
        }
        string Text
        {
            get; set;
        }
        private SpriteFont Font
        {
            get; set;
        }

        public TextRenderer(Actor actor, SpriteFont font, string startingText = "") : base(actor)
        {
            Text = startingText;
            Font = font;
            FontColor = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, this.actor.Position, FontColor, this.actor.Angle, new Vector2(), 1f, SpriteEffects.None, this.actor.Depth);
        }
    }
}
