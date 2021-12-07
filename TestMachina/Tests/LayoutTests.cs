using Machina.Components;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public abstract class LayoutTests<TElement, TGroup>
        where TElement : IElement
        where TGroup : IGroup<Actor>
    {
        [Fact]
        public void basic_vertical_layout()
        {
            var groupActor = new Actor("Group", null);
            var group = CreateGroup(groupActor, Orientation.Vertical, new Point(200, 300), new Point(50, 50));
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
            Group<Actor>.ExecuteLayout(group);

            Assert.Equal(110, e4.Position.Y);
        }

        [Fact]
        public void basic_vertical_layout_with_padding()
        {
            var groupActor = new Actor("Group", null);
            var group = CreateGroup(groupActor, Orientation.Vertical, new Point(200, 300), new Point(50, 50));
            group.SetPaddingBetweenElements(5);
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            var e4 = CreateLayoutElement(group, "E4", new Point(20, 20));
            Group<Actor>.ExecuteLayout(group);

            Assert.Equal(125, e4.Position.Y);
        }

        [Fact]
        public void basic_vertical_layout_stretch()
        {
            var groupActor = new Actor("Group", null);
            var group = CreateGroup(groupActor, Orientation.Vertical, new Point(200, 300), new Point(50, 50));
            group.SetPaddingBetweenElements(7);
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            e1.StretchVertically();
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            e2.StretchVertically();
            Group<Actor>.ExecuteLayout(group);

            Assert.Equal(50, e1.Position.Y);
            Assert.Equal(146, e1.Size.Y);
            Assert.Equal(203, e2.Position.Y);
            Assert.Equal(146, e2.Size.Y);
        }

        [Fact]
        public void complex_vertical_layout_stretch()
        {
            var groupActor = new Actor("Group", null);
            var group = CreateGroup(groupActor, Orientation.Vertical, new Point(200, 300), new Point(50, 50));
            var e1 = CreateLayoutElement(group, "E1", new Point(20, 20));
            e1.StretchVertically();
            var e2 = CreateLayoutElement(group, "E2", new Point(20, 20));
            e2.StretchHorizontally();
            var e3 = CreateLayoutElement(group, "E3", new Point(20, 20));
            e3.StretchVertically();
            Group<Actor>.ExecuteLayout(group);

            Assert.Equal(50, e1.Position.Y);
            Assert.Equal(140, e1.Size.Y);
            Assert.Equal(20, e2.Size.Y);
            Assert.Equal(140, e3.Size.Y);
            Assert.Equal(200, e2.Size.X);
        }

        [Fact]
        public void complex_horizontal_layout_using_uibuilder()
        {
            var scene = new Scene(null);
            var uiBuilder = new UIBuilder(UIStyle.Empty);
            var horizontalLayout = scene.AddActor("Layout");
            var group = CreateGroup(horizontalLayout, Orientation.Horizontal, new Point(256, 128), Point.Zero);
            group.SetPaddingBetweenElements(5);
            group.SetMarginSize(new Point(15, 15));

            IElement e1 = CreateLayoutElement(group, "e1", new Point(32, 32));
            IElement e2 = CreateLayoutElement(group, "e2", new Point(64, 32)).StretchVertically();
            IElement e3 = CreateLayoutElement(group, "e3", new Point(32, 32)).StretchHorizontally().StretchVertically();
            scene.FlushBuffers();
            Group<Actor>.ExecuteLayout(group);

            Assert.Equal(15, e1.Position.X);
            Assert.Equal(52, e2.Position.X);
            Assert.Equal(121, e3.Position.X);

            Assert.Equal(32, e1.Size.X);
            Assert.Equal(64, e2.Size.X);
            Assert.Equal(120, e3.Size.X);

            Assert.Equal(32, e1.Size.Y);
            Assert.Equal(98, e2.Size.Y);
            Assert.Equal(98, e3.Size.Y);
        }

        protected abstract TElement CreateLayoutElement(TGroup group, string name, Point size);
        protected abstract TGroup CreateGroup(Actor groupActor, Orientation orientation, Point size, Point position);
    }

    public class LayoutTests_WithComponent : LayoutTests<LayoutElementComponent, LayoutGroupComponent>
    {
        protected override LayoutElementComponent CreateLayoutElement(LayoutGroupComponent group, string name, Point size)
        {
            var actor = new Actor(name, null);
            new BoundingRect(actor, size);
            actor.transform.SetParent(group.actor);
            var e = new LayoutElementComponent(actor);
            group.actor.FlushBuffers();
            return e;
        }

        protected override LayoutGroupComponent CreateGroup(Actor groupActor, Orientation orientation, Point size, Point position)
        {
            groupActor.transform.Position = position.ToVector2();
            new BoundingRect(groupActor, size);
            return new LayoutGroupComponent(groupActor, orientation);
        }
    }

    public class LayoutTests_WithAbstraction : LayoutTests<Element, Group<Actor>>
    {
        protected override Group<Actor> CreateGroup(Actor groupActor, Orientation orientation, Point size, Point position)
        {
            var group = new Group<Actor>(orientation);
            group.Size = size;
            group.Position = position;
            return group;
        }

        protected override Element CreateLayoutElement(Group<Actor> group, string name, Point size)
        {
            var element = new Element();
            element.Size = size;
            group.AddElement(element);
            return element;
        }
    }
}