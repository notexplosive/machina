using Machina.Engine;

namespace Machina.Components
{
    internal interface ICheckboxStateProvider
    {
        bool GetIsChecked();
    }

    internal class RadioButtonState : BaseComponent, ICheckboxStateProvider
    {
        private readonly Clickable clickable;
        private readonly RadioButtonGroup group;
        private readonly bool startFilled;
        private bool isFilled;

        public RadioButtonState(Actor actor, RadioButtonGroup radioButtonGroup, bool startFilled) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += OnClick;
            this.group = radioButtonGroup;

            this.startFilled = startFilled;
        }

        public bool IsFilled
        {
            get => this.isFilled;
            set
            {
                this.isFilled = value;
                OnStateChange(value);
            }
        }

        public bool GetIsChecked()
        {
            return IsFilled;
        }

        public override void Start()
        {
            if (this.startFilled)
            {
                IsFilled = true;
            }
        }

        public override void OnDeleteFinished()
        {
            this.clickable.OnClick -= OnClick;
        }

        public void OnStateChange(bool newState)
        {
            this.group.OnElementStateChange(this, newState);
        }

        public void OnClick(MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                if (IsFilled == false)
                {
                    IsFilled = true;
                }
            }
        }
    }
}