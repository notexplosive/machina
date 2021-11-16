using System;
using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine.Debugging.Components
{
    public class WindowedConsoleRenderer : BaseComponent, ILogger
    {
        private readonly SpriteFont font;
        public readonly List<string> lines = new List<string>();
        private readonly Scrollbar scrollbar;

        public WindowedConsoleRenderer(Actor actor, Scrollbar scrollbar) : base(actor)
        {
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFontSmall");
            this.scrollbar = scrollbar;
            MachinaGame.Current.Runtime.CurrentCartridge.SceneLayers.PushLogger(this);
            MachinaGame.Print("Logger pushed");
        }

        public void Log(params object[] objects)
        {
            var strings = new List<string>();
            foreach (var obj in objects)
            {
                strings.Add(obj.ToString());
            }

            var output = string.Join("   ", strings);

            Console.WriteLine(output);
            this.lines.Add(output);
        }

        public override void OnDeleteFinished()
        {
            MachinaGame.Current.Runtime.CurrentCartridge.SceneLayers.PopLogger();
            MachinaGame.Print("Logger popped");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var worldY = 0;
            var camera = this.actor.scene.camera;
            var textHeight = this.font.LineSpacing;
            foreach (var line in this.lines)
            {
                if (worldY > camera.UnscaledPosition.Y - textHeight &&
                    worldY < camera.UnscaledPosition.Y + camera.UnscaledViewportSize.Y)
                {
                    spriteBatch.DrawString(this.font, line, new Vector2(0, worldY), Color.White);
                }

                worldY += textHeight;
            }

            var wasScrolledToBottom =
                this.scrollbar.CurrentScrollPercent == 1f || this.scrollbar.IsToSmallToNeedScrollbar();
            this.scrollbar.SetWorldBounds(new MinMax<int>(0, worldY));
            if (wasScrolledToBottom)
            {
                this.scrollbar.SetScrollPercent(1f);
            }
        }
    }
}