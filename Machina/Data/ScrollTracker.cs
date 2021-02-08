using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    class ScrollTracker
    {
        private int scrollDelta;
        private int previousScroll;
        public void Update()
        {
            var currentScroll = Mouse.GetState().ScrollWheelValue;
            this.scrollDelta = currentScroll - this.previousScroll;
            this.previousScroll = currentScroll;
        }

        public int ScrollDelta => this.scrollDelta / 120;
    }
}
