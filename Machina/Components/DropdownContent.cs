using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class DropdownContent : BaseComponent
    {
        public Action<DropdownItem> onOptionSelect;
        private readonly List<DropdownItem> items = new List<DropdownItem>();
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;
        private readonly BoundingRect parentBoundingRect;
        private readonly SpriteFont font;

        public DropdownItem FirstItem => this.items[0];

        public DropdownContent(Actor actor, SpriteFont font) : base(actor)
        {
            this.actor.Visible = false;
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.parentBoundingRect = this.actor.Parent.GetComponent<BoundingRect>();

            this.font = font;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {

        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (buttonState == ButtonState.Pressed && button == MouseButton.Left)
            {
                if (this.hoverable.IsHovered)
                {
                    int index = (int) (currentPosition.Y - transform.Position.Y) / this.font.LineSpacing;
                    if (index >= 0 && index < this.items.Count)
                        onOptionSelect?.Invoke(this.items[index]);
                }
                Hide();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = this.actor.transform.Position;
            foreach (var item in this.items)
            {
                spriteBatch.DrawString(this.font, item.text, position, Color.Black);
                position.Y += this.font.LineSpacing;
            }
        }

        public void Show()
        {
            this.actor.Visible = true;
            this.boundingRect.Width = parentBoundingRect.Width;
            this.boundingRect.Height = items.Count * this.font.LineSpacing;
            this.actor.transform.LocalPosition = new Vector2(0, this.parentBoundingRect.Height);
        }

        public void Hide()
        {
            this.actor.Visible = false;
        }

        public DropdownContent Add(string text)
        {
            var item = new DropdownItem();
            item.text = text;
            this.items.Add(item);
            return this;
        }

        public struct DropdownItem
        {
            public string text;
        }
    }
}
