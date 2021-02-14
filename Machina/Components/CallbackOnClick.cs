using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class CallbackOnClick : BaseComponent
    {
        private readonly Action callback;
        private readonly MouseButton button;
        private readonly Clickable clickable;

        public CallbackOnClick(Actor actor, MouseButton button, Action callback) : base(actor)
        {
            this.callback = callback;
            this.button = button;
            this.clickable = RequireComponent<Clickable>();
            clickable.onClick += FireCallback;
        }

        public override void OnDelete()
        {
            clickable.onClick -= FireCallback;
        }

        public void FireCallback(MouseButton button)
        {
            if (button == this.button)
            {
                this.callback?.Invoke();
            }
        }
    }
}
