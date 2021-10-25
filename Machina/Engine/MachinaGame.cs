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
    public enum DebugLevel
    {
        Off, // Completely disabled, can be enabled with hotkey
        Passive, // Show Console Output
        Active // Render DebugDraws
    }

    /// <summary>
    ///     Derive your Game class from MachinaGame and then populate the PostLoadContent with your code.
    ///     Your game should call the base constructor, even though it's abstract.
    /// </summary>
    public abstract class MachinaGame : Game
    {
        public static UIStyle defaultStyle;
        private static MouseCursor pendingCursor;

        public static readonly FrameStep GlobalFrameStep = new FrameStep();

        /// <summary>
        ///     Path to users AppData folder (or platform equivalent)
        /// </summary>
        public readonly string appDataPath;

        public readonly string gameTitle;

        private readonly KeyTracker keyTracker = new KeyTracker();
        private readonly MouseTracker mouseTracker = new MouseTracker();
        private readonly Point startingRenderResolution;

        private readonly ResizeBehavior startingResizeBehavior;

        protected readonly Point startingWindowSize;
        private readonly SingleFingerTouchTracker touchTracker = new SingleFingerTouchTracker();
        private Point currentWindowSize;
        private SceneLayers sceneLayers;
        protected SpriteBatch spriteBatch;

        protected MachinaGame(string gameTitle, string[] args, Point startingRenderResolution, Point startingWindowSize,
            ResizeBehavior resizeBehavior)
        {
            MachinaGame.Current = this;
            this.startingResizeBehavior = resizeBehavior;
            this.startingRenderResolution = startingRenderResolution;

            this.gameTitle = gameTitle;
            MachinaGame.CommandLineArgs = new CommandLineArgs(args);

            // TODO: I don't think this works on Android; also this should be moved to GamePlatform.cs
            this.appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", this.gameTitle);

            this.startingWindowSize = startingWindowSize;
            this.currentWindowSize = startingWindowSize;

            Content.RootDirectory = "Content";
            MachinaGame.Graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            MachinaGame.Assets = new AssetLibrary(this);
            MachinaGame.Random = new SeededRandom();
            
            SceneLayers = new SceneLayers(false,
                new GameCanvas(this.startingRenderResolution, this.startingResizeBehavior));
        }

        public static CommandLineArgs CommandLineArgs { get; private set; }

        public SceneLayers SceneLayers
        {
            get => this.sceneLayers;
            set
            {
                this.sceneLayers = value;
                CurrentGameCanvas.SetWindowSize(this.currentWindowSize);
            }
        }

        public GameCanvas CurrentGameCanvas => this.sceneLayers.gameCanvas as GameCanvas;

        public static SeededRandom Random { get; private set; }

        public static DebugLevel DebugLevel { get; set; }

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static IAssetLibrary Assets { get; private set; }

        public static MachinaGame Current { get; private set; }

        public Demo.Playback DemoPlayback { get; private set; }

        public static bool Fullscreen
        {
            set
            {
                if (value)
                {
                    MachinaGame.SetWindowSize(new Point(MachinaGame.Current.GraphicsDevice.DisplayMode.Width,
                        MachinaGame.Current.GraphicsDevice.DisplayMode.Height));
                    MachinaGame.Graphics.IsFullScreen = true;
                }
                else
                {
                    MachinaGame.SetWindowSize(MachinaGame.Current.startingWindowSize);
                    MachinaGame.Graphics.IsFullScreen = false;
                }

                MachinaGame.Graphics.ApplyChanges();
            }

            get => MachinaGame.Graphics.IsFullScreen;
        }

        public static SamplerState SamplerState { get; set; } = SamplerState.PointClamp;

        /// <summary>
        ///     TEST ONLY!!
        /// </summary>
        public static void SetSeededRandom(int seed)
        {
            var random = new SeededRandom
            {
                Seed = seed
            };
            MachinaGame.Random = random;
        }

        public static Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture)
        {
            if (rect.Width * rect.Height == 0)
            {
                return null;
            }

            var cropTexture = new Texture2D(MachinaGame.Current.GraphicsDevice, rect.Width, rect.Height);
            var data = new Color[rect.Width * rect.Height];
            sourceTexture.GetData(0, rect, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
        }

        public static void SetCursor(MouseCursor cursor)
        {
            MachinaGame.pendingCursor = cursor;
        }

        /// <summary>
        ///     Should only be used for tests
        /// </summary>
        /// <param name="assetLibrary"></param>
        public static void SetAssetLibrary(IAssetLibrary assetLibrary)
        {
            MachinaGame.Assets = assetLibrary;
        }

        protected static void SetWindowSize(Point windowSize)
        {
            MachinaGame.Print("Window size changed to", windowSize);
            MachinaGame.Graphics.PreferredBackBufferWidth = windowSize.X;
            MachinaGame.Graphics.PreferredBackBufferHeight = windowSize.Y;
            MachinaGame.Graphics.ApplyChanges();
            MachinaGame.Current.CurrentGameCanvas.SetWindowSize(new Point(windowSize.X, windowSize.Y));
        }

        protected override void Initialize()
        {
            Window.Title = this.gameTitle;
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            MachinaGame.Assets.LoadAllContent();
            Console.Out.WriteLine("Loading initial style");
            LoadInitialStyle();

#if DEBUG
            MachinaGame.DebugLevel = DebugLevel.Passive;
#endif

            Console.Out.WriteLine("Building SceneLayers");
            SceneLayers = new SceneLayers(true,
                new GameCanvas(this.startingRenderResolution, this.startingResizeBehavior));
            Console.Out.WriteLine("Building Canvas");
            CurrentGameCanvas.BuildCanvas(GraphicsDevice);
            Console.Out.WriteLine("Constructing SpriteBatch");
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            
            Console.Out.WriteLine("Settings Window Size");
            MachinaGame.SetWindowSize(this.startingWindowSize);

            if (GamePlatform.IsDesktop)
            {
                Window.TextInput += SceneLayers.AddPendingTextInput;
            }

            if (MachinaGame.DebugLevel >= DebugLevel.Passive)
            {
                MachinaGame.Print("Debug build detected");
            }

            var debugActor = this.sceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            MachinaGame.CommandLineArgs.RegisterEarlyValueArg("randomseed",
                arg => { MachinaGame.Random.Seed = (int) NoiseBasedRNG.SeedFromString(arg); });

            var demoName = Demo.MostRecentlySavedDemoPath;
            MachinaGame.CommandLineArgs.RegisterValueArg("demopath", arg => { demoName = arg; });

            var demoSpeed = 1;
            MachinaGame.CommandLineArgs.RegisterValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });

            MachinaGame.CommandLineArgs.RegisterEarlyFlagArg("debug",
                () => { MachinaGame.DebugLevel = DebugLevel.Active; });

            MachinaGame.CommandLineArgs.RegisterValueArg("demo", arg =>
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

            var shouldSkipSnapshot = MachinaGame.DebugLevel == DebugLevel.Off;
            MachinaGame.CommandLineArgs.RegisterFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });

