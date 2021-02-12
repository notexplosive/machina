using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class PanCameraOnScroll : BaseComponent
    {
        public readonly MinMax<int> worldBounds;
        private readonly int scrollIncrement;
        private float currentScroll;

        public float CurrentScroll
        {
            get => this.currentScroll;
            set
            {
                this.currentScroll = Math.Clamp(value, this.worldBounds.min, this.worldBounds.max);
            }
        }

        public float CurrentScrollPercent
        {
            get => (this.currentScroll - this.worldBounds.min) / this.TotalDistanceUnits;
            set
            {
                CurrentScroll = value * this.TotalDistanceUnits;
            }
        }

        public int TotalDistanceUnits => this.worldBounds.max - this.worldBounds.min;

        public PanCameraOnScroll(Actor actor, MinMax<int> scrollRange, int scrollIncrement = 24) : base(actor)
        {
            CurrentScroll = 0;
            this.worldBounds = scrollRange;
            this.scrollIncrement = scrollIncrement;
        }

        public override void Update(float dt)
        {
            this.actor.scene.camera.Position = new Vector2(this.actor.scene.camera.Position.X, CurrentScroll);
        }

        public override void OnScroll(int scrollDelta)
        {
            CurrentScroll -= (int) (scrollDelta * this.scrollIncrement * this.actor.scene.camera.Zoom);
        }
    }
}
