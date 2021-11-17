﻿namespace Machina.Engine
{
    using Machina.Engine.Cartridges;

    public static class MachinaBootstrap
    {
        public static void Run(MachinaGameSpecification specification, GameCartridge gameCartridge)
        {
            using (var game = new MachinaGame(specification, gameCartridge))
            {
                game.Run();
            }
        }
    }
}