using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestMachina.Tests
{
    public class SceneTests
    {
        [Fact]
        public void scene_can_get_all_actors()
        {
            var scene = new Scene(null);
            var o1 = scene.AddActor("other1");
            var child = scene.AddActor("Child");
            var parent = scene.AddActor("Parent");
            var o2 = scene.AddActor("other2");
            var grandChild = scene.AddActor("Grandchild");
            var o3 = scene.AddActor("other3");

            child.transform.SetParent(parent);
            grandChild.transform.SetParent(child);
            scene.FlushBuffers();

            var allActors = scene.GetAllActors();
            Assert.Equal(new Actor[] { o1, parent, child, grandChild, o2, o3 }, allActors.ToArray()); // All actors obtained in correct order
        }

        [Fact]
        public void scene_can_get_all_actors_ignoring_ones_about_to_be_removed()
        {
            var scene = new Scene(null);
            var o1 = scene.AddActor("other1");
            var child = scene.AddActor("Child");
            var parent = scene.AddActor("Parent");
            var o2 = scene.AddActor("other2");
            var grandChild = scene.AddActor("Grandchild");
            var o3 = scene.AddActor("other3");

            child.transform.SetParent(parent);
            grandChild.transform.SetParent(child);
            scene.FlushBuffers();
            o3.Destroy();

            var allActors = scene.GetAllActors();
            Assert.Equal(new Actor[] { o1, parent, child, grandChild, o2 }, allActors.ToArray()); // All actors obtained in correct order
        }

        [Fact]
        public void scene_can_get_all_actors_ignoring_hierarchys_about_to_be_removed()
        {
            var scene = new Scene(null);
            var o1 = scene.AddActor("other1");
            var child = scene.AddActor("Child");
            var parent = scene.AddActor("Parent");
            var o2 = scene.AddActor("other2");
            var grandChild = scene.AddActor("Grandchild");
            var o3 = scene.AddActor("other3");

            child.transform.SetParent(parent);
            grandChild.transform.SetParent(child);
            scene.FlushBuffers();
            child.Destroy();

            var allActors = scene.GetAllActors();
            Assert.Equal(new Actor[] { o1, parent, o2, o3 }, allActors.ToArray()); // All actors obtained in correct order
        }
    }
}
