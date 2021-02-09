using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class ResizeStatus
    {
        public readonly int idealWidth;
        public readonly int idealHeight;

        public ResizeStatus(int idealWidth, int idealHeight)
        {
            this.idealWidth = idealWidth;
            this.idealHeight = idealHeight;
        }

        public float ScaleFactor
        {
            get
            {
                var normalizedWidth = (float) Width / this.idealWidth;
                var normalizedHeight = (float) Height / this.idealHeight;

                return Math.Min(normalizedWidth, normalizedHeight);
            }
        }

        public bool Pending
        {
            get;
            private set;
        }
        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }

        public void Resize(int width, int height)
        {
            this.Pending = true;
            this.Width = width;
            this.Height = height;
        }

        public void FinishResize()
        {
            this.Pending = false;
        }
    }
}
