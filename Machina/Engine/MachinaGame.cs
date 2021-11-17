using System;
using System.IO;
using Machina.Components;
using Machina.Data;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine
{
    using Assets;
    using Machina.Engine.Cartridges;

    public enum DebugLevel
    {
        Off, // Completely disabled, can be enabled with hotkey
        Passive, // Show Console Output
        Active // Render DebugDraws
    }

    public static class MachinaClient
    {
        public static MachinaRuntime Runtime { get; private set; }

        public static void Setup(MachinaRuntime runtime)
        {
            Runtime = runtime;
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Runtime?.CurrentCartridge?.SceneLayers?.Logger.Log(objects);
            new StdOutConsoleLogger().Log(objects);
        }
    }

    public class MachinaGame : Game
    {
        /// <summary>
        /// Cartridge provided by client code
        /// </summary>
        public readonly GameCartridge gameCartridge;
        public static IAssetLibrary Assets { get; private set; }
        public static UIStyle defaultStyle;
        protected readonly MachinaGameSpecification specification;
        public MachinaRuntime Runtime => MachinaClient.Runtime;

        // MACHINA DESKTOP (lives in own Project, extends MachinaPlatform, which gets updated in Runtime)
        private MachinaWindow machinaWindow;
        private static MouseCursor pendingCursor;

        // Things that are going away (hopefully)
        public static NoiseBasedRNG RandomDirty = new NoiseBasedRNG((uint) DateTime.Now.Ticks & 0x0000FFFF);

        internal MachinaGame(MachinaGameSpecification specification, GameCartridge gameCartridge)
        {
            this.specification = specification;
            this.gameCartridge = gameCartridge;

            Content.RootDirectory = "Content";

            var graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            MachinaClient.Setup(new MachinaRuntime(this, graphics, this.specification));
            Assets = new AssetLibrary(this);
        }

        public static void SetCursor(MouseCursor cursor)
        {
            pendingCursor = cursor;
        }

        /// <summary>
        ///     Should only be used for tests
        /// </summary>
        /// <param name="assetLibrary"></param>
        public static void SetAssetLibrary(IAssetLibrary assetLibrary)
        {
            Assets = assetLibrary;
        }

        protected override void Initialize()
        {
            Window.Title = this.specification.gameTitle;
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // We cannot do this any earlier, GraphicsDevice doesn't exist until now
            Runtime.GraphicsDevice = GraphicsDevice;

            Console.Out.WriteLine("Constructing SpriteBatch");
            Runtime.spriteBatch = new SpriteBatch(Runtime.GraphicsDevice);

            this.machinaWindow = new MachinaWindow(this.specification.settings.startingWindowSize, Window, Runtime.Graphics, Runtime.GraphicsDevice);

            Console.Out.WriteLine("Applying settings");
            this.specification.settings.LoadSavedSettingsIfExist(Runtime.Graphics, Runtime.GraphicsDevice, Runtime.fileSystem);
            Console.Out.WriteLine("Settings Window Size");
            this.machinaWindow.SetWindowSize(this.specification.settings.startingWindowSize);

            var loadingCartridge = new LoadingScreenCartridge(this.specification.settings);
            Runtime.InsertCartridge(loadingCartridge, Window, this.machinaWindow);
            loadingCartridge.PrepareLoadingScreen(this.gameCartridge, Runtime, Assets as AssetLibrary, this.machinaWindow, FinishLoadingContent);
        }

        private void FinishLoadingContent()
        {
#if DEBUG
            Runtime.DebugLevel = DebugLevel.Passive;
#endif

            var defaultFont = Assets.GetSpriteFont("DefaultFontSmall");

            defaultStyle = new UIStyle(
                Assets.GetMachinaAsset<NinepatchSheet>("ui-button"),
                Assets.GetMachinaAsset<NinepatchSheet>("ui-button-hover"),
                Assets.GetMachinaAsset<NinepatchSheet>("ui-button-press"),
                Assets.GetMachinaAsset<NinepatchSheet>("ui-textbox-ninepatch"),
                Assets.GetMachinaAsset<NinepatchSheet>("ui-window-ninepatch"),
                Assets.GetMachinaAsset<NinepatchSheet>("ui-slider-ninepatch"),
                defaultFont,
                Assets.GetMachinaAsset<SpriteSheet>("ui-checkbox-radio-spritesheet"),
                Assets.GetMachinaAsset<Image>("ui-checkbox-checkmark-image"),
                Assets.GetMachinaAsset<Image>("ui-radio-fill-image")
            );

            if (Runtime.DebugLevel >= DebugLevel.Passive)
            {
                MachinaClient.Print("Debug build detected");
            }

            // Most cartridges get setup automatically but since the gamecartridge hasn't been inserted yet we have to do it early here
            this.gameCartridge.SetupSceneLayers(Runtime, specification, Window, machinaWindow);

            var debugActor = this.gameCartridge.SceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            var demoName = Demo.MostRecentlySavedDemoPath;
            var demoSpeed = 1;
            var shouldSkipSnapshot = Runtime.DebugLevel == DebugLevel.Off;

            this.specification.CommandLineArgs.RegisterEarlyFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("randomseed", SetRandomSeedFromString);
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demopath", arg => { demoName = arg; });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demo", arg =>
            {
                switch (arg)
                {
                    case "record":
                        new DemoRecorderComponent(debugActor, new Demo.Recorder(this.gameCartridge, demoName));
                        break;
                    case "playback":
                        Runtime.DemoPlayback = demoPlaybackComponent.SetDemo(this.gameCartridge, Demo.FromDisk_Sync(demoName, Runtime.fileSystem), demoName, demoSpeed);
                        break;
                    case "playback-nogui":
                        Runtime.DemoPlayback = demoPlaybackComponent.SetDemo(this.gameCartridge, Demo.FromDisk_Sync(demoName, Runtime.fileSystem), demoName, demoSpeed);
                        demoPlaybackComponent.ShowGui = false;
                        break;
                    default:
                        MachinaClient.Print("Unknown demo mode", arg);
                        break;
                }
            });

            this.specification.CommandLineArgs.RegisterEarlyFlagArg("debug",
                () => { Runtime.DebugLevel = DebugLevel.Active; });

            Runtime.SoundEffectPlayer = new SoundEffectPlayer(this.specification.settings);

#if DEBUG
            // PlayIntroAndLoadGame();
            InsertGameCartridgeAndRun();
#else
            PlayIntroAndLoadGame();
#endif
            // Currently we go [SetupDebugScene] -> [LoadGame] -> [LateSetup], hopefully the cartridge system will mitigate the need for this.
            if (GamePlatform.IsDesktop)
            {
                // NOTE: If we play the intro in a debug build this flag will not be honored, tech debt.
                new SnapshotTaker(debugActor, shouldSkipSnapshot);
            }
        }

        private void SetRandomSeedFromString(string seed)
        {
            this.gameCartridge.Random.Seed = (int) NoiseBasedRNG.SeedFromString(seed);
        }

        private void InsertGameCartridgeAndRun()
        {
            this.specification.CommandLineArgs.ExecuteEarlyArgs();
            Runtime.InsertCartridge(this.gameCartridge, Window, this.machinaWindow);
            this.specification.CommandLineArgs.ExecuteArgs();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            Runtime.spriteBatch.Dispose();
            Assets.UnloadAssets();
        }

        protected override void Update(GameTime gameTime)
        {
            pendingCursor = MouseCursor.Arrow;
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Runtime.Update(dt);

            Mouse.SetCursor(pendingCursor);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Runtime.Draw();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var scene in Runtime.CurrentCartridge.SceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }

        private void PlayIntroAndLoadGame()
        {
            // Steal control
            void OnEnd()
            {
                // Start the actual game
                InsertGameCartridgeAndRun();
            }
            Runtime.InsertCartridge(new IntroCartridge(this.specification.settings, OnEnd), Window, this.machinaWindow);
        }
    }
}