using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Machina.Engine;

namespace Machina.Components
{
    class BoundingRect : DataComponent
    {
        private Point size;
        private Vector2 offset;

        public BoundingRect(Actor actor, Point size, Vector2 offset) : base(actor)
        {
            this.size = size;
            this.offset = offset;
        }
        public BoundingRect(Actor actor, Point size) : this(actor, size, new Vector2(size.X, size.Y) / 2)
        {
        }

        public BoundingRect(Actor actor, int width, int height) : this(actor, new Point(width, height), new Vector2(width, height) / 2) { }

        public int Width
        {
            get
            {
                return this.size.X;
            }
            set
            {
                this.size.X = value;
            }
        }
        public int Height
        {
            get
            {
                return this.size.Y;
            }
            set
            {
                this.size.Y = value;
            }
        }
        public Rectangle Rect => new Rectangle((this.actor.position - offset).ToPoint(), this.size);

        public void SetOffsetToCenter()
        {
            this.offset.X = Width / 2;
            this.offset.Y = Height / 2;
        }

        /*
        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(Rect, Color.Pink);
        }
        */
    }
}
