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
        private string title = string.Empty;
        private Point minSize;
        private Point maxSize;
        private bool canBeResized;
        private WindowAction onMinimized;
        private bool canBeMinimized;
        private WindowAction onMaximize;
        private bool canBeMaximized;
        private WindowAction onClosed;
        private bool canBeClosed;
        private bool isScrollable;
        private int maxScrollPos;
        private Vector2 startingPosition;

        private readonly Point contentSize;
        private readonly List<Action<Scene>> perLayerSceneFunctions = new List<Action<Scene>>();

        public WindowBuilder(Point contentSize)
        {
            this.contentSize = contentSize;
        }

        public UIWindow Build(Scene creatingScene, UIStyle style)
        {
            var window = new UIWindow(creatingScene, this.contentSize, style);

            if (this.isScrollable)
                window.AddScrollbar(this.maxScrollPos);

            if (this.canBeResized)
                window.BecomeResizable(this.minSize, this.maxSize);

            window.Title = this.title;
            window.rootTransform.Position = this.startingPosition;

            return window;
        }

        public WindowBuilder Title(string title)
        {
            this.title = title;
            return this;
        }

        public WindowBuilder CanBeResized(Point minSize, Point maxSize)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.canBeResized = true;
            return this;
        }

        public WindowBuilder StartingPosition(Vector2 position)
        {
            this.startingPosition = position;
            return this;
        }

        public WindowBuilder CanBeClosed(WindowAction onClosed)
        {
            this.canBeClosed = true;
            this.onClosed += onClosed;
            return this;
        }

        public WindowBuilder CanBeMinimized(WindowAction onMinimized)
        {
            this.canBeMinimized = true;
            this.onMinimized += onMinimized;
            return this;
        }

        public WindowBuilder CanBeMaximized(WindowAction onMaximize)
        {
            this.canBeMaximized = true;
            this.onMaximize += onMaximize;
            return this;
        }

        public WindowBuilder Icon(Texture2D texture)
        {
            throw new NotImplementedException();
        }

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
    }
}
