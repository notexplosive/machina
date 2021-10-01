using Machina.Data;
using Machina.Engine;

namespace Machina.Components
{
    public class CheckboxState : BaseComponent, ICheckboxStateProvider, UIState<bool>
    {
        private readonly Clickable clickable;

        public CheckboxState(Actor actor, bool startingValue = false) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += OnClick;
            IsChecked = startingValue;
        }

        public bool IsChecked { get; set; }

        public bool GetIsChecked()
        {
            return IsChecked;
        }

        public bool State => IsChecked;

        public override void OnDeleteFinished()
        {
            this.clickable.OnClick -= OnClick;
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
