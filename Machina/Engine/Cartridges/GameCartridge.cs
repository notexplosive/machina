using Machina.Engine.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Cartridges
{
    public abstract class GameCartridge : Cartridge
    {
        protected GameCartridge(Point renderResolution, ResizeBehavior resizeBehavior) : base(renderResolution, resizeBehavior, false)
        {
        }

        /// <summary>
        /// Just before the loading screen runs it collects up all the assets it needs to load (namely the Textures, Sounds, SpriteFonts, etc in Content)
        /// If there are any other assets you want it to load (such as MachinaAssets or dynamically generated Textures) you can set them up here.
        /// </summary>
        /// <param name="loader">Use this to request assets to be loaded</param>
        /// <param name="graphicsDevice">MonoGame GraphicsDevice, you might need this to build textures dynamically</param>
        public abstract void PrepareDynamicAssets(AssetLoader loader, MachinaRuntime runtime);

        public void SetupSceneLayers(MachinaRuntime runtime, GameSpecification specification, IWindow machinaWindow)
        {
            BuildSceneLayers(runtime);

            SceneLayers.OnError += (exception) =>
            {
                runtime.InsertCartridge(new CrashCartridge(specification.settings, exception));
            };
        }
    }
}
