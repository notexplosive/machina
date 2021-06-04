using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class XYPair<T>
    {
        public T X;
        public T Y;

        public XYPair(T x, T y)
        {
            X = x;
            Y = y;
        }
    }
}
