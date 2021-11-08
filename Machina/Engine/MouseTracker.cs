using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        private bool firstFrame = true;
        private MouseState oldState;
        private int previousScroll;

        public MouseFrameState CalculateFrameState(MouseState currentState)
        {
            var oldMouseButtons = new ButtonState[3]
            {
                this.oldState.LeftButton,
                this.oldState.MiddleButton,
                this.oldState.RightButton
            };

            var currentButtons = new ButtonState[3]
            {
                currentState.LeftButton,
                currentState.MiddleButton,
                currentState.RightButton
            };

            var leftPressed = false;
            var middlePressed = false;
            var rightPressed = false;

            var leftReleased = false;
            var middleReleased = false;
            var rightReleased = false;

            for (var i = 0; i < 3; i++)
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

            var positionDelta = Vector2.Zero;

            if (!this.firstFrame)
            {
                // We hide this on the first frame because game will launch with a huge mouse delta otherwise
                positionDelta = (currentState.Position - this.oldState.Position).ToVector2();
            }

            var currentScroll = currentState.ScrollWheelValue;
            var scrollDelta = (currentScroll - this.previousScroll) / 120;
            this.previousScroll = currentScroll;

            var frameState = new MouseFrameState(pressedThisFrame, releasedThisFrame, currentState.Position,
                positionDelta, scrollDelta);

            this.oldState = currentState;
            this.firstFrame = false;

            return frameState;
        }
    }
}