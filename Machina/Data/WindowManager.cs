using System.Collections.Generic;
using System.Diagnostics;
using Machina.Engine;

namespace Machina.Data
{
    public class WindowManager
    {
        private readonly Depth baseDepth;
        private readonly UIBuilder uiBuilder;
        private readonly List<UIWindow> windows = new List<UIWindow>();

        public WindowManager(UIStyle style, Depth baseDepth)
        {
            this.uiBuilder = new UIBuilder(style);
            this.baseDepth = baseDepth;
        }

        public UIWindow CreateWindow(Scene creatingScene, WindowBuilder windowBuilder)
        {
            if (windowBuilder.CanBeClosed)
            {
                windowBuilder.OnClose(win => { this.windows.Remove(win); });
            }

            var window = windowBuilder.Build(creatingScene, this.uiBuilder.style);
            window.AnyPartOfWindowClicked += win => { SelectWindow(win); };
            this.windows.Add(window);
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
        ///     Put all the windows at their appropriate depths
        /// </summary>
        private void DepthSort()
        {
            for (var i = 0; i < this.windows.Count; i++)
            {
                var window = this.windows[i];
                var newDepth = this.baseDepth + new Depth(i * 10);
                window.rootTransform.Depth = newDepth;
            }
        }
    }
}