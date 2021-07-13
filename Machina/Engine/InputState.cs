using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public struct InputState
    {
        internal static readonly InputState Empty = new InputState(new MouseState(), new KeyboardState(), new GamePadState(), new TouchCollection());
        public MouseState mouseState;
        public KeyboardState keyboardState;
        public GamePadState gamepadState;
        public TouchCollection touches;

        public InputState(MouseState mouse, KeyboardState keyboard, GamePadState gamepad, TouchCollection touches)
        {
            this.mouseState = mouse;
            this.keyboardState = keyboard;
            this.gamepadState = gamepad;
            this.touches = touches;
        }

        public static InputState RawHumanInput
        {
            get => new InputState(Mouse.GetState(), Keyboard.GetState(), GamePad.GetState(PlayerIndex.One), TouchPanel.GetState());
        }
    }
}
