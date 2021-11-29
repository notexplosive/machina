using Machina.Components;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    using Assets;
    using Machina.Data;
    using Machina.Engine.Cartridges;
    using Machina.Engine.Input;
    using System;
    using System.IO;

    public class MachinaRuntime : IMachinaRuntime
    {
        private readonly GameSpecification specification;
        private readonly MachinaGame game;
        public readonly IPlatformContext platformContext;
        public readonly MachinaInput input = new MachinaInput();
        public WindowInterface WindowInterface { get; private set; }
        public Painter Painter { get; }

        public MachinaRuntime(MachinaGame game, GameSpecification specification, IPlatformContext platformContext, Painter painter)
        {
            this.specification = specification;
            this.game = game;
            this.platformContext = platformContext;
            this.Painter = painter;
        }

        public Demo.Playback DemoPlayback { get; set; }
        public DebugLevel DebugLevel { get; set; }

        public void InsertCartridge(Cartridge cartridge)
        {
            CurrentCartridge = cartridge;
            CurrentCartridge.Setup(this, this.specification);
            CurrentCartridge.CurrentGameCanvas.SetWindowSize(WindowInterface.CurrentWindowSize);
            MachinaClient.Graphics.ApplyChanges();
        }

        public void Quit()
        {
            this.game.Exit();
        }

        public Cartridge CurrentCartridge { get; private set; }

        public void RunDemo(string demoName)
        {
            if (CurrentCartridge is GameCartridge gameCartridge)
            {
                var demoActor = CurrentCartridge.SceneLayers.DebugScene.AddActor("DebugActor");
                var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
                DemoPlayback = demoPlaybackComponent.SetDemo(gameCartridge, Demo.FromDisk_Sync(demoName, MachinaClient.FileSystem), demoName, 1);
                demoPlaybackComponent.ShowGui = false;
            }
            else
            {
                MachinaClient.Print("Demo loading is only supported on GameCartridges");
            }
        }

        public void LateSetup(GameCartridge gameCartridge, WindowInterface windowInterface)
        {
            Console.Out.WriteLine("Constructing SpriteBatch");
            this.WindowInterface = windowInterface;

            Console.Out.WriteLine("Applying settings");
            this.specification.settings.LoadSavedSettingsIfExist(MachinaClient.FileSystem, windowInterface);
            Console.Out.WriteLine("Settings Window Size");
            this.WindowInterface.SetWindowSize(this.specification.settings.startingWindowSize);

            var loadingCartridge = new LoadingScreenCartridge(this.specification.settings);
            InsertCartridge(loadingCartridge);
            loadingCartridge.PrepareLoadingScreen(gameCartridge, this, MachinaClient.Assets as AssetLibrary, this.WindowInterface, FinishLoadingContent);
        }

        internal void Draw()
        {
            CurrentCartridge.SceneLayers.PreDraw(this.Painter.SpriteBatch);
            CurrentCartridge.CurrentGameCanvas.SetRenderTargetToCanvas(this.Painter);
            this.Painter.Clear(CurrentCartridge.SceneLayers.BackgroundColor);

            CurrentCartridge.SceneLayers.DrawOnCanvas(this.Painter.SpriteBatch);
            CurrentCartridge.CurrentGameCanvas.DrawCanvasToScreen(this, this.Painter);
            CurrentCartridge.SceneLayers.DrawDebugScene(this.Painter.SpriteBatch);
        }

        internal void Update(float dt)
        {
            FlushAndPrintLogBuffer();

            if (DemoPlayback != null && DemoPlayback.IsFinished == false)
            {
                for (var i = 0; i < DemoPlayback.playbackSpeed; i++)
                {
                    var frameState = DemoPlayback.UpdateAndGetInputFrameStates(dt);
                    DemoPlayback.PollHumanInput(this.input.GetHumanFrameState());
                    CurrentCartridge.SceneLayers.Update(dt, Matrix.Identity, frameState);
                }
            }
            else
            {
                CurrentCartridge.SceneLayers.Update(dt, Matrix.Identity, this.input.GetHumanFrameState());
            }
        }

        public void FlushAndPrintLogBuffer()
        {
            if (CurrentCartridge != null)
            {
                var messages = MachinaClient.LogBuffer.FlushAllMessages();
                foreach (var message in messages)
                {
                    CurrentCartridge.Logger.Log(message);
                }
            }
        }

        private void FinishLoadingContent(GameCartridge gameCartridge)
        {
#if DEBUG
            DebugLevel = DebugLevel.Passive;
#endif

            // Most cartridges get setup automatically but since the gamecartridge hasn't been inserted yet we have to do it early here
            gameCartridge.SetupSceneLayers(this, specification, WindowInterface);

            var debugActor = gameCartridge.SceneLayers.DebugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            var demoName = Demo.MostRecentlySavedDemoPath;
            var demoSpeed = 1;
            var shouldSkipSnapshot = DebugLevel == DebugLevel.Off;

            void SetRandomSeedFromString(string seed)
            {
                gameCartridge.Random.Seed = (int) NoiseBasedRNG.SeedFromString(seed);
            }

            this.specification.commandLineArgs.RegisterEarlyFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });
            this.specification.commandLineArgs.RegisterEarlyValueArg("randomseed", SetRandomSeedFromString);
            this.specification.commandLineArgs.RegisterEarlyValueArg("demopath", arg => { demoName = arg; });
            this.specification.commandLineArgs.RegisterEarlyValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });
            this.specification.commandLineArgs.RegisterEarlyValueArg("demo", arg =>
            {
                switch (arg)
                {
                    case "record":
                        new DemoRecorderComponent(debugActor, new Demo.Recorder(gameCartridge, demoName));
                        break;
                    case "playback":
                        DemoPlayback = demoPlaybackComponent.SetDemo(gameCartridge, Demo.FromDisk_Sync(demoName, MachinaClient.FileSystem), demoName, demoSpeed);
                        break;
                    case "playback-nogui":
                        DemoPlayback = demoPlaybackComponent.SetDemo(gameCartridge, Demo.FromDisk_Sync(demoName, MachinaClient.FileSystem), demoName, demoSpeed);
                        demoPlaybackComponent.ShowGui = false;
                        break;
                    default:
                        MachinaClient.Print("Unknown demo mode", arg);
                        break;
                }
            });

            this.specification.commandLineArgs.RegisterEarlyFlagArg("debug",
                () => { DebugLevel = DebugLevel.Active; });

