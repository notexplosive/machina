using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Machina.Data
{
    public enum LoopType
    {
        HoldLastFrame,
        Loop
    }
    public interface IFrameAnimation
    {
        public abstract int Length
        {
            get;
        }

        public abstract LoopType LoopType
        {
            get;
        }

        public int GetFrame(float elapsedTime);
    }
}
