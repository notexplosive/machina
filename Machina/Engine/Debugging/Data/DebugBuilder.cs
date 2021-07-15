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

        public static void CreateDebugDock(SceneLayers sceneLayers)
        {
            var rowHeight = 90;
            var iconWidth = 90;
            int titleHeight = 32;
            var dockMargin = 5;
            int iconPadding = 5;

            var dockActor = sceneLayers.debugScene.AddActor("Debug Dock");
            new InvokableDebugTool(dockActor, new KeyCombination(Keys.Tab, new ModifierKeys(true, false, false)));
            new BoundingRect(dockActor, new Point(iconWidth * 3 + iconPadding * 2 + dockMargin * 2, rowHeight * 2 + titleHeight + dockMargin * 2));
            new Hoverable(dockActor);
            new Draggable(dockActor);
            new MoveOnDrag(dockActor);
            new BoundingRectFill(dockActor, new Color(Color.Black, 0.5f));
            var group = new LayoutGroup(dockActor, Orientation.Vertical)
                .AddHorizontallyStretchedElement("Title", titleHeight, titleActor =>
                {
                    new BoundedTextRenderer(titleActor, "Machina Debug Dock", MachinaGame.defaultStyle.uiElementFont, Color.White, HorizontalAlignment.Center, VerticalAlignment.Center);
                })
                .SetMargin(dockMargin)
            ;

            LayoutGroup AddRow()
            {
                LayoutGroup row = null;
                group.AddHorizontallyStretchedElement("Row", rowHeight, rowActor =>
                {
                    row = new LayoutGroup(rowActor, Orientation.Horizontal)
                        .SetPaddingBetweenElements(iconPadding);
                });
                return row;
            }

            void AddIcon(LayoutGroup row, string iconName, Action callBack)
            {
                row.AddVerticallyStretchedElement("IconRootActor", iconWidth, iconActor =>
                 {
                     new Hoverable(iconActor);
                     new Clickable(iconActor);
                     var doubleClickable = new DoubleClickable(iconActor);
                     doubleClickable.DoubleClick += (button) =>
                     {
                         if (button == MouseButton.Left)
                         {
                             callBack?.Invoke();
                         }
                     };

                     new DebugIconHoverRenderer(iconActor);
                     new LayoutGroup(iconActor, Orientation.Vertical)
                        .SetMargin(5)
                        .AddHorizontallyStretchedElement("IconImage", 50, iconImageActor =>
                        {
                            new BoundingRectFill(iconImageActor, Color.Orange);
                        })
                        .AddBothStretchedElement("IconText", iconTextActor =>
                        {
                            new BoundedTextRenderer(iconTextActor, iconName, MachinaGame.Assets.GetSpriteFont("TinyFont"), Color.White, HorizontalAlignment.Center, VerticalAlignment.Top);
                        });
                 });
            }

            var windowManager = new WindowManager(MachinaGame.defaultStyle, dockActor.transform.Depth - 100);

            var row = AddRow();
            AddIcon(row, "Scene Graph Renderer", () => CreateSceneGraphRenderer(sceneLayers, windowManager));
            AddIcon(row, "Console Window", null);
            AddIcon(row, "Asset Viewer", null);
        }

        public static UIWindow CreateSceneGraphRenderer(SceneLayers sceneLayers, WindowManager windowManager)
        {
            var window = windowManager.CreateWindow(sceneLayers.debugScene,
                new WindowBuilder(new Point(300, 300))
                    .CanBeScrolled(900)
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
