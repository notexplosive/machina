using System;

namespace HelloGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new GameCore();
            game.Run();
        }
    }
}
