using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace TestMachina.Tests
{
    public class HitTestTests
    {
        [Fact]
        public void test_hoverables_in_a_single_scene()
        {
            var sceneLayers = new SceneLayers(new GameViewport(new Point(800, 600), ResizeBehavior.FillContent));
            var scene = sceneLayers.AddNewScene();
            var mousePoint = new Point(200, 200);
            var mouseState = new MouseState(mousePoint.X, mousePoint.Y, 0, ButtonState.Released, ButtonState.Released,
                ButtonState.Released, ButtonState.Released, ButtonState.Released);
            var unreachedHoverable =
                BuildHoverable(scene, Point.Zero, "Hoverable that is too far to be hovered", new Depth(5));
            var onPointHoverable = BuildHoverable(scene, mousePoint, "Hoverable that is exactly were the mouse is",
                new Depth(5));
            var behindHoverable = BuildHoverable(scene, mousePoint,
                "Hoverable that is exactly were the mouse is, but farther back", new Depth(6));

            // Need to update to push created interables into main iterable list
            scene.Update(0f);

            sceneLayers.Update(0, Matrix.Identity,
                new InputFrameState(KeyboardFrameState.Empty,
                    new MouseFrameState(MouseButtonList.None, MouseButtonList.None, mousePoint, Vector2.Zero, 0)));
            Assert.Equal(new Depth(5), scene.hitTester.Candidate.depth);
            Assert.False(unreachedHoverable.IsHovered);
            Assert.False(unreachedHoverable.IsSoftHovered);
            Assert.False(behindHoverable.IsHovered);
            Assert.True(behindHoverable.IsSoftHovered);
            Assert.True(onPointHoverable.IsHovered);
            Assert.True(onPointHoverable.IsSoftHovered);
        }

        [Fact]
        public void test_hoverables_across_multiple_scenes()
        {
            var sceneLayers = new SceneLayers(new GameViewport(new Point(800, 600), ResizeBehavior.FillContent));
            var lowerScene = sceneLayers.AddNewScene();
            var upperScene = sceneLayers.AddNewScene();
            var mousePoint = new Point(200, 200);
            var mouseState = new MouseState(mousePoint.X, mousePoint.Y, 0, ButtonState.Released, ButtonState.Released,
                ButtonState.Released, ButtonState.Released, ButtonState.Released);
            var onPointHoverable = BuildHoverable(upperScene, mousePoint, "Hoverable in upper scene", new Depth(5));
            var behindHoverable = BuildHoverable(lowerScene, mousePoint, "Hoverable in lower scene but closer depth",
                new Depth(1));

            // Push created iterables
            sceneLayers.UpdateWithNoInput(0f);

            sceneLayers.Update(0,
                new InputFrameState(KeyboardFrameState.Empty,
                    new MouseFrameState(MouseButtonList.None, MouseButtonList.None, mousePoint, Vector2.Zero, 0)));
            Assert.True(onPointHoverable.IsHovered);
            Assert.False(behindHoverable.IsHovered);
            Assert.True(behindHoverable.IsSoftHovered);
        }

        private Hoverable BuildHoverable(Scene scene, Point startingPosition, string name, Depth depth)
        {
            var actor = scene.AddActor(name, startingPosition.ToVector2());
            actor.transform.Depth = depth;
            new BoundingRect(actor, new Point(20, 20)).SetOffsetToCenter();
            return new Hoverable(actor);
        }
    }
}