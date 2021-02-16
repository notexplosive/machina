using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class FrameStepRenderer : BaseComponent
    {
        private readonly IFrameStep frameStep;
        private readonly SceneLayers sceneLayers;
        private readonly TweenChain enterTweenChain;
        private readonly TweenChain exitTweenChain;
        private TweenChain currentTweenChain;
        private int stepCount;

        public FrameStepRenderer(Actor actor, IFrameStep frameStep, SceneLayers sceneLayers) : base(actor)
        {
            this.frameStep = frameStep;
            this.sceneLayers = sceneLayers;
            this.enterTweenChain = new TweenChain()
                .AppendCallback(() => { this.actor.transform.Position = new Vector2(-64, 32); })
                .AppendPositionTween(this.actor, new Vector2(32, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                .AppendCallback(() => { this.currentTweenChain = null; })
            ;

            this.exitTweenChain = new TweenChain()
                .AppendPositionTween(this.actor, new Vector2(0, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                .AppendPositionTween(this.actor, new Vector2(-64, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                .AppendCallback(() => { this.currentTweenChain = null; })
            ;

            this.currentTweenChain = null;

            this.actor.transform.Position = new Vector2(-64, 32);
        }

        public override void Update(float dt)
        {
            if (this.currentTweenChain != null)
            {
                this.currentTweenChain.Update(dt);
            }
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

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Space && modifiers.control && state == ButtonState.Pressed)
            {
                this.frameStep.IsPaused = !this.frameStep.IsPaused;
                if (this.frameStep.IsPaused)
                {
                    this.currentTweenChain = enterTweenChain;
                }
                else
                {
                    this.currentTweenChain = exitTweenChain;
                }
                this.currentTweenChain.Refresh();
                this.stepCount = 0;
            }
        }

        public void Step()
        {
            if (this.frameStep.IsPaused)
            {
                foreach (var scene in sceneLayers.AllScenes())
                {
                    scene.Step();
                }
                this.stepCount++;
            }
        }
    }
}
