using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Machina.ThirdParty;
using System;
using Machina.Engine;

namespace HelloGame
{
    public class GameCore : MachinaGame
    {
        Scene gameScene;

        public GameCore() : base(new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnGameLoad()
        {
            gameScene = new Scene(this.gameCanvas);
            scenes.Add(gameScene);

            Assets.AddMachinaAsset("linkin-sprite-sheet", new GridBasedSpriteSheet(Assets.GetTexture("linkin"), new Point(16, 16)));
            Assets.AddMachinaAsset("test-ninepatch",
                new NinepatchSheet("test-nine-patch", new Rectangle(0, 0, 48, 48), new Rectangle(16, 16, 16, 16)));
            var progressBarThreepatch =
                Assets.AddMachinaAsset(
                    "progressbar-threepatch",
                    new NinepatchSheet("test-three-patch", new Rectangle(0, 0, 28, 24), new Rectangle(2, 0, 24, 24)));
            var pillarThreepatch =
                Assets.AddMachinaAsset(
                    "pillar-threepatch",
                    new NinepatchSheet("test-three-patch-vertical", new Rectangle(0, 0, 32, 32), new Rectangle(0, 8, 32, 16)));
            var defaultFont = Assets.GetSpriteFont("DefaultFont");
            var consoleFont = Assets.GetSpriteFont("ConsoleFont");

            var testNinepatch = Assets.GetMachinaAsset<NinepatchSheet>("test-ninepatch");
            var linkinSpriteSheet = Assets.GetMachinaAsset<SpriteSheet>("linkin-sprite-sheet");

            var standAnim = new LinearFrameAnimation(0, 5);
            var walkAnim = new LinearFrameAnimation(6, 3);
            var quickSwingAnim = new LinearFrameAnimation(9, 3);
            var longSwingAnim = new LinearFrameAnimation(11, 5);
            var finalSwingAnim = new LinearFrameAnimation(16, 7, LoopType.HoldLastFrame);

            //

            var ballActor = gameScene.AddActor("Ball", new Vector2(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2));
            new TextureRenderer(ballActor, Assets.GetTexture("ball"));
            new SpriteRenderer(ballActor, linkinSpriteSheet);
            new KeyboardMovement(ballActor);
            new TextRenderer(ballActor, defaultFont, "Hello world and this is a very long string");

            Actor linkin = gameScene.AddActor("Linkin", new Vector2(250, 250));
            var linkinRenderer = new SpriteRenderer(linkin, linkinSpriteSheet);
            new BoundingRect(linkin, 32, 32);
            new BoundingRectRenderer(linkin);
            new TweenChainComponent(linkin)
                .AddMoveTween(new Vector2(300, 300), 1, EaseFuncs.SineEaseIn)
                .AddWaitTween(2f)
                .AddMoveTween(new Vector2(1300, 0), 1, EaseFuncs.Linear);

            linkinRenderer.SetAnimation(walkAnim);
            linkinRenderer.SetupBoundingRect();

            var cameraScroller = gameScene.AddActor("CameraScroller");
            new ControlCamera(cameraScroller);

            var otherScene = new Scene();
            var microActor = otherScene.AddActor("MicroActor");
            microActor.Position = new Vector2(100, 500);
            new SpriteRenderer(microActor, linkinSpriteSheet).SetAnimation(standAnim);

            var sceneRenderBox = gameScene.AddActor("SceneRenderBox", new Vector2(200, 350));
            new BoundingRect(sceneRenderBox, new Point(160, 150));
            new Canvas(sceneRenderBox);
            new BoundingRectRenderer(sceneRenderBox);
            new SceneRenderer(sceneRenderBox, otherScene);
            otherScene.camera.Zoom = 1.5f;

            sceneRenderBox.parent.Set(linkin);

            ballActor.children.Add(sceneRenderBox);

            var ninepatchActor = gameScene.AddActor("Ninepatch", new Vector2(400, 400));
            new BoundingRect(ninepatchActor, new Point(400, 300));
            new NinepatchRenderer(ninepatchActor, testNinepatch);

            var progressBar = gameScene.AddActor("ProgressBar");
            progressBar.Position = new Vector2(500, 50);
            new BoundingRect(progressBar, new Point(500, 24));
            new ThreepatchRenderer(progressBar, progressBarThreepatch, Orientation.Horizontal);

            var pillar = gameScene.AddActor("Pillar");
            pillar.Position = new Vector2(300, 350);
            new BoundingRect(pillar, new Point(32, 500));
            new ThreepatchRenderer(pillar, pillarThreepatch, Orientation.Vertical);

            var mouse = gameScene.AddActor("gameCursor");
            new MouseCircle(mouse, 10);

            var miniMouse = otherScene.AddActor("miniCursor");
            new MouseCircle(miniMouse, 15);
            var cameraPanner = new PanCameraOnScroll(miniMouse, new MinMax<int>(0, 500));

            var scrollbar = gameScene.AddActor("Scrollbar");
            new BoundingRect(scrollbar, new Point(20, 20));
            new Scrollbar(scrollbar, sceneRenderBox.GetComponent<BoundingRect>(), cameraPanner);
            new BoundingRectRenderer(scrollbar);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (/*Keyboard.GetState().IsKeyDown(Keys.F4)*/ false)
            {
                if (!Graphics.IsFullScreen)
                {
                    Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                    Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                    Graphics.IsFullScreen = true;
                    Graphics.ApplyChanges();
                }
            }

            base.Update(gameTime);
        }
    }
}
