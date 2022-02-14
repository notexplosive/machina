using Machina.Data.Layout;
using System.Collections.Generic;

namespace Machina.Data.TextRendering
{
    public readonly struct FragmentAndRow
    {
        public BakedFlowLayout WholeLayout { get; }
        public BakedFlowLayout.BakedRow BakedRow { get; }
        public List<TextOutputFragment> Fragment { get; }

        public FragmentAndRow(BakedFlowLayout wholeLayout, BakedFlowLayout.BakedRow bakedRow, List<TextOutputFragment> fragment)
        {
            WholeLayout = wholeLayout;
            BakedRow = bakedRow;
            Fragment = fragment;
        }
    }
}
