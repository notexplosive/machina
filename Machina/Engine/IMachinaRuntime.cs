using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    public interface IMachinaRuntime
    {
        Painter Painter { get; }
        IWindow WindowInterface { get; }
        DebugLevel DebugLevel { get; set; }
        IPlatformContext PlatformContext { get; }
        Cartridge CurrentCartridge { get; }
        RenderTarget2D DefaultRenderTarget { get; }

        void Quit();
        void RunDemo(string demoPath);
    }
}