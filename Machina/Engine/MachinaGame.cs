using System;
using System.IO;
using Machina.Components;
using Machina.Data;
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

    public class MachinaGame : Game
    {
        /// <summary>
        /// Cartridge provided by client code
        /// </summary>
        public readonly GameCartridge gameCartridge;
        public static UIStyle defaultStyle;
        protected readonly GameSpecification specification;
        public MachinaRuntime Runtime => MachinaClient.Runtime;

        // MACHINA DESKTOP (lives in own Project, extends MachinaPlatform, which gets updated in Runtime)
        private MachinaWindow machinaWindow;
        private static MouseCursor pendingCursor;

        public MachinaGame(GameSpecification specification, GameCartridge gameCartridge, IPlatformContext platformContext)
        {
            this.specification = specification;
            this.gameCartridge = gameCartridge;
            Content.RootDirectory = "Content";

            var graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            MachinaClient.Setup(new MachinaRuntime(this, graphics, this.specification, platformContext), new AssetLibrary(this));
            platformContext.OnGameConstructed(this);
        }

        public static void SetCursor(MouseCursor cursor)
        {
            pendingCursor = cursor;
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
            this.specification.settings.LoadSavedSettingsIfExist(Runtime.fileSystem);
            Console.Out.WriteLine("Settings Window Size");
            this.machinaWindow.SetWindowSize(this.specification.settings.startingWindowSize);

            var loadingCartridge = new LoadingScreenCartridge(this.specification.settings);
            Runtime.InsertCartridge(loadingCartridge, Window, this.machinaWindow);
            loadingCartridge.PrepareLoadingScreen(this.gameCartridge, Runtime, MachinaClient.Assets as AssetLibrary, this.machinaWindow, FinishLoadingContent);
        }

        private void FinishLoadingContent()
        {
#if DEBUG
            Runtime.DebugLevel = DebugLevel.Passive;
#endif

            var defaultFont = MachinaClient.Assets.GetSpriteFont("DefaultFontSmall");

            defaultStyle = new UIStyle(
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-hover"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-press"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-textbox-ninepatch"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-window-ninepatch"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-slider-ninepatch"),
                defaultFont,
                MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("ui-checkbox-radio-spritesheet"),
                MachinaClient.Assets.GetMachinaAsset<Image>("ui-checkbox-checkmark-image"),
                MachinaClient.Assets.GetMachinaAsset<Image>("ui-radio-fill-image")
            );

            // Most cartridges get setup automatically but since the gamecartridge hasn't been inserted yet we have to do it early here
            this.gameCartridge.SetupSceneLayers(Runtime, specification, Window, machinaWindow);

            var debugActor = this.gameCartridge.SceneLayers.DebugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            var demoName = Demo.MostRecentlySavedDemoPath;
            var demoSpeed = 1;
            var shouldSkipSnapshot = Runtime.DebugLevel == DebugLevel.Off;

            this.specification.commandLineArgs.RegisterEarlyFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });
            this.specification.commandLineArgs.RegisterEarlyValueArg("randomseed", SetRandomSeedFromString);
            this.specification.commandLineArgs.RegisterEarlyValueArg("demopath", arg => { demoName = arg; });
            this.specification.commandLineArgs.RegisterEarlyValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });
            this.specification.commandLineArgs.RegisterEarlyValueArg("demo", arg =>
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

            this.specification.commandLineArgs.RegisterEarlyFlagArg("debug",
                () => { Runtime.DebugLevel = DebugLevel.Active; });

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
            this.gameCartridge.Random.Seed = (int)NoiseBasedRNG.SeedFromString(seed);
        }

        private void InsertGameCartridgeAndRun()
        {
            this.specification.commandLineArgs.ExecuteEarlyArgs();
            Runtime.InsertCartridge(this.gameCartridge, Window, this.machinaWindow);
            this.specification.commandLineArgs.ExecuteArgs();

            if (Runtime.DebugLevel >= DebugLevel.Passive)
            {
                MachinaClient.Print("Debug build detected");
            }
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            Runtime.spriteBatch.Dispose();
            MachinaClient.Assets.UnloadAssets();
        }

        protected override void Update(GameTime gameTime)
        {
            pendingCursor = MouseCursor.Arrow;
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
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