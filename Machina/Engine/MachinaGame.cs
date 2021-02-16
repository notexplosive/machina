using Machina.Components;
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
        private readonly Point startingWindowSize;
        protected readonly SceneLayers sceneLayers;
        protected SpriteBatch spriteBatch;
        public readonly GameCanvas gameCanvas;
        private ILogger logger;

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

        public MachinaGame(Point startingResolution, ResizeBehavior resizeBehavior)
        {
            Current = this;
            this.logger = new StdOutConsoleLogger();
            this.startingWindowSize = startingResolution;

            IFrameStep frameStep;
#if DEBUG
            frameStep = new FrameStep();
#else
            frameStep = new EmptyFrameStep();
#endif
            this.sceneLayers = new SceneLayers(new Scene(), frameStep);

            Assets = new AssetLibrary(this);
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            gameCanvas = new GameCanvas(startingResolution.X, startingResolution.Y, resizeBehavior);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = startingWindowSize.X;
            Graphics.PreferredBackBufferHeight = startingWindowSize.Y;
            Graphics.ApplyChanges();
            gameCanvas.OnResize(startingWindowSize.X, startingWindowSize.Y);
            gameCanvas.BuildCanvas(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Assets.LoadAllContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var consoleFont = Assets.DefaultFont;
            var debugActor = sceneLayers.debugScene.AddActor("DebugLogger");
            new FrameStepRenderer(debugActor, this.sceneLayers.frameStep, this.sceneLayers);
            this.logger = new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, Graphics));
            new EnableDebugOnHotkey(debugActor, new KeyCombination(Keys.OemTilde, new ModifierKeys(true, false, true)));

#if DEBUG
            DebugLevel = DebugLevel.Passive;
            Print("Debug build detected");

            new ActorTests();
            new HitTestTests();
            new CameraTransformsTests();
            new LayoutTests();
            new SceneTests();

            Print(TestGroup.RunAllRegisteredTests(), "tests passed");

#else
            this.debugLevel = DebugLevel.Off;
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

            if (gameCanvas.PendingResize)
            {
                Graphics.PreferredBackBufferWidth = gameCanvas.WindowSize.X;
                Graphics.PreferredBackBufferHeight = gameCanvas.WindowSize.Y;
                Graphics.ApplyChanges();
                gameCanvas.FinishResize();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            sceneLayers.PreDraw(spriteBatch);
            gameCanvas.PrepareToDrawOnCanvas(GraphicsDevice);
            GraphicsDevice.Clear(Color.DarkSlateGray); // Draw main background color
            sceneLayers.Draw(spriteBatch);

            gameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            gameCanvas.OnResize(Window.ClientBounds.Width, Window.ClientBounds.Height);
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
