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

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(this, TokenText, totalAvailableRectLocation, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, int substringLength)
        {
            return new RenderableText(this, TokenText.Substring(0, substringLength), totalAvailableRectLocation, nodeLocation);
        }

        public void Draw(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            spriteBatch.DrawString(GetFont(), text, origin.ToVector2(), Color, angle, additionalOffset.ToVector2() - offset.ToVector2(), 1f, SpriteEffects.None, depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth, Color dropShadowColor)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(GetFont(), text, origin.ToVector2(), finalDropShadowColor, angle, additionalOffset.ToVector2() - offset.ToVector2() - new Vector2(1, 1), 1f, SpriteEffects.None, depth + 1);
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
