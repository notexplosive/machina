using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public interface IFrameStep
    {
        public bool IsPaused
        {
            get; set;
        }

        public void Step(Scene[] scene);
        void Draw(SpriteBatch spriteBatch);
    }

    public class EmptyFrameStep : IFrameStep
    {
        public bool IsPaused
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public void Step(Scene[] scene)
        {
            // No op
        }
    }

    public class FrameStep : IFrameStep
    {
        private int stepCount;
        private bool isPaused;

        public bool IsPaused
        {
            get => this.isPaused;
            set
            {
                this.isPaused = value;
                this.stepCount = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var radius = 32;
            var center = new Point(radius, radius);
            spriteBatch.DrawCircle(new CircleF(center, radius), 25, Color.White, radius, 0.00000005f);
            spriteBatch.DrawCircle(new CircleF(center, radius), 25, Color.Black, 2, 0.00000001f);
            var point = new Angle(-this.stepCount / 60f, AngleType.Revolution).ToVector(radius * 0.8f);
            spriteBatch.DrawLine(center.ToVector2(), center.ToVector2() + point, Color.Black, 2, 0.00000001f);
        }

        public void Step(Scene[] scenes)
        {
            foreach (var scene in scenes)
            {
                scene.Update(1f / 60f);
            }
            this.stepCount++;
        }
    }
}
