﻿using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(IDrawableTextElement drawable, int characterPosition)
        {
            CharacterPosition = characterPosition;
            Drawable = drawable;

            if (drawable.TokenText == "\n")
            {
                WillBeRendered = false;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] { FlowLayoutInstruction.Linebreak, LayoutNode.NamelessLeaf(LayoutSize.Pixels(Point.Zero)) };
            }
            else
            {
                WillBeRendered = true;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] { LayoutNode.NamelessLeaf(LayoutSize.Pixels(drawable.Size)) };
            }
        }

        public readonly FlowLayout.LayoutNodeOrInstruction[] Nodes;
        public bool WillBeRendered { get; }
        public int CharacterPosition { get; }
        public IDrawableTextElement Drawable { get; }

        public int CharacterLength => Drawable.CharacterLength;

        public override string ToString()
        {
            return $"drawable: {Drawable} pos: {CharacterPosition} length: {CharacterLength} {WillBeRendered}";
        }
    }
}
