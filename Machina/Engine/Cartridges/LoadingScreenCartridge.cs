using System;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;

namespace Machina.Engine.Cartridges
{
    using Assets;
    using Microsoft.Xna.Framework.Graphics;

    public class LoadingScreenCartridge : Cartridge
    {
        public LoadingScreenCartridge(GameSettings gameSettings) : base(gameSettings.startingWindowSize, ResizeBehavior.FreeAspectRatio, true)
        {
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            SceneLayers.BackgroundColor = Color.Black;
        }

        public void PrepareLoadingScreen(GameCartridge gameCartridge, MachinaRuntime runtime, AssetLibrary assets, WindowInterface machinaWindow, Action<GameCartridge> onFinished)
        {
            var loader = assets.GetStaticAssetLoadTree();
            LoadDefaultStyle(loader, assets, runtime.Painter);
            gameCartridge.PrepareDynamicAssets(loader, runtime);

            var loadingScreen =
                new LoadingScreen(loader);

            var introScene = SceneLayers.AddNewScene();
            var loaderActor = introScene.AddActor("Loader");
            var adHoc = new AdHoc(loaderActor);

            adHoc.onPreDraw += (spriteBatch) =>
            {
                if (!loadingScreen.IsDoneUpdateLoading())
                {
                    // Waiting for this to complete before draw loading
                }
                else if (!loadingScreen.IsDoneDrawLoading())
                {
                    loadingScreen.IncrementDrawLoopLoad(assets, spriteBatch);
                }
            };

            adHoc.onDraw += (spriteBatch) =>
            {
                loadingScreen.Draw(spriteBatch, machinaWindow.CurrentWindowSize);
            };

            adHoc.onUpdate += (dt) =>
            {
                if (!loadingScreen.IsDoneUpdateLoading())
                {
                    var increment = 3;
                    for (var i = 0; i < increment; i++)
                    {
                        loadingScreen.Update(dt / increment);
                    }
                }

                if (loadingScreen.IsDoneDrawLoading() && loadingScreen.IsDoneUpdateLoading())
                {
                    onFinished(gameCartridge);
                }
            };
        }

        private void LoadDefaultStyle(AssetLoader loader, AssetLibrary library, Painter painter)
        {
            loader.ForceLoadAsset("images/button-ninepatches");
            loader.ForceLoadAsset("images/window");
            loader.ForceLoadAsset("fonts/DefaultFontSmall");

            library.AddMachinaAsset("ui-button",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8), painter));
            library.AddMachinaAsset("ui-button-hover",
                new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24),
                    new Rectangle(8 + 24, 8, 8, 8), painter));
            library.AddMachinaAsset("ui-button-press",
                new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24),
                    new Rectangle(8 + 48, 8, 8, 8), painter));
            library.AddMachinaAsset("ui-slider-ninepatch",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24),
                    new Rectangle(8, 152, 8, 8), painter));
            library.AddMachinaAsset("ui-checkbox-checkmark-image",
                new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            library.AddMachinaAsset("ui-radio-fill-image",
                new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            library.AddMachinaAsset("ui-checkbox-radio-spritesheet",
                new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            library.AddMachinaAsset("ui-textbox-ninepatch",
                new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24),
                    new Rectangle(8, 104, 8, 8), painter));
            library.AddMachinaAsset("ui-window-ninepatch",
                new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52), painter));

            MachinaClient.SetupDefaultStyle();
        }
    }
}