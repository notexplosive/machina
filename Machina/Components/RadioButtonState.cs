using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    interface ICheckboxStateProvider
    {
        bool GetIsChecked();
    }

    class RadioButtonState : BaseComponent, ICheckboxStateProvider
    {
        private readonly Clickable clickable;
        private readonly RadioButtonGroup group;
        private readonly bool startFilled;
        private bool isFilled;
        public bool IsFilled
        {
            get => isFilled; set
            {
                isFilled = value;
                OnStateChange(value);
            }
        }

        public RadioButtonState(Actor actor, RadioButtonGroup radioButtonGroup, bool startFilled) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            clickable.OnClick += OnClick;
            this.group = radioButtonGroup;

            this.startFilled = startFilled;
        }

        public override void Start()
        {
            if (this.startFilled)
            {
                this.IsFilled = true;
            }
        }

        public override void OnDeleteFinished()
        {
            clickable.OnClick -= OnClick;
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

        public bool GetIsChecked()
        {
            return this.IsFilled;
        }
    }
}
