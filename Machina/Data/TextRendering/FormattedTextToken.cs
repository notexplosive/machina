using Machina.Data.Layout;
using System;

namespace Machina.Data.TextRendering
{
    public readonly struct FormattedTextToken
    {
        public FormattedTextToken(IDrawableTextElement drawable)
        {
            Drawable = drawable;
        }

        public IDrawableTextElement Drawable { get; }

        public TextOutputFragment CreateOutputFragment(int characterIndex)
        {
            return new TextOutputFragment(Drawable, characterIndex);
        }
    }
}
