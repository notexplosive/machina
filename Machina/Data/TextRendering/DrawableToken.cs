using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public readonly struct DrawableToken : IDrawableTextElement
    {
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public string TokenText { get; }
        public int CharacterLength => TokenText.Length;

        public DrawableToken(string tokenText, IFontMetrics fontMetrics, Color color)
        {
            FontMetrics = fontMetrics;
            Color = color;
            TokenText = tokenText;

            var tokenSize = FontMetrics.MeasureStringRounded(TokenText);

            if (TokenText == "\n")
            {
                Size = new Point(0, tokenSize.Y);
            }
            else
            {
                Size = tokenSize;
            }
        }

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation, int? substringLength = null)
        {
            if (!substringLength.HasValue)
            {
                substringLength = TokenText.Length;
            }

            return new RenderableText(this, TokenText.Substring(0, substringLength.Value), totalAvailableRectLocation, nodeLocation);
        }

        public void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            spriteBatch.DrawString(GetFont(), text, args.ResultOrigin(), Color, args.Angle, args.ResultOffset(), 1f, SpriteEffects.None, args.Depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(GetFont(), text, args.ResultOrigin(), finalDropShadowColor, args.Angle, args.ResultOffset(), 1f, SpriteEffects.None, args.Depth);
        }

        private SpriteFont GetFont()
        {
            if (FontMetrics is SpriteFontMetrics spriteFontMetrics)
            {
                return spriteFontMetrics.Font;
            }

            throw new Exception("FontMetrics does not provide an actual font");
        }

        public Point SizeOfCharacter(int index)
        {
            return FontMetrics.MeasureStringRounded(TokenText[index].ToString());
        }

        public char GetCharacterAt(int characterIndex)
        {
            return TokenText[characterIndex];
        }
    }
}
