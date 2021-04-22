using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class DemoPlaybackComponent : BaseComponent
    {
        private Demo.Playback playback;
        private SpriteFont font;
        private string text;

        public DemoPlaybackComponent(Actor actor, Demo.Playback playback) : base(actor)
        {
            this.playback = playback;
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFont");
            this.text = "Playback";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var windowSize = this.actor.scene.sceneLayers.gameCanvas.WindowSize;
            var borderRect = new Rectangle(new Point(0, 0), windowSize);
            var padding = 2;
            borderRect.Inflate(-padding, -padding);
            spriteBatch.DrawRectangle(borderRect, Color.Lime, 2f, new Depth(10));


            var progressBarThickness = 32;
            var textSize = this.font.MeasureString(this.text);
            spriteBatch.DrawString(this.font, this.text, new Vector2(borderRect.Right - textSize.X - padding * 2, borderRect.Bottom - textSize.Y - padding * 2 - progressBarThickness), Color.Lime, 0, Vector2.Zero, 1f, SpriteEffects.None, new Depth(10));

            var progressRectOuter = new Rectangle(new Point(0, windowSize.Y - progressBarThickness), new Point(windowSize.X, progressBarThickness));
            var progressRectInner = new Rectangle(new Point(0, windowSize.Y - progressBarThickness), new Point((int) (windowSize.X * this.playback.Progress), progressBarThickness));

            progressRectOuter.Inflate(-padding * 2, -padding * 2);
            progressRectInner.Inflate(-padding * 2, -padding * 2);

            spriteBatch.DrawRectangle(progressRectOuter, Color.Lime, 2f, new Depth(10));
            spriteBatch.FillRectangle(progressRectInner, Color.ForestGreen, new Depth(12));

            var mouse = this.playback.LatestFrameState.mouseFrameState;
            var mousePos = mouse.RawWindowPosition;
            var cursorSize = 10;
            spriteBatch.DrawCircle(new CircleF(mousePos, cursorSize), cursorSize, Color.Lime, 1f, new Depth(5));

            if (mouse.ButtonsPressedThisFrame.left)
            {
                spriteBatch.DrawCircle(new CircleF(mousePos + new Point(-cursorSize, -cursorSize), 5), 5, Color.ForestGreen, 5f, new Depth(6));
            }

            if (mouse.ButtonsPressedThisFrame.middle)
            {
                spriteBatch.DrawCircle(new CircleF(mousePos + new Point(0, -cursorSize), 5), 5, Color.ForestGreen, 5f, new Depth(6));
            }

            if (mouse.ButtonsPressedThisFrame.right)
            {
                spriteBatch.DrawCircle(new CircleF(mousePos + new Point(cursorSize, -cursorSize), 5), 5, Color.ForestGreen, 5f, new Depth(6));
            }

            if (mouse.PositionDelta.LengthSquared() > 0)
            {
                spriteBatch.DrawLine(mousePos.ToVector2(), mousePos.ToVector2() - mouse.PositionDelta, Color.Lime, 1f, new Depth(5));
            }
        }

        public override void Update(float dt)
        {
            if (this.playback.IsFinished)
            {
                this.actor.RemoveComponent<DemoPlaybackComponent>();
            }
        }
    }
}
