using Machina.Engine;
using Microsoft.Xna.Framework;
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
        internal Action<MouseButton, Vector2, ButtonState> onMouseButton;

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

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state)
        {
            this.onMouseButton?.Invoke(button, currentPosition, state);
        }
    }
}
