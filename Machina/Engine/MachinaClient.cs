using System;
using Machina.Data;
using Machina.Engine.Debugging.Data;

namespace Machina.Engine
{
    using Assets;
    using System.IO;

    /// <summary>
    /// Static global accessor for all of the static global components in the game.
    /// </summary>
    public static class MachinaClient
    {
        public static SoundEffectPlayer SoundEffectPlayer { get; private set; } // Needs to be initialzed, should not be here!
        public static NoiseBasedRNG RandomDirty { get; } = new NoiseBasedRNG((uint) DateTime.Now.Ticks & 0x0000FFFF);
        public static FrameStep GlobalFrameStep { get; } = new FrameStep();
        public static IAssetLibrary Assets { get; private set; } = new EmptyAssetLibrary();
        internal static LogBuffer LogBuffer { get; } = new LogBuffer();
        public static MachinaFileSystem FileSystem { get; private set; } = new MachinaFileSystem("Unknown");


        public static void Setup(IAssetLibrary assetLibrary, GameSpecification specification)
        {
            SoundEffectPlayer = new SoundEffectPlayer(specification.settings);
            Assets = assetLibrary;
            FileSystem = new MachinaFileSystem(specification.gameTitle);
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
