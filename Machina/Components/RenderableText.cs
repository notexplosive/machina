﻿using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Components
{
    internal class RenderableText
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
    }
}