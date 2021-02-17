using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Machina.Engine;

namespace Machina.Components
{
    class BoundingRect : BaseComponent
    {
        private Point size;
        public Vector2 Offset
        {
            get; private set;
        }

        public Point Location => Rect.Location;

        public BoundingRect(Actor actor, Point size, Vector2 offset) : base(actor)
        {
            this.size = size;
            this.Offset = offset;
        }
        public BoundingRect(Actor actor, Point size) : this(actor, size, new Vector2(0, 0))
        {
        }

        public BoundingRect(Actor actor, int width, int height) : this(actor, new Point(width, height), new Vector2(0, 0)) { }

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
        public Rectangle Rect => new Rectangle((this.actor.transform.Position - Offset).ToPoint(), this.size);

        public Vector2 NormalizedCenter
        {
            get
            {
                return this.Offset;
            }
        }

        public Vector2 TopLeft => Rect.Location.ToVector2();

        public void SetOffsetToTopLeft()
        {
            Offset = new Vector2(0, 0);
        }

        public void SetOffsetToCenter()
        {
            Offset = new Vector2(Width / 2, Height / 2);
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(Rect, Color.Pink);
        }
    }
}
