namespace Machina.Engine
{
    public class EmptyMachinaRuntime : IMachinaRuntime
    {
        public Painter Painter => null;

        public IWindow WindowInterface => null;

        public DebugLevel DebugLevel { get; set; }

        public Cartridge CurrentCartridge => null;

        public void Quit()
        {
        }

        public void RunDemo(string demoPath)
        {
        }
    }
}