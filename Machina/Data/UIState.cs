using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public interface UIState<T>
    {
        public abstract T GetState();
    }
}
