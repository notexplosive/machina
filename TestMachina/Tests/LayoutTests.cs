using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestMachina.Tests
{
    public class LayoutTests
    {
        [Fact]
        public void basic_vertical_layout()
        {
            var groupActor = new Actor("Group", null);
            groupActor.transform.Position = new Vector2(50, 50);
            new BoundingRect(groupActor, new Point(200, 300));
            var group = new LayoutGroup(groupActor, Orientation.Vertical);
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
            group.ExecuteLayout();

            Assert.Equal(110, e4.actor.transform.Position.Y);
        }

        [Fact]
        public void basic_vertical_layout_with_padding()
        {
            var groupActor = new Actor("Group", null);
            groupActor.transform.Position = new Vector2(50, 50);
            new BoundingRect(groupActor, new Point(200, 300));
            var group = new LayoutGroup(groupActor, Orientation.Vertical);
            group.PaddingBetweenElements = 5;
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
            group.ExecuteLayout();

            Assert.Equal(125, e4.actor.transform.Position.Y);
        }

        [Fact]
        public void basic_vertical_layout_stretch()
        {
            var groupActor = new Actor("Group", null);
            groupActor.transform.Position = new Vector2(50, 50);
            new BoundingRect(groupActor, new Point(200, 300));
            var group = new LayoutGroup(groupActor, Orientation.Vertical);
            group.PaddingBetweenElements = 7;
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            e1.StretchVertically();
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            e2.StretchVertically();
            group.ExecuteLayout();

            Assert.Equal(50, e1.actor.transform.Position.Y);
            Assert.Equal(146, e1.boundingRect.Height);
            Assert.Equal(203, e2.actor.transform.Position.Y);
            Assert.Equal(146, e2.boundingRect.Height);
        }

        [Fact]
        public void complex_vertical_layout_stretch()
        {
            var groupActor = new Actor("Group", null);
            groupActor.transform.Position = new Vector2(50, 50);
            new BoundingRect(groupActor, new Point(200, 300));
            var group = new LayoutGroup(groupActor, Orientation.Vertical);
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            e1.StretchVertically();
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            e2.StretchHorizontally();
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            e3.StretchVertically();
            group.ExecuteLayout();

            Assert.Equal(50, e1.actor.transform.Position.Y);
            Assert.Equal(140, e1.boundingRect.Height);
            Assert.Equal(20, e2.boundingRect.Height);
            Assert.Equal(140, e3.boundingRect.Height);
            Assert.Equal(200, e2.boundingRect.Width);

        }

        [Fact]
        public void complex_horizontal_layout_using_uibuilder()
        {
            var scene = new Scene(null);
            var uiBuilder = new UIBuilder(UIStyle.Empty);
            var horizontalLayout = scene.AddActor("Layout");
            new BoundingRect(horizontalLayout, 256, 128);
            var uiGroup = new LayoutGroup(horizontalLayout, Orientation.Horizontal);
            uiGroup.PaddingBetweenElements = 5;
            uiGroup.SetMargin(15);

            var e1 = uiBuilder.BuildSpacer(uiGroup, new Point(32, 32), false, false);
            var e2 = uiBuilder.BuildSpacer(uiGroup, new Point(64, 32), false, true);
            var e3 = uiBuilder.BuildSpacer(uiGroup, new Point(32, 32), true, true);
            scene.FlushBuffers();
            uiGroup.ExecuteLayout();

            Assert.Equal(15, e1.transform.Position.X);
            Assert.Equal(52, e2.transform.Position.X);
            Assert.Equal(121, e3.transform.Position.X);

            Assert.Equal(32, e1.GetComponent<BoundingRect>().Width);
            Assert.Equal(64, e2.GetComponent<BoundingRect>().Width);
            Assert.Equal(120, e3.GetComponent<BoundingRect>().Width);

            Assert.Equal(32, e1.GetComponent<BoundingRect>().Height);
            Assert.Equal(98, e2.GetComponent<BoundingRect>().Height);
            Assert.Equal(98, e3.GetComponent<BoundingRect>().Height);
        }

        private LayoutElement CreateLayoutElement(LayoutGroup group, string name, Point size)
        {
            var actor = new Actor(name, null);
            new BoundingRect(actor, size);
            actor.transform.SetParent(group.actor);
            var e = new LayoutElement(actor);
            group.actor.FlushBuffers();
            return e;
        }
    }
}
