using System;
using Machina.Engine;
using Machina.Engine.Input;

namespace Machina.Components
{
    internal class CallbackOnClick : BaseComponent
    {
        private readonly MouseButton button;
        private readonly Action callback;
        private readonly Clickable clickable;

        public CallbackOnClick(Actor actor, Action callback, MouseButton button = MouseButton.Left) : base(actor)
        {
            this.callback = callback;
            this.button = button;
            this.clickable = RequireComponent<Clickable>();
            this.clickable.OnClick += FireCallback;
        }

        public override void OnDeleteFinished()
        {
            this.clickable.OnClick -= FireCallback;
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