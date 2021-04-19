using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class AdHoc : BaseComponent
    {
        public Action<float> onUpdate;
        public Action<SpriteBatch> onDraw;
        public Action<Keys, ButtonState, ModifierKeys> onKey;

        public AdHoc(Actor actor) : base(actor)
        {

        }

        public override void Update(float dt)
        {
            this.onUpdate?.Invoke(dt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.onDraw?.Invoke(spriteBatch);
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            this.onKey?.Invoke(key, state, modifiers);
        }
    }
}
