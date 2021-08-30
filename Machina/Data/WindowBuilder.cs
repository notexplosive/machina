using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class WindowBuilder
    {
        public bool CanBeClosed
        {
            get; private set;
        }

        public bool CanBeMaximized
        {
            get; private set;
        }

        public bool CanBeMinimized
        {
            get; private set;
        }


        private string title = string.Empty;
        private Point? minSize;
        private Point? maxSize;
        private bool canBeResized;
        private WindowAction onMinimized;
        private WindowAction onMaximized;
        private WindowAction onClosed;
        private bool isScrollable;
        private int maxScrollPos;
        private Vector2 startingPosition;
        private Action<UIWindow> onLaunch;
        private SpriteFrame icon;
        private Func<bool> shouldAllowKeyboardEventsLambda = () => false;
        private readonly Point contentSize;
        private readonly List<Action<Scene>> perLayerSceneFunctions = new List<Action<Scene>>();

        public WindowBuilder(Point contentSize)
        {
            this.contentSize = contentSize;
        }

        public UIWindow Build(Scene creatingScene, UIStyle style)
        {
            var window = new UIWindow(creatingScene, this.contentSize, CanBeClosed, CanBeMaximized, CanBeMinimized, this.icon, style);

            window.Closed += onClosed;
            window.Minimized += onMinimized;
            window.Maximized += onMaximized;

            if (this.isScrollable)
                window.AddScrollbar(this.maxScrollPos);

            if (this.canBeResized)
                window.BecomeResizable(this.minSize, this.maxSize);

            window.Title = this.title;
            window.rootTransform.Position = this.startingPosition;

            this.onLaunch?.Invoke(window);

            window.sceneRenderer.SetShouldAllowKeyboardEventsLambda(this.shouldAllowKeyboardEventsLambda);

            return window;
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

        public WindowBuilder DestroyOnClose()
        {
            return OnClose((win) => { win.Destroy(); });
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

        /// <summary>
        /// You should probably use OnLaunch instead.
        /// </summary>
        /// <param name="sceneCallback"></param>
        /// <returns></returns>
        public WindowBuilder AddSceneLayer(Action<Scene> sceneCallback)
        {
            this.perLayerSceneFunctions.Add(sceneCallback);
            return this;
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
