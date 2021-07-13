using Machina.Components;
using Machina.Data;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Data
{
    public class DebugBuilder
    {
        public static void CreateFramerateCounter(SceneLayers sceneLayers)
        {
            var framerateCounterActor = sceneLayers.debugScene.AddActor("FramerateCounter");
            new FrameRateCounter(framerateCounterActor);
        }

        public static void CreateFramestep(SceneLayers sceneLayers)
        {
            var frameStepActor = sceneLayers.debugScene.AddActor("FrameStepActor");
            var tool = new InvokableDebugTool(frameStepActor, new KeyCombination(Keys.Space, new ModifierKeys(true, false, false)));
            new FrameStepRenderer(frameStepActor, sceneLayers.frameStep, sceneLayers, tool);
            new BoundingRect(frameStepActor, new Point(64, 64));
            new Hoverable(frameStepActor);
            new Draggable(frameStepActor);
            new MoveOnDrag(frameStepActor);
        }

        public static UIWindow CreateSceneGraphRenderer(SceneLayers sceneLayers, WindowManager windowManager)
        {
            var window = windowManager.CreateWindow(sceneLayers.debugScene, new WindowBuilder(new Point(300, 300))
                .CanBeScrolled(300)
                .CanBeResized(new Point(300, 300), new Point(1920, 1080))
                .Title("Scene Graph")
                );

            var sceneGraphActor = window.scene.AddActor("SceneGraphActor");
            new SceneGraphRenderer(sceneGraphActor, sceneLayers, window.Scrollbar);

            return window;
        }

        public static Logger BuildOutputConsole(SceneLayers sceneLayers)
        {
            var consoleFont = MachinaGame.Assets.GetSpriteFont("DefaultFont");
            var debugActor = sceneLayers.debugScene.AddActor("DebugActor", depthAsInt: 100);
            new EnableDebugOnHotkey(debugActor, new KeyCombination(Keys.OemTilde, new ModifierKeys(true, false, true)));
            return new Logger(debugActor, new ConsoleOverlay(debugActor, consoleFont));
        }
    }
}
