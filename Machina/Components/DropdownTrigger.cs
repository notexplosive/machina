using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class DropdownTrigger : BaseComponent
    {
        private Clickable clickable;
        private DropdownContent content;
        private DropdownContent.DropdownItem selectedItem;
        private BoundedTextRenderer textRenderer;

        public DropdownTrigger(Actor actor, DropdownContent content) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.onClick += OnClick;
            this.content = content;
            this.content.onOptionSelect += OnOptionSelected;
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.selectedItem = content.FirstItem;
            this.textRenderer.Text = this.selectedItem.text;

        }

        private void OnOptionSelected(DropdownContent.DropdownItem item)
        {
            this.selectedItem = item;
            this.textRenderer.Text = this.selectedItem.text;
        }

        public override void OnDelete()
        {
            this.clickable.onClick -= OnClick;
            this.content.onOptionSelect -= OnOptionSelected;
        }

        private void OnClick(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                if (this.content.actor.Visible)
                {
                    this.content.Hide();
                }
                else
                {
                    this.content.Show();
                }
            }
        }
    }
}
