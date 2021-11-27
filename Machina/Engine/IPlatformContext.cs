namespace Machina.Engine
{
    public interface IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, IWindow window);
        public void OnGameConstructed(MachinaGame machinaGame);
    }

    /// <summary>
    /// The Platform Context of "inside the game"
    /// </summary>
    public class FenestraPlatformContext : IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, IWindow window)
        {
        }

        public void OnGameConstructed(MachinaGame machinaGame)
        {
        }
    }
}