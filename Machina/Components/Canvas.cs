using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;

namespace Machina.Components
{
    class Canvas : DrawOnlyComponent
    {
        public Action<SpriteBatch> DrawAdditionalContent;
        private BoundingRect boundingRect;
        private GraphicsDevice graphicsDevice;
        private RenderTarget2D renderTarget;
        Color backgroundColor = Color.Orange;

        public Canvas(Actor actor, GraphicsDevice graphicsDevice) : base(actor)
        {
            this.DrawAdditionalContent += (SpriteBatch spriteBatch) =>
            {
                spriteBatch.Begin();
                spriteBatch.DrawRectangle(new Rectangle(5, 5, 10, 10), Color.Red);
                spriteBatch.End();
            };

            this.boundingRect = RequireComponent<BoundingRect>();
            this.graphicsDevice = graphicsDevice;

            renderTarget = new RenderTarget2D(
                graphicsDevice,
                boundingRect.Width,
                boundingRect.Height,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public void DrawContent(SpriteBatch spriteBatch)
        {
            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            spriteBatch.DrawRectangle(new Rectangle(10, 10, 10, 10), Color.Red);
            spriteBatch.End();
            this.DrawAdditionalContent?.Invoke(spriteBatch);

            graphicsDevice.SetRenderTarget(null);
        }

        public override void EarlyDraw(SpriteBatch spriteBatch)
        {
            DrawContent(spriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(renderTarget, this.boundingRect.Rect, Color.White);
        }
    }
}
