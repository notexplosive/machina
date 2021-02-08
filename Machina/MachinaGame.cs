using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Machina
{
    /// <summary>
    /// Derive your Game class from MachinaGame and then populate the PostLoadContent with your code.
    /// </summary>
    public abstract class MachinaGame : Game
    {
        private Point startingWindowSize;
        protected SpriteBatch spriteBatch;
        protected readonly ResizeStatus resizing;
        protected readonly List<Scene> scenes = new List<Scene>();
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


        private Scene debugScene;
        private Logger logger;

        public MachinaGame(int windowWidth, int windowHeight)
        {
            Current = this;
            Assets = new AssetLibrary(this);
            Content.RootDirectory = "Content";

            Graphics = new GraphicsDeviceManager(this);
            resizing = new ResizeStatus(windowWidth, windowHeight);

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

            MachinaGame.Print("test");

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

            if (resizing.Pending)
            {
                Graphics.PreferredBackBufferWidth = resizing.Width;
                Graphics.PreferredBackBufferHeight = resizing.Height;
                Graphics.ApplyChanges();
                resizing.Pending = false;
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

            if (this.logger.DebugLevel >= DebugLevel.Passive)
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
            resizing.Pending = true;
            resizing.Width = Window.ClientBounds.Width;
            resizing.Height = Window.ClientBounds.Height;
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
