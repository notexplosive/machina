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
            var consoleFont = Assets.GetSpriteFont("ConsoleFont");

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

            // Button layout example
            {
                var layout = gameScene.AddActor("Layout", new Vector2(300, 300));
                new BoundingRect(layout, 256, 500);
                var uiGroup = new LayoutGroup(layout, Orientation.Vertical);
                uiGroup.PaddingBetweenElements = 5;
                uiGroup.SetMargin(15);
                new NinepatchRenderer(layout, defaultStyle.windowSheet, NinepatchSheet.GenerationDirection.Outer);

                uiBuilder.BuildButton(uiGroup, "Click me!", 32);
                uiBuilder.BuildCheckbox(uiGroup, "Check me out!", true);
                uiBuilder.BuildCheckbox(uiGroup, "Check me out!");

                var radioLayout = gameScene.AddActor("Inner Layout");
                new BoundingRect(radioLayout, new Point(32, 124));
                var innerGroup = new LayoutGroup(radioLayout, Orientation.Vertical);
                innerGroup.PaddingBetweenElements = 5;
                innerGroup.SetMargin(0);
                var innerGroupElement = new LayoutElement(radioLayout)
                    .StretchHorizontally();
                radioLayout.transform.SetParent(layout);
                uiBuilder.BuildLabel(innerGroup, "Section title:");
                uiBuilder.BuildRadioButton(innerGroup, "Choose me!");
                uiBuilder.BuildRadioButton(innerGroup, "Or me!", true);
                uiBuilder.BuildRadioButton(innerGroup, "What about me!");

                uiBuilder.BuildDropdownMenu(uiGroup,
                    new DropdownContent.DropdownItem("First"),
                    new DropdownContent.DropdownItem("Second but it's super loooong"),
                    new DropdownContent.DropdownItem("Third"));

                uiBuilder.BuildDropdownMenu(uiGroup,
                    new DropdownContent.DropdownItem("Other First"),
                    new DropdownContent.DropdownItem("Other Second"),
                    new DropdownContent.DropdownItem("Third?!?!!"));

                uiBuilder.BuildTextField(uiGroup);

                uiBuilder.BuildSlider(uiGroup);
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
