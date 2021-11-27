using System;
using Machina.Data;
using Machina.Engine.Debugging.Data;

namespace Machina.Engine
{
    using Assets;
    using Microsoft.Xna.Framework;
    using System.IO;

    /// <summary>
    /// Static global accessor for all of the static global components in the game.
    /// </summary>
    public static class MachinaClient
    {
        public static SoundEffectPlayer SoundEffectPlayer { get; private set; } // Needs to be initialzed, should not be here!
        public static GraphicsDeviceManager Graphics { get; private set; } // Needs to be initialized, cannot provide a default
        public static UIStyle DefaultStyle { get; private set; } // needs to be initialized
        public static NoiseBasedRNG RandomDirty { get; } = new NoiseBasedRNG((uint) DateTime.Now.Ticks & 0x0000FFFF);
        public static FrameStep GlobalFrameStep { get; } = new FrameStep();
        public static IAssetLibrary Assets { get; private set; } = new EmptyAssetLibrary();
        internal static LogBuffer LogBuffer { get; } = new LogBuffer();
        public static MachinaFileSystem FileSystem { get; private set; } = new MachinaFileSystem("Unknown");

        public static void Setup(IAssetLibrary assetLibrary, GameSpecification specification, GraphicsDeviceManager graphics, string devContentPath)
        {
            SoundEffectPlayer = new SoundEffectPlayer(specification.settings);
            Assets = assetLibrary;
            FileSystem = new MachinaFileSystem(specification.gameTitle, devContentPath);
            Graphics = graphics;
        }

        public static void SetupDefaultStyle()
        {
            DefaultStyle = new UIStyle(
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-hover"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-button-press"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-textbox-ninepatch"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-window-ninepatch"),
                MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("ui-slider-ninepatch"),
                MachinaClient.Assets.GetSpriteFont("DefaultFontSmall"),
                MachinaClient.Assets.GetMachinaAsset<SpriteSheet>("ui-checkbox-radio-spritesheet"),
                MachinaClient.Assets.GetMachinaAsset<Image>("ui-checkbox-checkmark-image"),
                MachinaClient.Assets.GetMachinaAsset<Image>("ui-radio-fill-image")
            );
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            LogBuffer.Add(objects);
        }
    }
}
