﻿using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation);
        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, string newText);
    }
}
