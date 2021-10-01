using System;
using Machina.Engine;

namespace Machina.Components
{
    public class SingleSelectable : BaseComponent
    {
        private readonly Clickable clickable;
        private readonly SingleSelector selector;
        public Action onDeselect;
        public Action onSelect;

        public SingleSelectable(Actor actor, SingleSelector selector) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += OnClick;
            this.selector = selector;

            this.onSelect += () => { };
            this.onDeselect += () => { };
        }

        public override void OnDeleteFinished()
        {
            this.clickable.OnClick -= OnClick;
        }

        private void OnClick(MouseButton obj)
        {
            this.selector.Select(this);
        }

        public bool IsSelected()
        {
            return this.selector.Selected == this;
        }
    }
}
