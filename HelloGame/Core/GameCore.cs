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
        private Scene gameScene;
        private Scene uiScene;

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
            gameScene = sceneLayers.AddNewScene(this.gameCanvas);
            uiScene = sceneLayers.AddNewScene(this.gameCanvas);

            Assets.AddMachinaAsset("hoop-sprite-sheet", new GridBasedSpriteSheet(Assets.GetTexture("hoop"), new Point(32, 32)));
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
            new BoundingRect(ballActor, Point.Zero);
            new SpriteRenderer(ballActor, linkinSpriteSheet).SetupBoundingRect();
            new KeyboardMovement(ballActor);
            new Hoverable(ballActor);
            new Draggable(ballActor);
            new MoveOnDrag(ballActor);
            ballActor.transform.Depth = 0.2f;
            ballActor.transform.Angle = MathF.PI / 2;

            Actor other = gameScene.AddActor("other", ballActor.transform.Position + new Vector2(100, 0), 0.5f, 0.3f);
            new SpriteRenderer(other, linkinSpriteSheet).SetupBoundingRect().SetAnimation(standAnim);
            new Hoverable(other);
            new Draggable(other);
            new MoveOnDrag(other);
            other.SetParent(ballActor);

            Actor other2 = gameScene.AddActor("other2", ballActor.transform.Position + new Vector2(25, 200), 1f, 0.4f);
            new SpriteRenderer(other2, linkinSpriteSheet).SetupBoundingRect().SetAnimation(standAnim);
            new Hoverable(other2);
            new Draggable(other2);
            new MoveOnDrag(other2);
            other2.SetParent(other);

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
            new PanAndZoomCamera(cameraScroller, Keys.LeftControl);

            var innerScene = new Scene();
            var microActor = innerScene.AddActor("MicroActor");
            microActor.transform.Position = new Vector2(10, 500);
            new BoundingRect(microActor, Point.Zero);
            new SpriteRenderer(microActor, linkinSpriteSheet).SetAnimation(standAnim).SetupBoundingRect();
            new Hoverable(microActor);

            var microActor2 = innerScene.AddActor("MicroActor");
            microActor2.transform.Position = new Vector2(10, 50);
            new BoundingRect(microActor2, Point.Zero);
            new SpriteRenderer(microActor2, linkinSpriteSheet).SetAnimation(standAnim).SetupBoundingRect();
            new Hoverable(microActor2);

            var sceneRenderBox = gameScene.AddActor("SceneRenderBox", new Vector2(200, 350));
            new BoundingRect(sceneRenderBox, new Point(160, 450));
            new Canvas(sceneRenderBox);
            new Hoverable(sceneRenderBox);
            new SceneRenderer(sceneRenderBox, innerScene, () => { return true; });
            innerScene.camera.Zoom = 1.5f;

            var ninepatchActor = gameScene.AddActor("Ninepatch", new Vector2(400, 400));
            new BoundingRect(ninepatchActor, new Point(400, 300));
            new NinepatchRenderer(ninepatchActor, testNinepatch);
            new Hoverable(ninepatchActor);
            new Clickable(ninepatchActor);
            new CallbackOnClick(ninepatchActor, MouseButton.Left, () => { ninepatchActor.Visible = !ninepatchActor.Visible; });

            var progressBar = gameScene.AddActor("ProgressBar");
            progressBar.transform.Position = new Vector2(500, 50);
            new BoundingRect(progressBar, new Point(500, 24));
            new ThreepatchRenderer(progressBar, progressBarThreepatch, Orientation.Horizontal);

            var pillar = uiScene.AddActor("Pillar");
            pillar.transform.Position = new Vector2(300, 350);
            new BoundingRect(pillar, new Point(32, 500));
            new ThreepatchRenderer(pillar, pillarThreepatch, Orientation.Vertical);
            new Hoverable(pillar);
            new TextRenderer(pillar, consoleFont, "Hello from the UI Scene!");
            pillar.transform.Depth = 1f;

            var mouse = gameScene.AddActor("gameCursor");
            new MouseCircle(mouse, 10, Color.BlueViolet);

            var uiMouse = uiScene.AddActor("gameCursor");
            new MouseCircle(uiMouse, 20, Color.CadetBlue);

            var scrollbarActor = gameScene.AddActor("Scrollbar");
            new BoundingRect(scrollbarActor, new Point(20, 20));
            new Hoverable(scrollbarActor);
            var scrollbar = new Scrollbar(scrollbarActor, sceneRenderBox.GetComponent<BoundingRect>(), innerScene.camera, new MinMax<int>(0, 500));
            new BoundingRectRenderer(scrollbarActor);

            var miniMouse = innerScene.AddActor("miniCursor");
            new ScrollbarListener(miniMouse, scrollbar);
            new MouseCircle(miniMouse, 15, Color.LightBlue);
            new PanAndZoomCamera(miniMouse, Keys.LeftShift);

            {
                var selectorActor = gameScene.AddActor("selector", new Vector2(100, 0));
                var selector = new SingleSelector(selectorActor);

                var selectable = gameScene.AddActor("selectable", new Vector2(80, 50));
                new BoundingRect(selectable, new Point(50, 50));
                new Hoverable(selectable);
                new Clickable(selectable);
                selector.BuildSelectable(selectable);

                var selectable2 = gameScene.AddActor("selectable2", new Vector2(130, 50));
                new BoundingRect(selectable2, new Point(50, 50));
                new Hoverable(selectable2);
                new Clickable(selectable2);
                selector.BuildSelectable(selectable2);

                var selectable3 = gameScene.AddActor("selectable3", new Vector2(180, 50));
                new BoundingRect(selectable3, new Point(50, 50));
                new Hoverable(selectable3);
                new Clickable(selectable3);
                selector.BuildSelectable(selectable3);
            }


            var uiBuilder = new UIBuilder(
                new UIStyle(
                    Assets.GetMachinaAsset<NinepatchSheet>("ui-button"),
                    Assets.GetMachinaAsset<NinepatchSheet>("ui-button-hover"),
                    Assets.GetMachinaAsset<NinepatchSheet>("ui-button-press"),
                    defaultFont,
                    Assets.GetMachinaAsset<SpriteSheet>("checkbox-bg-sprites"),
                    Assets.GetMachinaAsset<Image>("checkbox-checkmark-image")
                    ));

            // Button layout example
            {
                var layout = gameScene.AddActor("Layout", new Vector2(300, 100));
                new BoundingRect(layout, 256, 400);
                var uiGroup = new LayoutGroup(layout, Orientation.Vertical);
                uiGroup.PaddingBetweenElements = 5;
                uiGroup.SetMargin(15);

                uiBuilder.BuildButton(uiGroup, "Click me!", 32);
                uiBuilder.BuildCheckbox(uiGroup, "Check me out!");
            }

            // Horizontal layout example
            {
                var horizontalLayout = gameScene.AddActor("Layout", new Vector2(800, 200));
                new BoundingRect(horizontalLayout, 256, 128);
                var uiGroup = new LayoutGroup(horizontalLayout, Orientation.Horizontal);
                uiGroup.PaddingBetweenElements = 5;
                uiGroup.SetMargin(15);

                uiBuilder.BuildSpacer(uiGroup, new Point(32, 32), false, false);
                uiBuilder.BuildSpacer(uiGroup, new Point(64, 32), false, true);
                uiBuilder.BuildSpacer(uiGroup, new Point(32, 32), true, true);
            }
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
