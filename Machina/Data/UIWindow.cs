﻿using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class UIWindow
    {
        /*
         * [_Header_________]
         * | C O N T E N T s|
         * |               c|
         * |  View         r|
         * |               o|
         * |               l|
         * |               l|
         * L................|
         */

        public readonly SceneRenderer sceneRenderer;
        /// <summary>
        /// Primary scene
        /// </summary>
        public readonly Scene scene;
        /// <summary>
        /// Transform of the parent-most actor
        /// </summary>
        public readonly Transform rootTransform;
        private readonly BoundingRect rootBoundingRect;

        /// <summary>
        /// LayoutGroup of the "Content" area of the window
        /// </summary>
        private readonly LayoutGroup contentGroup;

        /// <summary>
        /// Actor for the "Canvas" portion of the window
        /// </summary>
        public readonly Actor canvasActor;
        private readonly UIStyle style;

        public UIWindow(Scene creatingScene, Point contentSize, UIStyle style)
        {
            this.style = style;
            var headerSize = 32;
            var windowRoot = creatingScene.AddActor("Window");
            new BoundingRect(windowRoot, contentSize + new Point(0, headerSize));
            new Hoverable(windowRoot);
            new Draggable(windowRoot);
            new MoveOnDrag(windowRoot);
            new NinepatchRenderer(windowRoot, style.windowSheet, NinepatchSheet.GenerationDirection.Outer);
            var rootGroup = new LayoutGroup(windowRoot, Orientation.Vertical);

            rootGroup.HorizontallyStretchedSpacer(32);

            SceneRenderer sceneRenderer_local = null;
            Actor canvasActor_local = null;
            LayoutGroup contentGroup = null;

            // "Content" means everything below the header
            rootGroup.AddBothStretchedElement("ContentGroup", contentActor =>
            {
                contentGroup = new LayoutGroup(contentActor, Orientation.Horizontal);
                contentGroup.AddBothStretchedElement("Canvas", viewActor =>
                {
                    canvasActor_local = viewActor;
                    new Canvas(viewActor);
                    new Hoverable(viewActor);
                    sceneRenderer_local = new SceneRenderer(viewActor);
                });
            });

            this.sceneRenderer = sceneRenderer_local;
            this.canvasActor = canvasActor_local;
            this.scene = this.sceneRenderer.primaryScene;
            this.rootTransform = windowRoot.transform;
            this.rootBoundingRect = windowRoot.GetComponent<BoundingRect>();
            this.contentGroup = contentGroup;
        }

        public void AddScrollbar(int maxScrollPos)
        {
            var scrollbarWidth = 20;
            rootBoundingRect.Width += scrollbarWidth;
            this.contentGroup.AddVerticallyStretchedElement("scrollbar", scrollbarWidth, scrollbarActor =>
            {
                new Hoverable(scrollbarActor);
                new NinepatchRenderer(scrollbarActor, this.style.buttonDefault, NinepatchSheet.GenerationDirection.Inner);

                var scrollbar = new Scrollbar(scrollbarActor, this.canvasActor.GetComponent<BoundingRect>(), this.scene.camera, new MinMax<int>(0, maxScrollPos), this.style.buttonHover);

                // Scrollbar listener could be applied to any actor, but we'll just create one in this case
                new ScrollbarListener(scene.AddActor("Scrollbar Listener"), scrollbar);
            });
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