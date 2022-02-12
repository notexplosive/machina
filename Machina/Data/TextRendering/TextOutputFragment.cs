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

                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.Spacer(drawable.Size),
                    FlowLayoutInstruction.Linebreak
                };
            }
            else
            {
                WillBeRendered = true;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.NamelessLeaf(LayoutSize.Pixels(drawable.Size))
                };
            }
        }

        public readonly FlowLayout.LayoutNodeOrInstruction[] Nodes;
        public bool WillBeRendered { get; }
        public int CharacterPosition { get; }
        public IDrawableTextElement Drawable { get; }

        public int CharacterLength => Drawable.CharacterLength;
    }
}
