using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public struct TweenAccessors<T> where T : struct
    {
        public readonly Func<T> getter;
        public readonly Action<T> setter;

        public TweenAccessors(Func<T> getter, Action<T> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
    }
}
