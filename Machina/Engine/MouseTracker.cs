using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    class MouseTracker
    {
        public List<MouseButton> ButtonsReleasedThisFrame
        {
            get;
            private set;
        }
        public List<MouseButton> ButtonsPressedThisFrame
        {
            get;
            private set;
        }

        public Vector2 PositionDelta
        {
            get; private set;
        }
        public Point RawWindowPosition
        {
            get; private set;
        }
        private MouseState oldState;
        private bool firstFrame = true;

        public void Calculate(MouseState currentState)
        {
            var oldMouseButtons = new ButtonState[3] {
                this.oldState.LeftButton,
                this.oldState.MiddleButton,
                this.oldState.RightButton,
            };

            var currentButtons = new ButtonState[3] {
                currentState.LeftButton,
                currentState.MiddleButton,
                currentState.RightButton,
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

            ButtonsReleasedThisFrame = releasedThisFrame;
            ButtonsPressedThisFrame = pressedThisFrame;
            RawWindowPosition = currentState.Position;

            if (!this.firstFrame)
            {
                // We hide this on the first frame because game will launch with a huge mouse delta otherwise
                PositionDelta = (currentState.Position - oldState.Position).ToVector2();
            }

            this.oldState = currentState;
            this.firstFrame = false;
        }
    }
}
