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
        private Strategy strategy;

        public GameCanvas(int idealWidth, int idealHeight, ResizeBehavior resizeBehavior)
        {
            this.idealSize = new Point(idealWidth, idealHeight);
            if (resizeBehavior == ResizeBehavior.FillContent)
            {
                strategy = new FillStrategy();
            }
            else
            {
                strategy = new MaintainDesiredResolutionStrategy();
            }
            OnResize(idealWidth, idealHeight);
        }

        public Rectangle CanvasRect
        {
            get
            {
                var canvasRect = CanvasSize;
                return new Rectangle((WindowSize.X - canvasRect.X) / 2, (WindowSize.Y - canvasRect.Y) / 2, canvasRect.X, canvasRect.Y);
            }
        }

        public float ScaleFactor
        {
            get
            {
                return this.strategy.GetScaleFactor(WindowSize, idealSize);
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
                return strategy.GetCanvasSize(WindowSize, idealSize);
            }
        }

        public Point WorldSize => this.idealSize;

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
            this.screenRenderTarget = strategy.BuildCanvas(graphicsDevice, WindowSize);
        }

        public void PrepareCanvas(GraphicsDevice graphicsDevice)
        {
            strategy.PrepareCanvas(graphicsDevice, this.screenRenderTarget);
        }

        public void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            strategy.DrawCanvas(graphicsDevice, spriteBatch, screenRenderTarget, CanvasRect);
        }

        private interface Strategy
        {
            void PrepareCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget);
            void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect);
            RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point windowSize);
            Point GetCanvasSize(Point windowSize, Point idealSize);
            float GetScaleFactor(Point windowSize, Point idealSize);
        }

        private class MaintainDesiredResolutionStrategy : Strategy
        {
            public float GetScaleFactor(Point windowSize, Point idealSize)
            {
                var normalizedWidth = (float) windowSize.X / idealSize.X;
                var normalizedHeight = (float) windowSize.Y / idealSize.Y;
                return Math.Min(normalizedWidth, normalizedHeight);
            }

            public RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point windowSize)
            {
                return new RenderTarget2D(
                    graphicsDevice,
                    windowSize.X,
                    windowSize.Y,
                    false,
                    graphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
            }

            public void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect)
            {
                graphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin();
                spriteBatch.Draw(screenRenderTarget,
                    canvasRect,
                    null, Color.White);

                spriteBatch.End();
            }

            public Point GetCanvasSize(Point windowSize, Point idealSize)
            {
                return (new Vector2(idealSize.X, idealSize.Y) * GetScaleFactor(windowSize, idealSize)).ToPoint();
            }

            public void PrepareCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget)
            {
                graphicsDevice.SetRenderTarget(screenRenderTarget);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        private class FillStrategy : Strategy
        {
            public Point GetCanvasSize(Point windowSize, Point idealSize)
            {
                return windowSize;
            }

            public void DrawCanvas()
            {
                // no-op
            }

            public RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point windowSize)
            {
                return null;
            }

            public float GetScaleFactor(Point windowSize, Point idealSize)
            {
                return 1f;
            }

            public void PrepareCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget)
            {
            }

            public void DrawCanvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect)
            {
            }
        }
    }
}
