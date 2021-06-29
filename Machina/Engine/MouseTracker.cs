using Machina.Data;
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

    public class MouseTracker
    {
        private int previousScroll;
        private MouseState oldState;
        private bool firstFrame = true;

        public MouseFrameState CalculateFrameState(MouseState currentState)
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

            var pressedThisFrameList = new List<MouseButton>();
            var releasedThisFrameList = new List<MouseButton>();

            bool leftPressed = false;
            bool middlePressed = false;
            bool rightPressed = false;

            bool leftReleased = false;
            bool middleReleased = false;
            bool rightReleased = false;


            for (int i = 0; i < 3; i++)
            {
                if (currentButtons[i] != oldMouseButtons[i])
                {
                    var mouseButton = (MouseButton) i;

                    if (currentButtons[i] == ButtonState.Pressed)
                    {

                        if (mouseButton == MouseButton.Left)
                        {
                            leftPressed = true;
                        }

                        if (mouseButton == MouseButton.Middle)
                        {
                            middlePressed = true;
                        }

                        if (mouseButton == MouseButton.Right)
                        {
                            rightPressed = true;
                        }
                    }

                    if (currentButtons[i] == ButtonState.Released)
                    {
                        if (mouseButton == MouseButton.Left)
                        {
                            leftReleased = true;
                        }

                        if (mouseButton == MouseButton.Middle)
                        {
                            middleReleased = true;
                        }

                        if (mouseButton == MouseButton.Right)
                        {
                            rightReleased = true;
                        }
                    }
                }
            }

            var pressedThisFrame = new MouseButtonList(leftPressed, middlePressed, rightPressed);
            var releasedThisFrame = new MouseButtonList(leftReleased, middleReleased, rightReleased);

            Vector2 positionDelta = Vector2.Zero;

            if (!this.firstFrame)
            {
                // We hide this on the first frame because game will launch with a huge mouse delta otherwise
                positionDelta = (currentState.Position - oldState.Position).ToVector2();
            }

            var currentScroll = currentState.ScrollWheelValue;
            var scrollDelta = (currentScroll - this.previousScroll) / 120;
            this.previousScroll = currentScroll;


            var frameState = new MouseFrameState(pressedThisFrame, releasedThisFrame, currentState.Position, positionDelta, scrollDelta);

            this.oldState = currentState;
            this.firstFrame = false;

            return frameState;
        }
    }
}
