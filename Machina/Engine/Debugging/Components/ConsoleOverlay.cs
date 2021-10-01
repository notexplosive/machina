﻿using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Engine.Debugging.Components
{
    internal class ConsoleOverlay : BaseComponent, IDebugOutputRenderer
    {
        private readonly List<string> messages;
        private readonly SpriteFont spriteFont;
        private readonly TweenChain tweenChain;
        private float opacity;

        public ConsoleOverlay(Actor actor, SpriteFont spriteFont) : base(actor)
        {
            this.spriteFont = spriteFont;
            this.messages = new List<string>();
            this.opacity = 0f;
            this.tweenChain = new TweenChain()
                .AppendWaitTween(3f)
                .AppendFloatTween(0f, 2f, EaseFuncs.QuadraticEaseIn,
                    new TweenAccessors<float>(() => { return this.opacity; }, val => { this.opacity = val; }))
                .AppendCallback(() => { this.messages.Clear(); });
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            var screenWidth = this.actor.scene.sceneLayers.gameCanvas.WindowSize.X;
            var i = 0;

            spriteBatch.FillRectangle(
                new Rectangle(0, 0, screenWidth, this.spriteFont.LineSpacing * this.messages.Count),
                new Color(Color.Black, this.opacity / 2), 0.001f);
            foreach (var message in this.messages)
            {
                spriteBatch.DrawString(this.spriteFont, message, new Vector2(8, this.spriteFont.LineSpacing * i),
                    new Color(1, 1, 1, this.opacity), 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                i++;
            }
        }

        public override void Update(float dt)
        {
            this.tweenChain.Update(dt);
        }

        private void RestartFade()
        {
            this.opacity = 1;
            this.tweenChain.Refresh();
        }
    }
}
