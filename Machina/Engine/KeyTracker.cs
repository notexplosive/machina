using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public struct ModifierKeys
    {
        public readonly bool control;
        public readonly bool alt;
        public readonly bool shift;

        public ModifierKeys(bool control, bool alt, bool shift)
        {
            this.control = control;
            this.alt = alt;
            this.shift = shift;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.control)
            {
                sb.Append('^');
            }
            if (this.shift)
            {
                sb.Append('+');
            }
            if (this.alt)
            {
                sb.Append('~');
            }
            return sb.ToString();
        }
    }

    class KeyTracker
    {
        private KeyboardState oldState;

        public ModifierKeys Modifiers
        {
            get;
            private set;
        }
        public Keys[] Pressed
        {
            get; private set;
        }
        public Keys[] Released
        {
            get;
            private set;
        }

        public void Calculate()
        {
            var currentState = Keyboard.GetState();
            var currentPressed = currentState.GetPressedKeys();
            var oldPressed = this.oldState.GetPressedKeys();
            var keysPressedThisFrame = new List<Keys>();
            var keysReleasedThisFrame = new List<Keys>();

            foreach (var current in currentPressed)
            {
                if (!oldState.IsKeyDown(current))
                {
                    keysPressedThisFrame.Add(current);
                }
            }

            foreach (var old in oldPressed)
            {
                if (!currentState.IsKeyDown(old))
                {
                    keysReleasedThisFrame.Add(old);
                }
            }

            bool control = false;
            bool alt = false;
            bool shift = false;

            if (currentState.IsKeyDown(Keys.LeftControl) || currentState.IsKeyDown(Keys.RightControl))
            {
                control = true;
            }

            if (currentState.IsKeyDown(Keys.LeftShift) || currentState.IsKeyDown(Keys.RightShift))
            {
                shift = true;
            }

            if (currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt))
            {
                alt = true;
            }

            Modifiers = new ModifierKeys(control, alt, shift);
            Pressed = keysPressedThisFrame.ToArray();
            Released = keysReleasedThisFrame.ToArray();

            this.oldState = currentState;
        }
    }
}
