using System;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    public class DropdownContent : BaseComponent
    {
        private readonly NinepatchSheet backgroundSheet;
        private readonly BoundingRect boundingRect;
        private readonly SpriteFont font;
        private readonly Hoverable hoverable;
        private readonly NinepatchSheet hoverSheet;
        private readonly List<DropdownItem> items = new List<DropdownItem>();
        private readonly int margin;
        private readonly BoundingRect triggerBoundingRect;
        private int hoveredIndex;
        public Action<DropdownItem> onOptionSelect;
        private Point totalRectSize;

        public DropdownContent(Actor actor, SpriteFont font, NinepatchSheet backgroundSheet,
            NinepatchSheet hoverSheet) : base(actor)
        {
            this.actor.Visible = false;
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.triggerBoundingRect = this.actor.transform.Parent.actor.GetComponent<BoundingRect>();
            this.totalRectSize = new Point(this.triggerBoundingRect.Size.X, 0);
            this.backgroundSheet = backgroundSheet;
            this.hoverSheet = hoverSheet;
            this.margin = 7;

            this.font = font;
        }

        public DropdownItem FirstItem => this.items[0];

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            var index = CalculateIndexOfHoverPosition(currentPosition);
            this.hoveredIndex = index;
        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (buttonState == ButtonState.Released && button == MouseButton.Left)
            {
                var index = CalculateIndexOfHoverPosition(currentPosition);
                if (index >= 0 && index < this.items.Count)
                {
                    this.onOptionSelect?.Invoke(this.items[index]);
                }

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
                var bgRect = new Rectangle(this.boundingRect.TopLeft.ToPoint(), this.totalRectSize);
                this.backgroundSheet.DrawFullNinepatch(spriteBatch, bgRect, NinepatchSheet.GenerationDirection.Inner,
                    transform.Depth - 1);

                if (this.hoveredIndex != -1 && this.items[this.hoveredIndex].Equals(item))
                {
                    // Hover
                    var hoverRect = new Rectangle(position.ToPoint(),
                        new Point(this.totalRectSize.X, this.font.LineSpacing));
                    this.hoverSheet.DrawFullNinepatch(spriteBatch, hoverRect, NinepatchSheet.GenerationDirection.Inner,
                        transform.Depth - 2);
                }

                spriteBatch.DrawString(this.font, item.text, position + new Vector2(this.margin, 0), Color.White, 0f,
                    Vector2.Zero, 1f, SpriteEffects.None, (transform.Depth - 3).AsFloat);
                position.Y += this.font.LineSpacing;
            }
        }

        public void Show()
        {
            this.actor.Visible = true;
            this.boundingRect.Width = this.triggerBoundingRect.Width;
            this.boundingRect.Height = this.items.Count * this.font.LineSpacing;
            this.actor.transform.LocalPosition = new Vector2(0, this.triggerBoundingRect.Height);

            this.totalRectSize = new Point(this.triggerBoundingRect.Width, 0);
            foreach (var item in this.items)
            {
                var textSize = this.font.MeasureString(item.text) + new Vector2(this.margin * 2, 0);
                if (textSize.X > this.totalRectSize.X)
                {
                    this.totalRectSize.X = (int) textSize.X;
                }

                this.totalRectSize.Y += (int) textSize.Y;
            }

            this.boundingRect.Width = this.totalRectSize.X;
        }

        public void Hide()
        {
            this.actor.Visible = false;
        }

        public DropdownContent Add(string text)
        {
            var item = new DropdownItem();
            item.text = text;
            return Add(item);
        }

        public DropdownContent Add(DropdownItem item)
        {
            this.items.Add(item);

            return this;
        }

        public struct DropdownItem
        {
            public string text;

            public DropdownItem(string text)
            {
                this.text = text;
            }
        }
    }
}