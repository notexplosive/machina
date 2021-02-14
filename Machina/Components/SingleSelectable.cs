﻿using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SingleSelectable : BaseComponent
    {
        public Action onSelect;
        public Action onDeselect;
        private readonly Clickable clickable;
        private readonly SingleSelector selector;

        public SingleSelectable(Actor actor, SingleSelector selector) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.onClick += OnClick;
            this.selector = selector;
        }

        public override void OnDelete()
        {
            this.clickable.onClick -= OnClick;
        }

        private void OnClick(MouseButton obj)
        {
            this.selector.Select(this);
        }
    }
}