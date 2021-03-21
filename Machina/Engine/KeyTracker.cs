using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Machina.Engine
{
    public struct ModifierKeys
    {
        public readonly bool control;
        public readonly bool alt;
        public readonly bool shift;

        public static ModifierKeys NoModifiers = new ModifierKeys(false, false, false);

        public ModifierKeys(bool control, bool alt, bool shift)
        {
            this.control = control;
            this.alt = alt;
            this.shift = shift;
        }

        public static bool operator ==(ModifierKeys a, ModifierKeys b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ModifierKeys a, ModifierKeys b)
        {
            return !a.Equals(b);
        }

        public bool None => !control && !alt && !shift;
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

        public override bool Equals(object obj)
        {
            if (obj is ModifierKeys other)
            {
                return this.GetHashCode() == obj.GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            var shift = this.shift ? 1 << 0 : 0;
            var ctrl = this.control ? 1 << 1 : 0;
            var alt = this.alt ? 1 << 2 : 0;

            return ctrl | alt | shift;
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

        public void Calculate(KeyboardState currentState)
        {
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
