using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public struct InputState
    {
        internal static readonly InputState Empty = new InputState(new MouseState(), new KeyboardState());
        public MouseState mouseState;
        public KeyboardState keyboardState;

        public InputState(MouseState mouse, KeyboardState keyboard)
        {
            this.mouseState = mouse;
            this.keyboardState = keyboard;
        }

        public static InputState RawHumanInput
        {
            get => new InputState(Mouse.GetState(), Keyboard.GetState());
        }
    }
}
