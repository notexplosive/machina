using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using Machina.Engine;

namespace Machina.Components
{
    public class BoundingRect : BaseComponent
    {
        private Point size;
        public Vector2 Offset
        {
            get; private set;
        }
        public Point Size => this.size;

        public BoundingRect SetSize(Point size)
        {
            this.size = size;
            onSizeChange?.Invoke(size);
            return this;
        }

        public int Area => this.Width * this.Height;

        public Point Location => Rect.Location;

        public Action<Point> onSizeChange;

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
                this.onSizeChange?.Invoke(new Point(value, this.size.Y));
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
                this.onSizeChange?.Invoke(new Point(this.size.X, value));
                this.size.Y = value;
            }
        }
        public Rectangle Rect => new Rectangle((this.actor.transform.Position - Offset).ToPoint(), this.size);
        public RectangleF RectF => new RectangleF(this.actor.transform.Position - Offset, this.size);


        public Vector2 NormalizedOffset
        {
            get
            {
                return this.Offset;
            }
        }

        public Vector2 TopLeft => Rect.Location.ToVector2();

        public BoundingRect SetOffsetToTopLeft()
        {
            Offset = new Vector2(0, 0);
            return this;
        }

        public BoundingRect SetOffsetToCenter()
        {
            Offset = new Vector2(Width / 2, Height / 2);
            return this;
        }

        public BoundingRect CenterToBounds()
        {
            var offsetAmount = new Vector2(Width / 2, Height / 2) - Offset;
            Offset += offsetAmount;
            transform.Position += offsetAmount;
            return this;
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(RectF, Color.Pink, 1f, transform.Depth);
        }

        public BoundingRect SetOffset(Vector2 point)
        {
            Offset = point;
            return this;
        }
    }
}
