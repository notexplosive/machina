using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public interface IGroup<TInnerData> : IElement
    {
        public IGroup<TInnerData> SetPaddingBetweenElements(int padding);
        public IGroup<TInnerData> SetMarginSize(Point marginSize);
        public IGroup<TInnerData> HorizontallyStretchedSpacer();
        public IGroup<TInnerData> VerticallyStretchedSpacer();
        public IElement AddElement(string name, Point size, Action<TInnerData> callback); // prefer AddSpecificElement
        public IGroup<TInnerData> AddVerticallyStretchedElement(string name, int width, Action<TInnerData> callback);
        public IGroup<TInnerData> AddHorizontallyStretchedElement(string name, int height, Action<TInnerData> callback);
        public IGroup<TInnerData> AddBothStretchedElement(string name, Action<TInnerData> callback);
        public IGroup<TInnerData> AddSpecificSizeElement(string name, Point point, Action<TInnerData> callback);
        public Orientation Orientation { get; }
        public int Padding { get; }
        public Point MarginSize { get; }
        public List<IElement> GetAllElements();
    }
}
