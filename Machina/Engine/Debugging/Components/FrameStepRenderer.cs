using Machina.Engine;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    internal class FrameStepRenderer : BaseComponent
    {
        private readonly IFrameStep frameStep;
        private readonly SceneLayers sceneLayers;
        private int stepCount;

        public FrameStepRenderer(Actor actor, IFrameStep frameStep, SceneLayers sceneLayers, InvokableDebugTool tool) :
            base(actor)
        {
            tool.onToolToggle += OnToggle;
            this.frameStep = frameStep;
            this.sceneLayers = sceneLayers;

            this.actor.transform.Position = new Vector2(-64, 32);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var radius = 32;
            var center = this.actor.transform.Position.ToPoint() + new Point(radius, radius);
            spriteBatch.DrawCircle(new CircleF(center, radius), 25, Color.White, radius, 0.55f);
            spriteBatch.DrawCircle(new CircleF(center, radius), 25, Color.Black, 2, 0.51f);
            var point = new Angle(-this.stepCount / 60f, AngleType.Revolution).ToVector(radius * 0.8f);
            spriteBatch.DrawLine(center.ToVector2(), center.ToVector2() + point, Color.Black, 2, 0.51f);
        }

        public override void OnScroll(int scrollDelta)
        {
            if (scrollDelta < 0)
            {
                Step();
            }
        }

        public void OnToggle(bool isPaused)
        {
            if (isPaused)
            {
                this.stepCount = 0;
            }

            this.frameStep.IsPaused = isPaused;
        }

        public void Step()
        {
            if (this.frameStep.IsPaused)
            {
                foreach (var scene in this.sceneLayers.AllScenes())
                {
                    scene.Step();
                }

                this.stepCount++;
            }
        }
    }
}