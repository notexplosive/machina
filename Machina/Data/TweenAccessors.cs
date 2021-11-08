using System;

namespace Machina.Data
{
    public struct TweenAccessors<T> where T : struct
    {
        public readonly Func<T> getter;
        public readonly Action<T> setter;

        public T CurrentValue
        {
            get => this.getter();
            set => this.setter(value);
        }

        public TweenAccessors(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public TweenAccessors(T startingValue)
        {
            var value = startingValue;
            this.getter = () => value;
            this.setter = setVal => value = setVal;
        }
    }
}