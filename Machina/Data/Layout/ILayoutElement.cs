using Microsoft.Xna.Framework;

namespace Machina.Data.Layout
{
    public interface ILayoutElement
    {
        Point Size { get; }
        Point Position { get; set; }
        Point Offset { get; }

        bool IsStretchedAlong(Orientation orientation);
        bool IsStretchPerpendicular(Orientation orientation);
        ILayoutElement SetHeight(int height);
        ILayoutElement SetWidth(int width);
        public ILayoutElement StretchHorizontally();
        public ILayoutElement StretchVertically();
    }
}
