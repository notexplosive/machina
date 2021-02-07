using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ConsoleOverlay : BaseComponent, IDebugOutputListener
    {
        private SpriteFont spriteFont;
        private GraphicsDeviceManager graphics;
        private List<string> messages;
        private float fadeTimer;
        private float totalFadeTimer = 5;

        public ConsoleOverlay(Actor actor, SpriteFont spriteFont, GraphicsDeviceManager graphics) : base(actor)
        {
            this.spriteFont = spriteFont;
            this.graphics = graphics;
            this.messages = new List<string>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var opacity = this.fadeTimer / this.totalFadeTimer;
            var screenWidth = graphics.PreferredBackBufferWidth;
            int i = 0;

            spriteBatch.FillRectangle(new Rectangle(0, 0, screenWidth, spriteFont.LineSpacing * messages.Count), new Color(Color.Black, opacity / 2));
            foreach (var message in this.messages)
            {
                spriteBatch.DrawString(spriteFont, message, new Vector2(8, spriteFont.LineSpacing * i), new Color(opacity, opacity, opacity, opacity));
                i++;
            }
        }

        public override void Update(float dt)
        {
            if (this.fadeTimer > 0)
            {
                this.fadeTimer -= dt;
            }
            else
            {
                this.fadeTimer = 0;
            }
        }

        public void OnMessageLog(string line)
        {
            this.fadeTimer = this.totalFadeTimer;
            this.messages.Add(line);

            while (this.messages.Count > 32)
            {
                this.messages.RemoveAt(0);
            }
        }
    }
}