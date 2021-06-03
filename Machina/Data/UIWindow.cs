using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class UIWindow
    {
        public readonly SceneRenderer sceneRenderer;
        /// <summary>
        /// Primary scene
        /// </summary>
        public readonly Scene scene;
        /// <summary>
        /// Transform of the parent-most actor
        /// </summary>
        public readonly Transform RootTransform;

        public UIWindow(Scene creatingScene, Point windowSize, UIStyle style)
        {
            var windowRoot = creatingScene.AddActor("Window");
            new BoundingRect(windowRoot, windowSize);
            var rootGroup = new LayoutGroup(windowRoot, Orientation.Vertical);
            new Hoverable(windowRoot);
            new Draggable(windowRoot);
            new MoveOnDrag(windowRoot);
            new NinepatchRenderer(windowRoot, style.windowSheet, NinepatchSheet.GenerationDirection.Outer);

            rootGroup.PixelSpacer(32);

            SceneRenderer sceneRenderer_local = null;

            rootGroup.AddBothStretchedElement("ContentGroup", contentActor =>
            {
                var contentGroup = new LayoutGroup(contentActor, Orientation.Horizontal);
                contentGroup.AddBothStretchedElement("View", viewActor =>
                {
                    new Canvas(viewActor);
                    new Hoverable(viewActor);
                    sceneRenderer_local = new SceneRenderer(viewActor);
                });
            });

            this.sceneRenderer = sceneRenderer_local;
            this.scene = this.sceneRenderer.primaryScene;
            this.RootTransform = windowRoot.transform;
        }

        public void Destroy()
        {
            RootTransform.actor.Destroy();
        }

        public void Delete()
        {
            RootTransform.actor.Delete();
        }
    }
}
