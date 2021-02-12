using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class SimpleButtonRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Clickable clickable;

        public SimpleButtonRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.clickable = RequireComponent<Clickable>();
            this.clickable.onClick += OnClick;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, this.clickable.IsHovered ? Color.Blue : Color.Red, this.clickable.IsHeldDown ? 5 : 1);
        }

        public void OnClick(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                MachinaGame.Print("Button Clicked!");
            }
        }
    }
}
