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
                    // Hit! We have a current touch and a prev touch
                    var touchUp = currTouch.State == TouchLocationState.Released && prevTouch.State == TouchLocationState.Pressed;
                    result = new SingleTouchFrameState(false, touchUp, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                }
                else
                {
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
