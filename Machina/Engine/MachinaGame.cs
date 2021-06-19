using Machina.Components;
using Machina.Data;
using Machina.Engine.Debugging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.IO;

namespace Machina.Engine
{
    public enum DebugLevel
    {
        Off,        // Completely disabled, can be enabled with hotkey
        Passive,    // Show Console Output
        Active      // Render DebugDraws
    }

    /// <summary>
    /// Derive your Game class from MachinaGame and then populate the PostLoadContent with your code.
    /// Your game should call the base constructor, even though it's abstract.
    /// </summary>
    public abstract class MachinaGame : Game
    {
        public readonly string gameTitle;

        public static CommandLineArgs CommandLineArgs
        {
            get; private set;
        }

        /// <summary>
        /// Path to users AppData folder (or platform equivalent)
        /// </summary>
        public readonly string appDataPath;
        /// <summary>
        /// Path of the "Content" folder for the built Exectuable
        /// </summary>
        public readonly string localContentPath;
        /// <summary>
        /// (brittle) Path of the "Content" folder in the dev environment
        /// </summary>
        public readonly string devContentPath;
        /// <summary>
        /// (brittle) Path to the screenshots folder in the dev environment
        /// </summary>
        public readonly string devScreenshotPath;

        protected readonly Point startingWindowSize;
        private SceneLayers sceneLayers;
        public SceneLayers SceneLayers
        {
            get => this.sceneLayers;
            set
            {
                this.sceneLayers = value;
                CurrentGameCanvas.SetWindowSize(this.currentWindowSize);
            }
        }
        protected SpriteBatch spriteBatch;
        public GameCanvas CurrentGameCanvas => (sceneLayers.gameCanvas as GameCanvas);
        private ILogger logger;
        public static UIStyle defaultStyle;
        private Point currentWindowSize;
        private static MouseCursor pendingCursor;

        public static SeededRandom Random
        {
            get;
            private set;
        }

        /// <summary>
        /// TEST ONLY!!
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
            Texture2D cropTexture = new Texture2D(Current.GraphicsDevice, rect.Width, rect.Height);
            Color[] data = new Color[rect.Width * rect.Height];
            sourceTexture.GetData(0, rect, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
        }

        public static DebugLevel DebugLevel
        {
            get; set;
        }

        public static GraphicsDeviceManager Graphics
        {
            get; private set;
        }
        public static IAssetLibrary Assets
        {
            get; private set;
        }

        public static void SetCursor(MouseCursor cursor)
        {
            pendingCursor = cursor;
        }

        /// <summary>
        /// Should only be used for tests
        /// </summary>
        /// <param name="assetLibrary"></param>
        public static void SetAssetLibrary(IAssetLibrary assetLibrary)
        {
            Assets = assetLibrary;
        }
        public static MachinaGame Current
        {
            get; private set;
        }
        public Demo.Playback DemoPlayback
        {
            get;
            private set;
        }
        public static bool Fullscreen
        {
            set
            {
                if (Graphics.IsFullScreen != value)
                {
                    if (value)
                    {
                        Graphics.PreferredBackBufferWidth = MachinaGame.Current.GraphicsDevice.DisplayMode.Width;
                        Graphics.PreferredBackBufferHeight = MachinaGame.Current.GraphicsDevice.DisplayMode.Height;
                        Graphics.IsFullScreen = true;
                    }
                    else
                    {
                        Graphics.PreferredBackBufferWidth = Current.startingWindowSize.X;
                        Graphics.PreferredBackBufferHeight = Current.startingWindowSize.Y;
                        Graphics.IsFullScreen = false;
                    }
                    Graphics.ApplyChanges();
                }
            }

            get => Graphics.IsFullScreen;
        }

        public static SamplerState SamplerState
        {
            get;
            set;
        } = SamplerState.PointClamp;

        private readonly KeyTracker keyTracker;
        private readonly MouseTracker mouseTracker;

        protected MachinaGame(string gameTitle, string[] args, Point startingRenderResolution, Point startingWindowSize, ResizeBehavior resizeBehavior)
        {
            this.gameTitle = gameTitle;
            CommandLineArgs = new CommandLineArgs(args);


            this.appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NotExplosive", this.gameTitle);
            this.localContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content");
            this.devContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Content");
            this.devScreenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "screenshots");
            this.logger = new StdOutConsoleLogger();
            this.startingWindowSize = startingWindowSize;
            this.currentWindowSize = startingWindowSize;

            Window.Title = gameTitle;
            Current = this;

            IFrameStep frameStep;
#if DEBUG
            frameStep = new FrameStep();
#else
            frameStep = new EmptyFrameStep();
#endif

            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            Assets = new AssetLibrary(this);
            this.sceneLayers = new SceneLayers(true, new GameCanvas(startingRenderResolution, resizeBehavior), frameStep);
            Window.TextInput += this.sceneLayers.AddPendingTextInput;

            this.keyTracker = new KeyTracker();
            this.mouseTracker = new MouseTracker();
            Random = new SeededRandom();
        }

        protected void SetWindowSize(Point windowSize)
        {
            Print("Window size changed to", windowSize);
            Graphics.PreferredBackBufferWidth = windowSize.X;
            Graphics.PreferredBackBufferHeight = windowSize.Y;
            Graphics.ApplyChanges();
            CurrentGameCanvas.SetWindowSize(new Point(windowSize.X, windowSize.Y));
        }

