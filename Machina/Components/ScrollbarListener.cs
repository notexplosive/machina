using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    /// <summary>
    /// If you have a scrollbar, you can only scroll it when hovering the bar. This ensures you can scroll it while anywhere else.
    /// Used primarily for SceneRenderers but there's no reason it can't be used elsewhere.
    /// </summary>
    class ScrollbarListener : BaseComponent
    {
        private readonly Scrollbar scrollbar;

        public ScrollbarListener(Actor actor, Scrollbar scrollbar) : base(actor)
        {
            this.scrollbar = scrollbar;
        }

        public override void OnScroll(int scrollDelta)
        {
            this.scrollbar.ApplyScrollDelta(scrollDelta);
        }
    }
}
