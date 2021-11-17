﻿using Machina.Data;

namespace Machina.Engine
{
    public class GameSpecification
    {
        public readonly string gameTitle;
        public readonly CommandLineArgs commandLineArgs;
        public readonly GameSettings settings;


        public GameSpecification(string gameTitle, string[] args, GameSettings startingSettings)
        {
            this.gameTitle = gameTitle;
            this.commandLineArgs = new CommandLineArgs(args);
            this.settings = startingSettings;
        }
    }
}