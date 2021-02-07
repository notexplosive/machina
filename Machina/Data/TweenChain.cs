using Machina.Components;
using Machina.ThirdParty;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    class TweenChain<T> where T : struct
    {
        private List<TweenChainItem> chain;
        private LerpFunc<T> lerpFunc;
        private Func<T> getter;
        private Action<T> setter;
        private int currentIndex;
        private Tween<T> currentTween;

        public TweenChain(LerpFunc<T> lerpFunc, Func<T> getter, Action<T> setter)
        {
            this.chain = new List<TweenChainItem>();
            this.lerpFunc = lerpFunc;
            this.getter = getter;
            this.setter = setter;
            this.currentIndex = 0;
        }

        public void AddTween(T destination, float duration, ScaleFunc scaleFunc)
        {
            chain.Add(new TweenChainItem(destination, duration, scaleFunc, this.getter));
        }

        private Tween<T> StartTween(TweenChainItem item)
        {
            var tween = item.Build(lerpFunc);
            tween.Start(item.getCurrent(), item.destination, item.duration, item.scaleFunc);
            return tween;
        }

        public void Update(float dt)
        {
            if (this.currentTween == null && this.chain.Count > this.currentIndex)
            {
                this.currentTween = StartTween(this.chain[this.currentIndex]);
                this.currentIndex++;
            }

            if (this.currentTween != null)
            {
                this.currentTween.Update(dt);
                this.setter(this.currentTween.CurrentValue);

                if (this.currentTween.State == TweenState.Stopped)
                {
                    this.currentTween = null;
                }
            }
        }

        private struct TweenChainItem
        {
            public readonly T destination;
            public readonly float duration;
            public readonly ScaleFunc scaleFunc;
            public readonly Func<T> getCurrent;

            public TweenChainItem(T destination, float duration, ScaleFunc scaleFunc, Func<T> getCurrent)
            {
                this.destination = destination;
                this.duration = duration;
                this.scaleFunc = scaleFunc;
                this.getCurrent = getCurrent;
            }

            public Tween<T> Build(LerpFunc<T> lerpFunc)
            {
                return new Tween<T>(lerpFunc);
            }
        }
    }
}
