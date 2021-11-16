using System;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    public class AdHoc : BaseComponent
    {
        public Action<SpriteBatch> onDraw;
        public Action<Keys, ButtonState, ModifierKeys> onKey;
        public Action<MouseButton, Vector2, ButtonState> onMouseButton;
        public Action<float> onUpdate;
        public Action<Vector2, Vector2, Vector2> onMouseUpdate;

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

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            this.onMouseUpdate?.Invoke(currentPosition, positionDelta, rawDelta);
        }
    }
}