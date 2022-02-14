using Machina.Data.Layout;
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

                Node = FlowLayoutInstruction.Linebreak;
            }
            else
            {
                WillBeRendered = true;
                Node = LayoutNode.NamelessLeaf(LayoutSize.Pixels(drawable.Size));
            }
        }

        public readonly FlowLayout.LayoutNodeOrInstruction Node;
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
