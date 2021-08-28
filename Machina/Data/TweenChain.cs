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
        private static TweenAccessors<float> dummyAccessors = new TweenAccessors<float>(dummyGetter, dummySetter);

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
            this.currentItem = null;
        }

        public TweenChain AppendWaitUntilTween(Func<bool> callbackFn)
        {
            return Append(new WaitUntilCallbackChainItem(callbackFn));
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

        public TweenChain AppendPointTween(Point targetVal, float duration, EaseFunc easeFunc, TweenAccessors<Point> accessors)
        {
            return Append(new ChainItem<Point>(targetVal, duration, easeFunc, accessors, PointLerp));
        }

        private static Point PointLerp(Point p1, Point p2, float amount)
        {
            return Vector2.Lerp(p1.ToVector2(), p2.ToVector2(), amount).ToPoint();
        }

        public MultiChainItem AppendMulticastTween()
        {
            var multiChainItem = new MultiChainItem();
            Append(multiChainItem);
            return multiChainItem;
        }

        private void StartNextTween()
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
                    // We recurse to queue up next item, this has a few interesting consequences:
                    // 1) callbacks execute instantly, therefore:
                    // 2) callbacks that queue up additional callbacks with no delay will infinite loop
                    Update(dt);
                }
            }
        }

        public bool IsDone()
        {
            return this.currentIndex == this.chainInternal.Count && this.currentItem == null; // this is clunky af
        }

        [Obsolete("Use IsDone() instead")]
        public bool IsFinished => this.currentIndex == this.chainInternal.Count && this.currentItem == null; // this is clunky af

        /// <summary>
        /// Maintains the current chain but puts us back at the beginning
        /// </summary>
        public void Refresh()
        {
            this.currentIndex = 0;
            this.currentItem = null;
            var chainCopy = new List<IChainItem>(this.chainInternal);
            foreach (var item in chainCopy)
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
                        if (!chain.IsDone())
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            public IChainItem StartTween()
            {
                foreach (var chain in new List<TweenChain>(chains))
                {
                    chain.StartNextTweenIfAble();
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

        private void StartNextTweenIfAble()
        {
            if (this.currentItem == null && this.chainInternal.Count > this.currentIndex)
                StartNextTween();
        }

        public class ChainItem<T> : IChainItem where T : struct
        {
            private readonly LerpFunc<T> lerpFunc;
            private readonly TweenAccessors<T> accessors;
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
                this.accessors = accessors;
                this.lerpFunc = lerp;
                this.tween = new Tween<T>(lerpFunc);
            }

            public void Refresh()
            {
                this.tween.Stop(StopBehavior.AsIs);
            }

            public IChainItem StartTween()
            {
                this.tween.Start(this.accessors.getter(), destination, duration, scaleFunc);
                return this;
            }

            public void Update(float dt)
            {
                this.tween.Update(dt);
                this.accessors.setter(this.tween.CurrentValue);
            }
        }

        /// <summary>
        /// Instantly execute a callback in the middle of the tween
        /// </summary>
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

        /// <summary>
        /// Pause the tween until the callback returns true
        /// </summary>
        public class WaitUntilCallbackChainItem : IChainItem
        {
            private readonly Func<bool> callbackFn;

            public WaitUntilCallbackChainItem(Func<bool> callbackFn)
            {
                this.callbackFn = callbackFn;
            }

            public bool IsComplete => this.callbackFn();

            public void Refresh()
            {
                // This function is intentionally left blank
            }

            public IChainItem StartTween()
            {
                callbackFn?.Invoke();
                return this;
            }

            public void Update(float dt)
            {
                // This function is intentionally left blank
            }
        }

        public static int LerpInt(int start, int end, float progress)
        {
            return (int)(start + (end - start) * progress);
        }
    }
}
