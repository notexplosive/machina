# Machina: A 2D Component Based Game Engine

## Summary

Machina is a game engine built on top of MonoGame that's meant to feel sorta like Unity but with a focus on 2D.

## Setting up a new Machina Project

### Pre-setup

To setup you'll need some starting software:

- `git`, obviously.
- At least some version of .NET Core. Both .NET Core 3.1 or .NET Core 5.0 are OK, both is best!
  - Run `dotnet --info` in the CLI and make sure you get something
- (Recommended) Visual Studio 2019
  - VS Code is OK, but VS2019 is the best experience (it's also what I use and what I know works)

### Actually setting up (Currently Windows only)

You will _not_ need to clone this repo to use Machina, you _can_, but won't need to. The following instructions require an **EMPTY** folder that has **NOT** been initialized with git.

1. Download the `auto-bootstrap.bat` script, should be at the root level of this repo
2. Put that script where you want your project to be in an **empty folder**. This will become your project folder. Run the script with the name of your poject as the first parameter (eg: `auto-bootstrap.bat MyCoolGame`)
3. This should do a bunch of stuff automagically, you now have some csproj files, an sln, and even a `.git` folder with an initial commit (unless something blew up)
4. Open the `MyCoolGame.sln`
5. Add MachinaAssets to sln. In the Solution Explorer, right click the `machina` folder. Click Add > Existing Project. Navigate to `MyCoolGame\machina\MachinaAssets` and click `MachinaAssets.shproj`. (I wish this were automatic)
6. Add MachinaAssets to main csproj. In the Solution Explorer, crack open MyCoolGame > Dependencies > Projects. Right click > Add Shared Project Reference. Select MachinaAssets. (I wish this were automatic)

### Boilerplate

We're almost there! We just need to setup an initial game that will use Machina. The following assumes your using `MyCoolGame` as your project and namespace. Replace all instances with your namespace.

1.  Replace Program.cs's contents with the following:

    ```cs
    using System;

    namespace MyCoolGame
    {
        public static class Program
        {
            [STAThread]
            static void Main(string[] args)
            {
                using (var game = new Game1(args))
                    game.Run();
            }
        }
    }
    ```

2.  Replace Game1.cs's content with the following:

    ```cs
    using Machina.Engine;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    namespace MyCoolGame
    {
        public class Game1 : MachinaGame
        {
            public Game1(string[] args) : base(
                "My Cool Game", // window title
                args, // args from command line
                new Point(1920,1080), // rendering resolution
                new Point(1600, 900), // window size
                ResizeBehavior.MaintainDesiredResolution) // does content stretch to window size or does it fill the content? (this setting means stretch)
            {
                // You probably don't want to add anything here
            }

            protected override void OnGameLoad()
            {
                // Your game code starts here!
            }
        }
    }
    ```

3.  Press F5 to run and you should see a purple window with a developer console printing at the top!

See `docs\components.md` for more information on getting started.
