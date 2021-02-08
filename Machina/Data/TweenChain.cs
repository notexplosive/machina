using Machina.Components;
using Machina.ThirdParty;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    class TweenChain
    {
        private readonly List<IChainItem> chain;
        private int currentIndex;
        private IChainItem currentItem;

        public TweenChain()
        {
            this.chain = new List<IChainItem>();
            this.currentIndex = 0;
        }

        public void Add(IChainItem item)
        {
            chain.Add(item);
        }

        private IChainItem StartTween(IChainItem item)
        {
            item.StartTween();
            return item;
        }

        public void Update(float dt)
        {
            if (this.currentItem == null && this.chain.Count > this.currentIndex)
            {
                this.currentItem = StartTween(this.chain[this.currentIndex]);
                this.currentIndex++;
            }

            if (this.currentItem != null)
            {
                this.currentItem.Update(dt);

                if (this.currentItem.IsComplete)
                {
                    this.currentItem = null;
                }
            }
        }

        public interface IChainItem
        {
            public void StartTween();
            public void Update(float dt);
            public bool IsComplete
            {
                get;
            }
        }

        public class ChainItem<T> : IChainItem where T : struct
        {
            private readonly LerpFunc<T> lerpFunc;
            private readonly Func<T> getter;
            private readonly Action<T> setter;
            public readonly T destination;
            public readonly float duration;
            public readonly EaseFunc scaleFunc;
            public readonly Tween<T> tween;

            public bool IsComplete => this.tween.State == TweenState.Stopped;

            public ChainItem(T destination, float duration, EaseFunc scaleFunc, Func<T> getter, Action<T> setter, LerpFunc<T> lerp)
            {
                this.destination = destination;
                this.duration = duration;
                this.scaleFunc = scaleFunc;
                this.getter = getter;
                this.setter = setter;
                this.lerpFunc = lerp;
                this.tween = new Tween<T>(lerpFunc);
            }

            public void StartTween()
            {
                this.tween.Start(getter(), destination, duration, scaleFunc);
            }

            public void Update(float dt)
            {
                this.tween.Update(dt);
                this.setter(this.tween.CurrentValue);
            }
        }
    }
}
