using Microsoft.Xna.Framework.Input;

namespace Machina.Data
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

        internal bool Match(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            return key == this.key && state == ButtonState.Pressed && modifiers == this.modifiers;
        }
    }
}
