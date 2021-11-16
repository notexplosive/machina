using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    using Assets;
    using System;

    public abstract class Cartridge
    {
        private bool hasBeenSetup = false;
        public SceneLayers SceneLayers { get; private set; }
        public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;
        public GameCanvas CurrentGameCanvas => this.SceneLayers.gameCanvas as GameCanvas;

        private readonly Point renderResolution;
        private readonly ResizeBehavior resizeBehavior;
        private readonly bool skipDebug;

        public SeededRandom Random { get; private set; }

        protected Cartridge(Point renderResolution, ResizeBehavior resizeBehavior, bool skipDebug = false)
        {
            this.renderResolution = renderResolution;
            this.resizeBehavior = resizeBehavior;
            this.skipDebug = skipDebug;
            Random = new SeededRandom();
        }

        public void Setup(GraphicsDevice graphicsDevice, MachinaGameSpecification specification, GameWindow window, MachinaWindow machinaWindow)
        {
            if (!this.hasBeenSetup)
            {
                BuildSceneLayers();

                CurrentGameCanvas.BuildCanvas(graphicsDevice);

                // platform specific!
                // should become something like platformContext.OnCartridgeSetup()
                if (GamePlatform.IsDesktop)
                {
                    machinaWindow.Resized += (size) => CurrentGameCanvas.SetWindowSize(size);
                    window.TextInput += SceneLayers.AddPendingTextInput;
                }

                OnGameLoad(specification);

                this.hasBeenSetup = true;
            }
        }

        public abstract void OnGameLoad(MachinaGameSpecification specification);

        protected void BuildSceneLayers()
        {
            if (SceneLayers == null)
            {
                // this is lazy initialized for the dumbest reason: to be debuggable it needs to have a font, the font doesn't come in until after loading is complete
                SceneLayers = new SceneLayers(!this.skipDebug, new GameCanvas(renderResolution, resizeBehavior));
            }
        }
    }
}