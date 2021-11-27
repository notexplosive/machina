namespace Machina.Engine
{
    public interface IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, IWindow window);
        public void OnGameConstructed(MachinaGame machinaGame);
    }
}