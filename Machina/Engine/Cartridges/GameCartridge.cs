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

        public abstract void PrepareDynamicAssets(AssetLoadTree loadTree, GraphicsDevice graphicsDevice);

        public void SetupSceneLayers(MachinaRuntime runtime, GameSpecification specification, GameWindow window, MachinaWindow machinaWindow)
        {
            BuildSceneLayers();

            SceneLayers.OnError += (exception) =>
            {
                runtime.InsertCartridge(new CrashCartridge(specification.settings, exception), window, machinaWindow);
            };
        }
    }
}
