namespace Machina.Data
{
    public enum LoopType
    {
        HoldLastFrame,
        Loop
    }

    public interface IFrameAnimation
    {
        public int Length { get; }

        public LoopType LoopType { get; }

        public int GetFrame(float elapsedTime);
    }
}