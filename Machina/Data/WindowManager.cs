using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public class WindowManager
    {
        private readonly UIBuilder uiBuilder;
        private readonly List<UIWindow> windows = new List<UIWindow>();

        public WindowManager(UIStyle style)
        {
            this.uiBuilder = new UIBuilder(style);
        }

        public UIWindow BuildWindow(Scene creatingScene, Point contentSize)
        {
            var window = new UIWindow(creatingScene, contentSize, this.uiBuilder.style);
            this.windows.Add(window);
            window.Closed += () => { windows.Remove(window); };
            return window;
        }
    }
}
