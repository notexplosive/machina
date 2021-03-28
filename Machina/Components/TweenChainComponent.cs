using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class TweenChainComponent : BaseComponent
    {

        TweenChain chain;

        public TweenChainComponent(Actor actor) : base(actor)
        {
            chain = new TweenChain();
        }

        public override void Update(float dt)
        {
            chain.Update(dt);
        }

        public TweenChainComponent AddTween<T>(T target, float duration, EaseFunc easeFunc, TweenAccessors<T> accessors, LerpFunc<T> lerp) where T : struct
        {
            chain.Append(new TweenChain.ChainItem<T>(target, duration, easeFunc, accessors, lerp));
            return this;
        }

        public TweenChainComponent AddMoveTween(Vector2 targetPos, float duration, EaseFunc easeFunc)
        {
            return AddTween(targetPos, duration, easeFunc, this.actor.PositionTweenAccessors(), Vector2.Lerp);
        }

        public TweenChainComponent AddLocalMoveTween(Vector2 targetPos, float duration, EaseFunc easeFunc)
        {
            return AddTween(targetPos, duration, easeFunc, this.actor.LocalPositionTweenAccessors(), Vector2.Lerp);
        }

        public TweenChainComponent AddFloatTween(float targetVal, float duration, EaseFunc easeFunc, TweenAccessors<float> accessors)
        {
            this.chain.AppendFloatTween(targetVal, duration, easeFunc, accessors);
            return this;
        }

        public TweenChainComponent AddWaitTween(float duration)
        {
            this.chain.AppendWaitTween(duration);
            return this;
        }

        internal TweenChainComponent AddCallback(Action callbackFn)
        {
            this.chain.AppendCallback(callbackFn);
            return this;
        }

        internal void AddIntTween(int targetVal, int duration, EaseFunc easeFunc, TweenAccessors<int> accessors)
        {
            this.chain.AppendIntTween(targetVal, duration, easeFunc, accessors);
        }
    }
}
