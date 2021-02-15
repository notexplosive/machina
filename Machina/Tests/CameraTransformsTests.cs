using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class CameraTransformsTests : TestGroup
    {
        private class MouseHarness : BaseComponent
        {
            private Action<Vector2, Vector2, Vector2> lambda;

            public MouseHarness(Actor actor, Action<Vector2, Vector2, Vector2> lambda) : base(actor)
            {
                this.lambda = lambda;
            }

            public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
            {
                this.lambda(currentPosition, positionDelta, rawDelta);
            }
        }

        public CameraTransformsTests() : base("Camera Transforms")
        {
            AddTest("Game Canvas Maintain Desired Resolution Pinning", test =>
            {
                var desiredSize = new Point(300, 200);
                var gameCanvas = new GameCanvas(desiredSize.X, desiredSize.Y, ResizeBehavior.MaintainDesiredResolution);
                var windowSizeBeforeResize = gameCanvas.WindowSize;
                var canvasRectBeforeResize = gameCanvas.CanvasRect;
                var resizeSize = new Point(500, 500);
                gameCanvas.OnResize(resizeSize.X, resizeSize.Y);

                test.Expect(desiredSize, windowSizeBeforeResize, "Window size matches desired size before resize");
                test.Expect(resizeSize, gameCanvas.WindowSize, "Window size is new size after resize");
                test.Expect(1.66666f, gameCanvas.ScaleFactor, "Scale factor adjusts to new window size");
                test.Expect(windowSizeBeforeResize, canvasRectBeforeResize.Size, "Canvas size before resize matches window size");
                test.Expect(new Rectangle(0, 83, 500, 333), gameCanvas.CanvasRect);
            });

            AddTest("Mouse Position Transform Integration", test =>
            {
                var sceneLayers = new SceneLayers(null);
                var gameCanvas = new GameCanvas(800, 600, ResizeBehavior.MaintainDesiredResolution);
                var scene = new Scene(gameCanvas);
                scene.camera.Zoom = 2.6f;
                scene.camera.Position = new Vector2(120, 240);

                Vector2 savedPosition = Vector2.Zero;
                Vector2 savedPositionDelta = Vector2.Zero;
                Vector2 savedRawDelta = Vector2.Zero;
                new MouseHarness(scene.AddActor("Mouse Harness"), (currentPosition, positionDelta, rawDelta) =>
                {
                    savedPosition = currentPosition;
                    savedPositionDelta = positionDelta;
                    savedRawDelta = rawDelta;
                });

                // Resize to a huge resolution that's a different aspect ratio so we can get a worst-case transform scenario
                gameCanvas.OnResize(1920, 1080);
                sceneLayers.Add(scene);
                var prevMouseState = new MouseState(200, 200, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                var currentMouseState = new MouseState(220, 250, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                sceneLayers.Update(0, Matrix.Identity, new InputState(prevMouseState, new KeyboardState()));
                sceneLayers.Update(0, Matrix.Identity, new InputState(currentMouseState, new KeyboardState()));

                test.Expect(new Vector2(361, 478), savedPosition, "Mouse Postion");
                test.Expect(new Vector2(4.2735047f, 10.683762f), savedPositionDelta, "Mouse Position Delta");
                test.Expect(new Vector2(20, 50), savedRawDelta, "Mouse raw delta");
            });
        }
    }
}
