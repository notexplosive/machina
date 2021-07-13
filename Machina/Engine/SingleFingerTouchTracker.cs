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
        private int? currentMainTouchId;
        private bool touchCommitted = false;

        public MouseFrameState CalculateFrameState(TouchCollection currentTouches)
        {
            SingleTouchFrameState result = new SingleTouchFrameState();


            if (this.currentMainTouchId.HasValue)
            {
                currentTouches.FindById(this.currentMainTouchId.Value, out TouchLocation currTouch);
                if (this.prevTouches.FindById(currTouch.Id, out TouchLocation prevTouch))
                {
                    if (!this.touchCommitted)
                    {
                        result = new SingleTouchFrameState(true, false, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                        this.touchCommitted = true;
                    }
                    else
                    {
                        result = new SingleTouchFrameState(false, false, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                    }
                }
                else
                {
                    result = new SingleTouchFrameState(false, true, currTouch.Position.ToPoint(), currTouch.Position - prevTouch.Position);
                    this.currentMainTouchId = null;
                    this.touchCommitted = false;
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
                        // Pick a current touch that hasn't been seen before, they are now our main touch
                        this.currentMainTouchId = currentTouch.Id;
                        this.touchCommitted = false;
                        result = new SingleTouchFrameState(false, false, currentTouch.Position.ToPoint(), Vector2.Zero);
                        break;
                    }
                }
            }

            this.prevTouches = currentTouches;

            return result;
        }
    }
}
