using Machina.Data;

namespace Machina.Engine
{
    public class GameSpecification
    {
        public readonly string gameTitle;
        public readonly CommandLineArgs commandLineArgs;
        /// <summary>
        /// Settings must be readonly, other things hold references to it and assume that they'll stay up to date
        /// </summary>
        public readonly GameSettings settings;


        public GameSpecification(string gameTitle, string[] args, GameSettings startingSettings)
        {
            this.gameTitle = gameTitle;
            this.commandLineArgs = new CommandLineArgs(args);
            this.settings = startingSettings;
        }
    }
}