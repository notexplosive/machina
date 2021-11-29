namespace Machina.Engine
{
    public interface IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, WindowInterface window);
        public void OnGameConstructed(MachinaGame machinaGame);
    }
}