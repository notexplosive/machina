using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System.Text;

namespace Machina.Data.Layout
{
    public class Element : IElement
    {
        private Point size;

        private bool isStretchedVertically = false;
        private bool isStretchedHorizontally = false;

        public IElement StretchVertically()
        {
            this.isStretchedVertically = true;
            return this;
        }

        public IElement StretchHorizontally()
        {
            this.isStretchedHorizontally = true;
            return this;
        }

        public bool IsStretchedAlong(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return this.isStretchedHorizontally;
            }

            if (orientation == Orientation.Vertical)
            {
                return this.isStretchedVertically;
            }

            return false;
        }

        public bool IsStretchPerpendicular(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return this.isStretchedVertically;
            }

            if (orientation == Orientation.Vertical)
            {
                return this.isStretchedHorizontally;
            }

            return false;
        }

        public Point Size { get => this.size; set => this.size = value; }
        public Point Offset { get; set; }
        public Point Position { get; set; }
        public Rectangle Rect => new Rectangle(Position - Offset, Size);
        public IElement SetHeight(int height)
        {
            this.size.Y = height;
            return this;
        }

        public IElement SetWidth(int width)
        {
            this.size.X = width;
            return this;
        }
    }
}
