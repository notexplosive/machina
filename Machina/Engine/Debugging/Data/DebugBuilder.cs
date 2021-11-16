using Machina.Components;
using Machina.Data;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
            var tool = new InvokableDebugTool(frameStepActor,
                new KeyCombination(Keys.Space, new ModifierKeys(true, false, false)));
            new FrameStepRenderer(frameStepActor, MachinaGame.Current.Runtime.GlobalFrameStep, sceneLayers, tool);
            new BoundingRect(frameStepActor, new Point(64, 64));
            new Hoverable(frameStepActor);
            new Draggable(frameStepActor);
            new MoveOnDrag(frameStepActor);
        }

        public static DebugDock CreateDebugDock(SceneLayers sceneLayers)
        {
            var dockActor = sceneLayers.debugScene.AddActor("Debug Dock");
            new InvokableDebugTool(dockActor, new KeyCombination(Keys.Tab, new ModifierKeys(true, false, false)));
            new BoundingRect(dockActor, Point.Zero);
            new Hoverable(dockActor);
            new Draggable(dockActor);
            new MoveOnDrag(dockActor);
            new BoundingRectFill(dockActor, new Color(Color.Black, 0.5f));
            new LayoutGroup(dockActor, Orientation.Vertical);
            var dock = new DebugDock(dockActor);

            dock.AddApp(new App("Scene Graph", true,
                new WindowBuilder(new Point(300, 300))
                    .CanBeScrolled(900)
                    .CanBeResized(new Point(300, 300), new Point(1920, 1080))
                    .Title("Scene Graph")
                    .DestroyViaCloseButton()
                    .OnLaunch(window =>
                    {
                        var sceneGraphActor = window.scene.AddActor("SceneGraphActor");
                        new BoundingRect(sceneGraphActor, Point.Zero);
                        new SceneGraphData(sceneGraphActor, sceneLayers);
                        new Hoverable(sceneGraphActor);
                        new SceneGraphUI(sceneGraphActor, dock.windowManager, sceneLayers.debugScene);
                        new SceneGraphRenderer(sceneGraphActor, window.Scrollbar);
                    })));
            dock.AddApp(new App("Console", true,
                new WindowBuilder(new Point(600, 300))
                    .CanBeScrolled(900)
                    .CanBeResized(new Point(300, 300), new Point(1920, 1080))
                    .DestroyViaCloseButton()
                    .Title("Machina Console")
                    .OnLaunch(window =>
                    {
                        var consoleActor = window.scene.AddActor("StaticConsole");
                        new WindowedConsoleRenderer(consoleActor, window.Scrollbar);
                    })));

            return dock;
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