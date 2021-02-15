using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using Machina.Data;
using Machina.Engine;

namespace Machina.Components
{
    class Canvas : BaseComponent
    {
        // Must supply a spriteBatch.Begin/End for each function
        public Action<SpriteBatch> DrawAdditionalContent;
        private BoundingRect boundingRect;
        private RenderTarget2D renderTarget;
        Color backgroundColor = Color.Black;

        /// <summary>
        /// Top left corner of the Canvas, assuming no rotation
        /// </summary>
        public Point TopLeftCorner
        {
            get => this.boundingRect.Rect.Location;
        }

        public Canvas(Actor actor) : base(actor)
        {
            this.DrawAdditionalContent += (SpriteBatch spriteBatch) =>
            {
                spriteBatch.Begin();
                spriteBatch.DrawRectangle(new Rectangle(5, 5, 10, 10), Color.Red, 1, this.actor.progeny.Depth);
                spriteBatch.DrawRectangle(new Rectangle(10, 10, 10, 10), Color.Red, 1, this.actor.progeny.Depth);
                spriteBatch.End();
            };

            this.boundingRect = RequireComponent<BoundingRect>();
            this.boundingRect.SetOffsetToCenter();

            var graphicsDevice = MachinaGame.Current.GraphicsDevice;

            renderTarget = new RenderTarget2D(
                graphicsDevice,
                boundingRect.Width,
                boundingRect.Height,
                false,
                graphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        public override void OnDelete()
        {
            renderTarget.Dispose();
        }

        public void DrawContent(SpriteBatch spriteBatch)
        {
            GraphicsDevice graphicsDevice = MachinaGame.Current.GraphicsDevice;
            graphicsDevice.SetRenderTarget(renderTarget);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphicsDevice.Clear(backgroundColor);

            this.DrawAdditionalContent?.Invoke(spriteBatch);

            graphicsDevice.SetRenderTarget(null);
        }

        public override void PreDraw(SpriteBatch spriteBatch)
        {
            DrawContent(spriteBatch);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(renderTarget, this.actor.progeny.Position, null, Color.White, this.actor.progeny.Angle, this.boundingRect.NormalizedCenter, 1f, SpriteEffects.None, this.actor.progeny.Depth);
        }
    }
}
