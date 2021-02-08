using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Machina
{
    public class MachinaGame : Game
    {
        protected readonly GraphicsDeviceManager graphics;
        protected ResizeStatus resizing;
        private Point startingWindowSize;

        public MachinaGame(int windowWidth, int windowHeight)
        {
            graphics = new GraphicsDeviceManager(this);
            resizing = new ResizeStatus(windowWidth, windowHeight);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            this.startingWindowSize = new Point(windowWidth, windowHeight);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = startingWindowSize.X;
            graphics.PreferredBackBufferHeight = startingWindowSize.Y;
            graphics.ApplyChanges();
            OnResize(null, new EventArgs());

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (resizing.Pending)
            {
                graphics.PreferredBackBufferWidth = resizing.Width;
                graphics.PreferredBackBufferHeight = resizing.Height;
                graphics.ApplyChanges();
                resizing.Pending = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            resizing.Pending = true;
            resizing.Width = Window.ClientBounds.Width;
            resizing.Height = Window.ClientBounds.Height;
        }
    }
}
