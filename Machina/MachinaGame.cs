using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Machina
{

    public abstract class MachinaGame : Game
    {
        private Point startingWindowSize;
        protected SpriteBatch spriteBatch;
        protected ResizeStatus resizing;
        protected readonly List<Scene> scenes = new List<Scene>();
        public static GraphicsDeviceManager graphics;
        public static AssetLibrary assets;
        public static MachinaGame current
        {
            get; private set;
        }

        Scene debugScene;

        public MachinaGame(int windowWidth, int windowHeight)
        {
            current = this;
            assets = new AssetLibrary(this);
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            resizing = new ResizeStatus(windowWidth, windowHeight);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);

            this.startingWindowSize = new Point(windowWidth, windowHeight);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = startingWindowSize.X;
            graphics.PreferredBackBufferHeight = startingWindowSize.Y;
            graphics.ApplyChanges();
            OnResize(null, new EventArgs());

            debugScene = new Scene();
            scenes.Add(debugScene);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            assets.LoadAllContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var consoleFont = assets.GetSpriteFont("MachinaDefaultFont");
            var debugActor = debugScene.AddActor();
            new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, graphics));

            PostLoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();
            assets.UnloadAssets();
        }

        protected abstract void PostLoadContent();

        protected override void Update(GameTime gameTime)
        {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Scene scene in scenes)
            {
                scene.Update(dt);
            }

            if (resizing.Pending)
            {
                graphics.PreferredBackBufferWidth = resizing.Width;
                graphics.PreferredBackBufferHeight = resizing.Height;
                graphics.ApplyChanges();
                resizing.Pending = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            foreach (var scene in scenes)
            {
                scene.PreDraw(spriteBatch);
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
            }

            if (Logger.current.DebugLevel >= DebugLevel.Passive)
            {
                foreach (var scene in scenes)
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
    }
}
