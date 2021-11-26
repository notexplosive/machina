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
            MachinaClient.Print("Window size changed to", windowSize);
            if (!GamePlatform.IsAndroid)
            {
                this.graphics.PreferredBackBufferWidth = windowSize.X;
                this.graphics.PreferredBackBufferHeight = windowSize.Y;
                this.graphics.ApplyChanges();
                Resized?.Invoke(windowSize);
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(this.GameWindow.ClientBounds.Width, this.GameWindow.ClientBounds.Height);
            Resized?.Invoke(windowSize);
        }
    }
}