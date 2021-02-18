using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class RadioButtonGroup : BaseComponent
    {
        private LayoutGroup layoutGroup;
        private int currentIndex = -1;

        public RadioButtonGroup(Actor actor) : base(actor)
        {
            this.layoutGroup = RequireComponent<LayoutGroup>();
        }

        public void OnElementStateChange(RadioButtonState radioButtonState, bool newState)
        {
            if (newState == true)
            {
                int i = 0;
                foreach (var element in layoutGroup.GetAllElements())
                {
                    if (element.actor == radioButtonState.actor)
                    {
                        this.currentIndex = i;
                    }
                    else
                    {
                        var radioState = element.actor.GetComponent<RadioButtonState>();
                        if (radioState != null)
                        {
                            radioState.IsFilled = false;
                        }
                    }
                    i++;
                }
            }
        }
    }
}
