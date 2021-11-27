namespace Machina.Engine
{
    public interface IMachinaRuntime
    {
        Painter Painter { get; }
        IWindow WindowInterface { get; }
        DebugLevel DebugLevel { get; set; }
        IPlatformContext PlatformContext { get; }
        Cartridge CurrentCartridge { get; }

        void Quit();
        void RunDemo(string demoPath);
    }
}