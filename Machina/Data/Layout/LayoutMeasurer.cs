﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    internal class LayoutMeasurer
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        public int GetMeasuredEdge(ILayoutEdge edge)
        {
            if (edge.IsConstant)
            {
                return edge.ActualSize;
            }

            return this.sizeLookupTable[edge];
        }

        public bool CanMeasureEdge(ILayoutEdge edge)
        {
            return this.sizeLookupTable.ContainsKey(edge);
        }
        public Point GetMeasuredSize(LayoutSize size)
        {
            return new Point(GetMeasuredEdge(size.Width), GetMeasuredEdge(size.Height));
        }

        public void Add(ILayoutEdge key, int value)
        {
            this.sizeLookupTable[key] = value;
        }
    }
}
