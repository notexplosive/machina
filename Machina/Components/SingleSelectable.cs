using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class SingleSelectable : BaseComponent
    {
        public Action onSelect;
        public Action onDeselect;
        private readonly Clickable clickable;
        private readonly SingleSelector selector;

        public SingleSelectable(Actor actor, SingleSelector selector) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += OnClick;
            this.selector = selector;

            onSelect += () => { };
            onDeselect += () => { };
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
