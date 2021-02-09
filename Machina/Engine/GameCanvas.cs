using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public enum ResizeBehavior
    {
        MaintainDesiredResolution,
        FillContent
    }

    public class GameCanvas
    {
        private readonly Point idealSize;
        private RenderTarget2D screenRenderTarget;
        private ResizeBehavior resizeBehavior;

        public GameCanvas(int idealWidth, int idealHeight, ResizeBehavior resizeBehavior)
        {
            this.idealSize = new Point(idealWidth, idealHeight);
            this.resizeBehavior = resizeBehavior;
        }

        public float ScaleFactor
        {
            get
            {
                var normalizedWidth = (float) WindowSize.X / this.idealSize.X;
                var normalizedHeight = (float) WindowSize.Y / this.idealSize.Y;
                return Math.Min(normalizedWidth, normalizedHeight);
            }
        }

        public bool PendingResize
        {
            get;
            private set;
        }
        public Point WindowSize
        {
            get;
            private set;
        }

        public Point CanvasSize
        {
            get
            {
                if (this.resizeBehavior == ResizeBehavior.MaintainDesiredResolution)
                {
                    return (new Vector2(this.idealSize.X, this.idealSize.Y) * ScaleFactor).ToPoint();
                }
                else
                {
                    return WindowSize;
                }
            }
        }

        public void OnResize(int windowWidth, int windowHeight)
        {
            this.PendingResize = true;
            this.WindowSize = new Point(windowWidth, windowHeight);
        }

        public void FinishResize()
        {
            this.PendingResize = false;
        }

        public void BuildCanvas(GraphicsDevice graphicsDevice)
        {
            if (this.resizeBehavior == ResizeBehavior.MaintainDesiredResolution)
            {
                this.screenRenderTarget = new RenderTarget2D(
                    graphicsDevice,
                    WindowSize.X,
                    WindowSize.Y,
                    false,
                    graphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
            }
        }

        public void PrepareCanvas(GraphicsDevice graphicsDevice)
        {
            if (this.resizeBehavior == ResizeBehavior.MaintainDesiredResolution)
            {
                graphicsDevice.SetRenderTarget(screenRenderTarget);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        public void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (this.resizeBehavior == ResizeBehavior.MaintainDesiredResolution)
            {
                graphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin();

                var canvasSize = CanvasSize;
                spriteBatch.Draw(screenRenderTarget,
                    new Rectangle((WindowSize.X - canvasSize.X) / 2, (WindowSize.Y - canvasSize.Y) / 2, canvasSize.X, canvasSize.Y),
                    null, Color.White);

                spriteBatch.End();
            }
        }
    }
}
