using System;
using System.Collections.Generic;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Engine.Input
{
    public class KeyTracker
    {
        private GamePadState oldGamePadState;
        private KeyboardState oldKeyState;

        public KeyboardFrameState CalculateFrameState(KeyboardState currentState, GamePadState currentGamePadState)
        {
            var currentPressed = currentState.GetPressedKeys();
            var oldPressed = this.oldKeyState.GetPressedKeys();
            var keysPressedThisFrame = new List<Keys>();
            var keysReleasedThisFrame = new List<Keys>();

            foreach (var current in currentPressed)
            {
                if (!this.oldKeyState.IsKeyDown(current))
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

            var control = false;
            var alt = false;
            var shift = false;

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
            ConvertGamepadButtonToKeyboard(currentGamePadState.Buttons.A, this.oldGamePadState.Buttons.A, Keys.Z);
            // B button -> Backspace
            ConvertGamepadButtonToKeyboard(currentGamePadState.Buttons.B, this.oldGamePadState.Buttons.B, Keys.Back);

            var currentThumbstickAsDPad = ConvertThumbstickToDPad(currentGamePadState.ThumbSticks.Left);
            var oldThumbstickAsDPad = ConvertThumbstickToDPad(this.oldGamePadState.ThumbSticks.Left);

            ConvertGamepadToKeyboard(currentGamePadState.DPad, this.oldGamePadState.DPad);
            ConvertGamepadToKeyboard(currentThumbstickAsDPad, oldThumbstickAsDPad);

            this.oldKeyState = currentState;
            this.oldGamePadState = currentGamePadState;
            return new KeyboardFrameState(keysPressedThisFrame.ToArray(), keysReleasedThisFrame.ToArray(),
                new ModifierKeys(control, alt, shift));
        }
    }
}