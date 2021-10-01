namespace Machina.Engine.Debugging.Data
{
    public interface IFrameStep
    {
        public bool IsPaused { get; set; }

        public void Step(Scene scene);
    }

    public class EmptyFrameStep : IFrameStep
    {
        public bool IsPaused
        {
            get => false;
            set { }
        }

        public void Step(Scene scene)
        {
            // No op
        }
    }

    public class FrameStep : IFrameStep
    {
        public bool IsPaused { get; set; }

        public void Step(Scene scene)
        {
            scene.Update(1f / 60f);
        }
    }
}
