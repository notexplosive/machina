using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class InvokableDebugTool : BaseComponent
    {
        public Action<bool> onToolToggle;
        private readonly KeyCombination invokingKeyCombo;
        private readonly TweenChain enterTweenChain;
        private readonly TweenChain exitTweenChain;
        private TweenChain currentTweenChain;
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
            if (this.invokingKeyCombo.Match(key, state, modifiers) && MachinaGame.DebugLevel >= DebugLevel.Passive)
            {
                this.toolActive = !this.toolActive;
                this.onToolToggle?.Invoke(this.toolActive);

                if (this.toolActive)
                {
                    this.currentTweenChain = enterTweenChain;
                }
                else
                {
                    this.currentTweenChain = exitTweenChain;
                }
                this.currentTweenChain.Refresh();
            }
        }
    }
}
