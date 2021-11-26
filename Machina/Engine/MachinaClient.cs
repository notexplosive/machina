﻿using System;
using Machina.Data;
using Machina.Engine.Debugging.Data;

namespace Machina.Engine
{
    using Assets;

    /// <summary>
    /// Static global accessor for all of the static global components in the game.
    /// </summary>
    public static class MachinaClient
    {
        public static MachinaRuntime Runtime { get; private set; } // Runtime should not be here!
        public static SoundEffectPlayer SoundEffectPlayer { get; private set; } // Needs to be initialzed, should not be here!
        public static NoiseBasedRNG RandomDirty { get; } = new NoiseBasedRNG((uint) DateTime.Now.Ticks & 0x0000FFFF);
        public static FrameStep GlobalFrameStep { get; } = new FrameStep();
        public static IAssetLibrary Assets { get; private set; } = new EmptyAssetLibrary();

        public static void Setup(MachinaRuntime runtime, IAssetLibrary assetLibrary)
        {
            Runtime = runtime;
            SoundEffectPlayer = new SoundEffectPlayer(runtime.Settings);
            Assets = assetLibrary;
        }

        /// <summary>
        ///     Print to the in-game debug console
        /// </summary>
        /// <param name="objects">Arbitrary list of any objects, converted with .ToString and delimits with spaces.</param>
        public static void Print(params object[] objects)
        {
            Runtime?.CurrentCartridge?.Logger.Log(objects);
            new StdOutConsoleLogger().Log(objects);
        }
    }
}