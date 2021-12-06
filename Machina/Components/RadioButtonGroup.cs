using Machina.Engine;

namespace Machina.Components
{
    internal class RadioButtonGroup : BaseComponent
    {
        private readonly LayoutGroupComponent layoutGroup;
        private int currentIndex = -1;

        public RadioButtonGroup(Actor actor) : base(actor)
        {
            this.layoutGroup = RequireComponent<LayoutGroupComponent>();
        }

        public Actor CurrentSelectedActor
        {
            get
            {
                if (this.currentIndex > 0)
                {
                    return transform.ChildAt(this.currentIndex);
                }

                return null;
            }
        }

        public void OnElementStateChange(RadioButtonState radioButtonState, bool newState)
        {
            if (newState)
            {
                var i = 0;
                foreach (var element in this.layoutGroup.GetAllElements())
                {
                    if ((element as LayoutElementComponent).actor == radioButtonState.actor)
                    {
                        this.currentIndex = i;
                    }
                    else
                    {
                        var radioState = (element as LayoutElementComponent).actor.GetComponent<RadioButtonState>();
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