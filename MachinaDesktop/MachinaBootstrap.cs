namespace MachinaDesktop
{
    using Machina.Engine;
    using Machina.Engine.Cartridges;

    public static class MachinaBootstrap
    {
        public static void Run(GameSpecification specification, GameCartridge gameCartridge)
        {
            using (var game = new MachinaGame(specification, gameCartridge, new DesktopPlatformContext()))
            {
                game.Run();
            }
        }
    }
}