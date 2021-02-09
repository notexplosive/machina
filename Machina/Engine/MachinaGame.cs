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
        private Point startingWindowSize;
        private ScrollTracker scrollTracker;
        private KeyTracker keyTracker;
        private MouseTracker mouseTracker;
        protected SpriteBatch spriteBatch;
        protected readonly ResizeStatus resizing;
        protected readonly List<Scene> scenes = new List<Scene>();
        private Scene debugScene;
        private ILogger logger;

        public static DebugLevel DebugLevel
        {
            get; private set;
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
        private Scene[] SceneLayers
        {
            get
            {
                Scene[] sceneLayers = new Scene[scenes.Count + 1];
                scenes.CopyTo(sceneLayers);
                sceneLayers[scenes.Count] = debugScene;
                return sceneLayers;
            }
        }

        public MachinaGame(int windowWidth, int windowHeight)
        {
            Current = this;
            this.logger = new ConsoleLogger();
            Assets = new AssetLibrary(this);
            Content.RootDirectory = "Content";

            Graphics = new GraphicsDeviceManager(this);
            resizing = new ResizeStatus(windowWidth, windowHeight);
            scrollTracker = new ScrollTracker();
            keyTracker = new KeyTracker();
            mouseTracker = new MouseTracker();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            this.startingWindowSize = new Point(windowWidth, windowHeight);
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = startingWindowSize.X;
            Graphics.PreferredBackBufferHeight = startingWindowSize.Y;
            Graphics.ApplyChanges();
            OnResize(null, new EventArgs());

            // debugScene does NOT get added to the `scenes` list because it's always on top
            debugScene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Assets.LoadAllContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var consoleFont = Assets.GetSpriteFont("MachinaDefaultFont");
            var debugActor = debugScene.AddActor();
            this.logger = new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, Graphics));

#if DEBUG
            DebugLevel = DebugLevel.Passive;
            Print("Debug build detected");
#else
            this.debugLevel = DebugLevel.Off;
#endif
            Print("DebugLevel set to:", DebugLevel);

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
            var sceneLayers = SceneLayers;
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Scene scene in sceneLayers)
            {
                scene.Update(dt);
            }

            var delta = scrollTracker.CalculateDelta();
            keyTracker.Calculate();
            mouseTracker.Calculate();

            foreach (Scene scene in sceneLayers)
            {
                scene.OnScroll(delta);
                if (mouseTracker.PositionDelta.LengthSquared() > 0)
                {
                    scene.OnMouseMove(mouseTracker.CurrentPosition, mouseTracker.PositionDelta);
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

            if (resizing.Pending)
            {
                Graphics.PreferredBackBufferWidth = resizing.Width;
                Graphics.PreferredBackBufferHeight = resizing.Height;
                Graphics.ApplyChanges();
                resizing.FinishResize();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var sceneLayers = SceneLayers;
            foreach (var scene in sceneLayers)
            {
                scene.PreDraw(spriteBatch);
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var scene in sceneLayers)
            {
                scene.Draw(spriteBatch);
            }

            if (DebugLevel >= DebugLevel.Passive)
            {
                foreach (var scene in sceneLayers)
                {
                    scene.DebugDraw(spriteBatch);
                }
            }

            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            resizing.Resize(Window.ClientBounds.Width, Window.ClientBounds.Height);

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
