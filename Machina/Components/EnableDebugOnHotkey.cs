using Machina.Engine;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public struct KeyCombination
    {
        public readonly Keys key;
        public readonly ModifierKeys modifiers;

        public KeyCombination(Keys key, ModifierKeys modifiers = new ModifierKeys())
        {
            this.key = key;
            this.modifiers = modifiers;
        }
    }

    class EnableDebugOnHotkey : BaseComponent
    {
        private KeyCombination keyCombo;

        public EnableDebugOnHotkey(Actor actor, KeyCombination keyCombo) : base(actor)
        {
            this.keyCombo = keyCombo;
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            if (keyCombo.key == key && keyCombo.modifiers == modifiers && buttonState == ButtonState.Pressed)
            {
                if (MachinaGame.DebugLevel == DebugLevel.Passive)
                {
                    MachinaGame.DebugLevel = DebugLevel.Active;
                }
                else
                {
                    MachinaGame.DebugLevel = DebugLevel.Passive;
                }

                MachinaGame.Print("DebugLevel set to ", MachinaGame.DebugLevel);
            }
        }
    }
}
