using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    /// <summary>
    ///     If you have a scrollbar, you can only scroll it when hovering the bar. This ensures you can scroll it while
    ///     anywhere else.
    ///     Used primarily for SceneRenderers but there's no reason it can't be used elsewhere.
    /// </summary>
    public class ScrollbarListener : BaseComponent
    {
        private readonly Scrollbar scrollbar;
        private ModifierKeys latestModifiers;

        public ScrollbarListener(Actor actor, Scrollbar scrollbar) : base(actor)
        {
            this.scrollbar = scrollbar;
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            this.latestModifiers = modifiers;
        }

        public override void OnScroll(int scrollDelta)
        {
            // todo: if(this.actor.scene.NothingHovered)
            if (this.latestModifiers.None)
            {
                this.scrollbar.ApplyScrollWheelDelta(scrollDelta);
            }
        }
    }
}