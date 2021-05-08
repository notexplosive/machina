using Machina.Components;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestMachina.Utility
{
    public class FakeComponent : BaseComponent
    {
        private Action onDeleteLambda;

        public FakeComponent(Actor actor, Action onDeleteLambda) : base(actor)
        {
            this.onDeleteLambda = onDeleteLambda;
        }

        public override void OnDelete()
        {
            this.onDeleteLambda();
        }
    }
}
