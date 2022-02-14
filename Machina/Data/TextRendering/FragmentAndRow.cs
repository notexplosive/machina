using Machina.Data.Layout;
using System.Collections.Generic;

namespace Machina.Data.TextRendering
{
    public readonly struct FragmentAndRow
    {
        public BakedFlowLayout.BakedRow BakedRow { get; }
        public List<TextOutputFragment> Fragment { get; }

        public FragmentAndRow(BakedFlowLayout.BakedRow bakedRow, List<TextOutputFragment> fragment)
        {
            BakedRow = bakedRow;
            Fragment = fragment;
        }
    }
}
