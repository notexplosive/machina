using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Machina
{

    public class MachinaGame : Game
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

        Scene debugScene;

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

            debugScene = new Scene();
            scenes.Add(debugScene);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Assets.LoadAllContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var consoleFont = Assets.GetSpriteFont("MachinaDefaultFont");
            var debugActor = debugScene.AddActor();
            new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, Graphics));

            PostLoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            spriteBatch.Dispose();
            Assets.UnloadAssets();
        }

        protected virtual void PostLoadContent()
        {
            // Derived class should put their code here
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Scene scene in scenes)
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
