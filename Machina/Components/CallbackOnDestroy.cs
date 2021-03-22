using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class CallbackOnDestroy : BaseComponent
    {
        private Action callback;

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
