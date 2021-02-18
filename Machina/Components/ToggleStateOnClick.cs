using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ToggleStateOnClick : BaseComponent
    {
        private readonly Clickable clickable;
        public bool IsChecked
        {
            get; set;
        }

        public ToggleStateOnClick(Actor actor, bool startingValue = false) : base(actor)
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
    }
}
