using System;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    internal class InvokableDebugTool : BaseComponent
    {
        private readonly TweenChain enterTweenChain;
        private readonly TweenChain exitTweenChain;
        private readonly KeyCombination invokingKeyCombo;
        private TweenChain currentTweenChain;
        public Action<bool> onToolToggle;
        private bool toolActive;

        public InvokableDebugTool(Actor actor, KeyCombination invokingKeyCombo) : base(actor)
        {
            this.invokingKeyCombo = invokingKeyCombo;

            this.enterTweenChain = new TweenChain()
                    .AppendCallback(() => { this.actor.transform.Position = new Vector2(-64, 32); })
                    .AppendPositionTween(this.actor, new Vector2(32, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                    .AppendCallback(() => { this.currentTweenChain = null; })
                ;

            this.exitTweenChain = new TweenChain()
                    .AppendPositionTween(this.actor, new Vector2(0, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                    .AppendPositionTween(this.actor, new Vector2(-512, 32), 0.25f, EaseFuncs.QuinticEaseOut)
                    .AppendCallback(() => { this.currentTweenChain = null; })
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
            if (this.invokingKeyCombo.Match(key, state, modifiers) && MachinaClient.Runtime.DebugLevel >= DebugLevel.Passive)
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
            this.currentTweenChain.Refresh();
        }

        public void Open()
        {
            this.toolActive = true;
            this.onToolToggle?.Invoke(this.toolActive);
            this.currentTweenChain = this.enterTweenChain;
            this.currentTweenChain.Refresh();
        }
    }
}