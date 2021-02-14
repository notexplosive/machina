using Machina.Components;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    internal class ActorTests : TestGroup
    {
        private class FakeComponent : BaseComponent
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

        public ActorTests() : base("Actor Tests")
        {
            AddTest(new Test("Component deletes when actor is deleted", test =>
            {
                var scene = new Scene();
                var actor = scene.AddActor("Terry Triangle");
                var deleteCount = 0;
                new FakeComponent(actor, () => { deleteCount++; });
                actor.Destroy();
                // Scene needs to update to flush the destroyed actors
                scene.Update(0f);

                test.Expect(1, deleteCount, "Deleted wrong number of times");
            }));

            AddTest(new Test("Test Add and remove Components", test =>
            {
                var actor = new Actor("Sally Sceneless", null);
                var deleteCount = 0;
                var createdComponent = new FakeComponent(actor, () => { deleteCount++; });
                FakeComponent actual = actor.GetComponent<FakeComponent>();
                actor.RemoveComponent<FakeComponent>();
                var actualAfterRemove_BeforeUpdate = actor.GetComponent<FakeComponent>();
                actor.Update(0);
                var actualAfterRemove_AfterUpdate = actor.GetComponent<FakeComponent>();

                test.ExpectNotNull(actual);
                test.Expect(createdComponent, actual);
                test.ExpectNotNull(actualAfterRemove_BeforeUpdate, "Removed component, before update");
                test.ExpectNull(actualAfterRemove_AfterUpdate, "Removed component, after update");
            }));
        }
    }
}
