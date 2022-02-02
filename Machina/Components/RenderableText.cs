using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Components
{
    public readonly struct RenderableText
    {
        public RenderableText(SpriteFont spriteFont, string textContent, Vector2 pivotPosition, Color textColor, Vector2 offsetFromPivot, float angle, Depth depth)
        {
            Font = spriteFont;
            Content = textContent;
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
        public SpriteFont Font { get; }
        public Color Color { get; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Content, PivotPosition, Color, Angle, OffsetFromPivot, 1f, SpriteEffects.None, Depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor)
        {
            var finalDropShadowColor = new Color(dropShadowColor, dropShadowColor.A / 255f * (Color.A / 255f));
            spriteBatch.DrawString(Font, Content, PivotPosition, finalDropShadowColor, Angle, OffsetFromPivot - new Vector2(1, 1), 1f, SpriteEffects.None, Depth + 1);
        }
    }
}