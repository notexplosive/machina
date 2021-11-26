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
            var assetTree = AssetLibrary.GetStaticAssetLoadTree();
            gameCartridge.PrepareDynamicAssets(assetTree, runtime);
            PrepareLoadInitialStyle(assetTree, runtime);

            var loadingScreen =
                new LoadingScreen(assetTree);

            var introScene = SceneLayers.AddNewScene();
            var loader = introScene.AddActor("Loader");
            var adHoc = new AdHoc(loader);

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
                        loadingScreen.Update(assets, dt / increment);
                    }
                }

                if (loadingScreen.IsDoneDrawLoading() && loadingScreen.IsDoneUpdateLoading())
                {
                    onFinished(gameCartridge);
                }
            };
        }

        private void PrepareLoadInitialStyle(AssetLoader loader, MachinaRuntime runtime)
        {
            loader.AddMachinaAssetCallback("ui-button",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 0, 24, 24), new Rectangle(8, 8, 8, 8), runtime));
            loader.AddMachinaAssetCallback("ui-button-hover",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(24, 0, 24, 24),
                    new Rectangle(8 + 24, 8, 8, 8), runtime));
            loader.AddMachinaAssetCallback("ui-button-press",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(48, 0, 24, 24),
                    new Rectangle(8 + 48, 8, 8, 8), runtime));
            loader.AddMachinaAssetCallback("ui-slider-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 144, 24, 24),
                    new Rectangle(8, 152, 8, 8), runtime));
            loader.AddMachinaAssetCallback("ui-checkbox-checkmark-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 6));
            loader.AddMachinaAssetCallback("ui-radio-fill-image",
                () => new Image(new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)), 7));
            loader.AddMachinaAssetCallback("ui-checkbox-radio-spritesheet",
                () => new GridBasedSpriteSheet("button-ninepatches", new Point(24, 24)));
            loader.AddMachinaAssetCallback("ui-textbox-ninepatch",
                () => new NinepatchSheet("button-ninepatches", new Rectangle(0, 96, 24, 24),
                    new Rectangle(8, 104, 8, 8), runtime));
            loader.AddMachinaAssetCallback("ui-window-ninepatch",
                () => new NinepatchSheet("window", new Rectangle(0, 0, 96, 96), new Rectangle(10, 34, 76, 52), runtime));
        }
    }
}