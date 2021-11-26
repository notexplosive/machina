using Machina.Components;
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
                if (Runtime.DebugLevel == DebugLevel.Passive)
                {
                    Runtime.DebugLevel = DebugLevel.Active;
                }
                else
                {
                    Runtime.DebugLevel = DebugLevel.Passive;
                }

                MachinaClient.Print("DebugLevel set to ", Runtime.DebugLevel);
            }
        }
    }
}