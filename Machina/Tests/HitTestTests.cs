using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class HitTestTests : TestGroup
    {
        public HitTestTests() : base("Hit Test Tests")
        {
            AddTest(new Test("Test Hoverables in a single scene", test =>
            {
                var sceneLayers = new SceneLayers(null, new EmptyFrameStep());
                var scene = new Scene(new GameCanvas(800, 600, ResizeBehavior.FillContent));
                sceneLayers.Add(scene);
                var mousePoint = new Point(200, 200);
                var mouseState = new MouseState(mousePoint.X, mousePoint.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                var unreachedHoverable = BuildHoverable(scene, Point.Zero, "Hoverable that is too far to be hovered", new Depth(5));
                var onPointHoverable = BuildHoverable(scene, mousePoint, "Hoverable that is exactly were the mouse is", new Depth(5));
                var behindHoverable = BuildHoverable(scene, mousePoint, "Hoverable that is exactly were the mouse is, but farther back", new Depth(6));

                // Need to update to push created interables into main iterable list
                scene.Update(0f);

                sceneLayers.Update(0, Matrix.Identity, new InputState(mouseState, new KeyboardState()));
                test.Expect(new Depth(5), scene.hitTester.Candidate.depth, "Lowest depth element was clicked");
                test.ExpectFalse(unreachedHoverable.IsHovered, "Unreached hoverable should not be hovered");
                test.ExpectFalse(unreachedHoverable.IsSoftHovered, "Unreached hoverable should not be soft hovered");
                test.ExpectFalse(behindHoverable.IsHovered, "Behind hoverable should not be hovered");
                test.ExpectTrue(behindHoverable.IsSoftHovered, "Behind hoverable is soft hovered");
                test.ExpectTrue(onPointHoverable.IsHovered, "Hovered thing is hovered");
                test.ExpectTrue(onPointHoverable.IsSoftHovered, "Hovered thing is soft hovered");
            }));

            AddTest(new Test("Test Hoverables across multiple scenes", test =>
            {
                var sceneLayers = new SceneLayers(null, new EmptyFrameStep());
                var lowerScene = new Scene(new GameCanvas(800, 600, ResizeBehavior.FillContent));
                var upperScene = new Scene(new GameCanvas(800, 600, ResizeBehavior.FillContent));
                sceneLayers.Add(lowerScene);
                sceneLayers.Add(upperScene);
                var mousePoint = new Point(200, 200);
                var mouseState = new MouseState(mousePoint.X, mousePoint.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                var onPointHoverable = BuildHoverable(upperScene, mousePoint, "Hoverable in upper scene", new Depth(5));
                var behindHoverable = BuildHoverable(lowerScene, mousePoint, "Hoverable in lower scene but closer depth", new Depth(1));

                // Push created iterables
                sceneLayers.UpdateWithNoInput(0f);

                sceneLayers.Update(0, new InputState(mouseState, new KeyboardState()));
                test.ExpectTrue(onPointHoverable.IsHovered, "Upper scene element is hovered");
                test.ExpectFalse(behindHoverable.IsHovered, "Lower scene element is hovered, despite having higher depth");
                test.ExpectTrue(behindHoverable.IsSoftHovered, "Lower scene element is soft hovered");
            }));
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

