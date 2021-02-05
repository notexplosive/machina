using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    struct ChooseFrameAnimation : IFrameAnimation
    {
        readonly int[] frames;
        private LoopType loopType;

        public ChooseFrameAnimation(params int[] listOfFrames)
        {
            this.loopType = LoopType.Loop;
            this.frames = listOfFrames;
        }

        public ChooseFrameAnimation(LoopType loopType, params int[] listOfFrames)
        {
            this.loopType = loopType;
            this.frames = listOfFrames;
        }

        public int Length => frames.Length;

        public LoopType LoopType => this.loopType;


        public int GetFrame(float elapsedTime)
        {
            Debug.Assert(this.frames != null && this.frames.Length != 0, "ChooseFrameAnimation was used but no frames were chosen");
            if (this.frames.Length == 1)
            {
                return this.frames[0];
            }


            if (this.loopType == LoopType.Loop)
            {
                return this.frames[(int) elapsedTime % this.frames.Length];
            }
            else
            {
                return Math.Min(this.frames[frames.Length], this.frames[(int) elapsedTime]);
            }
        }
    }
}
