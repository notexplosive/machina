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

        public void SetupSceneLayers()
        {
            // This function is only exposed on GameCartridge, normal cartridges cannot do this
            BuildSceneLayers();
        }
    }
}
