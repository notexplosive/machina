using System;
using System.Collections.Generic;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    public class WindowBuilder
    {
        private readonly Point contentSize;
        private bool canBeResized;
        private SpriteFrame icon;
        private bool isScrollable;
        private int maxScrollPos;
        private Point? maxSize;
        private Point? minSize;
        private WindowAction onClosed;
        private Action<UIWindow> onLaunch;
        private WindowAction onMaximized;
        private WindowAction onMinimized;
        private Func<bool> shouldAllowKeyboardEventsLambda = () => false;
        private Vector2 startingPosition;
        private CartridgeBundle? cartridgeBundle;

        private string title = string.Empty;

        public WindowBuilder(Point contentSize)
        {
            this.contentSize = contentSize;
        }

        public bool CanBeClosed { get; private set; }

        public bool CanBeMaximized { get; private set; }

        public bool CanBeMinimized { get; private set; }

        public UIWindow Build(Scene creatingScene, UIStyle style)
        {
            var window = new UIWindow(creatingScene, this.contentSize, CanBeClosed, CanBeMaximized, CanBeMinimized,
                this.icon, style, cartridgeBundle);

            window.Closed += this.onClosed;
            window.Minimized += this.onMinimized;
            window.Maximized += this.onMaximized;

            if (this.isScrollable)
            {
                window.AddScrollbar(this.maxScrollPos);
            }

            if (this.canBeResized)
            {
                window.BecomeResizable(this.minSize, this.maxSize);
            }

            window.Title = this.title;
            window.rootTransform.Position = this.startingPosition;

            this.onLaunch?.Invoke(window);

            window.sceneRenderer.SetShouldAllowKeyboardEventsLambda(this.shouldAllowKeyboardEventsLambda);

            return window;
        }

        public WindowBuilder SetCartridgeBundle(CartridgeBundle cartridgeBundle)
        {
            this.cartridgeBundle = cartridgeBundle;
            return this;
        }

        public WindowBuilder Title(string title)
        {
            this.title = title;
            return this;
        }

        public WindowBuilder Icon(SpriteFrame icon)
        {
            this.icon = icon;
            return this;
        }

        public WindowBuilder OnLaunch(Action<UIWindow> onLaunch)
        {
            this.onLaunch += onLaunch;
            return this;
        }

        public WindowBuilder CanBeResized(Point minSize, Point maxSize)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.canBeResized = true;
            return this;
        }

        public WindowBuilder CanBeResized()
        {
            this.canBeResized = true;
            return this;
        }

        public WindowBuilder StartingPosition(Vector2 position)
        {
            this.startingPosition = position;
            return this;
        }

        public WindowBuilder OnClose(WindowAction onClosed)
        {
            CanBeClosed = true;
            this.onClosed += onClosed;
            return this;
        }

        public WindowBuilder DestroyViaCloseButton()
        {
            return OnClose(win => { win.Close(); });
        }

        public WindowBuilder OnMinimize(WindowAction onMinimized)
        {
            CanBeMinimized = true;
            this.onMinimized += onMinimized;
            return this;
        }

        public WindowBuilder OnMaximize(WindowAction onMaximize)
        {
            CanBeMaximized = true;
            this.onMaximized += onMaximize;
            return this;
        }

        public WindowBuilder Icon(Texture2D texture)
        {
            throw new NotImplementedException();
        }

        public WindowBuilder CanBeScrolled(int maxScrollPos)
        {
            this.isScrollable = true;
            this.maxScrollPos = maxScrollPos;
            return this;
        }

        public WindowBuilder AllowKeyboardEvents()
        {
            this.shouldAllowKeyboardEventsLambda = () => true;
            return this;
        }

        public WindowBuilder AllowKeyboardEventsWhen(Func<bool> func)
        {
            this.shouldAllowKeyboardEventsLambda = func;
            return this;
        }
    }
}