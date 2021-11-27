using Machina.Components;
using Machina.Engine;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data
{
    public delegate void WindowAction(UIWindow window);

    public class UIWindow : IWindow
    {
        /// <summary>
        ///     Actor for the "Canvas" portion of the window
        /// </summary>
        public readonly Actor canvasActor;

        /// <summary>
        ///     LayoutGroup of the "Content" area of the window
        /// </summary>
        private readonly LayoutGroup contentGroup;

        private readonly int margin = 10;
        public event Action<Point> Resized;
        private readonly BoundingRect rootBoundingRect;

        /// <summary>
        ///     Transform of the parent-most actor
        /// </summary>
        public readonly Transform rootTransform;

        /// <summary>
        ///     Primary scene of the content within the window
        /// </summary>
        public readonly Scene scene;
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
        private readonly UIStyle style;
        private BoundedTextRenderer titleTextRenderer;

        public bool IsFullScreen { get; private set; }

        public UIWindow(Scene parentScene, Point contentSize, bool canBeClosed, bool canBeMaximized,
            bool canbeMinimized, SpriteFrame icon, UIStyle style)
        {
            this.style = style;
            var windowRoot = parentScene.AddActor("Window");
            this.rootBoundingRect = new BoundingRect(windowRoot, Point.Zero);
            SetSize(contentSize);

            var rootGroup = new LayoutGroup(windowRoot, Orientation.Vertical);

            rootGroup.SetMarginSize(new Point(this.margin, this.margin));
            rootGroup.AddHorizontallyStretchedElement("HeaderContent", 32, headerContentActor =>
            {
                new Hoverable(headerContentActor);
                new Draggable(headerContentActor).DragStart +=
                    (position, delta) => OnAnyPartOfWindowClicked(MouseButton.Left);
                new MoveOnDrag(headerContentActor, windowRoot.transform);

                var headerGroup = new LayoutGroup(headerContentActor, Orientation.Horizontal);
                if (icon != null)
                {
                    headerGroup.AddVerticallyStretchedElement("Icon", 32, iconActor =>
                    {
                        new SpriteRenderer(iconActor, icon.spriteSheet).SetAnimation(icon.animation);

                        iconActor.GetComponent<BoundingRect>().SetOffsetToCenter();
                    });
                    headerGroup.PixelSpacer(5); // Spacing between icon and title
                }

                headerGroup.AddBothStretchedElement("Title",
                    titleActor =>
                    {
                        this.titleTextRenderer = new BoundedTextRenderer(titleActor, "Window title goes here",
                            style.uiElementFont, Color.White, verticalAlignment: VerticalAlignment.Center,
                            depthOffset: -2).EnableDropShadow(Color.Black);
                    });

                if (canbeMinimized)
                {
                    CreateControlButton(headerGroup, windowRoot, win => Minimized?.Invoke(win),
                        this.style.minimizeButtonFrames);
                }

                if (canBeMaximized)
                {
                    CreateControlButton(headerGroup, windowRoot, win => Maximized?.Invoke(win),
                        this.style.maximizeButtonFrames);
                }

                if (canBeClosed)
                {
                    CreateControlButton(headerGroup, windowRoot, win => Closed?.Invoke(win),
                        this.style.closeButtonFrames);
                }
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
                                new BoundedCanvas(viewActor);
                                new Hoverable(viewActor);
                                new Clickable(viewActor).ClickStarted += OnAnyPartOfWindowClicked;
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
        }

        public Scrollbar Scrollbar { get; private set; }

        public string Title
        {
            get => this.titleTextRenderer.Text;
            set => this.titleTextRenderer.Text = value;
        }

        public BoundedCanvas Canvas => this.canvasActor.GetComponent<BoundedCanvas>();

        public event WindowAction Closed;
        public event WindowAction Minimized;
        public event WindowAction Maximized;
        public event WindowAction AnyPartOfWindowClicked;

        private void CreateControlButton(LayoutGroup headerGroup, Actor windowRoot, WindowAction controlButtonEvent,
            IFrameAnimation frames)
        {
            headerGroup.AddVerticallyStretchedElement("ControlButton", 32, closeButtonActor =>
            {
                new Hoverable(closeButtonActor);
                var clickable = new Clickable(closeButtonActor);
                new ButtonSpriteRenderer(closeButtonActor, this.style.uiSpriteSheet, frames);
                clickable.ClickStarted += OnAnyPartOfWindowClicked;
                clickable.OnClick += mouseButton =>
                {
                    if (mouseButton == MouseButton.Left)
                    {
                        controlButtonEvent.Invoke(this);
                    }
                };
            });
        }

        private void OnAnyPartOfWindowClicked(MouseButton mouseButton)
        {
            // mouseButton arg is ignored on purpose
            AnyPartOfWindowClicked?.Invoke(this);
        }

        public void AddScrollbar(int maxScrollPos)
        {
            var scrollbarWidth = 20;
            this.rootBoundingRect.Width += scrollbarWidth;
            Scrollbar scrollbar_local = null;
            this.contentGroup.AddVerticallyStretchedElement("scrollbar", scrollbarWidth, scrollbarActor =>
            {
                new Hoverable(scrollbarActor);
                new NinepatchRenderer(scrollbarActor, this.style.buttonDefault);

                scrollbar_local = new Scrollbar(scrollbarActor, this.canvasActor.GetComponent<BoundingRect>(),
                    this.scene.camera, new MinMax<int>(0, maxScrollPos), this.style.buttonHover);

                // Scrollbar listener could be applied to any actor, but we'll just create one in this case
                new ScrollbarListener(this.scene.AddActor("Scrollbar Listener"), scrollbar_local);
            });

            Scrollbar = scrollbar_local;
        }


        public BoundingRectResizer BecomeResizable(Point? minSize, Point? maxSize)
        {
            new Hoverable(this.rootTransform.actor);
            var resizer = new BoundingRectResizer(this.rootTransform.actor, new XYPair<int>(this.margin, this.margin), minSize,
                maxSize, rect =>
                {
                    rect.Y += this.margin;
                    rect.Height -= this.margin;
                    return rect;
                });
            resizer.Resized += (sender, eventArgs) =>
            {
                Resized?.Invoke(eventArgs.NewSize.ToPoint());
            };
            return resizer;
        }

        public void SetSize(Point contentSize)
        {
            var headerSize = 32;
            this.rootBoundingRect.SetSize(contentSize + new Point(0, headerSize) + new Point(this.margin * 2, this.margin * 2));
        }

        public Point CurrentSize => this.canvasActor.GetComponent<BoundingRect>().Size;

        public void Destroy()
        {
            foreach (var scene in this.scene.sceneLayers.AllScenes())
            {
                scene.DeleteAllActors();
            }

            this.rootTransform.actor.Destroy();
        }

        public void Delete()
        {
            this.rootTransform.actor.Delete();
        }

        public bool IsOpen()
        {
            return !this.rootTransform.actor.IsDestroyed;
        }

        public void SetFullscreen(bool fullscreen)
        {
            IsFullScreen = fullscreen;
            MachinaClient.Print("Not implemented");
        }

        public void ApplyChanges()
        {
            MachinaClient.Print("Apply changes (no op)");
        }

        public void AddOnTextInputEvent(EventHandler<TextInputEventArgs> callback)
        {
            MachinaClient.Print("Add OnTextInputEvent (no op??)");
        }
    }
}