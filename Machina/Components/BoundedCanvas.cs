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
    public class BoundedCanvas : BaseComponent
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

        public BoundedCanvas(Actor actor) : base(actor)
        {
            this.DrawAdditionalContent += (SpriteBatch spriteBatch) =>
            {
                if (MachinaGame.DebugLevel > DebugLevel.Passive)
                {
                    spriteBatch.Begin();
                    spriteBatch.DrawRectangle(new Rectangle(5, 5, 10, 10), Color.Red, 1, this.actor.transform.Depth);
                    spriteBatch.DrawRectangle(new Rectangle(10, 10, 10, 10), Color.Red, 1, this.actor.transform.Depth);
                    spriteBatch.End();
                }
            };

            this.boundingRect = RequireComponent<BoundingRect>();
            this.boundingRect.onSizeChange += BuildRenderTarget;

            if (this.boundingRect.Area > 0)
            {
                BuildRenderTarget(this.boundingRect.Size);
            }
        }

        private void BuildRenderTarget(Point newSize)
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }

            if (newSize.X * newSize.Y > 0)
            {

                var graphicsDevice = MachinaGame.Current.GraphicsDevice;
                renderTarget = new RenderTarget2D(
                        graphicsDevice,
                        newSize.X,
                        newSize.Y,
                        false,
                        graphicsDevice.PresentationParameters.BackBufferFormat,
                        DepthFormat.Depth24);
            }
        }

        public override void OnDeleteFinished()
        {
            this.boundingRect.onSizeChange -= BuildRenderTarget;
            if (renderTarget != null)
            {
                renderTarget.Dispose();
            }
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
            if (this.renderTarget != null)
            {
                spriteBatch.Draw(renderTarget, this.actor.transform.Position, null, Color.White, this.actor.transform.Angle, this.boundingRect.NormalizedOffset, 1f, SpriteEffects.None, this.actor.transform.Depth.AsFloat);
            }
        }
    }
}
