using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Machina.Engine.Cartridges
{
    class CrashCartridge : Cartridge
    {
        private readonly Exception exception;

        public CrashCartridge(GameSettings settings, Exception exception) : base(settings.startingWindowSize, ResizeBehavior.FillContent, true)
        {
            this.exception = exception;
        }

        public override void OnGameLoad(MachinaGameSpecification specification, MachinaRuntime runtime)
        {
            SceneLayers.BackgroundColor = Color.Maroon;
            var scene = SceneLayers.AddNewScene();
            var rootActor = scene.AddActor("Main");

            var errorText = this.exception.Message + "\n\n" + this.exception.StackTrace;

            var filePath = $"{Path.Join(runtime.fileSystem.AppDataPath, $"crashdump-{DateTime.Now.ToFileTime()}.txt")}";
            runtime.fileSystem.WriteStringToAppData(errorText, filePath, true);

            var titleText = "Game Crashed, sorry about that :(";
            var contactInfoText = $"You can also get this message in text form at:\n{filePath}\nReach out to @NotExplosive on Twitter so I can fix it";

            new BoundingRect(rootActor, Point.Zero);
            new BoundingRectToViewportSize(rootActor);
            new LayoutGroup(rootActor, Orientation.Vertical)
                .AddHorizontallyStretchedElement("title", 64, titleActor =>
                {
                    new BoundedTextRenderer(titleActor, titleText, MachinaGame.defaultStyle.uiElementFont);
                })
                .AddHorizontallyStretchedElement("contact-info", 80, contactInfoActor =>
                {
                    new BoundedTextRenderer(contactInfoActor, contactInfoText, MachinaGame.defaultStyle.uiElementFont, Color.White, HorizontalAlignment.Left, VerticalAlignment.Top, Overflow.Ignore);
                })
                .AddBothStretchedElement("content", contentActor =>
                {
                    new BoundedTextRenderer(contentActor, errorText, MachinaGame.defaultStyle.uiElementFont);
                });
        }
    }
}
