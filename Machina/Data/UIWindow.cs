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
        public readonly Transform rootTransform;
        /// <summary>
        /// Actor for the "Content" portion of the window
        /// </summary>
        public readonly Actor contentActor;

        public UIWindow(Scene creatingScene, Point contentSize, UIStyle style)
        {
            var headerSize = 32;
            var windowRoot = creatingScene.AddActor("Window");
            new BoundingRect(windowRoot, contentSize + new Point(0, headerSize));
            var rootGroup = new LayoutGroup(windowRoot, Orientation.Vertical);
            new Hoverable(windowRoot);
            new Draggable(windowRoot);
            new MoveOnDrag(windowRoot);
            new NinepatchRenderer(windowRoot, style.windowSheet, NinepatchSheet.GenerationDirection.Outer);

            rootGroup.PixelSpacer(headerSize);

            SceneRenderer sceneRenderer_local = null;
            Actor contentActor_local = null;

            rootGroup.AddBothStretchedElement("ContentGroup", contentActor =>
            {
                contentActor_local = contentActor;
                var contentGroup = new LayoutGroup(contentActor, Orientation.Horizontal);
                contentGroup.AddBothStretchedElement("View", viewActor =>
                {
                    new Canvas(viewActor);
                    new Hoverable(viewActor);
                    sceneRenderer_local = new SceneRenderer(viewActor);
                });
            });

            this.sceneRenderer = sceneRenderer_local;
            this.contentActor = contentActor_local;
            this.scene = this.sceneRenderer.primaryScene;
            this.rootTransform = windowRoot.transform;
        }

        public void Destroy()
        {
            rootTransform.actor.Destroy();
        }

        public void Delete()
        {
            rootTransform.actor.Delete();
        }
    }
}
