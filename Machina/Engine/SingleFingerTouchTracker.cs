using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class SingleFingerTouchTracker
    {
        private TouchCollection prevTouches;
        private TouchLocation? currentMainTouch;

        public MouseFrameState CalculateFrameState(TouchCollection touches)
        {
            var currentTouches = touches;
            SingleTouchFrameState result = new SingleTouchFrameState();


            if (this.currentMainTouch.HasValue)
            {
                var currTouch = this.currentMainTouch.Value;
                if (this.prevTouches.FindById(currTouch.Id, out TouchLocation prevTouch))
                {
                    result = new SingleTouchFrameState(false, false, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                }
                else
                {
                    result = new SingleTouchFrameState(false, true, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                    this.currentMainTouch = null;
                }
            }
            else
            {
                var prevToCurrent = new Dictionary<TouchLocation, TouchLocation>();
                foreach (var currentTouch in currentTouches)
                {
                    if (this.prevTouches.FindById(currentTouch.Id, out TouchLocation prevTouch))
                    {
                        prevToCurrent.Add(prevTouch, currentTouch);
                    }
                    else
                    {
                        // Pick a current touch that hasn't been seen before, he's now our main touch
                        this.currentMainTouch = currentTouch;
                        result = new SingleTouchFrameState(true, false, currentTouch.Position.ToPoint(), Vector2.Zero);
                    }
                }
            }

            this.prevTouches = currentTouches;

            return result;
        }
    }
}
