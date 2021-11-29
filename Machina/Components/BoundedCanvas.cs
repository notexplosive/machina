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
                if (Runtime.DebugLevel > DebugLevel.Passive)
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
                this.renderTarget = Runtime.Painter.BuildRenderTarget(newSize);
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
            Runtime.Painter.SetRenderTarget(this.renderTarget);
            Runtime.Painter.Clear(BackgroundColor);

            this.DrawAdditionalContent?.Invoke(spriteBatch);

            Runtime.Painter.ClearRenderTarget();
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