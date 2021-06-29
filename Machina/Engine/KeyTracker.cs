using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private KeyboardState oldKeyState;
        private GamePadState oldGamePadState;

        public KeyboardFrameState CalculateFrameState(KeyboardState currentState, GamePadState currentGamePadState)
        {
            var currentPressed = currentState.GetPressedKeys();
            var oldPressed = this.oldKeyState.GetPressedKeys();
            var keysPressedThisFrame = new List<Keys>();
            var keysReleasedThisFrame = new List<Keys>();

            foreach (var current in currentPressed)
            {
                if (!oldKeyState.IsKeyDown(current))
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



            void ConvertGamepadButtonToKeyboard(ButtonState newState, ButtonState oldState, Keys key)
            {
                if (newState != oldState)
                {
                    if (newState == ButtonState.Pressed)
                    {
                        keysPressedThisFrame.Add(key);
                    }
                    else
                    {
                        keysReleasedThisFrame.Add(key);
                    }
                }
            }

            GamePadDPad ConvertThumbstickToDPad(Vector2 vec)
            {
                var down = ButtonState.Released;
                var left = ButtonState.Released;
                var right = ButtonState.Released;
                var up = ButtonState.Released;
                if (vec.Length() > 0.5f)
                {
                    var angle = vec.ToAngle();
                    if (angle <= MathF.PI / 4 && angle > -MathF.PI / 4)
                    {
                        down = ButtonState.Pressed;
                    }

                    if (angle > MathF.PI / 4 && angle <= 3 * MathF.PI / 4)
                    {
                        right = ButtonState.Pressed;
                    }

                    if (angle > 3 * MathF.PI / 4 || angle < -3 * MathF.PI / 2)
                    {
                        up = ButtonState.Pressed;
                    }

                    if (angle <= -MathF.PI / 4 && angle >= -3 * MathF.PI / 2)
                    {
                        left = ButtonState.Pressed;
                    }
                }

                return new GamePadDPad(up, down, left, right);
            }

            void ConvertGamepadToKeyboard(GamePadDPad current, GamePadDPad old)
            {
                // DPad -> Arrow Keys
                ConvertGamepadButtonToKeyboard(current.Right, old.Right, Keys.Right);
                ConvertGamepadButtonToKeyboard(current.Left, old.Left, Keys.Left);
                ConvertGamepadButtonToKeyboard(current.Up, old.Up, Keys.Up);
                ConvertGamepadButtonToKeyboard(current.Down, old.Down, Keys.Down);
            }

            // A button -> Space
            ConvertGamepadButtonToKeyboard(currentGamePadState.Buttons.A, this.oldGamePadState.Buttons.A, Keys.Space);
            // B button -> Backspace
            ConvertGamepadButtonToKeyboard(currentGamePadState.Buttons.B, this.oldGamePadState.Buttons.B, Keys.Back);

            var currentThumbstickAsDPad = ConvertThumbstickToDPad(currentGamePadState.ThumbSticks.Left);
            var oldThumbstickAsDPad = ConvertThumbstickToDPad(this.oldGamePadState.ThumbSticks.Left);

            ConvertGamepadToKeyboard(currentGamePadState.DPad, this.oldGamePadState.DPad);
            ConvertGamepadToKeyboard(currentThumbstickAsDPad, oldThumbstickAsDPad);

            this.oldKeyState = currentState;
            this.oldGamePadState = currentGamePadState;
            return new KeyboardFrameState(keysPressedThisFrame.ToArray(), keysReleasedThisFrame.ToArray(), new ModifierKeys(control, alt, shift));
        }
    }
}
