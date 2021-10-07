using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    public class PanAndZoomCamera : BaseComponent
    {
        private readonly Keys zoomModifier;
        private bool isPanning;
        private bool isRotating;
        private bool zoomModifierIsDown;

        public PanAndZoomCamera(Actor actor, Keys zoomModifier) : base(actor)
        {
            this.zoomModifier = zoomModifier;
        }

        public override void OnScroll(int scrollDelta)
        {
            if (this.zoomModifierIsDown)
            {
                this.actor.scene.camera.AdjustZoom((float)scrollDelta / 4);
            }
        }

        public override void OnKey(Keys key, ButtonState buttonState, ModifierKeys modifiers)
        {
            if (key == this.zoomModifier)
            {
                this.zoomModifierIsDown = buttonState == ButtonState.Pressed;
            }
        }

        public override void OnMouseButton(MouseButton mouseButton, Vector2 currentPosition, ButtonState buttonState)
        {
            if (mouseButton == MouseButton.Middle)
            {
                this.isPanning = buttonState == ButtonState.Pressed;
            }

            if (mouseButton == MouseButton.Right && this.zoomModifierIsDown)
            {
                // this.isRotating = buttonState == ButtonState.Pressed;
            }
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {
            if (this.isPanning)
            {
                this.actor.scene.camera.ScaledPosition -= positionDelta.ToPoint();
            }

            if (this.isRotating)
            {
                this.actor.scene.camera.Rotation += rawDelta.X / 100;
            }
        }
    }
}
