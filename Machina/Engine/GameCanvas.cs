using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class GameCanvas
    {
        private readonly Point idealSize;

        public GameCanvas(int idealWidth, int idealHeight)
        {
            this.idealSize = new Point(idealWidth, idealHeight);
        }

        public float ScaleFactor
        {
            get
            {
                var normalizedWidth = (float) WindowWidth / this.idealSize.X;
                var normalizedHeight = (float) WindowHeight / this.idealSize.Y;

                return Math.Min(normalizedWidth, normalizedHeight);
            }
        }

        public bool PendingResize
        {
            get;
            private set;
        }
        public int WindowWidth
        {
            get;
            private set;
        }
        public int WindowHeight
        {
            get;
            private set;
        }

        public Point CanvasSize => (new Vector2(this.idealSize.X, this.idealSize.Y) * ScaleFactor).ToPoint();

        public void Resize(int windowWidth, int windowHeight)
        {
            this.PendingResize = true;
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
        }

        public void FinishResize()
        {
            this.PendingResize = false;
        }
    }
}
