﻿using Machina.Components;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    class TweenChain
    {
        private readonly List<IChainItem> chainInernal;
        private int currentIndex;
        private IChainItem currentItem;
        private TweenAccessors<float> dummyAccessors = new TweenAccessors<float>(dummyGetter, dummySetter);

        public Tween<T> CurrentTween<T>() where T : struct
        {
            var current = this.currentItem;
            if (current == null)
            {
                current = this.chainInernal[this.currentIndex];
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
            this.chainInernal = new List<IChainItem>();
            this.currentIndex = 0;
        }

        public TweenChain Append(IChainItem item)
        {
            chainInernal.Add(item);
            return this;
        }

        public TweenChain AppendFloatTween(float targetVal, float duration, EaseFunc easeFunc, TweenAccessors<float> accessors)
        {
            return Append(new ChainItem<float>(targetVal, duration, easeFunc, accessors, FloatTween.LerpFloat));
        }

        public TweenChain AppendWaitTween(float duration)
        {
            return Append(new ChainItem<float>(0, duration, EaseFuncs.Linear, dummyAccessors, FloatTween.LerpFloat));
        }

        public TweenChain AppendIntTween(int targetVal, int duration, EaseFunc easeFunc, TweenAccessors<int> accessors)
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

        internal TweenChain AppendVectorTween(Vector2 targetVal, float duration, EaseFunc easeFunc, TweenAccessors<Vector2> accessors)
        {
            return Append(new ChainItem<Vector2>(targetVal, duration, easeFunc, accessors, Vector2.Lerp));
        }

        public void Update(float dt)
        {
            if (this.currentItem == null && this.chainInernal.Count > this.currentIndex)
            {
                this.currentItem = this.chainInernal[this.currentIndex].StartTween();
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

        /// <summary>
        /// Maintains the current chain but puts us back at the beginning
        /// </summary>
        public void Refresh()
        {
            this.currentIndex = 0;
            this.currentItem = null;
            foreach (var item in this.chainInernal)
            {
                item.Refresh();
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

            /// <summary>
            /// Risky, you need to know the exact type of tween you're looking for.
            /// If the Chain is in a callback you'll get null
            /// </summary>
            /// <typeparam name="U">Tween subtype</typeparam>
            /// <returns></returns>
            public Tween<U> GetTween<U>() where U : struct;
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

            public Tween<U> GetTween<U>() where U : struct
            {
                return this.tween as Tween<U>;
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

            public Tween<T> GetTween<T>() where T : struct
            {
                return null;
            }

            public void Refresh()
            {
                IsComplete = false;
            }

            public IChainItem StartTween()
            {
                callbackFn();
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
