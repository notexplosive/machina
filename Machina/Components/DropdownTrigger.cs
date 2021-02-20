using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class DropdownTrigger : BaseComponent
    {
        private DropdownContent.DropdownItem selectedItem;
        private readonly Clickable clickable;
        private readonly DropdownContent content;
        private readonly BoundedTextRenderer textRenderer;
        private readonly IFrameAnimation frames;
        private readonly SpriteSheet spriteSheet;
        private readonly NinepatchSheet backgroundSheet;
        private readonly BoundingRect boundingRect;

        private bool Deployed => this.content.actor.Visible;

        public DropdownTrigger(Actor actor, DropdownContent content, SpriteSheet spriteSheet, IFrameAnimation frames, NinepatchSheet backgroundSheet) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.clickable.onClick += OnClick;
            this.content = content;
            this.content.onOptionSelect += OnOptionSelected;
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.selectedItem = content.FirstItem;
            this.textRenderer.Text = " " + this.selectedItem.text; // awkward space character
            this.frames = frames;
            this.spriteSheet = spriteSheet;
            this.backgroundSheet = backgroundSheet;
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int frameIndex = 2;
            if (!Deployed)
            {
                if (this.clickable.IsHovered)
                {
                    frameIndex = 1;

                    if (this.clickable.IsPrimedForLeftMouseButton)
                    {
                        frameIndex = 2;
                    }
                }
                else
                {
                    frameIndex = 0;
                }
            }

            var rect = this.boundingRect.Rect;
            this.backgroundSheet.DrawFullNinepatch(spriteBatch, rect, NinepatchSheet.GenerationDirection.Inner, transform.Depth + 1);

            var drawPos = new Vector2(rect.Right, this.transform.Position.Y) + new Vector2(-rect.Height / 2, rect.Height / 2);
            this.spriteSheet.DrawFrame(spriteBatch, this.frames.GetFrame(frameIndex), drawPos, 1f, 0f, new PointBool(false, false), transform.Depth - 1, Color.White);
        }

        private void OnOptionSelected(DropdownContent.DropdownItem item)
        {
            this.selectedItem = item;
            this.textRenderer.Text = " " + this.selectedItem.text; // awkward space character
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
