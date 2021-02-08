using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class ResizeStatus
    {
        public readonly int aspectWidth;
        public readonly int aspectHeight;

        public ResizeStatus(int aspectWidth, int aspectHeight)
        {
            this.aspectWidth = aspectWidth;
            this.aspectHeight = aspectHeight;
        }

        public float ScaleFactor
        {
            get
            {
                var normalizedWidth = (float) Width / this.aspectWidth;
                var normalizedHeight = (float) Height / this.aspectHeight;

                return Math.Min(normalizedWidth, normalizedHeight);
            }
        }

        public Rectangle ViewportRect
        {
            get
            {
                return new Rectangle();
            }
        }

        public bool Pending
        {
            get;
            internal set;
        }
        public int Width
        {
            get;
            internal set;
        }
        public int Height
        {
            get;
            internal set;
        }
    }
}
