using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public struct FormattedTextToken
    {
        public FormattedTextToken(string tokenText, FormattedTextFragment parentFragment)
        {
            ShouldBeCounted = false;
            ParentFragment = parentFragment;
            Text = tokenText;

            if (tokenText == "\n")
            {
            }
            else
            {
                // move this to TextOuputFragment?
                ShouldBeCounted = true;
            }
        }

        public bool ShouldBeCounted { get; }
        public FormattedTextFragment ParentFragment { get; }
        public string Text { get; }
        public Point Size => ParentFragment.FontMetrics.MeasureStringRounded(Text);

        public TextOutputFragment OutputFragment(int characterPosition)
        {
            return new TextOutputFragment(Text, ParentFragment.FontMetrics, ParentFragment.Color, characterPosition);
        }
    }
}
