using Machina.Components;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    using Assets;
    using Machina.Engine.Input;

    public class MachinaRuntime
    {
        public SoundEffectPlayer SoundEffectPlayer;
        public SpriteBatch spriteBatch;
        public readonly GraphicsDeviceManager Graphics;
        public readonly FrameStep GlobalFrameStep = new FrameStep();
        public readonly MachinaInput input = new MachinaInput();

        public MachinaRuntime(GraphicsDeviceManager graphics)
        {
            Graphics = graphics;
        }

        public Demo.Playback DemoPlayback { get; set; }
        public DebugLevel DebugLevel { get; set; }
        public GraphicsDevice GraphicsDevice { get; internal set; }

        public void InsertCartridge(Cartridge cartridge, MachinaGameSpecification specification, GameWindow window, MachinaWindow machinaWindow)
        {
            CurrentCartridge = cartridge;
            CurrentCartridge.Setup(GraphicsDevice, specification, window, machinaWindow);
            CurrentCartridge.CurrentGameCanvas.SetWindowSize(machinaWindow.CurrentWindowSize);
            Graphics.ApplyChanges();
        }

        public Cartridge CurrentCartridge { get; private set; }

        public void RunDemo(string demoName)
        {
            var demoActor = CurrentCartridge.SceneLayers.debugScene.AddActor("DebugActor");
            var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
            DemoPlayback = demoPlaybackComponent.SetDemo(Demo.FromDisk_Sync(demoName), demoName, 1);
            demoPlaybackComponent.ShowGui = false;
        }

        internal void Draw(AssetLibrary assets, bool isDoneUpdateLoading, LoadingScreen loadingScreen, MachinaWindow machinaWindow)
        {
            if (!isDoneUpdateLoading)
            {
                loadingScreen.Draw(this.spriteBatch, machinaWindow.CurrentWindowSize, GraphicsDevice);
            }
            else if (!loadingScreen.IsDoneDrawLoading())
            {
                loadingScreen.IncrementDrawLoopLoad(assets, spriteBatch);
                loadingScreen.Draw(spriteBatch, machinaWindow.CurrentWindowSize, GraphicsDevice);
            }
            else
            {
                CurrentCartridge.SceneLayers.PreDraw(spriteBatch);
                CurrentCartridge.CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
                GraphicsDevice.Clear(CurrentCartridge.SceneLayers.BackgroundColor);

                CurrentCartridge.SceneLayers.DrawOnCanvas(spriteBatch);
                CurrentCartridge.CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
                CurrentCartridge.SceneLayers.DrawDebugScene(spriteBatch);
            }
        }

        internal void Update(float dt, bool isDoneUpdateLoading, AssetLibrary assets, LoadingScreen loadingScreen)
        {
            if (!isDoneUpdateLoading)
            {
                var library = assets;
                var increment = 3;
                for (var i = 0; i < increment; i++)
                {
                    loadingScreen.Update(library, dt / increment);
                }
            }
            else if (!loadingScreen.IsDoneDrawLoading())
            {
                // waiting for draw load
            }
            else
            {
                if (DemoPlayback != null && DemoPlayback.IsFinished == false)
                {
                    for (var i = 0; i < DemoPlayback.playbackSpeed; i++)
                    {
                        var frameState = DemoPlayback.UpdateAndGetInputFrameStates(dt);
                        DemoPlayback.PollHumanInput(this.input.GetHumanFrameState());
                        CurrentCartridge.SceneLayers.Update(dt, Matrix.Identity, frameState);
                    }
                }
                else
                {
                    CurrentCartridge.SceneLayers.Update(dt, Matrix.Identity, this.input.GetHumanFrameState());
                }
            }
        }
    }
}