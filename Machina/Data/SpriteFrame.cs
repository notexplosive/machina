using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class SpriteFrame
    {
        public readonly SpriteSheet spriteSheet;
        public readonly ChooseFrameAnimation animation;

        public SpriteFrame(SpriteSheet spriteSheet, int frame)
        {
            this.spriteSheet = spriteSheet;
            this.animation = new ChooseFrameAnimation(frame);
        }
    }
}
