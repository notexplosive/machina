﻿using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine.Debugging.Components
{
    internal class EnableDebugOnHotkey : BaseComponent
    {
        private readonly KeyCombination keyCombo;

        public EnableDebugOnHotkey(Actor actor, KeyCombination keyCombo) : base(actor)
        {
            this.keyCombo = keyCombo;
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            if (this.keyCombo.key == key && this.keyCombo.modifiers == modifiers && buttonState == ButtonState.Pressed)
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