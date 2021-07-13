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
        private Point? minSize;
        private Point? maxSize;
        private WindowAction onMinimized;
        private bool canBeMinimized;
        private WindowAction onMaximize;
        private bool canBeMaximized;
        private bool isScrollable;
        private int maxScrollPos;
        private readonly List<Action<Scene>> perLayerSceneFunctions = new List<Action<Scene>>();


        public UIWindow Build()
        {
            throw new NotImplementedException();
        }

        public WindowBuilder Title(string title)
        {
            this.title = title;
            return this;
        }

        public WindowBuilder IsResizable(Point maxSize, Point minSize)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
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

        public WindowBuilder IsScrollable(int maxScrollPos)
        {
            this.isScrollable = true;
            this.maxScrollPos = maxScrollPos;
            return this;
        }
    }
}
