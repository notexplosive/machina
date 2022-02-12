using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public struct FormattedTextToken
    {
        public FormattedTextToken(string tokenText, FormattedTextFragment parentFragment)
        {
            ParentFragment = parentFragment;
            TokenText = tokenText;
        }

        public FormattedTextFragment ParentFragment { get; }
        public string TokenText { get; }
        public Point Size => ParentFragment.FontMetrics.MeasureStringRounded(TokenText);
    }
}
