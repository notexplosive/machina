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
        private readonly int margin = 10;

        public event Action Closed;
        public event Action AnyPartOfWindowClicked;

        public UIWindow(Scene creatingScene, Point contentSize, UIStyle style)
        {
            this.style = style;
            var headerSize = 32;
            var windowRoot = creatingScene.AddActor("Window");
            new BoundingRect(windowRoot, contentSize + new Point(0, headerSize));

            var rootGroup = new LayoutGroup(windowRoot, Orientation.Vertical);

            rootGroup.SetMargin(this.margin);
            rootGroup.AddHorizontallyStretchedElement("HeaderContent", 32, headerContentActor =>
             {
                 new Hoverable(headerContentActor);
                 new Draggable(headerContentActor).DragStart += vec => AnyPartOfWindowClicked?.Invoke();
                 new MoveOnDrag(headerContentActor, windowRoot.transform);

                 new LayoutGroup(headerContentActor, Orientation.Horizontal)
                 .AddVerticallyStretchedElement("Icon", 32, iconActor =>
                 {
                     new SpriteRenderer(iconActor, style.uiSpriteSheet)
                         .SetAnimation(new ChooseFrameAnimation(1));

                     iconActor.GetComponent<BoundingRect>().SetOffsetToCenter();
                 })
                 .PixelSpacer(5) // Spacing between icon and title
                 .AddBothStretchedElement("Title", titleActor =>
                 {
                     new BoundedTextRenderer(titleActor, "Window title goes here", style.uiElementFont, Color.White, verticalAlignment: VerticalAlignment.Center, depthOffset: -2).EnableDropShadow(Color.Black);
                 })
                 .AddVerticallyStretchedElement("CloseButton", 32, closeButtonActor =>
                 {
                     new Hoverable(closeButtonActor);
                     var clickable = new Clickable(closeButtonActor);
                     new ButtonSpriteRenderer(closeButtonActor, this.style.uiSpriteSheet, this.style.closeButtonFrames);
                     clickable.ClickStarted += () => { AnyPartOfWindowClicked?.Invoke(); };
                     clickable.onClick += mouseButton =>
                     {
                         if (mouseButton == MouseButton.Left)
                         {
                             Closed?.Invoke();
                             windowRoot.Destroy();
                         }
                     };
                 })
                 .AddVerticallyStretchedElement("CloseButton", 32, closeButtonActor =>
                 {
                     new Hoverable(closeButtonActor);
                     var clickable = new Clickable(closeButtonActor);
                     new ButtonSpriteRenderer(closeButtonActor, this.style.uiSpriteSheet, this.style.closeButtonFrames);
                     clickable.ClickStarted += () => { AnyPartOfWindowClicked?.Invoke(); };
                     clickable.onClick += mouseButton =>
                     {
                         if (mouseButton == MouseButton.Left)
                         {
                             Closed?.Invoke();
                             windowRoot.Destroy();
                         }
                     };
                 })
                 .AddVerticallyStretchedElement("CloseButton", 32, closeButtonActor =>
                 {
                     new Hoverable(closeButtonActor);
                     var clickable = new Clickable(closeButtonActor);
                     new ButtonSpriteRenderer(closeButtonActor, this.style.uiSpriteSheet, this.style.closeButtonFrames);
                     clickable.ClickStarted += () => { AnyPartOfWindowClicked?.Invoke(); };
                     clickable.onClick += mouseButton =>
                     {
                         if (mouseButton == MouseButton.Left)
                         {
                             Closed?.Invoke();
                             windowRoot.Destroy();
                         }
                     };
                 });
             });


            // These variables with _local at the end of their name are later assigned to readonly values
            // The locals are assigned in lambdas which is illegal for readonly assignment
            SceneRenderer sceneRenderer_local = null;
            Actor canvasActor_local = null;
            LayoutGroup contentGroup_local = null;

            // "Content" means everything below the header

            rootGroup.AddBothStretchedElement("ContentGroup", contentActor =>
                {
                    new NinepatchRenderer(contentActor, style.windowSheet, NinepatchSheet.GenerationDirection.Outer);

                    contentGroup_local = new LayoutGroup(contentActor, Orientation.Horizontal)
                        .AddBothStretchedElement("Canvas", viewActor =>
                        {
                            canvasActor_local = viewActor;
                            new Canvas(viewActor);
                            new Hoverable(viewActor);
                            new Clickable(viewActor).ClickStarted += () => { AnyPartOfWindowClicked?.Invoke(); };
                            sceneRenderer_local = new SceneRenderer(viewActor);
                        })
                        ;
                })
                ;

            this.sceneRenderer = sceneRenderer_local;
            this.canvasActor = canvasActor_local;
            this.scene = this.sceneRenderer.primaryScene;
            this.rootTransform = windowRoot.transform;
            this.rootBoundingRect = windowRoot.GetComponent<BoundingRect>();
            this.contentGroup = contentGroup_local;

            this.rootTransform.FlushBuffers();
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

        public BoundingRectResizer AddResizer(Point minSize, Point maxSize)
        {
            new Hoverable(rootTransform.actor);
            return new BoundingRectResizer(rootTransform.actor, new XYPair<int>(this.margin, this.margin), minSize, maxSize);
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
