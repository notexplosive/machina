﻿using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.Layout
{
    public struct FlowLayoutStyle
    {
        public static FlowLayoutStyle Empty => new FlowLayoutStyle();

        public int PaddingBetweenRows { get; }
        public int PaddingBetweenItemsInEachRow { get; }
        public Alignment Alignment { get; }
        public OverflowRule OverflowRule { get; }
        public Point Margin { get; }

        public FlowLayoutStyle(
            Point margin = default,
            int paddingBetweenRows = default,
            int paddingBetweenItemsInEachRow = default,
            Alignment alignment = default,
            OverflowRule overflowRule = default)
        {
            if (overflowRule == default)
            {
                overflowRule = OverflowRule.PermitExtraRows;
            }

            Margin = margin;
            PaddingBetweenItemsInEachRow = paddingBetweenItemsInEachRow;
            PaddingBetweenRows = paddingBetweenRows;
            Alignment = alignment;
            OverflowRule = overflowRule;
        }
    }
}