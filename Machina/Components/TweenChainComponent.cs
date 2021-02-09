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

        public TweenChainComponent AddTween<T>(T target, float duration, EaseFunc easeFunc, Func<T> getter, Action<T> setter, LerpFunc<T> lerp) where T : struct
        {
            chain.Append(new TweenChain.ChainItem<T>(target, duration, easeFunc, getter, setter, lerp));
            return this;
        }

        public TweenChainComponent AddMoveTween(Vector2 targetPos, float duration, EaseFunc easeFunc)
        {
            return AddTween(targetPos, duration, easeFunc, this.actor.GetPosition, this.actor.SetPosition, Vector2.Lerp);
        }

        public TweenChainComponent AddFloatTween(float targetVal, float duration, EaseFunc easeFunc, Func<float> getter, Action<float> setter)
        {
            this.chain.AppendFloatTween(targetVal, duration, easeFunc, getter, setter);
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
    }
}
