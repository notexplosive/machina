using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class FrameRateCounter : BaseComponent
    {
        private readonly SpriteFont font;
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
            var screenWidth = this.actor.scene.sceneLayers.gameCanvas.WindowSize.X;
            var now = DateTime.Now;
            float drawDt = (float) (now - this.past).TotalSeconds;
            this.past = now;
            float drawFps = 1 / drawDt;
            var text = ((int) this.fps).ToString() + "/" + Math.Round(drawFps, MidpointRounding.ToEven);
            spriteBatch.DrawString(this.font, text, new Vector2(screenWidth - this.font.MeasureString(text).X - 5, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.O && modifiers.Control && state == ButtonState.Pressed)
            {
                this.actor.Visible = !this.actor.Visible;
            }
        }
    }
}
