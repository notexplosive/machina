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
        
        public MachinaWindow(Point startingWindowSize, GameWindow window, GraphicsDeviceManager graphics, GraphicsDevice device)
        {
            this.startingWindowSize = startingWindowSize;
            this.window = window;
            this.graphics = graphics;
            this.device = device;
            
            window.AllowUserResizing = true;
            window.ClientSizeChanged += OnResize;
        }

        public Point CurrentWindowSize => new Point(window.ClientBounds.Width, window.ClientBounds.Height);
        
        public bool Fullscreen
        {
            set
            {
                if (value)
                {
                    SetWindowSize(new Point(device.DisplayMode.Width,
                        device.DisplayMode.Height));
                    graphics.IsFullScreen = true;
                }
                else
                {
                    SetWindowSize(this.startingWindowSize);
                    graphics.IsFullScreen = false;
                }

                graphics.ApplyChanges();
            }

            get => graphics.IsFullScreen;
        }

        public void SetWindowSize(Point windowSize)
        {
            MachinaGame.Print("Window size changed to", windowSize);
            graphics.PreferredBackBufferWidth = windowSize.X;
            graphics.PreferredBackBufferHeight = windowSize.Y;
            graphics.ApplyChanges();
            Resized?.Invoke(windowSize);
        }
        
        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(window.ClientBounds.Width, window.ClientBounds.Height);
            Resized?.Invoke(windowSize);
        }
    }
}