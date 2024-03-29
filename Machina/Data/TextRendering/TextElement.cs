﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public readonly struct TextElement : IDrawableTextElement
    {
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public string TokenText { get; }
        public int CharacterLength => TokenText.Length;

        public TextElement(string tokenText, IFontMetrics fontMetrics, Color color)
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

        public RenderableText CreateRenderableText(Point origin, Point topLeft, Point offset, int? substringLength = null)
        {
            if (!substringLength.HasValue)
            {
                substringLength = TokenText.Length;
            }

            return new RenderableText(this, TokenText.Substring(0, substringLength.Value), origin, topLeft, offset);
        }

        public void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            var textOffset = args.GetTextOffset();
            spriteBatch.DrawString(GetFont(), text, args.ResultOrigin(), Color, args.Angle, textOffset, 1f, SpriteEffects.None, args.Depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor)
        {
            var textOffset = args.GetTextOffset();
            var finalDropShadowColor = dropShadowColor.WithMultipliedOpacity(dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(GetFont(), text, args.ResultOrigin(), finalDropShadowColor, args.Angle, textOffset, 1f, SpriteEffects.None, args.Depth);
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

        public IDrawableTextElement ShrinkBy(int amountThatMustBeRemoved)
        {
            var chopIndex = 0;
            for (int i = 0; i < TokenText.Length; i++)
            {
                var chunkSize = FontMetrics.MeasureStringRounded(TokenText[i..]).X;

                if (chunkSize > amountThatMustBeRemoved)
                {
                    chopIndex = i;
                }
            }

            return new TextElement(TokenText[0..chopIndex], FontMetrics, Color);
        }

        public IDrawableTextElement AppendEllipse()
        {
            return new TextElement(TokenText + EllipseString, FontMetrics, Color);
        }

        public int EllipseWidth()
        {
            return FontMetrics.MeasureStringRounded(EllipseString).X;
        }

        public static readonly string EllipseString = "...";

        public override string ToString()
        {
            return $"{TokenText}";
        }
    }
}
