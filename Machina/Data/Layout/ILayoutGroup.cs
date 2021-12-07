using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public interface ILayoutGroup<TInnerData> : ILayoutElement
    {
        public ILayoutGroup<TInnerData> SetPaddingBetweenElements(int padding);
        public ILayoutGroup<TInnerData> SetMarginSize(Point marginSize);
        public ILayoutGroup<TInnerData> AddHorizontallyStretchedSpacer();
        public ILayoutGroup<TInnerData> AddVerticallyStretchedSpacer();
        public ILayoutElement AddElement(string name, Point size, Action<TInnerData> callback); // prefer AddSpecificElement
        public ILayoutGroup<TInnerData> AddVerticallyStretchedElement(string name, int width, Action<TInnerData> callback);
        public ILayoutGroup<TInnerData> AddHorizontallyStretchedElement(string name, int height, Action<TInnerData> callback);
        public ILayoutGroup<TInnerData> AddBothStretchedElement(string name, Action<TInnerData> callback);
        public ILayoutGroup<TInnerData> AddSpecificSizeElement(string name, Point point, Action<TInnerData> callback);
        public Orientation Orientation { get; }
        public int Padding { get; }
        public Point MarginSize { get; }
        public List<ILayoutElement> GetAllElements();
    }
}
