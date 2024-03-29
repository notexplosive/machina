﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    public enum ResizeBehavior
    {
        KeepAspectRatio,
        FreeAspectRatio
    }

    public interface IGameViewport
    {
        public Point ViewportSize { get; }

        public float ScaleFactor { get; }

        public Rectangle CanvasRect { get; }

        public Point WindowSize { get; }
    }

    public class GameViewport : IGameViewport
    {
        private readonly IResizeStrategy resizeStrategy;
        private RenderTarget2D internalCanvas;

        public GameViewport(Point viewportSize, ResizeBehavior resizeBehavior)
        {
            ViewportSize = viewportSize;
            if (resizeBehavior == ResizeBehavior.FreeAspectRatio)
            {
                this.resizeStrategy = new FillStrategy();
            }
            else
            {
                this.resizeStrategy = new MaintainDesiredResolutionStrategy();
            }

            SetWindowSize(viewportSize);
        }

        public Rectangle CanvasRect
        {
            get
            {
                var canvasSize = this.resizeStrategy.GetCanvasSize(WindowSize, ViewportSize);
                var canvasPos = new Point((WindowSize.X - canvasSize.X) / 2, (WindowSize.Y - canvasSize.Y) / 2);
                return new Rectangle(canvasPos.X, canvasPos.Y, canvasSize.X, canvasSize.Y);
            }
        }

        public float ScaleFactor => this.resizeStrategy.GetScaleFactor(WindowSize, ViewportSize);

        public Point WindowSize { get; private set; }

        public Point ViewportSize { get; }

        public void SetWindowSize(Point windowSize)
        {
            WindowSize = windowSize;
        }

        public void BuildCanvas(Painter painter)
        {
            this.internalCanvas = this.resizeStrategy.BuildCanvas(painter, ViewportSize);
        }

        public void SetRenderTargetToCanvas(Painter painter)
        {
            this.resizeStrategy.SetRenderTargetToCanvas(painter, this.internalCanvas);
        }

        public void DrawCanvasToScreen(MachinaRuntime runtime, Painter painter)
        {
            this.resizeStrategy.DrawCanvasToScreen(this.internalCanvas, CanvasRect, runtime, painter);
        }

        private interface IResizeStrategy
        {
            void SetRenderTargetToCanvas(Painter painter, RenderTarget2D screenRenderTarget);

            void DrawCanvasToScreen(RenderTarget2D screenRenderTarget, Rectangle canvasRect, MachinaRuntime runtime, Painter painter);

            RenderTarget2D BuildCanvas(Painter painter, Point viewportSize);
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

            public RenderTarget2D BuildCanvas(Painter painter, Point viewportSize)
            {
                return painter.BuildRenderTarget(viewportSize);
            }

            public void DrawCanvasToScreen(RenderTarget2D canvas, Rectangle canvasRect, MachinaRuntime runtime, Painter painter)
            {
                painter.ClearRenderTarget();
                painter.SpriteBatch.Begin(SpriteSortMode.BackToFront, null, runtime.CurrentCartridge.UsedSamplerState, DepthStencilState.DepthRead);
                painter.SpriteBatch.Draw(canvas,
                    canvasRect,
                    null, Color.White);

                painter.SpriteBatch.End();
            }

            public Point GetCanvasSize(Point windowSize, Point viewportSize)
            {
                return (new Vector2(viewportSize.X, viewportSize.Y) * GetScaleFactor(windowSize, viewportSize))
                    .ToPoint();
            }

            public void SetRenderTargetToCanvas(Painter painter, RenderTarget2D screenRenderTarget)
            {
                painter.SetRenderTarget(screenRenderTarget);
            }
        }

        private class FillStrategy : IResizeStrategy
        {
            public Point GetCanvasSize(Point windowSize, Point viewportSize)
            {
                return windowSize;
            }

            public RenderTarget2D BuildCanvas(Painter painter, Point viewportSize)
            {
                // no-op
                return null;
            }

            public float GetScaleFactor(Point windowSize, Point viewportSize)
            {
                // no-op
                return 1f;
            }

            public void SetRenderTargetToCanvas(Painter painter, RenderTarget2D screenRenderTarget)
            {
                // no-op
            }

            public void DrawCanvasToScreen(RenderTarget2D screenRenderTarget, Rectangle canvasRect, MachinaRuntime runtime, Painter painter)
            {
                // no-op
            }
        }
    }
}