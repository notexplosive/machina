using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Machina.Components
{
    public class SnapshotTaker : BaseComponent
    {
        private DateTime lastSnapshotTime;
        private bool pendingSnapshot;
        private double waitDuration;
        private readonly bool doNotUseTimer;
        private readonly string screenshotPath;

        public SnapshotTaker(Actor actor, bool doNotUseTimer) : base(actor)
        {
            this.lastSnapshotTime = DateTime.Now;
            this.waitDuration = 5;
            this.doNotUseTimer = doNotUseTimer;
            this.pendingSnapshot = !doNotUseTimer;
#if DEBUG
            this.screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "screenshots");
#else
            this.screenshotPath = MachinaGame.Current.appDataPath;
#endif
        }

        public override void Update(float dt)
        {
            if (!this.doNotUseTimer)
            {
                var currentTime = DateTime.Now;
                var timeSince = currentTime - lastSnapshotTime;
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
                MachinaGame.Print("Snapshot taken");
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
            var texture = MachinaGame.Current.SceneLayers.RenderToTexture(spriteBatch);
            var currentTime = DateTime.Now;

            Directory.CreateDirectory(this.screenshotPath);
            using (FileStream destStream = File.Create(Path.Combine(this.screenshotPath, currentTime.ToFileTimeUtc().ToString() + ".png")))
            {
                texture.SaveAsPng(destStream, texture.Width, texture.Height);
            }
            texture.Dispose();
        }
    }
}
