using Machina;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HelloGame
{
    public class GameCore : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        static AssetLibrary assets;
        private ResizeStatus resizing;
        Scene gameScene;

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            assets = new AssetLibrary(this);
            resizing = new ResizeStatus(1600, 900);

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }

        private void OnResize(object sender, EventArgs e)
        {
            resizing.Pending = true;
            resizing.Width = Window.ClientBounds.Width;
            resizing.Height = Window.ClientBounds.Height;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            gameScene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            assets.LoadAllTextures();


            Actor linkin = gameScene.AddActor(new Vector2(200, 200));

            var linkinSpriteSheet = new GridBasedSpriteSheet(assets.GetTexture("linkin"), new Point(16, 16));
            new SpriteRenderer(linkin, linkinSpriteSheet);

            var standAnim = new LinearFrameAnimation(0, 5);
            var walkAnim = new LinearFrameAnimation(6, 3);
            var quickSwingAnim = new LinearFrameAnimation(9, 3);
            var longSwingAnim = new LinearFrameAnimation(11, 5);
            var finalSwingAnim = new LinearFrameAnimation(16, 7, LoopType.HoldLastFrame);

            linkin.GetComponent<SpriteRenderer>().SetAnimation(walkAnim);

            new BoundingRect(linkin, 32, 32);
            new BoundingRectRenderer(linkin);

            linkin.GetComponent<SpriteRenderer>().SetupBoundingRect();

            var cameraScroller = gameScene.AddActor();
            new ZoomCameraOnScroll(cameraScroller);

            var sceneRenderBox = gameScene.AddActor(new Vector2(200, 350));

            var windowNinepatch = new NinepatchSpriteSheet(assets.GetTexture("test-nine-patch"), GraphicsDevice, new Rectangle(0, 0, 48, 48), new Rectangle(16, 16, 16, 16));
            var ninepatchActor = gameScene.AddActor(new Vector2(400, 400));
            new BoundingRect(ninepatchActor, new Point(400, 300));
            new NinepatchRenderer(ninepatchActor, windowNinepatch);

            new BoundingRect(sceneRenderBox, new Point(160, 150));


            var progressBarThreepatch = new NinepatchSpriteSheet(assets.GetTexture("test-three-patch"), GraphicsDevice, new Rectangle(0, 0, 28, 24), new Rectangle(2, 0, 24, 24));
            var progressBar = gameScene.AddActor();
            progressBar.position = new Vector2(500, 50);
            new BoundingRect(progressBar, new Point(500, 24));
            new ThreepatchRenderer(progressBar, progressBarThreepatch, Orientation.Horizontal);

            var pillarThreepatch = new NinepatchSpriteSheet(assets.GetTexture("test-three-patch-vertical"), GraphicsDevice, new Rectangle(0, 0, 32, 32), new Rectangle(0, 8, 32, 16));
            var pillar = gameScene.AddActor();
            pillar.position = new Vector2(300, 350);
            new BoundingRect(pillar, new Point(32, 500));
            new ThreepatchRenderer(pillar, pillarThreepatch, Orientation.Vertical);

            var otherScene = new Scene();
            var microActor = otherScene.AddActor();
            microActor.position = new Vector2(100, 100);
            new SpriteRenderer(microActor, linkinSpriteSheet).SetAnimation(standAnim);

            new Canvas(sceneRenderBox, GraphicsDevice);
            new BoundingRectRenderer(sceneRenderBox);
            new SceneRenderer(sceneRenderBox, otherScene);

            var ballActor = gameScene.AddActor(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));
            new SpriteRenderer(ballActor, linkinSpriteSheet);
            new TextureRenderer(ballActor, assets.GetTexture("ball"));
            new KeyboardMovement(ballActor);
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

            // camera.AdjustZoom((float) (scrollDelta / 100) / 10);
            gameScene.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);

            if (resizing.Pending)
            {
                gameScene.camera.NativeScaleFactor = resizing.ScaleFactor;
                graphics.PreferredBackBufferWidth = resizing.Width;
                graphics.PreferredBackBufferHeight = resizing.Height;
                graphics.ApplyChanges();
                //camera.UpdateProjection(resizing.Width, resizing.Height);
                resizing.Pending = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            gameScene.PreDraw(spriteBatch);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            gameScene.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
