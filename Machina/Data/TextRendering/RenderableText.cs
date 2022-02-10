using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct RenderableText
    {
        public RenderableText(IFontMetrics fontMetrics, string text, int characterPosition, Point pivotPosition, Color textColor, Point drawOffset, Rectangle layoutNodeOfLine)
        {
            var offsetFromPivot = (layoutNodeOfLine.Location + drawOffset).ToVector2();
            offsetFromPivot.Floor();
            offsetFromPivot = -offsetFromPivot;

            CharacterPosition = characterPosition;
            FontMetrics = fontMetrics;
            Content = text;
            Color = textColor;
            OffsetFromPivot = offsetFromPivot;
            PivotPosition = pivotPosition;
        }

        public string Content { get; }
        public Point PivotPosition { get; }
        public Vector2 OffsetFromPivot { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public int CharacterPosition { get; }
        public object CharacterLength => Content.Length;

        private SpriteFont GetFont()
        {
            if (FontMetrics is SpriteFontMetrics spriteFontMetrics)
            {
                return spriteFontMetrics.Font;
            }

            throw new Exception("FontMetrics does not provide an actual font");
        }

        public void Draw(SpriteBatch spriteBatch, float angle, Depth depth)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                return;
            }

            spriteBatch.DrawString(GetFont(), Content, PivotPosition.ToVector2(), Color, angle, OffsetFromPivot, 1f, SpriteEffects.None, depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor, float angle, Depth depth)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(GetFont(), Content, PivotPosition.ToVector2(), finalDropShadowColor, angle, OffsetFromPivot - new Vector2(1, 1), 1f, SpriteEffects.None, depth + 1);
        }

        public override string ToString()
        {
            return $"`{Content}` at {PivotPosition} offset by {OffsetFromPivot}";
        }
    }

}
