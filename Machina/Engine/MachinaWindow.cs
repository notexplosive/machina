namespace Machina.Engine
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class MachinaWindow
    {
        private readonly Point startingWindowSize;
        private readonly GameWindow window;
        private readonly GraphicsDeviceManager graphics;
        private readonly GraphicsDevice device;

        public event Action<Point> Resized;

        public MachinaWindow(Point startingWindowSize, GameWindow window, GraphicsDeviceManager graphics,
            GraphicsDevice device)
        {
            this.startingWindowSize = startingWindowSize;
            this.window = window;
            this.graphics = graphics;
            this.device = device;

            window.AllowUserResizing = true;
            window.ClientSizeChanged += OnResize;
        }

        public Point CurrentWindowSize => new Point(this.window.ClientBounds.Width, this.window.ClientBounds.Height);

        public bool Fullscreen
        {
            set
            {
                if (value)
                {
                    SetWindowSize(new Point(this.device.DisplayMode.Width, this.device.DisplayMode.Height));
                    this.graphics.IsFullScreen = true;
                }
                else
                {
                    SetWindowSize(this.startingWindowSize);
                    this.graphics.IsFullScreen = false;
                }

                this.graphics.ApplyChanges();
            }

            get => this.graphics.IsFullScreen;
        }

        public void SetWindowSize(Point windowSize)
        {
            MachinaGame.Print("Window size changed to", windowSize);
            this.graphics.PreferredBackBufferWidth = windowSize.X;
            this.graphics.PreferredBackBufferHeight = windowSize.Y;
            this.graphics.ApplyChanges();
            Resized?.Invoke(windowSize);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(this.window.ClientBounds.Width, this.window.ClientBounds.Height);
            Resized?.Invoke(windowSize);
        }
    }
}