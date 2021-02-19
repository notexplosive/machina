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

            AddTest(new Test("Transform transforms without rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50));
                var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200));
                var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(-20, 40));

                child1.transform.SetParent(parent);
                child2.transform.SetParent(parent);
                grandChild.transform.SetParent(child1);
                scene.FlushBuffers();

                test.Expect(2, parent.transform.ChildCount, "ChildCount returns number of immediate children");
                test.Expect(parent.transform.Position + new Vector2(50, 50), child1.transform.Position, "Position is preserved after setting parent (child1)");
                test.Expect(parent.transform.Position + new Vector2(-30, 200), child2.transform.Position, "Position is preserved after setting parent (child2)");
                test.Expect(child1.transform.Position + new Vector2(-20, 40), grandChild.transform.Position, "Position is preserved after setting parent (grandChild)");
                test.Expect(new Vector2(50, 50), child1.transform.LocalPosition, "Local position sanity check (child1)");
                test.Expect(new Vector2(-30, 200), child2.transform.LocalPosition, "Local position sanity check (child2)");
                test.Expect(new Vector2(-20, 40), grandChild.transform.LocalPosition, "Local position sanity check (grandchild)");
                test.Expect(parent.transform.LocalPosition, parent.transform.Position, "If an actor has no parent, its local position is equivalent to its position");
            }));

            AddTest(new Test("Transform transforms with initial rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80), MathF.PI / 2);
                var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50), MathF.PI / 2);
                var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200), 0.15f);
                var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(0, 50), -0.1f);

                child1.transform.SetParent(parent);
                child2.transform.SetParent(parent);
                grandChild.transform.SetParent(child1);
                scene.FlushBuffers();

                test.Expect(new Vector2(50, -50), child1.transform.LocalPosition, "Local position does not change with starting rotation (child1)");
                test.Expect(new Vector2(200, 30), child2.transform.LocalPosition, "Local position does not change with starting rotation (child2)");
                test.Expect(new Vector2(50, 0), grandChild.transform.LocalPosition, "Local position does not change with starting rotation (granchild)");
            }));

            AddTest(new Test("Transform transforms with rotation after parent assignment", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50));
                var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200));
                var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(-20, 40));


                child1.transform.SetParent(parent);
                child2.transform.SetParent(parent);
                grandChild.transform.SetParent(child1);
                test.Expect(0, parent.transform.ChildCount, "ChildCount does not update until the next Update()");
                scene.FlushBuffers();
                // Assign angle AFTER parent assignment and an update
                parent.transform.Angle = MathF.PI / 2;

                test.Expect(2, parent.transform.ChildCount, "ChildCount returns number of immediate children");

                test.Expect(new Vector2(30, 130), child1.transform.Position, "Position after rotation (child1)");
                test.Expect(new Vector2(-120, 50), child2.transform.Position, "Position after rotation (child2)");
                test.Expect(new Vector2(-10, 110), grandChild.transform.Position, "Position after rotation (grandChild)");

                test.Expect(new Vector2(50, 50), child1.transform.LocalPosition, "Local position does not change after rotation (child1)");
                test.Expect(new Vector2(-30, 200), child2.transform.LocalPosition, "Local position does not change after rotation (child2)");
                test.Expect(new Vector2(-20, 40), grandChild.transform.LocalPosition, "Local position does not change after rotation (grandchild)");
                test.Expect(parent.transform.LocalPosition, parent.transform.Position, "If an actor has no parent, its local position is equivalent to its position");
            }));

            AddTest(new Test("Set Rotation then Set Parent", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0), 0f);

                parent.transform.Angle = MathF.PI / 2;
                child.transform.SetParent(parent);
                scene.FlushBuffers();

                test.Expect(new Vector2(180, 80), child.transform.Position, "Child position is unaffected");
                test.Expect(0, child.transform.Angle, "Child does not inherit angle");

                test.Expect(new Vector2(0, -100), child.transform.LocalPosition, "Child local position takes rotation into account");
                test.Expect(-MathF.PI / 2, child.transform.LocalAngle, "Child local rotation takes rotation into account");
            }));

            AddTest(new Test("Set Parent then Set Rotation", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                scene.FlushBuffers();
                parent.transform.Angle = MathF.PI / 2;

                test.Expect(new Vector2(80, 180), child.transform.Position, "Child position has been changed relative to rotation");
                test.Expect(MathF.PI / 2, child.transform.Angle, "Child inherits angle");

                test.Expect(new Vector2(100, 0), child.transform.LocalPosition, "Child local position is same relative position as the start");
                test.Expect(0f, child.transform.LocalAngle, "Child local rotation is zero");
            }));

            AddTest(new Test("Set parent multiple times in one frame", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var parent2 = scene.AddActor("Otto Other Parent", new Vector2(-80, -80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                child.transform.SetParent(parent2);
                child.transform.SetParent(parent2);
                child.transform.SetParent(parent);
                child.transform.SetParent(parent2);
                scene.FlushBuffers();

                test.Expect(0, parent.transform.ChildCount, "First parent has zero children during update");
                test.Expect(1, parent2.transform.ChildCount, "Second parent has 1 child during update");

                test.Expect(0, parent.transform.ChildCount, "First parent has zero children after update");
                test.Expect(1, parent2.transform.ChildCount, "Second parent has 1 child after update");
            }));

            AddTest(new Test("Setting parent to null will set it to have no parent", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                scene.FlushBuffers();

                child.transform.Position = new Vector2(300, 300);
                child.transform.SetParent(null);
                scene.FlushBuffers();

                test.Expect(new Vector2(300, 300), child.transform.Position, "Child is not moved by unsetting its parent");
                test.ExpectNull(child.transform.Parent, "Child's parent is null");
                test.Expect(2, scene.GetAllActors().Count, "Scene is aware of both actors");
            }));

            AddTest("Setting parent to yourself should be a no-op", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));

                parent.transform.SetParent(parent);

                test.ExpectNull(parent.transform.Parent, "Setting non-parented object's parent to null should still have null parent");
            });

            AddTest("Set position on parented object", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                scene.FlushBuffers();
                child.transform.Position = new Vector2(-50, -50);

                test.Expect(new Vector2(-50, -50), child.transform.Position, "Child gets set to assigned world position");
            });

            AddTest("Set position on parented object does not get overridden by setting local position", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                scene.FlushBuffers();
                child.transform.Position = new Vector2(-50, -50);
                parent.transform.LocalPosition = new Vector2(80, 80);

                test.Expect(new Vector2(-50, -50), child.transform.Position, "Child gets set to assigned world position");
            });

            AddTest("Setting local position moves the actor", test =>
            {
                var scene = new Scene();
                var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
                var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

                child.transform.SetParent(parent);
                scene.FlushBuffers();
                child.transform.LocalPosition = new Vector2(-20, -20);

                test.Expect(new Vector2(-20, -20), child.transform.LocalPosition, "Local position was set");
                test.Expect(new Vector2(60, 60), child.transform.Position, "Global position was set");
            });

            AddTest("Set LocalDepth basic", test =>
            {
                var parent = new Actor("Peter Parent", null);
                var child = parent.transform.AddActorAsChild("Carrie Child");
                parent.transform.Depth = 0.5f;
                child.transform.LocalDepth = 0.1f;

                test.Expect(0.5f, parent.transform.Depth, "Depth of parent is set as expected");
                test.Expect(0.1f, child.transform.LocalDepth, "LocalDepth of child is set as expected");
                test.Expect(0.6f, child.transform.Depth, "Depth of child is set as expected");
            });

            AddTest("Set LocalAngle basic", test =>
            {
                var parent = new Actor("Peter Parent", null);
                var child = parent.transform.AddActorAsChild("Carrie Child");
                parent.transform.Angle = 0.5f;
                child.transform.LocalAngle = 0.1f;

                test.Expect(0.5f, parent.transform.Angle, "Angle of parent is set as expected");
                test.Expect(0.1f, child.transform.LocalAngle, "LocalAngle of child is set as expected");
                test.Expect(0.6f, child.transform.Angle, "Angle of child is set as expected");
            });

            AddTest("Local depth in a long hierarchy", test =>
            {
                var parent = new Actor("Peter Parent", null);
                var child = parent.transform.AddActorAsChild("Carrie Child");
                parent.transform.Depth = 0.5f;
                child.transform.LocalDepth = 0.1f;
                var grandchild = parent.transform.AddActorAsChild("Gary Grandchild");
                grandchild.transform.LocalDepth = 0.1f;

                test.Expect(0.6f, grandchild.transform.Depth, "Grandchild depth is set as expected");
            });
        }
    }
}
