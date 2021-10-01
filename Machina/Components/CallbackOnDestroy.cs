using System;
using Machina.Engine;

namespace Machina.Components
{
    public class CallbackOnDestroy : BaseComponent
    {
        private readonly Action callback;

        public CallbackOnDestroy(Actor actor, Action callback) : base(actor)
        {
            this.callback = callback;
        }

        public override void OnActorDestroy()
        {
            this.callback();
        }
    }
}
