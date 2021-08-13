# Machina: A 2D Component Based Game Engine

## Summary

Machina is a game engine built on top of MonoGame that's meant to feel sorta like Unity but with a focus on 2D.

## Setting up

### Pre-setup

To setup you'll need some starting software:

- `git`, obviously.
- At least some version of .NET Core. Both .NET Core 3.1 or .NET Core 5.0 are OK, both is best!
  - Run `dotnet --info` in the CLI and make sure you get something
- Then you'll want the MonoGame Content Pipeline tool, run `dotnet tool install --global dotnet-mgcb-editor`
- Then `mgcb-editor --register`
- Next you'll need the MonoGame Project Templates, run `dotnet new --install MonoGame.Templates.CSharp`
- (Recommended) Visual Studio 2019
  - VS Code is OK but VS2019 is the best experience

### Actually setting up (Currently Windows only)

You will _not_ need to clone this repo to use Machina, you _can_, but won't need to. The following instructions require an **EMPTY** folder that has **NOT** been initialized with git.

1. Download the `auto-bootstrap.bat` script, should be at the root level of this repo
2. Put that script where you want your project to be.
3. Run the script in the command line in an **empty folder**. This will become your project folder. Run the script with the name of your poject as the first parameter (eg: `auto-bootstrap.bat MyCoolGame`)
4. This should do a bunch of stuff automagically, you now have some csproj files, an sln, and even a `.git` folder with an initial commit (unless something blew up)
