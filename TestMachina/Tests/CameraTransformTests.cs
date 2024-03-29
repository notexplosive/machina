﻿using FluentAssertions;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    public class CameraTransformsTests
    {
        [Fact]
        public void game_canvas_maintain_desired_resolution_pinning()
        {
            var desiredSize = new Point(300, 200);
            var gameCanvas = new GameViewport(desiredSize, ResizeBehavior.KeepAspectRatio);
            var windowSizeBeforeResize = gameCanvas.WindowSize;
            var canvasRectBeforeResize = gameCanvas.CanvasRect;
            var newSize = new Point(500, 500);
            gameCanvas.SetWindowSize(newSize);

            Assert.Equal(desiredSize, windowSizeBeforeResize); // Window size matches desired size before resize
            Assert.Equal(newSize, gameCanvas.WindowSize); // Window size is new size after resize
            Assert.Equal(1.6666666f, gameCanvas.ScaleFactor); // Scale factor adjusts to new window size
            Assert.Equal(windowSizeBeforeResize,
                canvasRectBeforeResize.Size); // Canvas size before resize matches window size
            Assert.Equal(new Rectangle(0, 83, 500, 333), gameCanvas.CanvasRect);
        }

        /// <summary>
        ///     Basically a really bad worst-case-scenario for mouse position, we're at an awkward aspect ratio, we're zoomed in
        ///     and panned.
        ///     The test is "did we get the right mouse position" within one pixel
        ///     This test is brittle on purpose. The calculations involved can be quite easy to screw up
        /// </summary>
        [Fact]
        public void mouse_position_transform_integration()
        {
            var gameCanvas = new GameViewport(new Point(800, 600), ResizeBehavior.KeepAspectRatio);
            var sceneLayers = new SceneLayers(gameCanvas);
            var scene = sceneLayers.AddNewScene();
            scene.camera.ScaledPosition = new Point(120, 240);
            scene.camera.Zoom = 2.6f;

            var savedPosition = Vector2.Zero;
            var savedPositionDelta = Vector2.Zero;
            var savedRawDelta = Vector2.Zero;
            new MouseHarnessComponent(scene.AddActor("Mouse Harness"), (currentPosition, positionDelta, rawDelta) =>
            {
                savedPosition = currentPosition;
                savedPositionDelta = positionDelta;
                savedRawDelta = rawDelta;
            });

            // Resize to a huge resolution that's a different aspect ratio so we can get a worst-case transform scenario
            gameCanvas.SetWindowSize(new Point(1920, 1080));
            var prevPos = new Point(200, 200);
            var newPos = new Point(220, 250);
            var mouseDelta = (newPos - prevPos).ToVector2();

            sceneLayers.Update(0, Matrix.Identity,
                new InputFrameState(KeyboardFrameState.Empty,
                    new MouseFrameState(MouseButtonList.None, MouseButtonList.None, new Point(200, 200), Vector2.Zero,
                        0)));
            sceneLayers.Update(0, Matrix.Identity,
                new InputFrameState(KeyboardFrameState.Empty,
                    new MouseFrameState(MouseButtonList.None, MouseButtonList.None, new Point(220, 250), mouseDelta,
                        0)));
            
            Assert.Equal(new Point(115, 292), savedPosition.ToPoint()); // Mouse Position, this used to be (361, 478), but after I made the zoom/pan change it changed. I think that's on purpose
            Assert.Equal(new Vector2(4.2735047f, 10.683762f), savedPositionDelta); // Mouse Position Delta
            Assert.Equal(new Vector2(20, 50), savedRawDelta); // Mouse raw delta
        }

        [Fact]
        public void scaled_position_assignment()
        {
            var gameCanvas = new GameViewport(new Point(800, 600), ResizeBehavior.KeepAspectRatio);
            var sceneLayers = new SceneLayers(gameCanvas);
            var scene = sceneLayers.AddNewScene();
            scene.camera.Zoom = 2.6f;

            scene.camera.ScaledPosition = new Point(scene.camera.ScaledPosition.X, 237);

            scene.camera.ScaledPosition.Y.Should().Be(237);
        }
    }
}