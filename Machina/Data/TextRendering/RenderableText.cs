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
        public RenderableText(IDrawableTextElement drawable, string text, Point pivotPosition, Point offset)
        {
            Drawable = drawable;
            Text = text;
            Offset = offset;
            Origin = pivotPosition;
        }

        public IDrawableTextElement Drawable { get; }
        public string Text { get; }
        public Point Origin { get; }
        public Point Offset { get; }

        public void Draw(SpriteBatch spriteBatch, Point additionalOffset, float angle, Depth depth)
        {
            var args = new TextDrawingArgs
            {
                Origin = Origin,
                Position = Offset,
                Angle = angle,
                AdditionalOffset = additionalOffset,
                Depth = depth
            };

            Drawable.Draw(spriteBatch, Text, args);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor, Point additionalOffset, float angle, Depth depth)
        {
            var args = new TextDrawingArgs
            {
                Origin = Origin,
                Position = Offset + new Point(1),
                Angle = angle,
                AdditionalOffset = additionalOffset,
                Depth = depth + 1
            };

            Drawable.DrawDropShadow(spriteBatch, Text, args, dropShadowColor);
        }

        public override string ToString()
        {
            return $"`{Text}` at {Origin} offset by {Offset}";
        }
    }

}