        protected override void Initialize()
        {
            CurrentGameCanvas.BuildCanvas(GraphicsDevice);
            this.IsMouseVisible = true;
            SetWindowSize(this.startingWindowSize);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Assets.LoadAllContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var consoleFont = Assets.GetSpriteFont("DefaultFont");
            var debugActor = sceneLayers.debugScene.AddActor("DebugActor", depthAsInt: 100);
            this.logger = new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont));
            new EnableDebugOnHotkey(debugActor, new KeyCombination(Keys.OemTilde, new ModifierKeys(true, false, true)));

            // Load initial assets
            Assets.AddMachinaAsset("ui-button", new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8)));
            Assets.AddMachinaAsset("ui-button-hover", new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24), new Rectangle(8 + 24, 8, 8, 8)));
            Assets.AddMachinaAsset("ui-button-press", new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24), new Rectangle(8 + 48, 8, 8, 8)));
            Assets.AddMachinaAsset("ui-slider-ninepatch", new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24), new Rectangle(8, 152, 8, 8)));
            Assets.AddMachinaAsset("ui-checkbox-checkmark-image", new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            Assets.AddMachinaAsset("ui-radio-fill-image", new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            Assets.AddMachinaAsset("ui-checkbox-radio-spritesheet", new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            Assets.AddMachinaAsset("ui-textbox-ninepatch",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24), new Rectangle(8, 104, 8, 8)));
            Assets.AddMachinaAsset("ui-window-ninepatch",
                new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52)));

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

#if DEBUG
            var windowManager = new WindowManager(defaultStyle, Depth.Middle);
            DebugBuilder.CreateFramerateCounter(sceneLayers);
            DebugBuilder.CreateFramestep(sceneLayers);
            DebugBuilder.CreateSceneGraphRenderer(sceneLayers, windowManager);

            DebugLevel = DebugLevel.Passive;
            Print("Debug build detected");

#else
            DebugLevel = DebugLevel.Off;
#endif
            var demoPlaybackComponent = new DemoPlaybackComponent(debugActor);


            CommandLineArgs.RegisterEarlyValueArg("randomseed", arg =>
            {
                MachinaGame.Random.SetSeedFromString(arg);
            });

            var demoName = Demo.MostRecentlySavedDemoPath;
            CommandLineArgs.RegisterValueArg("demopath", arg =>
            {
                demoName = arg;
            });

            CommandLineArgs.RegisterValueArg("demo", arg =>
            {
                switch (arg)
                {
                    case "record":
                        new DemoRecorderComponent(debugActor, new Demo.Recorder(demoName));
                        break;
                    case "playback":
                        DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName);
                        break;
                    case "playback-nogui":
                        DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName);
                        demoPlaybackComponent.ShowGui = false;
                        break;
                    default:
                        MachinaGame.Print("Unknown demo mode", arg);
                        break;
                }
            });

            bool shouldSkipSnapshot = DebugLevel == DebugLevel.Off;
            CommandLineArgs.RegisterFlagArg("skipsnapshot", () =>
            {
                shouldSkipSnapshot = true;
            });



#if DEBUG
            LoadGame();
#else
            PlayIntroAndLoadGame();
#endif

            new SnapshotTaker(debugActor, shouldSkipSnapshot);
        }

        private void LoadGame()
        {
            CommandLineArgs.ExecuteEarlyArgs();
            OnGameLoad();
            CommandLineArgs.ExecuteArgs();
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

            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (DemoPlayback != null && DemoPlayback.IsFinished == false)
            {
                var frameState = DemoPlayback.UpdateAndGetInputFrameStates(dt);
                sceneLayers.Update(dt, Matrix.Identity, frameState);
            }
            else
            {
                var inputState = InputState.RawHumanInput;
                var humanInputFrameState = new InputFrameState(keyTracker.Calculate(inputState.keyboardState, inputState.gamepadState), mouseTracker.Calculate(inputState.mouseState));
                sceneLayers.Update(dt, Matrix.Identity, humanInputFrameState);
            }

            Mouse.SetCursor(pendingCursor);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            sceneLayers.PreDraw(spriteBatch);
            CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
            GraphicsDevice.Clear(sceneLayers.BackgroundColor);

            sceneLayers.DrawOnCanvas(spriteBatch);
            CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
            sceneLayers.DrawDebugScene(spriteBatch);
            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            var windowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            this.currentWindowSize = windowSize;
            CurrentGameCanvas.SetWindowSize(windowSize);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            foreach (var scene in sceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }

        void PlayIntroAndLoadGame()
        {
            var oldSceneLayers = SceneLayers;
            var windowSize = this.startingWindowSize;
            var desiredWidth = 1920 / 4;
            float ratio = (float) windowSize.X / desiredWidth;
            var gameCanvas = new GameCanvas(new Vector2(windowSize.X / ratio, windowSize.Y / ratio).ToPoint(), ResizeBehavior.MaintainDesiredResolution);
            gameCanvas.BuildCanvas(GraphicsDevice);
            var introLayers = new SceneLayers(true, gameCanvas, new FrameStep());
            var introScene = introLayers.AddNewScene();

            introLayers.BackgroundColor = Color.Black;
            var textActor = introScene.AddActor("text");
            var boundingRect = new BoundingRect(textActor, 20, 20);
            new BoundingRectToViewportSize(textActor);
            new BoundedTextRenderer(textActor, "", MachinaGame.Assets.GetSpriteFont("LogoFont"), Color.White, HorizontalAlignment.Center, VerticalAlignment.Center);
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
        /// Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Current?.logger.Log(objects);
        }
    }
}
