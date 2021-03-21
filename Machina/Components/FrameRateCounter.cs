using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class FrameRateCounter : BaseComponent
    {
        private SpriteFont font;
        private DateTime past;
        private Point screenSize;
        private float fps;

        public FrameRateCounter(Actor actor, Point screenSize) : base(actor)
        {
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFont");
            this.past = DateTime.Now;
            this.screenSize = screenSize;
        }

        public override void Update(float dt)
        {
            this.fps = 1 / dt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var now = DateTime.Now;
            float drawDt = (float) (now - this.past).TotalSeconds;
            this.past = now;
            float drawFps = 1 / drawDt;
            var text = ((int) this.fps).ToString() + "/" + Math.Round(drawFps, MidpointRounding.ToEven);
            spriteBatch.DrawString(this.font, text, new Vector2(this.screenSize.X - this.font.MeasureString(text).X, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
