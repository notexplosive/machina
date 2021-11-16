using System;
using System.IO;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    public class SnapshotTaker : BaseComponent
    {
        private readonly bool doNotUseTimer;
        private readonly string screenshotPath;
        private DateTime lastSnapshotTime;
        private bool pendingSnapshot;
        private double waitDuration;

        public SnapshotTaker(Actor actor, bool doNotUseTimer) : base(actor)
        {
            this.lastSnapshotTime = DateTime.Now;
            this.waitDuration = 5;
            this.doNotUseTimer = doNotUseTimer;
            this.pendingSnapshot = !doNotUseTimer;
#if DEBUG
            this.screenshotPath =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "screenshots"));
#else
            this.screenshotPath = MachinaGame.Current.appDataPath;
#endif
        }

        public override void Update(float dt)
        {
            if (!this.doNotUseTimer)
            {
                var currentTime = DateTime.Now;
                var timeSince = currentTime - this.lastSnapshotTime;
                if (timeSince.TotalSeconds >= this.waitDuration)
                {
                    this.pendingSnapshot = true;
                    this.lastSnapshotTime = currentTime;
                    this.waitDuration *= 2;
                }
            }
        }

        public override void PreDraw(SpriteBatch spriteBatch)
        {
            if (this.pendingSnapshot)
            {
                this.pendingSnapshot = false;
                SaveSnapshotAndDisposeTexture(spriteBatch);
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.F12 && state == ButtonState.Pressed)
            {
                this.pendingSnapshot = true;
            }
        }

        public void SaveSnapshotAndDisposeTexture(SpriteBatch spriteBatch)
        {
            var texture = MachinaGame.Current.CurrentCartridge.SceneLayers.RenderToTexture(spriteBatch);
            var currentTime = DateTime.Now;

            Directory.CreateDirectory(this.screenshotPath);
            using (var destStream =
                File.Create(Path.Combine(this.screenshotPath, currentTime.ToFileTimeUtc() + ".png")))
            {
                texture.SaveAsPng(destStream, texture.Width, texture.Height);
                MachinaGame.Print("Snapshot taken", this.screenshotPath);
            }

            texture.Dispose();
        }
    }
}