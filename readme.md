# Machina: A 2D Component Based Game Engine

## Summary

> A game engine for people that like Unity in theory, but...

Machina is a game engine built on top of MonoGame that's meant to feel sorta like Unity but with a focus on 2D. It has built in libraries for:

- Scene Graph (unity-like)
- Component System (very unity-like!)
- Coroutines (again, unity-like)
- Tweening (similar to DOTween, the best unity asset store asset)
- Sprite Animation (unity-like without the clunky-ass UI)
- Built-in components for building UI, such as Layout, HitTesting, and Click-and-Drag
- Incremental Asset loading
- And a surprisingly robust windowing system

Plus a few other things that I'm piggybacking off of other people's technology to use:

- Deterministic and Platform Agnostic Random Number generators from [this GDC talk](https://www.youtube.com/watch?v=LWFzPP8ZbdU)
- Primitive rendering (lines/circles/rectangles) from MonoGame.Extended

## Getting Started

Before we even begin, let me start off by saying that this engine is a totally 1-man operation. Keeping docs like these up to date is a lot of work. If you really are genuinely interested in trying out Machina. I recommend just [reaching out to me directly](https://twitter.com/notexplosive) rather than following these docs.

### Pre-setup

To setup you'll need some starting software:

- `git`, obviously.
- .NET Core 3.1
  - Run `dotnet --info` in the CLI and make sure it's installed
- (Recommended) Visual Studio 2019 or JetBrains Rider
  - VS Code is OK, but VS2019/Rider really give you the best experience.

### Creating a Machina Project

To create a Machina Project you do _not_ need to clone this repo (at least not directly). Ultimately you'll pull this repo down as a submodule. But you don't need to do that manually (although that is an option).

To automate creating a Machina project, I created a neat tool called [Neato](https://notexplosive.itch.io/neato). At time of writing Neato is only built for Windows but could work on all platforms. You might need to [build it from source](https://github.com/notexplosive/neato) (sorry!).

OK enough preamble, let's make a project!

- Download (or build) `neato` and put it somewhere you can invoke it, for these docs I'll assume you put it in `.\neato\neato.exe`. (apologies for my windows-centric paths, I assume windows folks will need more help with the command line)
- Navigate to where you want to create your project in your file system.
- `.\neato\neato.exe new-project MyCoolGame` where `MyCoolGame` is your project name
- Let Neato work it's magic, hopefully you get all 🔵s
- 🔶Neato will finish with some warnings about manual steps. The instructions below will help you with those.

### Boilerplate

We're almost there! We just need to setup an initial game that will use Machina. The following assumes your using `MyCoolGame` as your project and namespace. Replace all instances with your namespace.

### 🔶 Replace the default Game1.cs and Program.cs with the Machina boilerplate

1.  Replace Program.cs's contents with the following:

    ```cs
    // Program.cs

    namespace MyCoolGame
    {
        using Machina.Data;
        using Machina.Engine;
        using Microsoft.Xna.Framework;
        using System;

        public static class Program
        {
            [STAThread]
            private static void Main(string[] args)
            {
                MachinaBootstrap.Run(
                    new GameSpecification(
                        // This is what will show up in the window titlebar.
                        // It's also the name of the directory your game will have in AppData (or platform equivalent)
                        "My Cool Game",
                        // Passthru for command line args
                        args,
                        // ...this might change in the near future, for you supply the starting resolution of your game.
                        new GameSettings(new Point(1600, 900))
                        ),
                    // MyCoolGame Cartridge, defined in the next part
                    new MyCoolGame()
                    );
            }
        }
    }
    ```

2.  Delete Game1.cs, where we're going, we don't need Game1.cs. You will need to create a `MyCoolGame` class. But instead of providing you code samples, I'll instead provide you with a set of principles you can use to build your own. Start with this:

    ```cs
    using Machina.Engine.Cartridges;

    namespace MyCoolGame
    {
        public class MyCoolGame : GameCartridge
        {
            // No code?! (and red squigglies?!)
        }
    }
    ```

3.  If you write the above code, you probably got some red squigglies in Visual Studio. Good. Now `CTRL + .` on `GameCartridge` and select `Implement abstract class`. This should populate MyCoolGame with some functions. Hover over them to see their summaries to understand what they do.

4.  Now `CTRL + .` on `GameCartridge` again. This time select `Generate constructor`
5.  Remove the `throws` and you should be good to go! With one small hiccup...

### 🔶 Add MachinaAssets.shproj .\machina\MachinaAssets to the sln

Boy this doc is getting long huh... thanks for sticking with me this far. It's been quite an adventure for both of us (can you believe this is the _better_ version?)

Neato automated a bunch of stuff for us but it can't add shared projects to a solution because apparently Microsoft didn't think that was worth adding to the `dotnet` CLI.

1. In Visual Studio 2019, right click the `machina` directory and select `Existing Project`
2. Navigate to `MyCoolGame\machina\MachinaAssets\MachinaAssets.shproj`
3. Select it.

### 🔶 Add a shared project reference of MachinaAssets to your game.

just... one... more... thing...

Now that the shared project is in your solution. Now you have to plug it into your game.

1. In Visual Studio 2019, right click the `MyCoolGame` project, click `Add` and then `Shared Project Reference`. In the Shared Projects tab, hit the checkbox next to MachinaAssets.

### It is now safe to make a video game.

1.  Press F5 to run and you should see a loading screen followed by a purple window. You did it!

See `docs\components.md` for more information on getting started.
