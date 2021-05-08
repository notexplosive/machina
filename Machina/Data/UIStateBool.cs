using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    [Serializable]
    public class UIStateBool : UIState<bool>
    {
        private bool value;

        public UIStateBool(bool startingValue)
        {
            this.value = startingValue;
        }

        public bool State
        {
            get => this.value;
            set => this.value = value;
        }
    }
}
