using Machina.Components;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class TweenChain
    {
        private readonly List<IChainItem> chainInternal;
        private int currentIndex;
        private IChainItem currentItem;
        private TweenAccessors<float> dummyAccessors = new TweenAccessors<float>(dummyGetter, dummySetter);

        public Tween<T> CurrentTween<T>() where T : struct
        {
            var current = this.currentItem;
            if (current == null)
            {
                current = this.chainInternal[this.currentIndex];
            }

            if (current != null)
            {
                return (current as ChainItem<T>).tween;
            }
            else
            {
                return null;
            }
        }

        private static float dummyGetter() => 0f;
        private static void dummySetter(float val)
        {
        }

        public TweenChain()
        {
            this.chainInternal = new List<IChainItem>();
            this.currentIndex = 0;
        }

        public void Clear()
        {
            this.chainInternal.Clear();
            this.currentIndex = 0;
        }

        public TweenChain Append(IChainItem item)
        {
            chainInternal.Add(item);
            return this;
        }

        /*
         * This would work if we had a way to obtain a LerpFunction for a type
        public TweenChain Append<T>(T targetVal, float duration, EaseFunc easeFunc, TweenAccessors<T> accessors) where T : struct
        {
            return Append(new ChainItem<T>(targetVal, duration, easeFunc, accessors, GetLerpFunction<T>()));
        }
        */

        public TweenChain AppendFloatTween(float targetVal, float duration, EaseFunc easeFunc, TweenAccessors<float> accessors)
        {
            return Append(new ChainItem<float>(targetVal, duration, easeFunc, accessors, FloatTween.LerpFloat));
        }

        public TweenChain AppendWaitTween(float duration)
        {
            return Append(new ChainItem<float>(0, duration, EaseFuncs.Linear, dummyAccessors, FloatTween.LerpFloat));
        }

        public TweenChain AppendIntTween(int targetVal, float duration, EaseFunc easeFunc, TweenAccessors<int> accessors)
        {
            return Append(new ChainItem<int>(targetVal, duration, easeFunc, accessors, LerpInt));
        }

        public TweenChain AppendCallback(Action func)
        {
            return Append(new CallbackChainItem(func));
        }

        public TweenChain AppendPositionTween(Actor actor, Vector2 targetVal, float duration, EaseFunc easeFunc)
        {
            return Append(new ChainItem<Vector2>(targetVal, duration, easeFunc, TweenDataFunctions.PositionTweenAccessors(actor), Vector2.Lerp));
        }

        public TweenChain AppendVectorTween(Vector2 targetVal, float duration, EaseFunc easeFunc, TweenAccessors<Vector2> accessors)
        {
            return Append(new ChainItem<Vector2>(targetVal, duration, easeFunc, accessors, Vector2.Lerp));
        }

        public MultiChainItem AppendMulticastTween()
        {
            var multiChainItem = new MultiChainItem();
            Append(multiChainItem);
            return multiChainItem;
        }

        public void StartNextTween()
        {
            if (this.chainInternal.Count > 0)
            {
                this.currentItem = this.chainInternal[this.currentIndex].StartTween();
                this.currentIndex++;
            }
        }

        public void Update(float dt)
        {
            if (this.currentItem == null && this.chainInternal.Count > this.currentIndex)
            {
                StartNextTween();
            }

            if (this.currentItem != null)
            {
                this.currentItem.Update(dt);

                if (this.currentItem.IsComplete)
                {
                    this.currentItem = null;
                    Update(dt); // recurse to queue up next item, this means callbacks execute instantly
                }
            }
        }

        public bool IsFinished => this.currentIndex == this.chainInternal.Count && this.currentItem == null; // this is clunky af

        /// <summary>
        /// Maintains the current chain but puts us back at the beginning
        /// </summary>
        public void Refresh()
        {
            this.currentIndex = 0;
            this.currentItem = null;
            foreach (var item in this.chainInternal)
            {
                item.Refresh();
            }

            StartNextTween();
        }

        /// <summary>
        /// Skip to the end of the tween, skipping everything in between
        /// </summary>
        public void SkipToEnd()
        {
            this.currentIndex = this.chainInternal.Count;
            this.currentItem = null;
        }

        /// <summary>
        /// This could go wrong in a lot of ways, it's also kinda slow
        /// </summary>
        public void FinishRestOfTween_Dangerous()
        {
            while (currentItem != null)
            {
                this.Update(1f / 60);
            }
        }

        public interface IChainItem
        {
            public IChainItem StartTween();
            public void Update(float dt);
            public bool IsComplete
            {
                get;
            }
            public void Refresh();
        }

        public class MultiChainItem : IChainItem
        {
            public readonly List<TweenChain> chains = new List<TweenChain>();

            public bool IsComplete
            {
                get
                {
                    foreach (var chain in this.chains)
                    {
                        if (!chain.IsFinished)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public IChainItem StartTween()
            {
                foreach (var chain in chains)
                {
                    chain.StartNextTween();
                }
                return this;
            }

            public void Refresh()
            {
                foreach (var chain in chains)
                {
                    chain.Refresh();
                }
            }

            public void Update(float dt)
            {
                foreach (var chain in chains)
                {
                    chain.Update(dt);
                }
            }

            /// <summary>
            /// Adds a new TweenChain channel.
            /// CAUTION: If you have two channels that manipulate the same field, last one added will take precedence
            /// </summary>
            /// <returns></returns>
            public TweenChain AddChannel()
            {
                var chain = new TweenChain();
                this.chains.Add(chain);
                return chain;
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

            public ChainItem(T destination, float duration, EaseFunc scaleFunc, TweenAccessors<T> accessors, LerpFunc<T> lerp)
            {
                this.destination = destination;
                this.duration = duration;
                this.scaleFunc = scaleFunc;
                this.getter = accessors.getter;
                this.setter = accessors.setter;
                this.lerpFunc = lerp;
                this.tween = new Tween<T>(lerpFunc);
            }

            public void Refresh()
            {
                this.tween.Stop(StopBehavior.AsIs);
            }

            public IChainItem StartTween()
            {
                this.tween.Start(getter(), destination, duration, scaleFunc);
                return this;
            }

            public void Update(float dt)
            {
                this.tween.Update(dt);
                this.setter(this.tween.CurrentValue);
            }
        }

        public class CallbackChainItem : IChainItem
        {
            private readonly Action callbackFn;

            public CallbackChainItem(Action callbackFn)
            {
                this.callbackFn = callbackFn;
            }

            public bool IsComplete
            {
                get; private set;
            }

            public void Refresh()
            {
                IsComplete = false;
            }

            public IChainItem StartTween()
            {
                callbackFn?.Invoke();
                IsComplete = true;
                return this;
            }

            public void Update(float dt)
            {
                // This function is intentionally left blank
            }
        }

        public static int LerpInt(int start, int end, float progress)
        {
            return (int) (start + (end - start) * progress);
        }
    }
}
