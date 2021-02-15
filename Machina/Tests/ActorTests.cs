using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
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
                scene.FlushBuffers();

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
                actor.FlushBuffers();
                var actualAfterRemove_AfterUpdate = actor.GetComponent<FakeComponent>();

                test.ExpectNotNull(actual);
                test.Expect(createdComponent, actual);
                test.ExpectNotNull(actualAfterRemove_BeforeUpdate, "Removed component, before update");
                test.ExpectNull(actualAfterRemove_AfterUpdate, "Removed component, after update");
            }));

            AddTest(new Test("Progeny transforms without rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child1 = scene.AddActor("Carrie Child", parent.Position + new Vector2(50, 50));
                var child2 = scene.AddActor("Caleb Child", parent.Position + new Vector2(-30, 200));
                var grandChild = scene.AddActor("Garry Grandchild", child1.Position + new Vector2(-20, 40));

                child1.SetParent(parent);
                child2.SetParent(parent);
                grandChild.SetParent(child1);
                scene.FlushBuffers();

                test.Expect(2, parent.ChildCount, "ChildCount returns number of immediate children");
                test.Expect(parent.Position + new Vector2(50, 50), child1.Position, "Position is preserved after setting parent (child1)");
                test.Expect(parent.Position + new Vector2(-30, 200), child2.Position, "Position is preserved after setting parent (child2)");
                test.Expect(child1.Position + new Vector2(-20, 40), grandChild.Position, "Position is preserved after setting parent (grandChild)");
                test.Expect(new Vector2(50, 50), child1.LocalPosition, "Local position sanity check (child1)");
                test.Expect(new Vector2(-30, 200), child2.LocalPosition, "Local position sanity check (child2)");
                test.Expect(new Vector2(-20, 40), grandChild.LocalPosition, "Local position sanity check (grandchild)");
                test.Expect(parent.LocalPosition, parent.Position, "If an actor has no parent, its local position is equivalent to its position");
            }));

            AddTest(new Test("Progeny transforms with initial rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80), MathF.PI / 2);
                var child1 = scene.AddActor("Carrie Child", parent.Position + new Vector2(50, 50), MathF.PI / 2);
                var child2 = scene.AddActor("Caleb Child", parent.Position + new Vector2(-30, 200), 0.15f);
                var grandChild = scene.AddActor("Garry Grandchild", child1.Position + new Vector2(0, 50), -0.1f);

                child1.SetParent(parent);
                child2.SetParent(parent);
                grandChild.SetParent(child1);
                scene.FlushBuffers();

                test.Expect(new Vector2(50, -50), child1.LocalPosition, "Local position does not change with starting rotation (child1)");
                test.Expect(new Vector2(200, 30), child2.LocalPosition, "Local position does not change with starting rotation (child2)");
                test.Expect(new Vector2(50, 0), grandChild.LocalPosition, "Local position does not change with starting rotation (granchild)");
            }));

            AddTest(new Test("Progeny transforms with rotation after parent assignment", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child1 = scene.AddActor("Carrie Child", parent.Position + new Vector2(50, 50));
                var child2 = scene.AddActor("Caleb Child", parent.Position + new Vector2(-30, 200));
                var grandChild = scene.AddActor("Garry Grandchild", child1.Position + new Vector2(-20, 40));


                child1.SetParent(parent);
                child2.SetParent(parent);
                grandChild.SetParent(child1);
                test.Expect(0, parent.ChildCount, "ChildCount does not update until the next Update()");
                scene.FlushBuffers();
                // Assign angle AFTER parent assignment and an update
                parent.Angle = MathF.PI / 2;

                test.Expect(2, parent.ChildCount, "ChildCount returns number of immediate children");

                test.Expect(new Vector2(30, 130), child1.Position, "Position after rotation (child1)");
                test.Expect(new Vector2(-120, 50), child2.Position, "Position after rotation (child2)");
                test.Expect(new Vector2(-10, 110), grandChild.Position, "Position after rotation (grandChild)");

                test.Expect(new Vector2(50, 50), child1.LocalPosition, "Local position does not change after rotation (child1)");
                test.Expect(new Vector2(-30, 200), child2.LocalPosition, "Local position does not change after rotation (child2)");
                test.Expect(new Vector2(-20, 40), grandChild.LocalPosition, "Local position does not change after rotation (grandchild)");
                test.Expect(parent.LocalPosition, parent.Position, "If an actor has no parent, its local position is equivalent to its position");
            }));

            AddTest(new Test("Set Rotation then Set Parent", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.Position + new Vector2(100, 0), 0f);

                parent.Angle = MathF.PI / 2;
                child.SetParent(parent);
                scene.FlushBuffers();

                test.Expect(new Vector2(180, 80), child.Position, "Child position is unaffected");
                test.Expect(0, child.Angle, "Child does not inherit angle");

                test.Expect(new Vector2(0, -100), child.LocalPosition, "Child local position takes rotation into account");
                test.Expect(-MathF.PI / 2, child.LocalAngle, "Child local rotation takes rotation into account");
            }));

            AddTest(new Test("Set Parent then Set Rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.Position + new Vector2(100, 0), 0f);

                child.SetParent(parent);
                scene.FlushBuffers();
                parent.Angle = MathF.PI / 2;

                test.Expect(new Vector2(80, 180), child.Position, "Child position has been changed relative to rotation");
                test.Expect(MathF.PI / 2, child.Angle, "Child inherits angle");

                test.Expect(new Vector2(100, 0), child.LocalPosition, "Child local position is same relative position as the start");
                test.Expect(0f, child.LocalAngle, "Child local rotation is zero");
            }));
        }
    }
}
