using Machina.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Input
{
    public class MachinaInput
    {
        public readonly KeyTracker keyTracker = new KeyTracker();
        public readonly MouseTracker mouseTracker = new MouseTracker();
        public readonly SingleFingerTouchTracker touchTracker = new SingleFingerTouchTracker();

        public InputFrameState GetHumanFrameState()
        {
            var inputState = InputState.RawHumanInput;

            MouseFrameState mouseFrameState;

            if (GamePlatform.IsMobile)
            {
                mouseFrameState = this.touchTracker.CalculateFrameState(inputState.touches);
            }
            else
            {
                mouseFrameState = this.mouseTracker.CalculateFrameState(inputState.mouseState);
            }

            return new InputFrameState(
                this.keyTracker.CalculateFrameState(inputState.keyboardState, inputState.gamepadState),
                mouseFrameState);
        }
    }
}
