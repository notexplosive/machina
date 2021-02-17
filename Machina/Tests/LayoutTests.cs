using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class LayoutTests : TestGroup
    {
        public LayoutTests() : base("Layout Tests")
        {
            AddTest("Basic Vertical Layout", test =>
            {
                var groupActor = new Actor("Group", null);
                groupActor.transform.Position = new Vector2(50, 50);
                new BoundingRect(groupActor, new Point(200, 300));
                var group = new LayoutGroup(groupActor);
                var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
                var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
                var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
                var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
                group.ExecuteLayout();

                test.Expect(110, e4.actor.transform.Position.Y, "E4 is at expected Y pos");
            });

            AddTest("Basic Vertical Layout with Padding", test =>
            {
                var groupActor = new Actor("Group", null);
                groupActor.transform.Position = new Vector2(50, 50);
                new BoundingRect(groupActor, new Point(200, 300));
                var group = new LayoutGroup(groupActor);
                group.PaddingBetweenElements = 5;
                var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
                var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
                var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
                var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
                group.ExecuteLayout();

                test.Expect(125, e4.actor.transform.Position.Y, "E4 is at expected Y pos");
            });

            AddTest("Basic Vertical Layout Stretch", test =>
            {
                var groupActor = new Actor("Group", null);
                groupActor.transform.Position = new Vector2(50, 50);
                new BoundingRect(groupActor, new Point(200, 300));
                var group = new LayoutGroup(groupActor);
                group.PaddingBetweenElements = 7;
                var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
                e1.StretchVertically = true;
                var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
                e2.StretchVertically = true;
                group.ExecuteLayout();

                test.Expect(50, e1.actor.transform.Position.Y, "E1 is at expected Y pos");
                test.Expect(143, e1.boundingRect.Height, "E1 is expected height");
                test.Expect(200, e2.actor.transform.Position.Y, "E2 is at expected Y pos");
                test.Expect(143, e2.boundingRect.Height, "E2 is expected height");
            });

            AddTest("Complex Vertical Layout Stretch", test =>
            {
                var groupActor = new Actor("Group", null);
                groupActor.transform.Position = new Vector2(50, 50);
                new BoundingRect(groupActor, new Point(200, 300));
                var group = new LayoutGroup(groupActor);
                var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
                e1.StretchVertically = true;
                var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
                e2.StretchHorizontally = true;
                var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
                e3.StretchVertically = true;
                group.ExecuteLayout();

                test.Expect(50, e1.actor.transform.Position.Y, "E1 is at expected Y pos");
                test.Expect(140, e1.boundingRect.Height, "E1 is expected height");
                test.Expect(20, e2.boundingRect.Height, "E2 is expected height");
                test.Expect(140, e3.boundingRect.Height, "E3 is expected height");
                test.Expect(200, e2.boundingRect.Width, "E2 stretched horizontally");

            });
        }

        private LayoutElement CreateLayoutElement(LayoutGroup group, string name, Point size)
        {
            var actor = new Actor(name, null);
            new BoundingRect(actor, size);
            var e = new LayoutElement(actor);
            actor.SetParent(group.actor);
            group.actor.FlushBuffers();
            return e;
        }
    }
}
