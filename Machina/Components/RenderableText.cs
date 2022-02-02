using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Components
{
    public readonly struct RenderableText
    {
        public RenderableText(SpriteFont spriteFont, string textContent, Vector2 pivotPosition, Color textColor, Vector2 offsetFromPivot)
        {
            Font = spriteFont;
            Content = textContent;
            Color = textColor;
            OffsetFromPivot = offsetFromPivot;
            PivotPosition = pivotPosition;
        }

        public string Content { get; }
        public Vector2 PivotPosition { get; }
        public Vector2 OffsetFromPivot { get; }
        public SpriteFont Font { get; }
        public Color Color { get; }

        public void Draw(SpriteBatch spriteBatch, float angle, Depth depth)
        {
            spriteBatch.DrawString(Font, Content, PivotPosition, Color, angle,
                    OffsetFromPivot, 1f, SpriteEffects.None, depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor, float angle, Depth depth)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(Font, Content, PivotPosition, finalDropShadowColor, angle, OffsetFromPivot - new Vector2(1, 1), 1f, SpriteEffects.None, depth + 1);
        }
    }
}