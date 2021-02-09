using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ConsoleOverlay : BaseComponent, IDebugOutputRenderer
    {
        private SpriteFont spriteFont;
        private GraphicsDeviceManager graphics;
        private List<string> messages;
        private float opacity;
        private TweenChain tweenChain;

        public ConsoleOverlay(Actor actor, SpriteFont spriteFont, GraphicsDeviceManager graphics) : base(actor)
        {
            this.spriteFont = spriteFont;
            this.graphics = graphics;
            this.messages = new List<string>();
            this.opacity = 0f;
            this.tweenChain = new TweenChain()
                .AppendWaitTween(3f)
                .AppendFloatTween(0f, 2f, EaseFuncs.QuadraticEaseIn, new TweenAccessors<float>(() => { return this.opacity; }, (val) => { this.opacity = val; }));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var screenWidth = graphics.PreferredBackBufferWidth;
            int i = 0;

            spriteBatch.FillRectangle(new Rectangle(0, 0, screenWidth, spriteFont.LineSpacing * messages.Count), new Color(Color.Black, opacity / 2));
            foreach (var message in this.messages)
            {
                spriteBatch.DrawString(spriteFont, message, new Vector2(8, spriteFont.LineSpacing * i), new Color(1, 1, 1, opacity));
                i++;
            }
        }

        public override void Update(float dt)
        {
            this.tweenChain.Update(dt);
        }

        public void OnMessageLog(string line)
        {
            RestartFade();
            this.messages.Add(line);

            while (this.messages.Count > 15)
            {
                this.messages.RemoveAt(0);
            }
        }

        private void RestartFade()
        {
            this.opacity = 1;
            this.tweenChain.Refresh();
        }
    }
}