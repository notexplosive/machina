using Machina.Data;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;

namespace Machina.Engine
{
    public struct ModifierKeys
    {
        private readonly bool control;
        private readonly bool alt;
        private readonly bool shift;

        public static ModifierKeys NoModifiers = new ModifierKeys(false, false, false);

        public ModifierKeys(bool control, bool alt, bool shift)
        {
            this.control = control;
            this.alt = alt;
            this.shift = shift;
        }

        public ModifierKeys(int encodedInt) : this()
        {
            this.control = (encodedInt & 1) == 1;
            this.alt = (encodedInt & 2) == 2;
            this.shift = (encodedInt & 4) == 4;
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
        public bool Control => control && !alt && !shift;
        public bool ControlAlt => control && alt && !shift;
        public bool Alt => !control && alt && !shift;
        public bool Shift => !control && !alt && shift;
        public bool AltShift => !control && alt && shift;
        public bool ControlShift => control && !alt && shift;
        public bool ControlAltShift => control && alt && shift;

        public int EncodedInt => (Bool2Int(control) << 2) | (Bool2Int(alt) << 1) | (Bool2Int(shift) << 0);
        public int Bool2Int(bool b)
        {
            return b ? 1 : 0;
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

    public class KeyTracker
    {
        private KeyboardState oldState;

        public KeyboardFrameState Calculate(KeyboardState currentState)
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

            this.oldState = currentState;
            return new KeyboardFrameState(keysPressedThisFrame.ToArray(), keysReleasedThisFrame.ToArray(), new ModifierKeys(control, alt, shift));
        }
    }
}
