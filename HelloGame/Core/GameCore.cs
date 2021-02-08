using Machina;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Machina.ThirdParty;
using System;

namespace HelloGame
{
    public class GameCore : MachinaGame
    {
        private SpriteBatch spriteBatch;
        static AssetLibrary assets;
        Scene gameScene;
        Scene debugScene;

        public GameCore() : base(1600, 900)
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            assets = new AssetLibrary(this);
        }

        protected override void Initialize()
        {
            gameScene = new Scene(this.resizing);
            debugScene = new Scene();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            assets.LoadEverything();
            var linkinSpriteSheet = new GridBasedSpriteSheet(assets.GetTexture("linkin"), new Point(16, 16));
            var windowNinepatch = new NinepatchSpriteSheet(assets.GetTexture("test-nine-patch"), GraphicsDevice, new Rectangle(0, 0, 48, 48), new Rectangle(16, 16, 16, 16));
            var progressBarThreepatch = new NinepatchSpriteSheet(assets.GetTexture("test-three-patch"), GraphicsDevice, new Rectangle(0, 0, 28, 24), new Rectangle(2, 0, 24, 24));
            var pillarThreepatch = new NinepatchSpriteSheet(assets.GetTexture("test-three-patch-vertical"), GraphicsDevice, new Rectangle(0, 0, 32, 32), new Rectangle(0, 8, 32, 16));
            var consoleFont = assets.GetSpriteFont("ConsoleFont");
            var defaultFont = assets.GetSpriteFont("DefaultFont");

            var standAnim = new LinearFrameAnimation(0, 5);
            var walkAnim = new LinearFrameAnimation(6, 3);
            var quickSwingAnim = new LinearFrameAnimation(9, 3);
            var longSwingAnim = new LinearFrameAnimation(11, 5);
            var finalSwingAnim = new LinearFrameAnimation(16, 7, LoopType.HoldLastFrame);

            var debugActor = debugScene.AddActor();
            new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont, graphics));

            //

            var ballActor = gameScene.AddActor(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            new TextureRenderer(ballActor, assets.GetTexture("ball"));
            new SpriteRenderer(ballActor, linkinSpriteSheet);
            new KeyboardMovement(ballActor);
            new TextRenderer(ballActor, defaultFont, "Hello world and this is a very long string");

            Actor linkin = gameScene.AddActor(new Vector2(250, 250));
            var linkinRenderer = new SpriteRenderer(linkin, linkinSpriteSheet);
            new BoundingRect(linkin, 32, 32);
            new BoundingRectRenderer(linkin);
            new TweenChainComponent(linkin)
                .AddMoveTween(new Vector2(300, 300), 1, EaseFuncs.SineEaseIn)
                .AddWaitTween(2f)
                .AddMoveTween(new Vector2(1300, 0), 1, EaseFuncs.Linear);

            linkinRenderer.SetAnimation(walkAnim);
            linkinRenderer.SetupBoundingRect();

            var cameraScroller = gameScene.AddActor();
            new ZoomCameraOnScroll(cameraScroller);

            var otherScene = new Scene();
            var microActor = otherScene.AddActor();
            microActor.position = new Vector2(100, 100);
            new SpriteRenderer(microActor, linkinSpriteSheet).SetAnimation(standAnim);

            var sceneRenderBox = gameScene.AddActor(new Vector2(200, 350));
            new BoundingRect(sceneRenderBox, new Point(160, 150));
            new Canvas(sceneRenderBox, GraphicsDevice);
            new BoundingRectRenderer(sceneRenderBox);
            new SceneRenderer(sceneRenderBox, otherScene);

            var ninepatchActor = gameScene.AddActor(new Vector2(400, 400));
            new BoundingRect(ninepatchActor, new Point(400, 300));
            new NinepatchRenderer(ninepatchActor, windowNinepatch);

            var progressBar = gameScene.AddActor();
            progressBar.position = new Vector2(500, 50);
            new BoundingRect(progressBar, new Point(500, 24));
            new ThreepatchRenderer(progressBar, progressBarThreepatch, Orientation.Horizontal);

            var pillar = gameScene.AddActor();
            pillar.position = new Vector2(300, 350);
            new BoundingRect(pillar, new Point(32, 500));
            new ThreepatchRenderer(pillar, pillarThreepatch, Orientation.Vertical);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (/*Keyboard.GetState().IsKeyDown(Keys.F4)*/ false)
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                    graphics.IsFullScreen = true;
                    graphics.ApplyChanges();
                }
            }

            float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            gameScene.Update(dt);
            debugScene.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            gameScene.PreDraw(spriteBatch);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            gameScene.Draw(spriteBatch);
            debugScene.Draw(spriteBatch);
            if (Logger.current.DebugLevel >= DebugLevel.Passive)
            {
                gameScene.DebugDraw(spriteBatch);
                debugScene.DebugDraw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
