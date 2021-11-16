using Machina.Data;

namespace Machina.Engine
{
    public class MachinaGameSpecification
    {
        public readonly string gameTitle;
        public readonly CommandLineArgs CommandLineArgs;
        public readonly GameSettings settings;


        public MachinaGameSpecification(string gameTitle, string[] args, GameSettings startingSettings)
        {
            this.gameTitle = gameTitle;
            this.CommandLineArgs = new CommandLineArgs(args);
            this.settings = startingSettings;
        }
    }
}