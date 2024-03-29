namespace Machina.Engine
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class WindowInterface
    {
        private readonly Point startingWindowSize;
        private readonly GraphicsDeviceManager graphics;
        private readonly GraphicsDevice device;

        public event Action<Point> Resized;
        public GameWindow GameWindow { get; }

        public WindowInterface(Point startingWindowSize, GameWindow window, GraphicsDeviceManager graphics,
            GraphicsDevice device)
        {
            this.startingWindowSize = startingWindowSize;
            this.graphics = graphics;
            this.device = device;
            GameWindow = window;

            window.AllowUserResizing = true;
            window.ClientSizeChanged += OnResize;
        }

        public Point CurrentWindowSize => new Point(this.GameWindow.ClientBounds.Width, this.GameWindow.ClientBounds.Height);

        public bool IsFullScreen => this.graphics.IsFullScreen;

        public void SetWindowSize(Point windowSize)
        {
            MachinaClient.Print("Window size changed to", windowSize);
            if (!GamePlatform.IsAndroid)
            {
                this.graphics.PreferredBackBufferWidth = windowSize.X;
                this.graphics.PreferredBackBufferHeight = windowSize.Y;
                this.graphics.ApplyChanges();
                Resized?.Invoke(windowSize);
            }
        }

        public void ApplyChanges()
        {
            this.graphics.ApplyChanges();

        }

        public void SetFullscreen(bool state)
        {
            if (state)
            {
                graphics.PreferredBackBufferWidth = device.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = device.DisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = this.startingWindowSize.X;
                graphics.PreferredBackBufferHeight = this.startingWindowSize.Y;
                graphics.IsFullScreen = false;
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(this.GameWindow.ClientBounds.Width, this.GameWindow.ClientBounds.Height);
            Resized?.Invoke(windowSize);
        }
    }
}