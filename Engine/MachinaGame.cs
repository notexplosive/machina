using Machina.Components;
using Machina.Data;
using Machina.Tests;
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
        public static SeededRandom Random
        {
            get;
            private set;
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
        public static AssetLibrary Assets
        {
            get; private set;
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
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

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
                new NinepatchSheet("button-ninepatches", new Rectangle(24, 96, 24, 24), new Rectangle(32, 104, 8, 8)));

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
                Assets.GetMachinaAsset<Image>("ui-radio-fill-image"),
                new LinearFrameAnimation(0, 3),
                new LinearFrameAnimation(3, 3),
                new LinearFrameAnimation(9, 3),
                new LinearFrameAnimation(15, 3)
            );

#if DEBUG
            {
                var framerateCounterActor = sceneLayers.debugScene.AddActor("FramerateCounter");
                new FrameRateCounter(framerateCounterActor);
            }

            // Framestep
            {
                var frameStepActor = sceneLayers.debugScene.AddActor("FrameStepActor");
                var tool = new InvokableDebugTool(frameStepActor, new KeyCombination(Keys.Space, new ModifierKeys(true, false, false)));
                new FrameStepRenderer(frameStepActor, this.sceneLayers.frameStep, this.sceneLayers, tool);
                new BoundingRect(frameStepActor, new Point(64, 64));
                new Hoverable(frameStepActor);
                new Draggable(frameStepActor);
                new MoveOnDrag(frameStepActor);
            }

            // Scene graph renderer
            {
                var sceneGraphContainer = sceneLayers.debugScene.AddActor("SceneGraphContainer");
                new BoundingRect(sceneGraphContainer, new Point(350, 350));
                new LayoutGroup(sceneGraphContainer, Orientation.Vertical);
                var tool = new InvokableDebugTool(sceneGraphContainer, new KeyCombination(Keys.Tab, new ModifierKeys(true, false, false)));
                new Hoverable(sceneGraphContainer);
                new Draggable(sceneGraphContainer);
                new MoveOnDrag(sceneGraphContainer);
                new NinepatchRenderer(sceneGraphContainer, defaultStyle.windowSheet, NinepatchSheet.GenerationDirection.Outer);

                var spacer = sceneGraphContainer.transform.AddActorAsChild("Spacer");
                new BoundingRect(spacer, new Point(32, 32));
                new LayoutElement(spacer).StretchHorizontally();

                var content = sceneGraphContainer.transform.AddActorAsChild("Content");
                content.transform.LocalDepth = new Depth(-1);
                new BoundingRect(content, Point.Zero);
                new LayoutElement(content).StretchHorizontally().StretchVertically();
                new LayoutGroup(content, Orientation.Horizontal);

                var view = content.transform.AddActorAsChild("SceneGraphRendererView");
                view.transform.LocalDepth = new Depth(-1);
                new BoundingRect(view, Point.Zero).SetOffsetToTopLeft();
                new LayoutElement(view).StretchHorizontally().StretchVertically();
                new Canvas(view);
                new Hoverable(view);
                var sceneRenderer = new SceneRenderer(view, () => { return false; });
                var sceneGraphContent = sceneRenderer.primaryScene;


                var scrollbarActor = content.transform.AddActorAsChild("Scrollbar");
                scrollbarActor.transform.LocalDepth = new Depth(-1);
                new BoundingRect(scrollbarActor, new Point(32, 0));
                new LayoutElement(scrollbarActor).StretchVertically();
                new Hoverable(scrollbarActor);
                new NinepatchRenderer(scrollbarActor, defaultStyle.buttonDefault, NinepatchSheet.GenerationDirection.Inner);


                var scrollbar = new Scrollbar(scrollbarActor, view.GetComponent<BoundingRect>(), sceneGraphContent.camera, new MinMax<int>(0, 900), defaultStyle.buttonHover);

                var sceneGraphActor = sceneGraphContent.AddActor("SceneGraphActor");
                new SceneGraphRenderer(sceneGraphActor, this.sceneLayers, scrollbar);
                new ScrollbarListener(sceneGraphActor, scrollbar);
            }


            DebugLevel = DebugLevel.Passive;
            Print("Debug build detected");

            // Run tests
            new ActorTests();
            new HitTestTests();
            new CameraTransformsTests();
            new LayoutTests();
            new SceneTests();

            Print(TestGroup.RunAllRegisteredTests(), "tests passed");

#else
            DebugLevel = DebugLevel.Off;
#endif

            CommandLineArgs.RegisterValueArg("randomseed", arg =>
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
                        Demo.FromDisk(demoName, demo =>
                        {
                            DemoPlayback = new Demo.Playback(demo);
                            new DemoPlaybackComponent(debugActor, DemoPlayback, demoName);
                        });
                        break;
                    default:
                        MachinaGame.Print("Unknown demo mode", arg);
                        break;
                }
            });


            CommandLineArgs.RegisterValueArg("playbackspeed", arg =>
            {
                DemoPlayback.SpeedMultiplier = int.Parse(arg);
            });

            bool shouldSkipSnapshot = false;
            CommandLineArgs.RegisterFlagArg("skipsnapshot", () =>
            {
                shouldSkipSnapshot = true;
            });

            OnGameLoad();
            CommandLineArgs.ExecuteArgs();

            new SnapshotTaker(debugActor, shouldSkipSnapshot);
#if DEBUG
#else
            PlayLogoIntro();
#endif
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
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (DemoPlayback != null && DemoPlayback.IsFinished == false)
            {
                for (int i = 0; i < this.DemoPlayback.SpeedMultiplier; i++)
                {
                    var frameStates = DemoPlayback.UpdateAndGetInputFrameStates(dt);
                    foreach (var frameState in frameStates)
                    {
                        sceneLayers.Update(dt / frameStates.Length, Matrix.Identity, frameState);
                    }
                }
            }
            else
            {
                var inputState = InputState.RawHumanInput;
                var humanInputFrameState = new InputFrameState(keyTracker.Calculate(inputState.keyboardState), mouseTracker.Calculate(inputState.mouseState));
                sceneLayers.Update(dt, Matrix.Identity, humanInputFrameState);
            }


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
                scene.OnDelete();
            }
        }

        void PlayLogoIntro()
        {
            var oldLayers = SceneLayers;
            var windowSize = this.startingWindowSize;
            var desiredWidth = 400;
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
                // Restore control
                SceneLayers = oldLayers;
            }

            new CallbackOnDestroy(textActor, onEnd);
        }

        /// <summary>
        /// Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Current.logger.Log(objects);
        }
    }
}
