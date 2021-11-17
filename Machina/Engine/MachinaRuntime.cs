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
    using System.IO;

    public class MachinaRuntime
    {
        public SoundEffectPlayer SoundEffectPlayer;
        public SpriteBatch spriteBatch;
        private readonly GameSpecification specification;
        public readonly GraphicsDeviceManager Graphics;
        private readonly MachinaGame game;
        public readonly FrameStep GlobalFrameStep = new FrameStep();
        public readonly MachinaInput input = new MachinaInput();
        public readonly MachinaFileSystem fileSystem;


        public MachinaRuntime(MachinaGame game, GraphicsDeviceManager graphics, GameSpecification specification)
        {
            this.specification = specification;
            Graphics = graphics;
            this.game = game;

            // TODO: I don't think this works on Android, might need some alternative
            var appDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NotExplosive", this.specification.gameTitle);

            this.fileSystem = new MachinaFileSystem(appDataPath);
        }

        public Demo.Playback DemoPlayback { get; set; }
        public DebugLevel DebugLevel { get; set; }

        public GraphicsDevice GraphicsDevice { get; internal set; }

        public void InsertCartridge(Cartridge cartridge, GameWindow window, MachinaWindow machinaWindow)
        {
            CurrentCartridge = cartridge;
            CurrentCartridge.Setup(this, GraphicsDevice, this.specification, window, machinaWindow);
            CurrentCartridge.CurrentGameCanvas.SetWindowSize(machinaWindow.CurrentWindowSize);
            Graphics.ApplyChanges();
        }

        public void Quit()
        {
            this.game.Exit();
        }

        public Cartridge CurrentCartridge { get; private set; }

        public void RunDemo(string demoName)
        {
            if (CurrentCartridge is GameCartridge gameCartridge)
            {
                var demoActor = CurrentCartridge.SceneLayers.debugScene.AddActor("DebugActor");
                var demoPlaybackComponent = new DemoPlaybackComponent(demoActor);
                DemoPlayback = demoPlaybackComponent.SetDemo(gameCartridge, Demo.FromDisk_Sync(demoName, this.fileSystem), demoName, 1);
                demoPlaybackComponent.ShowGui = false;
            }
            else
            {
                MachinaClient.Print("Demo loading is only supported on GameCartridges");
            }
        }

        internal void Draw()
        {
            CurrentCartridge.SceneLayers.PreDraw(spriteBatch);
            CurrentCartridge.CurrentGameCanvas.SetRenderTargetToCanvas(GraphicsDevice);
            GraphicsDevice.Clear(CurrentCartridge.SceneLayers.BackgroundColor);

            CurrentCartridge.SceneLayers.DrawOnCanvas(spriteBatch);
            CurrentCartridge.CurrentGameCanvas.DrawCanvasToScreen(GraphicsDevice, spriteBatch, this);
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