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
        public void Calculate(MouseState currentState)
        {
            var currentScroll = currentState.ScrollWheelValue;
            this.ScrollDelta = (currentScroll - this.previousScroll) / 120;
            this.previousScroll = currentScroll;
        }
    }
}
