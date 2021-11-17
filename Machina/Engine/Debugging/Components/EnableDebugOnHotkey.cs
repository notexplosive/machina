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
                if (this.actor.scene.sceneLayers.runtime.DebugLevel == DebugLevel.Passive)
                {
                    this.actor.scene.sceneLayers.runtime.DebugLevel = DebugLevel.Active;
                }
                else
                {
                    this.actor.scene.sceneLayers.runtime.DebugLevel = DebugLevel.Passive;
                }

                MachinaGame.Print("DebugLevel set to ", this.actor.scene.sceneLayers.runtime.DebugLevel);
            }
        }
    }
}