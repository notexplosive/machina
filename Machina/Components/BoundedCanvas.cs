using System;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    public class BoundedCanvas : BaseComponent
    {
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color TintColor { get; set; } = Color.White;

        private readonly BoundingRect boundingRect;

        // Must supply a spriteBatch.Begin/End for each function
        public Action<SpriteBatch> DrawAdditionalContent;
        private RenderTarget2D renderTarget;

        public BoundedCanvas(Actor actor) : base(actor)
        {
            this.DrawAdditionalContent += spriteBatch =>
            {
                if (MachinaGame.Current.Runtime.DebugLevel > DebugLevel.Passive)
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

        /// <summary>
        ///     Top left corner of the Canvas, assuming no rotation
        /// </summary>
        public Point TopLeftCorner => this.boundingRect.Rect.Location;

        private void BuildRenderTarget(Point newSize)
        {
            if (this.renderTarget != null)
            {
                this.renderTarget.Dispose();
                this.renderTarget = null;
            }

            if (newSize.X * newSize.Y > 0)
            {
                var graphicsDevice = MachinaGame.Current.Runtime.GraphicsDevice;
                this.renderTarget = new RenderTarget2D(
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
            if (this.renderTarget != null)
            {
                this.renderTarget.Dispose();
            }
        }

        public void DrawContent(SpriteBatch spriteBatch)
        {
            var graphicsDevice = MachinaGame.Current.Runtime.GraphicsDevice;
            graphicsDevice.SetRenderTarget(this.renderTarget);

            graphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
            graphicsDevice.Clear(BackgroundColor);

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
                spriteBatch.Draw(this.renderTarget, this.actor.transform.Position, null, TintColor,
                    this.actor.transform.Angle, this.boundingRect.NormalizedOffset, 1f, SpriteEffects.None,
                    this.actor.transform.Depth.AsFloat);
            }
        }
    }
}