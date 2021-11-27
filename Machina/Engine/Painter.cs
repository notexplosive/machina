using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class Painter
    {
        private GraphicsDevice GraphicsDevice { get; }
        public SpriteBatch SpriteBatch { get; }

        public Painter(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
        }

        public Texture2D CreateTexture(Point size)
        {
            return new Texture2D(GraphicsDevice, size.X, size.Y);
        }

        public void ClearRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(null);
        }

        public void Clear(Color color)
        {
            GraphicsDevice.Clear(color);
        }

        public RenderTarget2D BuildRenderTarget(Point size)
        {
            return new RenderTarget2D(
                    GraphicsDevice,
                    size.X,
                    size.Y,
                    false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24);
        }

        public Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture)
        {
            if (rect.Width * rect.Height == 0)
            {
                return null;
            }

            var cropTexture = new Texture2D(GraphicsDevice, rect.Width, rect.Height);
            var data = new Color[rect.Width * rect.Height];
            sourceTexture.GetData(0, rect, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
        }
    }
}
