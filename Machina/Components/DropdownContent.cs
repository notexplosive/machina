using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private readonly BoundingRect triggerBoundingRect;
        private readonly SpriteFont font;
        private int hoveredIndex;
        private Point totalRectSize;
        private int margin;
        private readonly NinepatchSheet backgroundSheet;
        private readonly NinepatchSheet hoverSheet;

        public DropdownItem FirstItem => this.items[0];

        public DropdownContent(Actor actor, SpriteFont font, NinepatchSheet backgroundSheet, NinepatchSheet hoverSheet) : base(actor)
        {
            this.actor.Visible = false;
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.triggerBoundingRect = this.actor.Parent.GetComponent<BoundingRect>();
            this.totalRectSize = this.boundingRect.Size;
            this.backgroundSheet = backgroundSheet;
            this.hoverSheet = hoverSheet;
            this.margin = 7;

            this.font = font;
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            int index = CalculateIndexOfHoverPosition(currentPosition);
            this.hoveredIndex = index;
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (buttonState == ButtonState.Pressed && button == MouseButton.Left)
            {
                int index = CalculateIndexOfHoverPosition(currentPosition);
                if (index >= 0 && index < this.items.Count)
                    onOptionSelect?.Invoke(this.items[index]);
                Hide();
            }
        }

        private int CalculateIndexOfHoverPosition(Vector2 mousePos)
        {
            var index = (int) (mousePos.Y - transform.Position.Y) / this.font.LineSpacing;
            if (this.hoverable.IsHovered && index >= 0 && index < this.items.Count)
            {
                return index;
            }
            return -1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var position = this.actor.transform.Position;
            foreach (var item in this.items)
            {
                // BG fill
                var bgRect = new Rectangle(this.boundingRect.TopLeft.ToPoint(), totalRectSize);
                this.backgroundSheet.DrawFullNinepatch(spriteBatch, bgRect, transform.Depth + 0.0001f);

                if (hoveredIndex != -1 && items[hoveredIndex].Equals(item))
                {
                    // Hover
                    var hoverRect = new Rectangle(position.ToPoint(), new Point(this.totalRectSize.X, this.font.LineSpacing));
                    this.hoverSheet.DrawFullNinepatch(spriteBatch, hoverRect, transform.Depth + 0.00001f);
                }
                spriteBatch.DrawString(this.font, item.text, position + new Vector2(this.margin, 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, transform.Depth - 0.00001f);
                position.Y += this.font.LineSpacing;
            }
        }

        public void Show()
        {
            this.actor.Visible = true;
            this.boundingRect.Width = triggerBoundingRect.Width;
            this.boundingRect.Height = items.Count * this.font.LineSpacing;
            this.actor.transform.LocalPosition = new Vector2(0, this.triggerBoundingRect.Height);
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
            var textSize = this.font.MeasureString(item.text) + new Vector2(this.margin * 2, 0);

            if (textSize.X > this.totalRectSize.X)
            {
                totalRectSize.X = (int) textSize.X;
            }

            this.totalRectSize.Y += (int) textSize.Y;

            return this;
        }

        public struct DropdownItem
        {
            public string text;
        }
    }
}
