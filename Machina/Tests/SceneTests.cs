using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class SceneTests : TestGroup
    {
        public SceneTests() : base("Scene Tests")
        {
            AddTest("Scene can get all actors", test =>
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
                test.Expect(new Actor[] { o1, parent, child, grandChild, o2, o3 }, allActors.ToArray(), "All actors obtained in correct order");
            });

            AddTest("Scene can get all actors, ignoring ones about to be removed", test =>
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
                test.Expect(new Actor[] { o1, parent, child, grandChild, o2 }, allActors.ToArray(), "All actors obtained in correct order");
            });

            AddTest("Scene can get all actors, ignoring hierarchy's about to be removed", test =>
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
                test.Expect(new Actor[] { o1, parent, o2, o3 }, allActors.ToArray(), "All actors obtained in correct order");
            });
        }
    }
}
