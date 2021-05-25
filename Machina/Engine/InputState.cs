using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public struct InputState
    {
        internal static readonly InputState Empty = new InputState(new MouseState(), new KeyboardState(), new GamePadState());
        public MouseState mouseState;
        public KeyboardState keyboardState;
        public GamePadState gamepadState;

        public InputState(MouseState mouse, KeyboardState keyboard, GamePadState gamepad)
        {
            this.mouseState = mouse;
            this.keyboardState = keyboard;
            this.gamepadState = gamepad;
        }

        public static InputState RawHumanInput
        {
            get => new InputState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));
        }
    }
}
