using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    using Assets;
    using Machina.Engine.Debugging.Data;
    using System;
    using System.Collections.Generic;

    public abstract class Cartridge
    {
        private bool hasBeenSetup = false;
        public SceneLayers SceneLayers { get; private set; }
        public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;
        public GameViewport CurrentGameCanvas => this.SceneLayers.gameCanvas as GameViewport;
        public Stack<ILogger> loggerStack = new Stack<ILogger>();

        public ILogger Logger
        {
            get
            {
                if (loggerStack.Count == 0)
                {
                    return new StdOutConsoleLogger();
                }

                return loggerStack.Peek();
            }
        }

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

        public void Setup(MachinaRuntime runtime, GameSpecification specification)
        {
            if (!this.hasBeenSetup)
            {
                BuildSceneLayers(runtime);

                CurrentGameCanvas.BuildCanvas(runtime.Painter);

                runtime.platformContext.OnCartridgeSetup(this, runtime.WindowInterface);
                runtime.WindowInterface.Resized += (size) => CurrentGameCanvas.SetWindowSize(size);

                OnGameLoad(specification, runtime);

                this.hasBeenSetup = true;
            }
        }

        /// <summary>
        /// This is executed right after the loading screen finishes. This is your "Main" function.
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="runtime"></param>
        public abstract void OnGameLoad(GameSpecification specification, MachinaRuntime runtime);

        protected void BuildSceneLayers(MachinaRuntime runtime)
        {
            if (SceneLayers == null)
            {
                // this is lazy initialized for the dumbest reason: to be debuggable it needs to have a font, the font doesn't come in until after loading is complete
                SceneLayers = new SceneLayers(new GameViewport(renderResolution, resizeBehavior), runtime);
                if (!this.skipDebug)
                {
                    SceneLayers.BuildDebugScene(this);
                }
            }
        }

        public void PushLogger(ILogger newLogger)
        {
            loggerStack.Push(newLogger);
        }

        public void PopLogger()
        {
            loggerStack.Pop();
        }
    }
}