using System;
using FluentAssertions;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    public class ActorTests
    {
        [Fact]
        public void Destroying_actor_destroys_component()
        {
            var scene = new Scene(null);
            var actor = scene.AddActor("Terry Triangle");
            var deleteCount = 0;
            new FakeComponent(actor, () => { deleteCount++; });
            actor.Destroy();
            scene.FlushBuffers();

            Assert.Equal(1, deleteCount);
        }

        [Fact]
        public void Destroying_component_destroys_after_flush()
        {
            var actor = new Actor("Sally Sceneless", null);
            var deleteCount = 0;
            var createdComponent = new FakeComponent(actor, () => { deleteCount++; });
            var actual = actor.GetComponent<FakeComponent>();
            actor.RemoveComponent<FakeComponent>();
            var actual_AfterRemove_BeforeUpdate = actor.GetComponent<FakeComponent>();
            actor.FlushBuffers();
            var actual_AfterRemove_AfterUpdate = actor.GetComponent<FakeComponent>();

            Assert.NotNull(actual);
            Assert.Equal(createdComponent, actual);
            Assert.NotNull(actual_AfterRemove_BeforeUpdate);
            Assert.Null(actual_AfterRemove_AfterUpdate);
        }

        [Fact]
        public void Destroying_parents_sanity_scheck()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50));
            var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200));
            var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(-20, 40));

            child1.transform.SetParent(parent);
            child2.transform.SetParent(parent);
            grandChild.transform.SetParent(child1);
            scene.FlushBuffers();

            Assert.Equal(2, parent.transform.ChildCount);
            Assert.Equal(parent.transform.Position + new Vector2(50, 50),
                child1.transform.Position); // Position is preserved after setting parent (child1)
            Assert.Equal(parent.transform.Position + new Vector2(-30, 200),
                child2.transform.Position); // Position is preserved after setting parent (child2)
            Assert.Equal(child1.transform.Position + new Vector2(-20, 40),
                grandChild.transform.Position); // Position is preserved after setting parent (grandChild)
            Assert.Equal(new Vector2(50, 50), child1.transform.LocalPosition); // Local position sanity check (child1)
            Assert.Equal(new Vector2(-30, 200), child2.transform.LocalPosition); // Local position sanity check (child2)
            Assert.Equal(new Vector2(-20, 40),
                grandChild.transform.LocalPosition); // Local position sanity check (grandchild)
            Assert.Equal(parent.transform.LocalPosition,
                parent.transform
                    .Position); // If an actor has no parent, its local position is equivalent to its position
        }

        [Fact]
        public void Destroying_parent_with_starting_rotation()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80), MathF.PI / 2);
            var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50), MathF.PI / 2);
            var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200), 0.15f);
            var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(0, 50), -0.1f);

            child1.transform.SetParent(parent);
            child2.transform.SetParent(parent);
            grandChild.transform.SetParent(child1);
            scene.FlushBuffers();

            Assert.Equal(new Vector2(50, -50),
                child1.transform.LocalPosition); // Local position does not change with starting rotation (child1)
            Assert.True(
                new Vector2(200, 30).ApproximateEqual(child2.transform
                    .LocalPosition)); // Local position does not change with starting rotation (child2)
            Assert.True(
                new Vector2(50, 0).ApproximateEqual(grandChild.transform
                    .LocalPosition)); // Local position does not change with starting rotation (granchild)
        }

        [Fact]
        public void Destroying_parent_with_late_rotation()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child1 = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(50, 50));
            var child2 = scene.AddActor("Caleb Child", parent.transform.Position + new Vector2(-30, 200));
            var grandChild = scene.AddActor("Garry Grandchild", child1.transform.Position + new Vector2(-20, 40));

            child1.transform.SetParent(parent);
            child2.transform.SetParent(parent);
            grandChild.transform.SetParent(child1);
            Assert.Equal(0, parent.transform.ChildCount); // ChildCount does not update until the next Update()
            scene.FlushBuffers();
            // Assign angle AFTER parent assignment and an update
            parent.transform.Angle = MathF.PI / 2;

            Assert.Equal(2, parent.transform.ChildCount); // ChildCount returns number of immediate children

            Assert.True(
                new Vector2(30, 130).ApproximateEqual(child1.transform.Position)); // Position after rotation (child1)
            Assert.True(
                new Vector2(-120, 50).ApproximateEqual(child2.transform.Position)); // Position after rotation (child2)
            Assert.True(
                new Vector2(-10, 110).ApproximateEqual(grandChild.transform
                    .Position)); // Position after rotation (grandChild)

            Assert.Equal(new Vector2(50, 50),
                child1.transform.LocalPosition); // Local position does not change after rotation (child1)
            Assert.True(
                new Vector2(-30, 200).ApproximateEqual(child2.transform
                    .LocalPosition)); // Local position does not change after rotation (child2)
            Assert.True(
                new Vector2(-20, 40).ApproximateEqual(grandChild.transform
                    .LocalPosition)); // Local position does not change after rotation (grandchild)
            Assert.Equal(parent.transform.LocalPosition,
                parent.transform
                    .Position); // If an actor has no parent, its local position is equivalent to its position
        }

        [Fact]
        public void Rotate_parent_and_then_gain_child()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            parent.transform.Angle = MathF.PI / 2;
            child.transform.SetParent(parent);
            scene.FlushBuffers();

            Assert.Equal(new Vector2(180, 80), child.transform.Position); // Child position is unaffected
            Assert.Equal(0, child.transform.Angle); // Child does not inherit angle

            Assert.True(
                new Vector2(0, -100).ApproximateEqual(child.transform
                    .LocalPosition)); // Child local position takes rotation into account
            Assert.Equal(-MathF.PI / 2, child.transform.LocalAngle); // Child local rotation takes rotation into account
        }

        [Fact]
        public void moving_parent_with_children()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = parent.transform.AddActorAsChild("Carrie Child", new Vector2(100, 0));
            var grandChild = child.transform.AddActorAsChild("Garry Grandchild", new Vector2(100, 0));
            scene.FlushBuffers();

            parent.transform.Position = new Vector2(0, 0);

            child.transform.Position.Should().Be(new Vector2(100, 0));
            grandChild.transform.Position.Should().Be(new Vector2(200, 0));
        }

        [Fact]
        public void Rotate_parent_with_child()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            scene.FlushBuffers();
            parent.transform.Angle = MathF.PI / 2;

            Assert.True(
                new Vector2(80, 180).ApproximateEqual(child.transform
                    .Position)); // Child position has been changed relative to rotation
            Assert.Equal(MathF.PI / 2, child.transform.Angle); // Child inherits angle

            Assert.Equal(new Vector2(100, 0),
                child.transform.LocalPosition); // Child local position is same relative position as the start
            Assert.Equal(0f, child.transform.LocalAngle); // Child local rotation is zero
        }

        [Fact]
        public void Count_children()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var parent2 = scene.AddActor("Otto Other Parent", new Vector2(-80, -80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            child.transform.SetParent(parent2);
            child.transform.SetParent(parent2);
            child.transform.SetParent(parent);
            child.transform.SetParent(parent2);
            scene.FlushBuffers();

            Assert.Equal(0, parent.transform.ChildCount); // First parent has zero children during update
            Assert.Equal(1, parent2.transform.ChildCount); // Second parent has 1 child during update

            Assert.Equal(0, parent.transform.ChildCount); // First parent has zero children after update
            Assert.Equal(1, parent2.transform.ChildCount); // Second parent has 1 child after update
        }

        [Fact]
        public void Child_becomes_orphan()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            scene.FlushBuffers();

            child.transform.Position = new Vector2(300, 300);
            child.transform.SetParent(null);
            scene.FlushBuffers();

            Assert.Equal(new Vector2(300, 300), child.transform.Position); // Child is not moved by unsetting its parent
            Assert.Null(child.transform.Parent); // Child's parent is null
            Assert.Equal(2, scene.GetAllActors().Count); // Scene is aware of both actors
        }

        [Fact]
        public void Set_parent_to_self_is_null()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));

            parent.transform.SetParent(parent);

            Assert.Null(parent.transform.Parent);
        }

        [Fact]
        public void Assign_child_world_position()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            scene.FlushBuffers();
            child.transform.Position = new Vector2(-50, -50);

            Assert.Equal(new Vector2(-50, -50), child.transform.Position); // Child gets set to assigned world position
        }

        [Fact]
        public void Assign_child_global_position_and_parent_local_position()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            scene.FlushBuffers();
            child.transform.Position = new Vector2(-50, -50);
            parent.transform.LocalPosition = new Vector2(80, 80);

            Assert.Equal(new Vector2(-50, -50), child.transform.Position); // Child gets set to assigned world position
        }

        [Fact]
        public void Assign_child_local_position()
        {
            var scene = new Scene(null);
            var parent = scene.AddActor("Peter Parent", new Vector2(80, 80));
            var child = scene.AddActor("Carrie Child", parent.transform.Position + new Vector2(100, 0));

            child.transform.SetParent(parent);
            scene.FlushBuffers();
            child.transform.LocalPosition = new Vector2(-20, -20);

            Assert.Equal(new Vector2(-20, -20), child.transform.LocalPosition); // Local position was set
            Assert.Equal(new Vector2(60, 60), child.transform.Position); // Global position was set
        }

        [Fact]
        public void Assign_depth_and_local_depth()
        {
            var parent = new Actor("Peter Parent", null);
            var child = parent.transform.AddActorAsChild("Carrie Child");
            parent.transform.Depth = new Depth(1);
            child.transform.LocalDepth = new Depth(2);

            Assert.Equal(new Depth(1), parent.transform.Depth); // Depth of parent is set as expected
            Assert.Equal(new Depth(2), child.transform.LocalDepth); // LocalDepth of child is set as expected
            Assert.Equal(new Depth(3), child.transform.Depth); // Depth of child is set as expected
        }

        [Fact]
        public void Assign_angle_and_local_angle()
        {
            var parent = new Actor("Peter Parent", null);
            var child = parent.transform.AddActorAsChild("Carrie Child");
            parent.transform.Angle = 0.5f;
            child.transform.LocalAngle = 0.1f;

            Assert.Equal(0.5f, parent.transform.Angle); // Angle of parent is set as expected
            Assert.Equal(0.1f, child.transform.LocalAngle); // LocalAngle of child is set as expected
            Assert.Equal(0.6f, child.transform.Angle); // Angle of child is set as expected
        }

        [Fact]
        public void Assign_grand_child_depth()
        {
            var parent = new Actor("Peter Parent", null);
            var child = parent.transform.AddActorAsChild("Carrie Child");
            parent.transform.Depth = new Depth(1);
            child.transform.LocalDepth = new Depth(2);
            var grandchild = child.transform.AddActorAsChild("Gary Grandchild");
            grandchild.transform.LocalDepth = new Depth(3);

            Assert.Equal(new Depth(6), grandchild.transform.Depth); // Grandchild depth is set as expected
        }
    }
}