namespace Machina.Engine
{
    using Microsoft.Xna.Framework;
    using System;

    public interface IWindow
    {
        public event Action<Point> Resized;
        public Point CurrentSize { get; }
        bool IsFullScreen { get; }

        public void SetSize(Point windowSize);
        void AddOnTextInputEvent(EventHandler<TextInputEventArgs> callback);
        void ApplyChanges();
        void SetFullscreen(bool state);
    }
}