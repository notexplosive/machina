using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Components
{
    public class DemoRecorderComponent : BaseComponent
    {
        private readonly SpriteFont font;
        private readonly Demo.Recorder recorder;
        private readonly string text;

        public DemoRecorderComponent(Actor actor, Demo.Recorder recorder) : base(actor)
        {
            this.recorder = recorder;
            this.font = MachinaGame.Assets.GetSpriteFont("DefaultFont");
            this.text = "DEMO REC " + this.recorder.fileName + " CTRL+P to save recording";
        }

        public override void Update(float dt)
        {
            this.recorder.AddEntry(dt, this.actor.scene.sceneLayers.CurrentInputFrameState);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle(new Point(0, 0), this.actor.scene.sceneLayers.gameCanvas.WindowSize);
            var padding = 2;
            rect.Inflate(-padding, -padding);
            spriteBatch.DrawRectangle(rect, Color.Red, 2f, new Depth(10));

            var textSize = this.font.MeasureString(this.text);
            spriteBatch.DrawString(this.font, this.text,
                new Vector2(rect.Right - textSize.X - padding * 2, rect.Bottom - textSize.Y - padding * 2), Color.Red);
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.P && modifiers.Control && state == ButtonState.Released)
            {
                this.recorder.WriteDemoToDisk();
            }
        }
    }
}