namespace Machina.Engine
{
    using Microsoft.Xna.Framework;

    public interface IWindow
    {
        public Point CurrentSize { get; }
        public void SetSize(Point windowSize);
    }
}