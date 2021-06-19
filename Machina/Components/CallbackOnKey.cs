using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class CallbackOnKey : BaseComponent
    {
        private readonly Keys key;
        private readonly ModifierKeys modif;
        private readonly Action callback;

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
