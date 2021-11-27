namespace MachinaDesktop
{
    using Machina.Engine;
    using Machina.Engine.Cartridges;

    public static class MachinaBootstrap
    {
        public static void Run(GameSpecification specification, GameCartridge gameCartridge, string devContentPath)
        {
            using (var game = new MachinaGame(specification, gameCartridge, new DesktopPlatformContext(), devContentPath))
            {
                game.Run();
            }
        }
    }
}