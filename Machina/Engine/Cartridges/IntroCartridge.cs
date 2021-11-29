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
            const int desiredWidth = 1920 / 4;
            var AspectRatio = (float) settings.startingWindowSize.X / desiredWidth;
            return new Vector2(settings.startingWindowSize.X / AspectRatio, settings.startingWindowSize.Y / AspectRatio).ToPoint();
        }

        public IntroCartridge(GameSettings settings, Action onEnd) : base(RenderResolution(settings), ResizeBehavior.KeepAspectRatio)
        {
            this.onEnd = onEnd;
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            var introScene = SceneLayers.AddNewScene();

            var textActor = introScene.AddActor("text");
            new BoundingRect(textActor, 20, 20);
            new BoundingRectToViewportSize(textActor);
            new BoundedTextRenderer(textActor, "", MachinaClient.Assets.GetSpriteFont("LogoFont"), Color.White,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            new IntroTextAnimation(textActor);
            new CallbackOnDestroy(textActor, onEnd);
        }
    }
}