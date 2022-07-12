using System;
using ExTween;
using ExTween.MonoGame;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    internal class InvokableDebugTool : BaseComponent
    {
        private readonly SequenceTween enterTweenChain;
        private readonly SequenceTween exitTweenChain;
        private readonly KeyCombination invokingKeyCombo;
        private SequenceTween currentTweenChain;
        public Action<bool> onToolToggle;
        private bool toolActive;

        public InvokableDebugTool(Actor actor, KeyCombination invokingKeyCombo) : base(actor)
        {
            this.invokingKeyCombo = invokingKeyCombo;

            var tweenablePosition = new TweenableVector2(
                () => this.actor.transform.Position,
                val => this.actor.transform.Position = val);

            this.enterTweenChain = new SequenceTween()
                    .Add(new CallbackTween(() => { this.actor.transform.Position = new Vector2(-500, 32); }))
                    .Add(new Tween<Vector2>(tweenablePosition,  new Vector2(32, 32), 0.25f, Ease.QuadSlowFast))
                    .Add(new CallbackTween(() => { this.currentTweenChain = null; }))
                ;

            this.exitTweenChain = new SequenceTween()
                    .Add(new Tween<Vector2>(tweenablePosition, new Vector2(0, 32), 0.25f, Ease.QuadFastSlow))
                    .Add(new Tween<Vector2>(tweenablePosition, new Vector2(-512, 32), 0.25f, Ease.QuadFastSlow))
                    .Add(new CallbackTween(() => { this.currentTweenChain = null; }))
                ;

            // start out off screen
            this.actor.transform.Position = new Vector2(-512, 32);

            this.currentTweenChain = null;
        }

        public override void Update(float dt)
        {
            if (this.currentTweenChain != null)
            {
                this.currentTweenChain.Update(dt);
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (this.invokingKeyCombo.Match(key, state, modifiers) && Runtime.DebugLevel >= DebugLevel.Passive)
            {
                if (!this.toolActive)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        public void Close()
        {
            this.toolActive = false;
            this.onToolToggle?.Invoke(this.toolActive);
            this.currentTweenChain = this.exitTweenChain;
            this.currentTweenChain.Reset();
        }

        public void Open()
        {
            this.toolActive = true;
            this.onToolToggle?.Invoke(this.toolActive);
            this.currentTweenChain = this.enterTweenChain;
            this.currentTweenChain.Reset();
        }
    }
}
