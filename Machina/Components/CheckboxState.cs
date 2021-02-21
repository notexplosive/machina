using Machina.Data;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class CheckboxState : BaseComponent, ICheckboxStateProvider, UIState<bool>
    {
        private readonly Clickable clickable;
        public bool IsChecked
        {
            get; set;
        }

        public CheckboxState(Actor actor, bool startingValue = false) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            clickable.onClick += OnClick;
            this.IsChecked = startingValue;
        }

        public override void OnDelete()
        {
            clickable.onClick -= OnClick;
        }

        public void OnClick(MouseButton mouseButton)
        {
            if (mouseButton == MouseButton.Left)
            {
                IsChecked = !IsChecked;
            }
        }

        public bool GetIsChecked()
        {
            return IsChecked;
        }

        public bool GetState()
        {
            return IsChecked;
        }
    }
}
