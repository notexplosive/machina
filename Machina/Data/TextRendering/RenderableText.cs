using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct RenderableText
    {
        public RenderableText(IFontMetrics fontMetrics, TextLine line, Vector2 pivotPosition, Color textColor, Point drawOffset, float angle, Depth depth, int xAdjustment, int yAdjustment)
        {
            var offsetFromPivot = new Vector2(line.positionRelativeToTopLeftOfRect.X + xAdjustment, line.positionRelativeToTopLeftOfRect.Y + yAdjustment) + drawOffset.ToVector2() -
                      pivotPosition;
            offsetFromPivot.Floor();
            offsetFromPivot = -offsetFromPivot;

            FontMetrics = fontMetrics;
            Content = line.textContent;
            Color = textColor;
            OffsetFromPivot = offsetFromPivot;
            PivotPosition = pivotPosition;
            Angle = angle;
            Depth = depth;
        }

        public string Content { get; }
        public Vector2 PivotPosition { get; }
        public float Angle { get; }
        public Depth Depth { get; }
        public Vector2 OffsetFromPivot { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }

        private SpriteFont GetFont()
        {
            if (FontMetrics is SpriteFontMetrics spriteFontMetrics)
            {
                return spriteFontMetrics.Font;
            }

            throw new Exception("FontMetrics does not provide an actual font");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(GetFont(), Content, PivotPosition, Color, Angle, OffsetFromPivot, 1f, SpriteEffects.None, Depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(GetFont(), Content, PivotPosition, finalDropShadowColor, Angle, OffsetFromPivot - new Vector2(1, 1), 1f, SpriteEffects.None, Depth + 1);
        }
    }

}
