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
        public RenderableText(IDrawableTextElement element, string text, Point pivotPosition, Point offset)
        {
            Element = element;
            Text = text;
            Offset = offset;
            Origin = pivotPosition;
        }

        public IDrawableTextElement Element { get; }
        public string Text { get; }
        public Point Origin { get; }
        public Point Offset { get; }

        public void Draw(SpriteBatch spriteBatch, Point drawOffset, float angle, Depth depth)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return;
            }

            Element.Draw(spriteBatch, this, angle, drawOffset, depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor, Point drawOffset, float angle, Depth depth)
        {
            Element.DrawDropShadow(spriteBatch, this, angle, drawOffset, depth, dropShadowColor);
        }

        public override string ToString()
        {
            return $"`{Text}` at {Origin} offset by {Offset}";
        }
    }

}
