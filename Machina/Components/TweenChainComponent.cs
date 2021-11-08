using System;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;

namespace Machina.Components
{
    public class TweenChainComponent : BaseComponent
    {
        private readonly TweenChain chain;

        public TweenChainComponent(Actor actor) : base(actor)
        {
            this.chain = new TweenChain();
        }

        public override void Update(float dt)
        {
            this.chain.Update(dt);
        }

        public TweenChainComponent AddTween<T>(T target, float duration, EaseFunc easeFunc, TweenAccessors<T> accessors,
            LerpFunc<T> lerp) where T : struct
        {
            this.chain.Append(new TweenChain.ChainItem<T>(target, duration, easeFunc, accessors, lerp));
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

        public TweenChainComponent AddFloatTween(float targetVal, float duration, EaseFunc easeFunc,
            TweenAccessors<float> accessors)
        {
            this.chain.AppendFloatTween(targetVal, duration, easeFunc, accessors);
            return this;
        }

        public TweenChainComponent AddWaitTween(float duration)
        {
            this.chain.AppendWaitTween(duration);
            return this;
        }

        public TweenChainComponent AddCallback(Action callbackFn)
        {
            this.chain.AppendCallback(callbackFn);
            return this;
        }

        public void AddIntTween(int targetVal, int duration, EaseFunc easeFunc, TweenAccessors<int> accessors)
        {
            this.chain.AppendIntTween(targetVal, duration, easeFunc, accessors);
        }

        public void StopAndClearAllTweens()
        {
            this.chain.Clear();
        }

        public void AddVectorTween(Vector2 targetVal, float duration, EaseFunc easeFunc,
            TweenAccessors<Vector2> accessors)
        {
            this.chain.AppendVectorTween(targetVal, duration, easeFunc, accessors);
        }

        public TweenChain.MultiChainItem AddMulticastTween()
        {
            return this.chain.AppendMulticastTween();
        }
    }
}