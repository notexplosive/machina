using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class VerticalCameraPanOnScroll : BaseComponent
    {
        public readonly MinMax<int> scrollRange;
        private readonly int scrollIncrement;

        public int CurrentScroll
        {
            get; private set;
        }

        public VerticalCameraPanOnScroll(Actor actor, MinMax<int> scrollRange, int scrollIncrement = 24) : base(actor)
        {
            CurrentScroll = 0;
            this.scrollRange = scrollRange;
            this.scrollIncrement = scrollIncrement;
        }

        public override void Update(float dt)
        {
            this.actor.scene.camera.Position = new Vector2(this.actor.scene.camera.Position.X, this.CurrentScroll);
        }

        public override void OnScroll(int scrollDelta)
        {
            CurrentScroll -= scrollDelta * this.scrollIncrement;
            CurrentScroll = Math.Clamp(CurrentScroll, this.scrollRange.min, this.scrollRange.max);
        }
    }
}
