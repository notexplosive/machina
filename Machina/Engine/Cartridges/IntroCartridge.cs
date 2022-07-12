using System;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;

namespace Machina.Engine.Cartridges
{
    public class IntroCartridge : Cartridge
    {
        private readonly Action onEnd;

        public static Point RenderResolution(GameSettings settings)
        {
            int desiredWidth = MachinaClient.Graphics.GraphicsDevice.Viewport.Width;
            var aspect = (float) settings.startingWindowSize.X / desiredWidth;
            return new Vector2(settings.startingWindowSize.X / aspect, settings.startingWindowSize.Y / aspect).ToPoint();
        }

        public IntroCartridge(GameSettings settings, Action onEnd) : base(RenderResolution(settings), ResizeBehavior.KeepAspectRatio)
        {
            this.onEnd = onEnd;
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            var introScene = SceneLayers.AddNewScene();

            SceneLayers.BackgroundColor = new Color(0.1f, 0.1f, 0.1f);
            
            var textActor = introScene.AddActor("text");
            new IntroTextAnimation(textActor, new Vector2(MachinaClient.Graphics.GraphicsDevice.Viewport.Width, MachinaClient.Graphics.GraphicsDevice.Viewport.Height));
            new CallbackOnDestroy(textActor, onEnd);
        }
    }
}