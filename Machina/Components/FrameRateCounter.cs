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
        private float fps;

        public FrameRateCounter(Actor actor) : base(actor)
        {
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFont");
            this.past = DateTime.Now;
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
            spriteBatch.DrawString(this.font, ((int) this.fps).ToString() + "/" + Math.Round(drawFps, MidpointRounding.ToEven), Vector2.Zero, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