#if DEBUG
            // PlayIntroAndLoadGame(gameCartridge);
            InsertGameCartridgeAndRun(gameCartridge);
#else
            PlayIntroAndLoadGame(gameCartridge);
#endif
            // Currently we go [SetupDebugScene] -> [LoadGame] -> [LateSetup], hopefully the cartridge system will mitigate the need for this.
            if (GamePlatform.IsDesktop)
            {
                // NOTE: If we play the intro in a debug build this flag will not be honored, tech debt.
                new SnapshotTaker(debugActor, shouldSkipSnapshot);
            }
        }

        private void InsertGameCartridgeAndRun(GameCartridge gameCartridge)
        {
            this.specification.commandLineArgs.ExecuteEarlyArgs();
            InsertCartridge(gameCartridge);
            this.specification.commandLineArgs.ExecuteArgs();

            if (DebugLevel >= DebugLevel.Passive)
            {
                MachinaClient.Print("Debug build detected");
            }
        }


        private void PlayIntroAndLoadGame(GameCartridge gameCartridge)
        {
            // Steal control
            void OnEnd()
            {
                // Start the actual game
                InsertGameCartridgeAndRun(gameCartridge);
            }
            InsertCartridge(new IntroCartridge(this.specification.settings, OnEnd));
        }
    }
}