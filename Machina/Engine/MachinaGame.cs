using Machina.Components;
using Machina.Data;
using Machina.Tests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;

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
        private readonly Point startingResolution;
        private readonly Point startingWindowSize;
        protected readonly SceneLayers sceneLayers;
        protected SpriteBatch spriteBatch;
        public readonly GameCanvas gameCanvas;
        private ILogger logger;
        public static UIStyle defaultStyle;

        internal static Texture2D CropTexture(Rectangle rect, Texture2D sourceTexture)
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

        protected MachinaGame(Point startingRenderResolution, Point startingWindowSize, ResizeBehavior resizeBehavior)
        {
            Current = this;
            this.logger = new StdOutConsoleLogger();
            this.startingResolution = startingRenderResolution;
            this.startingWindowSize = startingWindowSize;

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

            gameCanvas = new GameCanvas(startingRenderResolution.X, startingRenderResolution.Y, resizeBehavior);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            Assets = new AssetLibrary(this);
            this.sceneLayers = new SceneLayers(true, gameCanvas, frameStep);
            Window.TextInput += this.sceneLayers.OnTextInput;
        }

        protected void SetWindowSize(Point windowSize)
        {
            Print("Window size changed to", windowSize);
            Graphics.PreferredBackBufferWidth = windowSize.X;
            Graphics.PreferredBackBufferHeight = windowSize.Y;
            Graphics.ApplyChanges();
            gameCanvas.SetWindowSize(windowSize.X, windowSize.Y);
        }

        protected override void Initialize()
        {
            gameCanvas.BuildCanvas(GraphicsDevice, this.startingResolution);
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

                var view = content.transform.AddActorAsChild("View");
                view.transform.LocalDepth = new Depth(-1);
                new BoundingRect(view, Point.Zero).SetOffsetToTopLeft();
                new LayoutElement(view).StretchHorizontally().StretchVertically();
                new Canvas(view);
                new Hoverable(view);
                var sceneRenderer = new SceneRenderer(view, () => { return true; });
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




#if DEBUG
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

            OnGameLoad();
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

            sceneLayers.Update(dt, Matrix.Identity, InputState.Raw);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            sceneLayers.PreDraw(spriteBatch);
            gameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
            GraphicsDevice.Clear(Color.DarkSlateGray); // Draw main background color

            sceneLayers.DrawOnCanvas(spriteBatch);
            gameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
            sceneLayers.DrawDebugScene(spriteBatch);
            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            gameCanvas.SetWindowSize(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            foreach (var scene in sceneLayers.AllScenes())
            {
                scene.OnDelete();
            }
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
