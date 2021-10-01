using System;
using System.Diagnostics;

namespace Machina.Data
{
    public struct ChooseFrameAnimation : IFrameAnimation
    {
        private readonly int[] frames;

        public ChooseFrameAnimation(params int[] listOfFrames)
        {
            LoopType = LoopType.Loop;
            this.frames = listOfFrames;
        }

        public ChooseFrameAnimation(LoopType loopType, params int[] listOfFrames)
        {
            LoopType = loopType;
            this.frames = listOfFrames;
        }

        public int Length => this.frames.Length;

        public LoopType LoopType { get; }

        public int GetFrame(float elapsedTime)
        {
            Debug.Assert(this.frames != null && this.frames.Length != 0,
                "ChooseFrameAnimation was used but no frames were chosen");
            if (this.frames.Length == 1)
            {
                return this.frames[0];
            }

            if (LoopType == LoopType.Loop)
            {
                return this.frames[(int) elapsedTime % this.frames.Length];
            }

            return Math.Min(this.frames[this.frames.Length], this.frames[(int) elapsedTime]);
        }
    }
}
