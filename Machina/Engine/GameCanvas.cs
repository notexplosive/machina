using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class GameCanvas
    {
        private readonly Point idealSize;
        private RenderTarget2D screenRenderTarget;

        public GameCanvas(int idealWidth, int idealHeight)
        {
            this.idealSize = new Point(idealWidth, idealHeight);
        }

        public float ScaleFactor
        {
            get
            {
                var normalizedWidth = (float) WindowWidth / this.idealSize.X;
                var normalizedHeight = (float) WindowHeight / this.idealSize.Y;

                return Math.Min(normalizedWidth, normalizedHeight);
            }
        }

        public bool PendingResize
        {
            get;
            private set;
        }
        public int WindowWidth
        {
            get;
            private set;
        }
        public int WindowHeight
        {
            get;
            private set;
        }

        public Point CanvasSize => (new Vector2(this.idealSize.X, this.idealSize.Y) * ScaleFactor).ToPoint();

        public void Resize(int windowWidth, int windowHeight)
        {
            this.PendingResize = true;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        public void FinishResize()
        {
            this.PendingResize = false;
        }

        public void BuildCanvas(GraphicsDevice graphicsDevice)
        {
            this.screenRenderTarget = new RenderTarget2D(
                graphicsDevice,
                WindowWidth,
                WindowHeight,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public void PrepareDraw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetRenderTarget(screenRenderTarget);
            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        }

        public void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            graphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();
            var canvasSize = CanvasSize;
            spriteBatch.Draw(screenRenderTarget,
                new Rectangle((WindowWidth - canvasSize.X) / 2, (WindowHeight - canvasSize.Y) / 2, canvasSize.X, canvasSize.Y),
                null, Color.White);
            spriteBatch.End();
        }
    }
}
