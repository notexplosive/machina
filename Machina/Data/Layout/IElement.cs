using Microsoft.Xna.Framework;

namespace Machina.Data.Layout
{
    public interface IElement
    {
        Point Size { get; }
        Point Position { get; set; }
        Point Offset { get; }

        bool IsStretchedAlong(Orientation orientation);
        bool IsStretchPerpendicular(Orientation orientation);
        IElement SetHeight(int height);
        IElement SetWidth(int width);
        public IElement StretchHorizontally();
        public IElement StretchVertically();
    }
}
