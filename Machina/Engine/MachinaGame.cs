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
    using AssetLibrary;
    using MonoGame.Extended;

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
        protected virtual GameSettings StartingSettings => new GameSettings();
        public static SoundEffectPlayer SoundEffectPlayer;
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
        private SceneLayers sceneLayers;
        private SpriteBatch spriteBatch;
        private LoadingScreen loadingScreen;
        private bool isDoneUpdateLoading = false;

        protected MachinaGame(string gameTitle, string[] args, Point startingRenderResolution, Point startingWindowSize,
            ResizeBehavior resizeBehavior)
        {
            Current = this;
            startingResizeBehavior = resizeBehavior;
            this.startingRenderResolution = startingRenderResolution;

            this.gameTitle = gameTitle;
            CommandLineArgs = new CommandLineArgs(args);

            // TODO: I don't think this works on Android; also this should be moved to GamePlatform.cs
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", this.gameTitle);

            this.startingWindowSize = startingWindowSize;

            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            Assets = new AssetLibrary.AssetLibrary(this);
            Random = new SeededRandom();

            SceneLayers = new SceneLayers(false,
                new GameCanvas(this.startingRenderResolution, startingResizeBehavior));
        }

        protected static CommandLineArgs CommandLineArgs { get; private set; }

        public SceneLayers SceneLayers
        {
            get => sceneLayers;
            set
            {
                sceneLayers = value;
                CurrentGameCanvas.SetWindowSize(CurrentWindowSize);
            }
        }

        public Point CurrentWindowSize => new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

        public GameCanvas CurrentGameCanvas => sceneLayers.gameCanvas as GameCanvas;

        public static SeededRandom Random { get; private set; }

        public static DebugLevel DebugLevel { get; set; }

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static IAssetLibrary Assets { get; private set; }

        public static MachinaGame Current { get; private set; }

        private Demo.Playback DemoPlayback { get; set; }

        public static bool Fullscreen
        {
            set
            {
                if (value)
                {
                    SetWindowSize(new Point(Current.GraphicsDevice.DisplayMode.Width,
                        Current.GraphicsDevice.DisplayMode.Height));
                    Graphics.IsFullScreen = true;
                }
                else
                {
                    SetWindowSize(Current.startingWindowSize);
                    Graphics.IsFullScreen = false;
                }

                Graphics.ApplyChanges();
            }

            get => Graphics.IsFullScreen;
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
            Random = random;
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

        protected static void SetWindowSize(Point windowSize)
        {
            Print("Window size changed to", windowSize);
            Graphics.PreferredBackBufferWidth = windowSize.X;
            Graphics.PreferredBackBufferHeight = windowSize.Y;
            Graphics.ApplyChanges();
            Current.CurrentGameCanvas.SetWindowSize(new Point(windowSize.X, windowSize.Y));
        }

        protected override void Initialize()
        {
            Window.Title = gameTitle;
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Console.Out.WriteLine("Settings Window Size");
            SetWindowSize(startingWindowSize);
            Console.Out.WriteLine("Constructing SpriteBatch");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
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

            Console.Out.WriteLine("Building SceneLayers");
            SceneLayers = new SceneLayers(true,
                new GameCanvas(startingRenderResolution, startingResizeBehavior));
            Console.Out.WriteLine("Building Canvas");
            CurrentGameCanvas.BuildCanvas(GraphicsDevice);

            if (GamePlatform.IsDesktop)
            {
                Window.TextInput += SceneLayers.AddPendingTextInput;
            }

            if (DebugLevel >= DebugLevel.Passive)
            {
                Print("Debug build detected");
            }

            var debugActor = sceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);

            CommandLineArgs.RegisterEarlyValueArg("randomseed",
                arg => { Random.Seed = (int) NoiseBasedRNG.SeedFromString(arg); });

            var demoName = Demo.MostRecentlySavedDemoPath;
            CommandLineArgs.RegisterValueArg("demopath", arg => { demoName = arg; });

            var demoSpeed = 1;
            CommandLineArgs.RegisterValueArg("demospeed", arg => { demoSpeed = int.Parse(arg); });

            CommandLineArgs.RegisterEarlyFlagArg("debug",
                () => { DebugLevel = DebugLevel.Active; });

            CommandLineArgs.RegisterValueArg("demo", arg =>
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
                        Print("Unknown demo mode", arg);
                        break;
                }
            });

            var shouldSkipSnapshot = DebugLevel == DebugLevel.Off;
            CommandLineArgs.RegisterFlagArg("skipsnapshot", () => { shouldSkipSnapshot = true; });

            StartingSettings.LoadSavedSettingsIfExist();
            SoundEffectPlayer = new SoundEffectPlayer(StartingSettings);

#if DEBUG
            LoadGame();
#else
            PlayIntroAndLoadGame();
