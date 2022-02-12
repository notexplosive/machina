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
                var linebreakSize = new Point(0, drawable.Size.Y);
                Size = linebreakSize;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.Spacer(linebreakSize),
                    FlowLayoutInstruction.Linebreak
                };
            }
            else
            {
                WillBeRendered = true;
                Size = drawable.Size;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.NamelessLeaf(LayoutSize.Pixels(Size))
                };
            }
        }

        public Point Size { get; }
        public string Text => Drawable.TokenText;
        public readonly FlowLayout.LayoutNodeOrInstruction[] Nodes;
        public bool WillBeRendered { get; }
        public int CharacterPosition { get; }
        public IDrawableTextElement Drawable { get; }

        public int CharacterLength => Drawable.TokenText.Length;

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(Drawable.FontMetrics, Drawable.TokenText, totalAvailableRectLocation, Drawable.Color, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, string newText)
        {
            return new RenderableText(Drawable.FontMetrics, newText, totalAvailableRectLocation, Drawable.Color, nodeLocation);
        }
    }
}
