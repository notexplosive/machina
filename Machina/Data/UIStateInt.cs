using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    [Serializable]
    public class UIStateInt : UIState<int>
    {
        private int value;

        public UIStateInt(int startingValue)
        {
            this.value = startingValue;
        }

        public int State
        {
            get => this.value; set => this.value = value;
        }
    }
}
