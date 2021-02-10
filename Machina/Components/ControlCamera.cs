using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class ControlCamera : BaseComponent
    {
        private bool isPanning;
        private bool isRotating;
        private bool controlIsPressed;

        public ControlCamera(Actor actor) : base(actor)
        {
        }

        public override void OnScroll(int scrollDelta)
        {
            if (this.controlIsPressed)
            {
                this.actor.scene.camera.AdjustZoom((float) scrollDelta / 4);
            }
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            if (key == Keys.LeftControl)
            {
                this.controlIsPressed = buttonState == ButtonState.Pressed;
            }
        }

        public override void OnMouseButton(MouseButton mouseButton, Point currentPosition, ButtonState buttonState)
        {
            if (mouseButton == MouseButton.Middle)
            {
                this.isPanning = buttonState == ButtonState.Pressed;
            }

            if (mouseButton == MouseButton.Right)
            {
                this.isRotating = buttonState == ButtonState.Pressed;
            }
        }

        public override void OnMouseMove(Point currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.isPanning)
            {
                this.actor.scene.camera.Position += positionDelta;
            }

            if (this.isRotating)
            {
                this.actor.scene.camera.Rotation += (float) rawDelta.X / 100;
            }
        }
    }
}
