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
        public readonly MinMax<int> bounds;
        private readonly int scrollIncrement;

        public int CurrentScroll
        {
            get; private set;
        }

        public PanCameraOnScroll(Actor actor, MinMax<int> scrollRange, int scrollIncrement = 24) : base(actor)
        {
            CurrentScroll = 0;
            this.bounds = scrollRange;
            this.scrollIncrement = scrollIncrement;
        }

        public override void Update(float dt)
        {
            this.actor.scene.camera.Position = new Vector2(this.actor.scene.camera.Position.X, this.CurrentScroll);
        }

        public override void OnScroll(int scrollDelta)
        {
            CurrentScroll -= (int) (scrollDelta * this.scrollIncrement * this.actor.scene.camera.Zoom);
            CurrentScroll = Math.Clamp(CurrentScroll, this.bounds.min, this.bounds.max);
        }
    }
}