#if DEBUG
            LoadGame();
#else
            PlayIntroAndLoadGame();
#endif

            if (GamePlatform.IsDesktop)
            {
                new SnapshotTaker(debugActor, shouldSkipSnapshot);
            }
        }

        public void RunDemo(string demoName)
        {
            var demoActor = this.sceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
            DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, 1);
            demoPlaybackComponent.ShowGui = false;
        }

        private void LoadInitialStyle()
        {
            // Load initial assets
            MachinaGame.Assets.AddMachinaAsset("ui-button",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8)));
            MachinaGame.Assets.AddMachinaAsset("ui-button-hover",
                new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24), new Rectangle(8 + 24, 8, 8, 8)));
            MachinaGame.Assets.AddMachinaAsset("ui-button-press",
                new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24), new Rectangle(8 + 48, 8, 8, 8)));
            MachinaGame.Assets.AddMachinaAsset("ui-slider-ninepatch",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24), new Rectangle(8, 152, 8, 8)));
            MachinaGame.Assets.AddMachinaAsset("ui-checkbox-checkmark-image",
                new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            MachinaGame.Assets.AddMachinaAsset("ui-radio-fill-image",
                new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            MachinaGame.Assets.AddMachinaAsset("ui-checkbox-radio-spritesheet",
                new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            MachinaGame.Assets.AddMachinaAsset("ui-textbox-ninepatch",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24), new Rectangle(8, 104, 8, 8)));
            MachinaGame.Assets.AddMachinaAsset("ui-window-ninepatch",
                new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52)));

            var defaultFont = MachinaGame.Assets.GetSpriteFont("DefaultFontSmall");

            MachinaGame.defaultStyle = new UIStyle(
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-button"),
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-hover"),
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-press"),
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-textbox-ninepatch"),
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-window-ninepatch"),
                MachinaGame.Assets.GetMachinaAsset<NinepatchSheet>("ui-slider-ninepatch"),
                defaultFont,
                MachinaGame.Assets.GetMachinaAsset<SpriteSheet>("ui-checkbox-radio-spritesheet"),
                MachinaGame.Assets.GetMachinaAsset<Image>("ui-checkbox-checkmark-image"),
                MachinaGame.Assets.GetMachinaAsset<Image>("ui-radio-fill-image")
            );
        }

        private void LoadGame()
        {
            MachinaGame.CommandLineArgs.ExecuteEarlyArgs();
            OnGameLoad();
            MachinaGame.CommandLineArgs.ExecuteArgs();
        }

        public static void Quit()
        {
            MachinaGame.Current.Exit();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            this.spriteBatch.Dispose();
            MachinaGame.Assets.UnloadAssets();
        }

        protected abstract void OnGameLoad();

        protected override void Update(GameTime gameTime)
        {
            MachinaGame.pendingCursor = MouseCursor.Arrow;
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (DemoPlayback != null && DemoPlayback.IsFinished == false)
            {
                for (var i = 0; i < DemoPlayback.playbackSpeed; i++)
                {
                    var frameState = DemoPlayback.UpdateAndGetInputFrameStates(dt);
                    DemoPlayback.PollHumanInput(GetHumanFrameState());
                    this.sceneLayers.Update(dt, Matrix.Identity, frameState);
                }
            }
            else
            {
                this.sceneLayers.Update(dt, Matrix.Identity, GetHumanFrameState());
            }

            Mouse.SetCursor(MachinaGame.pendingCursor);
            base.Update(gameTime);
        }

        private InputFrameState GetHumanFrameState()
        {
            var inputState = InputState.RawHumanInput;

            MouseFrameState mouseFrameState;

            if (GamePlatform.IsMobile)
            {
                mouseFrameState = this.touchTracker.CalculateFrameState(inputState.touches);
            }
            else
            {
                mouseFrameState = this.mouseTracker.CalculateFrameState(inputState.mouseState);
            }

            return new InputFrameState(
                this.keyTracker.CalculateFrameState(inputState.keyboardState, inputState.gamepadState),
                mouseFrameState);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.sceneLayers.PreDraw(this.spriteBatch);
            CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
            GraphicsDevice.Clear(this.sceneLayers.BackgroundColor);

            this.sceneLayers.DrawOnCanvas(this.spriteBatch);
            CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, this.spriteBatch);
            this.sceneLayers.DrawDebugScene(this.spriteBatch);
            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            this.currentWindowSize = windowSize;
            CurrentGameCanvas.SetWindowSize(windowSize);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var scene in this.sceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }

        private void PlayIntroAndLoadGame()
        {
            var oldSceneLayers = SceneLayers;
            var windowSize = this.startingWindowSize;
            var desiredWidth = 1920 / 4;
            var ratio = (float) windowSize.X / desiredWidth;
            var gameCanvas = new GameCanvas(new Vector2(windowSize.X / ratio, windowSize.Y / ratio).ToPoint(),
                ResizeBehavior.MaintainDesiredResolution);
            gameCanvas.BuildCanvas(GraphicsDevice);
            var introLayers = new SceneLayers(true, gameCanvas);
            var introScene = introLayers.AddNewScene();

            introLayers.BackgroundColor = Color.Black;
            var textActor = introScene.AddActor("text");
            var boundingRect = new BoundingRect(textActor, 20, 20);
            new BoundingRectToViewportSize(textActor);
            new BoundedTextRenderer(textActor, "", MachinaGame.Assets.GetSpriteFont("LogoFont"), Color.White,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            new IntroTextAnimation(textActor);

            // Steal control
            SceneLayers = introLayers;

            void onEnd()
            {
                // Start the actual game
                SceneLayers = oldSceneLayers;
                LoadGame();
            }

            new CallbackOnDestroy(textActor, onEnd);
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            MachinaGame.Current?.SceneLayers?.Logger.Log(objects);
            new StdOutConsoleLogger().Log(objects);
        }
    }
}
