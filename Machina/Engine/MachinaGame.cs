using Machina.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private readonly ScrollTracker scrollTracker;
        private readonly KeyTracker keyTracker;
        private readonly MouseTracker mouseTracker;
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
            Assets = new AssetLibrary(this);
            Content.RootDirectory = "Content";

            Graphics = new GraphicsDeviceManager(this);
            gameCanvas = new GameCanvas(startingResolution.X, startingResolution.Y, resizeBehavior);

            scrollTracker = new ScrollTracker();
            keyTracker = new KeyTracker();
            mouseTracker = new MouseTracker();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            this.startingWindowSize = startingResolution;

            this.sceneLayers = new SceneLayers(new Scene());
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

            var consoleFont = Assets.GetSpriteFont("MachinaDefaultFont");
            var debugActor = sceneLayers.debugScene.AddActor("DebugLogger");
            this.logger = new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, Graphics));
            new EnableDebugOnHotkey(debugActor, new KeyCombination(Keys.OemTilde, new ModifierKeys(true, false, true)));

#if DEBUG
            DebugLevel = DebugLevel.Passive;
            Print("Debug build detected");
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
            var scenes = sceneLayers.AllScenes();
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Update happens BEFORE input processing, this way we can set initial state for input processing during Update()
            // and then modify that state in input processing.
            foreach (Scene scene in scenes)
            {
                scene.Update(dt);
            }

            // Input Processing
            var delta = scrollTracker.CalculateDelta();
            keyTracker.Calculate();
            mouseTracker.Calculate(gameCanvas.CanvasRect.Location, gameCanvas.ScaleFactor);

            foreach (Scene scene in scenes)
            {
                // At this point the raw and processed deltas are equal, downstream (Scene and below) they will differ
                scene.OnMouseUpdate(mouseTracker.CurrentPosition, mouseTracker.PositionDelta, mouseTracker.PositionDelta);

                if (delta != 0)
                {
                    scene.OnScroll(delta);
                }

                foreach (var key in keyTracker.Released)
                {
                    scene.OnKey(key, ButtonState.Released, keyTracker.Modifiers);
                }

                foreach (var mouseButton in mouseTracker.Pressed)
                {
                    scene.OnMouseButton(mouseButton, mouseTracker.CurrentPosition, ButtonState.Pressed);
                }

                foreach (var mouseButton in mouseTracker.Released)
                {
                    scene.OnMouseButton(mouseButton, mouseTracker.CurrentPosition, ButtonState.Released);
                }

                foreach (var key in keyTracker.Pressed)
                {
                    scene.OnKey(key, ButtonState.Pressed, keyTracker.Modifiers);
                }
            }

            var willApproveCandidate = true;
            // Traverse scenes in reverse draw order (top to bottom)
            for (int i = scenes.Length - 1; i >= 0; i--)
            {
                var scene = scenes[i];
                var candidate = scene.hitTester.Candidate;
                if (!candidate.IsEmpty())
                {
                    candidate.approvalCallback?.Invoke(willApproveCandidate);
                    willApproveCandidate = false;
                }
            }

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
            var scenes = sceneLayers.AllScenes();
            foreach (var scene in scenes)
            {
                scene.PreDraw(spriteBatch);
            }

            gameCanvas.PrepareCanvas(GraphicsDevice);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
            }

            if (DebugLevel > DebugLevel.Passive)
            {
                foreach (var scene in scenes)
                {
                    scene.DebugDraw(spriteBatch);
                }
            }

            gameCanvas.DrawCanvas(GraphicsDevice, spriteBatch);

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
                scene.OnRemove();
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
