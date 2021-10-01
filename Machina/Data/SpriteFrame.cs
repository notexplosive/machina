namespace Machina.Data
{
    public class SpriteFrame
    {
        public readonly ChooseFrameAnimation animation;
        public readonly SpriteSheet spriteSheet;

        public SpriteFrame(SpriteSheet spriteSheet, int frame)
        {
            this.spriteSheet = spriteSheet;
            this.animation = new ChooseFrameAnimation(frame);
        }
    }
}
