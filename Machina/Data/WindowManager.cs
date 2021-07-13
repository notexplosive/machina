using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public class WindowManager
    {
        private readonly UIBuilder uiBuilder;
        private readonly Depth baseDepth;
        private readonly List<UIWindow> windows = new List<UIWindow>();

        public WindowManager(UIStyle style, Depth baseDepth)
        {
            this.uiBuilder = new UIBuilder(style);
            this.baseDepth = baseDepth;
        }

        public UIWindow CreateWindow(Scene creatingScene, Point contentSize, WindowBuilder windowBuilder)
        {
            var window = windowBuilder.Build(creatingScene, contentSize, this.uiBuilder.style);
            this.windows.Add(window);
            window.Closed += () => { windows.Remove(window); };
            window.AnyPartOfWindowClicked += () => { SelectWindow(window); };
            SelectWindow(window);

            return window;
        }

        public void SelectWindow(UIWindow window)
        {
            var index = this.windows.IndexOf(window);
            Debug.Assert(index != -1);
            if (index != 0)
            {
                this.windows.Remove(window);
                this.windows.Insert(0, window);
            }

            DepthSort();
        }

        /// <summary>
        /// Put all the windows at their appropriate depths
        /// </summary>
        private void DepthSort()
        {
            for (int i = 0; i < this.windows.Count; i++)
            {
                var window = this.windows[i];
                window.rootTransform.Depth = this.baseDepth + new Depth(i * 10);
            }
        }
    }
}
