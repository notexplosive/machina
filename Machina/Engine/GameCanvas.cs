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

    public interface IGameCanvas
    {
        public Point ViewportSize
        {
            get;
        }

        public float ScaleFactor
        {
            get;
        }

        public Rectangle CanvasRect
        {
            get;
        }
        public Point WindowSize
        {
            get;
        }
    }

    public class GameCanvas : IGameCanvas
    {
        private RenderTarget2D screenRenderTarget;
        private readonly IResizeStrategy resizeStrategy;

        public GameCanvas(int idealWidth, int idealHeight, ResizeBehavior resizeBehavior)
        {
            ViewportSize = new Point(idealWidth, idealHeight);
            if (resizeBehavior == ResizeBehavior.FillContent)
            {
                resizeStrategy = new FillStrategy();
            }
            else
            {
                resizeStrategy = new MaintainDesiredResolutionStrategy();
            }
            SetWindowSize(idealWidth, idealHeight);
        }

        public Rectangle CanvasRect
        {
            get
            {
                var canvasSize = resizeStrategy.GetCanvasSize(WindowSize, ViewportSize);
                return new Rectangle((WindowSize.X - canvasSize.X) / 2, (WindowSize.Y - canvasSize.Y) / 2, canvasSize.X, canvasSize.Y);
            }
        }

        public float ScaleFactor
        {
            get
            {
                return this.resizeStrategy.GetScaleFactor(WindowSize, ViewportSize);
            }
        }

        public Point WindowSize
        {
            get;
            private set;
        }

        public Point ViewportSize
        {
            get; private set;
        }

        public void SetWindowSize(int windowWidth, int windowHeight)
        {
            WindowSize = new Point(windowWidth, windowHeight);
        }

        public void BuildCanvas(GraphicsDevice graphicsDevice, Point canvasSize)
        {
            this.screenRenderTarget = resizeStrategy.BuildCanvas(graphicsDevice, canvasSize);
        }

        public void SetRenderTargetToCanvas(GraphicsDevice graphicsDevice)
        {
            resizeStrategy.SetRenderTargetToCanvas(graphicsDevice, this.screenRenderTarget);
        }

        public void DrawCanvasToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            resizeStrategy.DrawCanvasToScreen(graphicsDevice, spriteBatch, screenRenderTarget, CanvasRect);
        }

        private interface IResizeStrategy
        {
            void SetRenderTargetToCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget);
            void DrawCanvasToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect);
            RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point viewportSize);
            Point GetCanvasSize(Point windowSize, Point viewportSize);
            float GetScaleFactor(Point windowSize, Point viewportSize);
        }

        private class MaintainDesiredResolutionStrategy : IResizeStrategy
        {
            public float GetScaleFactor(Point windowSize, Point viewportSize)
            {
                var normalizedWidth = (float) windowSize.X / viewportSize.X;
                var normalizedHeight = (float) windowSize.Y / viewportSize.Y;
                return Math.Min(normalizedWidth, normalizedHeight);
            }

            public RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point viewportSize)
            {
                return new RenderTarget2D(
                    graphicsDevice,
                    viewportSize.X,
                    viewportSize.Y,
                    false,
                    graphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
            }

            public void DrawCanvasToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect)
            {
                graphicsDevice.SetRenderTarget(null);

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.DepthRead, null, null);
                spriteBatch.Draw(screenRenderTarget,
                    canvasRect,
                    null, Color.White);

                spriteBatch.End();
            }

            public Point GetCanvasSize(Point windowSize, Point viewportSize)
            {
                return (new Vector2(viewportSize.X, viewportSize.Y) * GetScaleFactor(windowSize, viewportSize)).ToPoint();
            }

            public void SetRenderTargetToCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget)
            {
                graphicsDevice.SetRenderTarget(screenRenderTarget);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }

        private class FillStrategy : IResizeStrategy
        {
            public Point GetCanvasSize(Point windowSize, Point viewportSize)
            {
                return windowSize;
            }

            public void DrawCanvas()
            {
                // no-op
            }

            public RenderTarget2D BuildCanvas(GraphicsDevice graphicsDevice, Point viewportSize)
            {
                // no-op
                return null;
            }

            public float GetScaleFactor(Point windowSize, Point viewportSize)
            {
                // no-op
                return 1f;
            }

            public void SetRenderTargetToCanvas(GraphicsDevice graphicsDevice, RenderTarget2D screenRenderTarget)
            {
                // no-op
            }

            public void DrawCanvasToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D screenRenderTarget, Rectangle canvasRect)
            {
                // no-op
            }
        }
    }
}
