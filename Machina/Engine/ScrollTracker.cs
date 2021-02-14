using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class ScrollTracker
    {
        public int ScrollDelta
        {
            get; private set;
        }
        private int previousScroll;
        public void Calculate()
        {
            var currentScroll = Mouse.GetState().ScrollWheelValue;
            this.ScrollDelta = currentScroll - this.previousScroll;
            this.previousScroll = currentScroll;

        }
    }
}
