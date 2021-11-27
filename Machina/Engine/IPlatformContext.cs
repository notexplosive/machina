namespace Machina.Engine
{
    public interface IPlatformContext
    {
        public void OnCartridgeSetup(Cartridge cartridge, OSWindow window);
        public void OnGameConstructed(MachinaGame machinaGame);
    }
}