using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging
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

        public static void CreateSceneGraphRenderer(SceneLayers sceneLayers, WindowManager windowManager)
        {
            // Scene graph renderer
            {
                var sceneGraphContainer = windowManager.CreateWindow(sceneLayers.debugScene, new Point(300, 300));
                var scrollbar = sceneGraphContainer.AddScrollbar(300);
                sceneGraphContainer.AddResizer(new Point(300, 300), new Point(1920, 1080));
                var tool = new InvokableDebugTool(sceneGraphContainer.rootTransform.actor, new KeyCombination(Keys.Tab, new ModifierKeys(true, false, false)));

                var sceneGraphActor = sceneGraphContainer.scene.AddActor("SceneGraphActor");
                new SceneGraphRenderer(sceneGraphActor, sceneLayers, scrollbar);
            }
        }
    }
}
