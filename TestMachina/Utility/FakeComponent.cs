using System;
using Machina.Components;
using Machina.Engine;

namespace TestMachina.Utility
{
    public class FakeComponent : BaseComponent
    {
        private readonly Action onDeleteLambda;

        public FakeComponent(Actor actor, Action onDeleteLambda) : base(actor)
        {
            this.onDeleteLambda = onDeleteLambda;
        }

        public override void OnDeleteFinished()
        {
            this.onDeleteLambda();
        }
    }
}
