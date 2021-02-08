using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    class ScrollTracker
    {
        private int previousScroll;
        public int CalculateDelta()
        {
            var currentScroll = Mouse.GetState().ScrollWheelValue;
            var scrollDelta = currentScroll - this.previousScroll;
            this.previousScroll = currentScroll;
            return scrollDelta / 120;
        }
    }
}
