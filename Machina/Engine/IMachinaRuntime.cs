namespace Machina.Engine
{
    public interface IMachinaRuntime
    {
        Painter Painter { get; }
        OSWindow WindowInterface { get; }
        DebugLevel DebugLevel { get; set; }
        Cartridge CurrentCartridge { get; }

        void Quit();
        void RunDemo(string demoPath);
    }
}