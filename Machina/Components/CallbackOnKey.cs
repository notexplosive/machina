using System;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    public class CallbackOnKey : BaseComponent
    {
        private readonly Action callback;
        private readonly Keys key;
        private readonly ModifierKeys modif;

        public CallbackOnKey(Actor actor, Keys key, ModifierKeys modif, Action callback) : base(actor)
        {
            this.key = key;
            this.modif = modif;
            this.callback = callback;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == this.key && state == ButtonState.Pressed && modifiers == this.modif)
            {
                this.callback();
            }
        }
    }
}