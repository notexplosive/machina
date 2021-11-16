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
    using Machina.Engine.Input;

    public enum DebugLevel
    {
        Off, // Completely disabled, can be enabled with hotkey
        Passive, // Show Console Output
        Active // Render DebugDraws
    }

    public class IntroCartridge : Cartridge
    {
        private readonly Action onEnd;

        public static Point RenderResolution(GameSettings settings)
        {
            const int desiredWidth = 1920 / 4;
            var AspectRatio = (float) settings.startingWindowSize.X / desiredWidth;
            return new Vector2(settings.startingWindowSize.X / AspectRatio, settings.startingWindowSize.Y / AspectRatio).ToPoint();
        }

        public IntroCartridge(GameSettings settings, Action onEnd) : base(RenderResolution(settings), ResizeBehavior.MaintainDesiredResolution)
        {
            this.onEnd = onEnd;
        }

        public override void OnGameLoad(MachinaGameSpecification specification)
        {
            var introScene = SceneLayers.AddNewScene();

            var textActor = introScene.AddActor("text");
            new BoundingRect(textActor, 20, 20);
            new BoundingRectToViewportSize(textActor);
            new BoundedTextRenderer(textActor, "", MachinaGame.Assets.GetSpriteFont("LogoFont"), Color.White,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            new IntroTextAnimation(textActor);
            new CallbackOnDestroy(textActor, onEnd);
        }
    }

    /// <summary>
    ///     Derive your Game class from MachinaGame and then populate the PostLoadContent with your code.
    ///     Your game should call the base constructor, even though it's abstract.
    /// </summary>
    public abstract class MachinaGame : Game
    {
        // PLATFORM (one of, internal)
        /// <summary>
        ///     Path to users AppData folder (or platform equivalent)
        /// </summary>
        public readonly string appDataPath;
        /// <summary>
        /// Cartridge provided by client code
        /// </summary>
        public readonly Cartridge gameCartridge;
        public static IAssetLibrary Assets { get; private set; }
        public static UIStyle defaultStyle;


        // MACHINA DESKTOP (lives in own Project, extends MachinaPlatform, which gets updated in Runtime)
        private readonly MachinaWindow machinaWindow;
        private static MouseCursor pendingCursor;


        // RUNTIME (one of, internal)
        public static SoundEffectPlayer SoundEffectPlayer;
        public static readonly FrameStep GlobalFrameStep = new FrameStep();
        private SpriteBatch spriteBatch;
        public static DebugLevel DebugLevel { get; set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        private Demo.Playback DemoPlayback { get; set; }
        private readonly MachinaInput input = new MachinaInput();
        /// <summary>
        /// Currently loaded cartridge
        /// </summary>
        public Cartridge CurrentCartridge
        {
            get => this.cartridge;
            set
            {
                this.cartridge = value;
                this.cartridge.Setup(GraphicsDevice, this.specification, Window, this.machinaWindow);
                this.cartridge.CurrentGameCanvas.SetWindowSize(this.machinaWindow.CurrentWindowSize);
            }
        }
        private Cartridge cartridge;


        // Loading Screen Cartridge
        private LoadingScreen loadingScreen;

        // Things that are going away (hopefully)
        public static MachinaGame Current { get; private set; }

        protected readonly MachinaGameSpecification specification;
        private bool isDoneUpdateLoading = false;
        public static NoiseBasedRNG RandomDirty = new NoiseBasedRNG((uint) DateTime.Now.Ticks & 0x0000FFFF);

        protected MachinaGame(MachinaGameSpecification specification, Cartridge gameCartridge)
        {
            Current = this;
            this.specification = specification;
            this.gameCartridge = gameCartridge;

            // TODO: I don't think this works on Android; also this should be moved to GamePlatform.cs
            this.appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", this.specification.gameTitle);

            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };


            this.machinaWindow = new MachinaWindow(this.specification.settings.startingWindowSize, Window, Graphics, GraphicsDevice);

            Assets = new Assets.AssetLibrary(this);
        }

        public static Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture)
        {
            if (rect.Width * rect.Height == 0)
            {
                return null;
            }

            var cropTexture = new Texture2D(Current.GraphicsDevice, rect.Width, rect.Height);
            var data = new Color[rect.Width * rect.Height];
            sourceTexture.GetData(0, rect, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
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
            Console.Out.WriteLine("Settings Window Size");
            this.machinaWindow.SetWindowSize(this.specification.settings.startingWindowSize);
            Console.Out.WriteLine("Constructing SpriteBatch");
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            SetupLoadingScreen();
        }

        private void FinishLoadingContent()
        {
#if DEBUG
            DebugLevel = DebugLevel.Passive;
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

            if (DebugLevel >= DebugLevel.Passive)
            {
                Print("Debug build detected");
            }


            // Most cartridges get setup automatically but since the gamecartridge hasn't been inserted yet we have to do it early here
            this.gameCartridge.SetupSceneLayers();

            var debugActor = this.gameCartridge.SceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            var demoName = Demo.MostRecentlySavedDemoPath;
            var demoSpeed = 1;
            var shouldSkipSnapshot = DebugLevel == DebugLevel.Off;

            this.specification.CommandLineArgs.RegisterEarlyFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("randomseed", SetRandomSeedFromString);
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demopath", arg => { demoName = arg; });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });
            this.specification.CommandLineArgs.RegisterEarlyValueArg("demo", arg =>
            {
                switch (arg)
                {
                    case "record":
                        new DemoRecorderComponent(debugActor, new Demo.Recorder(demoName));
                        break;
                    case "playback":
                        DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, demoSpeed);
                        break;
                    case "playback-nogui":
                        DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, demoSpeed);
                        demoPlaybackComponent.ShowGui = false;
                        break;
                    default:
                        MachinaGame.Print("Unknown demo mode", arg);
                        break;
                }
            });

            this.specification.settings.LoadSavedSettingsIfExist();

            this.specification.CommandLineArgs.RegisterEarlyFlagArg("debug",
                () => { DebugLevel = DebugLevel.Active; });

            SoundEffectPlayer = new SoundEffectPlayer(this.specification.settings);


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

            this.isDoneUpdateLoading = true;
        }

        private void SetRandomSeedFromString(string seed)
        {
            this.gameCartridge.Random.Seed = (int) NoiseBasedRNG.SeedFromString(seed);
        }

        public void RunDemo(string demoName)
        {
            var demoActor = CurrentCartridge.SceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
            DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, 1);
            demoPlaybackComponent.ShowGui = false;
        }

        private void PrepareLoadInitialStyle(AssetLoadTree loadTree)
        {
            loadTree.AddMachinaAssetCallback("ui-button",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-button-hover",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24),
                    new Rectangle(8 + 24, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-button-press",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24),
                    new Rectangle(8 + 48, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-slider-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24),
                    new Rectangle(8, 152, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-checkbox-checkmark-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            loadTree.AddMachinaAssetCallback("ui-radio-fill-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            loadTree.AddMachinaAssetCallback("ui-checkbox-radio-spritesheet",
                () => new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            loadTree.AddMachinaAssetCallback("ui-textbox-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24),
                    new Rectangle(8, 104, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-window-ninepatch",
                () => new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52)));
        }

        private void InsertGameCartridgeAndRun()
        {
            this.specification.CommandLineArgs.ExecuteEarlyArgs();
            CurrentCartridge = this.gameCartridge;
            this.specification.CommandLineArgs.ExecuteArgs();
            CurrentCartridge.CurrentGameCanvas.SetWindowSize(this.machinaWindow.CurrentWindowSize);
            Graphics.ApplyChanges();
        }

        public static void Quit()
        {
            Current.Exit();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            this.spriteBatch.Dispose();
            Assets.UnloadAssets();
        }

        protected override void Update(GameTime gameTime)
        {
            pendingCursor = MouseCursor.Arrow;
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (!this.isDoneUpdateLoading)
            {
                var library = Assets as Assets.AssetLibrary;
                var increment = 3;
                for (var i = 0; i < increment; i++)
                {
                    this.loadingScreen.Update(library, dt / increment);
                }
            }
            else if (!this.loadingScreen.IsDoneDrawLoading())
            {
                // waiting for draw load
            }
            else
            {
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

            Mouse.SetCursor(pendingCursor);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!this.isDoneUpdateLoading)
            {
                this.loadingScreen.Draw(this.spriteBatch, this.machinaWindow.CurrentWindowSize, GraphicsDevice);
            }
            else if (!this.loadingScreen.IsDoneDrawLoading())
            {
                this.loadingScreen.IncrementDrawLoopLoad(Assets as Assets.AssetLibrary, this.spriteBatch);
                this.loadingScreen.Draw(this.spriteBatch, this.machinaWindow.CurrentWindowSize, GraphicsDevice);
            }
            else
            {
                CurrentCartridge.SceneLayers.PreDraw(this.spriteBatch);
                CurrentCartridge.CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
                GraphicsDevice.Clear(CurrentCartridge.SceneLayers.BackgroundColor);

                CurrentCartridge.SceneLayers.DrawOnCanvas(this.spriteBatch);
                CurrentCartridge.CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, this.spriteBatch);
                CurrentCartridge.SceneLayers.DrawDebugScene(this.spriteBatch);
            }

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var scene in CurrentCartridge.SceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }

        private void SetupLoadingScreen()
        {
            var assetTree = AssetLibrary.GetStaticAssetLoadTree();
            this.gameCartridge.PrepareDynamicAssets(assetTree, GraphicsDevice);
            PrepareLoadInitialStyle(assetTree);

            this.loadingScreen =
                new LoadingScreen(assetTree, FinishLoadingContent);
        }

        private void PlayIntroAndLoadGame()
        {
            // Steal control
            void OnEnd()
            {
                // Start the actual game
                InsertGameCartridgeAndRun();
            }
            CurrentCartridge = new IntroCartridge(this.specification.settings, OnEnd);
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Current?.CurrentCartridge?.SceneLayers?.Logger.Log(objects);
            new StdOutConsoleLogger().Log(objects);
        }
    }
}