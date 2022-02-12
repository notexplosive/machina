﻿using Machina.Data.Layout;
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

        public void Draw(SpriteBatch spriteBatch, Point drawOffset, float angle, Depth depth)
        {
            Drawable.Draw(spriteBatch, this, angle, drawOffset, depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, Color dropShadowColor, Point drawOffset, float angle, Depth depth)
        {
            Drawable.DrawDropShadow(spriteBatch, this, angle, drawOffset, depth, dropShadowColor);
        }

        public override string ToString()
        {
            return $"`{Text}` at {Origin} offset by {Offset}";
        }
    }

}
