using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// Specify the start point and length of the animation, animation starts at firstFrame, and ends on firstFrame + length
    /// </summary>
    public struct LinearFrameAnimation : IFrameAnimation
    {
        public readonly int length;
        public readonly LoopType loopType;
        public readonly int firstFrame;

        public LinearFrameAnimation(LinearFrameAnimation copy) : this(copy.firstFrame, copy.length, copy.loopType) { }
        public LinearFrameAnimation(int firstFrame = 0, int length = 1, LoopType loop = LoopType.Loop)
        {
            this.firstFrame = firstFrame;
            this.length = length;
            this.loopType = loop;
        }
        public bool Equals(LinearFrameAnimation other)
        {
            return other.firstFrame == this.firstFrame && other.length == this.length && other.loopType == this.loopType;
        }

        public int LastFrame => this.firstFrame + length - 1;

        public int Length => this.length;

        public LoopType LoopType => this.loopType;

        public int GetFrame(float elapsedTime)
        {
            if (length == 1)
            {
                return this.firstFrame;
            }

            if (this.loopType == LoopType.Loop)
            {
                float alongDuration = elapsedTime % length;
                return (int) (alongDuration + this.firstFrame);
            }
            else
            {
                var frame = (int) elapsedTime + this.firstFrame;
                return Math.Min(LastFrame, frame);
            }
        }
    }
}
