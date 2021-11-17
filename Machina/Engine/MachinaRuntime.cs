using Machina.Components;
using Machina.Engine.Debugging.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    using Assets;
    using Machina.Engine.Cartridges;
    using Machina.Engine.Input;
    using System;

    public class MachinaRuntime
    {
        public SoundEffectPlayer SoundEffectPlayer;
        public SpriteBatch spriteBatch;
        private readonly MachinaGameSpecification specification;
        public readonly GraphicsDeviceManager Graphics;
        public readonly FrameStep GlobalFrameStep = new FrameStep();
        public readonly MachinaInput input = new MachinaInput();

        public MachinaRuntime(GraphicsDeviceManager graphics, MachinaGameSpecification specification)
        {
            this.specification = specification;
            Graphics = graphics;
        }

        public Demo.Playback DemoPlayback { get; set; }
        public DebugLevel DebugLevel { get; set; }

        public GraphicsDevice GraphicsDevice { get; internal set; }

        public void InsertCartridge(Cartridge cartridge, GameWindow window, MachinaWindow machinaWindow)
        {
            CurrentCartridge = cartridge;
            CurrentCartridge.Setup(GraphicsDevice, this.specification, window, machinaWindow);
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

        internal void Draw()
        {
            CurrentCartridge.SceneLayers.PreDraw(spriteBatch);
            CurrentCartridge.CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
            GraphicsDevice.Clear(CurrentCartridge.SceneLayers.BackgroundColor);

            CurrentCartridge.SceneLayers.DrawOnCanvas(spriteBatch);
            CurrentCartridge.CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch);
            CurrentCartridge.SceneLayers.DrawDebugScene(spriteBatch);
        }

        internal void Update(float dt)
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