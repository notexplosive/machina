using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    class MouseTracker
    {
        public List<MouseButton> Released
        {
            get;
            private set;
        }
        public List<MouseButton> Pressed
        {
            get;
            private set;
        }

        public Vector2 PositionDelta
        {
            get; private set;
        }
        public Point CurrentPosition
        {
            get; private set;
        }
        private MouseState oldState;
        private bool firstFrame = true;

        public void Calculate()
        {
            var oldMouseButtons = new ButtonState[3] {
                this.oldState.LeftButton,
                this.oldState.MiddleButton,
                this.oldState.RightButton,
            };

            var mouseState = Mouse.GetState();
            var currentButtons = new ButtonState[3] {
                mouseState.LeftButton,
                mouseState.MiddleButton,
                mouseState.RightButton,
            };

            var pressedThisFrame = new List<MouseButton>();
            var releasedThisFrame = new List<MouseButton>();

            for (int i = 0; i < 3; i++)
            {
                if (currentButtons[i] != oldMouseButtons[i])
                {
                    if (currentButtons[i] == ButtonState.Pressed)
                    {
                        pressedThisFrame.Add((MouseButton) i);
                    }

                    if (currentButtons[i] == ButtonState.Released)
                    {
                        releasedThisFrame.Add((MouseButton) i);
                    }
                }
            }

            this.Released = releasedThisFrame;
            this.Pressed = pressedThisFrame;
            this.CurrentPosition = mouseState.Position;

            if (!this.firstFrame)
            {
                // We hide this on the first frame because game will launch with a huge mouse delta otherwise
                this.PositionDelta = (oldState.Position - mouseState.Position).ToVector2();
            }

            this.oldState = mouseState;
            this.firstFrame = false;
        }
    }
}
