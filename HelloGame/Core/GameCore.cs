using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Machina.ThirdParty;
using System;
using Machina.Engine;
using System.Collections.Generic;

namespace HelloGame
{
    public class GameCore : MachinaGame
    {
        private Scene gameScene;
        private Scene uiScene;
        private GameSettings settings;

        public GameCore() : base(new Point(1600, 900), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnGameLoad()
        {

            void Intro()
            {
                var oldLayers = this.sceneLayers;
                var gameCanvas = new GameCanvas(new Point(300, 300), ResizeBehavior.MaintainDesiredResolution);
                gameCanvas.BuildCanvas(GraphicsDevice);
                var introLayers = new SceneLayers(true, gameCanvas, this.sceneLayers.frameStep);
                var introScene = introLayers.AddNewScene();
                var textActor = introScene.AddActor("text");
                new TextRenderer(textActor, MachinaGame.Assets.GetSpriteFont("LogoFont"), "notexplosive.net");

                var backgroundActor = introScene.AddActor("background");
                backgroundActor.transform.Depth += 10;
                new BoundingRect(backgroundActor, new Point(300, 300));
                new BoundingRectRenderer(backgroundActor);

                // Steal control
                this.sceneLayers = introLayers;

                Action onEnd = () =>
                {
                    // Restore control
                    this.sceneLayers = oldLayers;
                };

                var timer = introScene.AddActor("Timer");
                new DestroyTimer(timer, 5);
                new CallbackOnDestroy(timer, onEnd);
            }

            gameScene = sceneLayers.AddNewScene();
            uiScene = sceneLayers.AddNewScene();

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
            var consoleFont = Assets.GetSpriteFont("DefaultFontSmall");

            var testNinepatch = Assets.GetMachinaAsset<NinepatchSheet>("test-ninepatch");
            var linkinSpriteSheet = Assets.GetMachinaAsset<SpriteSheet>("linkin-sprite-sheet");

            var standAnim = new LinearFrameAnimation(0, 5);
            var walkAnim = new LinearFrameAnimation(6, 3);
            var quickSwingAnim = new LinearFrameAnimation(9, 3);
            var longSwingAnim = new LinearFrameAnimation(11, 5);
            var finalSwingAnim = new LinearFrameAnimation(16, 7, LoopType.HoldLastFrame);

            //

            var cameraScroller = gameScene.AddActor("CameraScroller");
            new PanAndZoomCamera(cameraScroller, Keys.LeftControl);

            var uiBuilder = new UIBuilder(defaultStyle);

            /*
            IEnumerator<ICoroutineAction> testCoroutine()
            {
                MachinaGame.Print("Waited 0 second");
                yield return new WaitSeconds(1);
                MachinaGame.Print("Waited 1 second");
                yield return new WaitSeconds(1);
                MachinaGame.Print("Waited 2 seconds");
                yield return new WaitUntil(() => { return true; });
                MachinaGame.Print("Instant");
                yield return null;
            }

            IEnumerator<ICoroutineAction> testCoroutine2()
            {
                MachinaGame.Print("Waited 0 second");
                yield return new WaitSeconds(0.25f);
                MachinaGame.Print("Waited 0.25 second");
                yield return new WaitSeconds(0.25f);
                MachinaGame.Print("Waited 0.5 second");
                yield return new WaitSeconds(0.25f);
                MachinaGame.Print("Waited 0.75 second");
            }
            */

            // gameScene.StartCoroutine(testCoroutine());
            // gameScene.StartCoroutine(testCoroutine2());


            // Button layout example
            {
                var layout = gameScene.AddActor("Layout", new Vector2(300, 300));
                new BoundingRect(layout, 256, 500);
                var uiGroup = new LayoutGroup(layout, Orientation.Vertical);
                uiGroup.PaddingBetweenElements = 5;
                uiGroup.SetMargin(15);
                new NinepatchRenderer(layout, defaultStyle.windowSheet, NinepatchSheet.GenerationDirection.Outer);

                uiBuilder.BuildLabel(uiGroup, "Graphics Settings");
                settings.fullscreenState = uiBuilder.BuildCheckbox(uiGroup, "Fullscreen", false);
                uiBuilder.BuildLabel(uiGroup, "Music Volume");
                settings.musicVolumeState = uiBuilder.BuildSlider(uiGroup);
                uiBuilder.BuildLabel(uiGroup, "Sound Volume");
                settings.soundVolumeState = uiBuilder.BuildSlider(uiGroup);
                uiBuilder.BuildSpacer(uiGroup, Point.Zero, true, true);
                uiBuilder.BuildButton(uiGroup, "Apply", () => { settings.Apply(); });
                uiBuilder.BuildTextField(uiGroup);
            }

            Intro();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }
    }
}