#endif

            if (GamePlatform.IsDesktop)
            {
                new SnapshotTaker(debugActor, shouldSkipSnapshot);
            }
            
            this.isDoneUpdateLoading = true;
        }

        protected void RunDemo(string demoName)
        {
            var demoActor = sceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
            DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, 1);
            demoPlaybackComponent.ShowGui = false;
        }

        private void PrepareLoadInitialStyle(AssetLoadTree loadTree)
        {
            loadTree.AddMachinaAssetCallback("ui-button",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-button-hover",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24), new Rectangle(8 + 24, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-button-press",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24), new Rectangle(8 + 48, 8, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-slider-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24), new Rectangle(8, 152, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-checkbox-checkmark-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            loadTree.AddMachinaAssetCallback("ui-radio-fill-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            loadTree.AddMachinaAssetCallback("ui-checkbox-radio-spritesheet",
                () => new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            loadTree.AddMachinaAssetCallback("ui-textbox-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24), new Rectangle(8, 104, 8, 8)));
            loadTree.AddMachinaAssetCallback("ui-window-ninepatch",
                () => new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52)));
        }

        private void LoadGame()
        {
            CommandLineArgs.ExecuteEarlyArgs();
            OnGameLoad();
            CommandLineArgs.ExecuteArgs();
            CurrentGameCanvas.SetWindowSize(CurrentWindowSize); 
            Graphics.ApplyChanges();
        }

        public static void Quit()
        {
            Current.Exit();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();
            Assets.UnloadAssets();
        }

        protected abstract void OnGameLoad();

        protected override void Update(GameTime gameTime)
        {
            pendingCursor = MouseCursor.Arrow;
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (!this.isDoneUpdateLoading)
            {
                var library = Assets as AssetLibrary.AssetLibrary;
                var increment = 3;
                for (int i = 0; i < increment; i++)
                {
                    this.loadingScreen.Update(library, dt / increment);
                }
            }else if (!this.loadingScreen.IsDoneDrawLoading())
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
                        DemoPlayback.PollHumanInput(GetHumanFrameState());
                        sceneLayers.Update(dt, Matrix.Identity, frameState);
                    }
                }
                else
                {
                    sceneLayers.Update(dt, Matrix.Identity, GetHumanFrameState());
                }
            }

            Mouse.SetCursor(pendingCursor);
            base.Update(gameTime);
        }

        private InputFrameState GetHumanFrameState()
        {
            var inputState = InputState.RawHumanInput;

            MouseFrameState mouseFrameState;

            if (GamePlatform.IsMobile)
            {
                mouseFrameState = touchTracker.CalculateFrameState(inputState.touches);
            }
            else
            {
                mouseFrameState = mouseTracker.CalculateFrameState(inputState.mouseState);
            }

            return new InputFrameState(
                keyTracker.CalculateFrameState(inputState.keyboardState, inputState.gamepadState),
                mouseFrameState);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!this.isDoneUpdateLoading)
            {
                this.loadingScreen.Draw(spriteBatch, CurrentWindowSize, GraphicsDevice);
            }
            else if(!this.loadingScreen.IsDoneDrawLoading())
            {
                this.loadingScreen.IncrementDrawLoopLoad(Assets as AssetLibrary.AssetLibrary, this.spriteBatch);
                this.loadingScreen.Draw(spriteBatch, CurrentWindowSize, GraphicsDevice);
            }
            else
            {
                sceneLayers.PreDraw(spriteBatch);
                CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
                GraphicsDevice.Clear(sceneLayers.BackgroundColor);

                sceneLayers.DrawOnCanvas(spriteBatch);
                CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
                sceneLayers.DrawDebugScene(spriteBatch);
            }

            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            CurrentGameCanvas.SetWindowSize(windowSize);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var scene in sceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }
        
        private void SetupLoadingScreen()
        {
            var assetTree = AssetLibrary.AssetLibrary.GetStaticAssetLoadTree();
            PrepareDynamicAssets(assetTree);
            PrepareLoadInitialStyle(assetTree);
            
            this.loadingScreen =
                new LoadingScreen(assetTree, FinishLoadingContent);
        }

        protected abstract void PrepareDynamicAssets(AssetLoadTree tree);

        private void PlayIntroAndLoadGame()
        {
            const int desiredWidth = 1920 / 4;
            var oldSceneLayers = SceneLayers;
            var ratio = (float) startingWindowSize.X / desiredWidth;
            var gameCanvas = new GameCanvas(
                new Vector2(startingWindowSize.X / ratio, startingWindowSize.Y / ratio).ToPoint(),
                ResizeBehavior.MaintainDesiredResolution);
            gameCanvas.BuildCanvas(GraphicsDevice);
            var introLayers = new SceneLayers(true, gameCanvas);
            var introScene = introLayers.AddNewScene();

            var textActor = introScene.AddActor("text");
            new BoundingRect(textActor, 20, 20);
            new BoundingRectToViewportSize(textActor);
            new BoundedTextRenderer(textActor, "", Assets.GetSpriteFont("LogoFont"), Color.White,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            new IntroTextAnimation(textActor);

            // Steal control
            SceneLayers = introLayers;

            void OnEnd()
            {
                // Start the actual game
                SceneLayers = oldSceneLayers;
                LoadGame();
            }

            new CallbackOnDestroy(textActor, OnEnd);
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Current?.SceneLayers?.Logger.Log(objects);
            new StdOutConsoleLogger().Log(objects);
        }
    }
}