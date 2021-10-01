using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Machina.Components
{
    /// <summary>
    ///     ClickAction means an object was clicked (mouse down)
    /// </summary>
    /// <param name="button"></param>
    public delegate void ClickAction(MouseButton button);

    public class Clickable : BaseComponent
    {
        private readonly Hoverable hoverable;
        private bool leftButtonDown;
        private bool middleButtonDown;
        private bool rightButtonDown;

        public Clickable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
        }

        public bool IsPrimedForAnyButton => this.hoverable.IsHovered &&
                                            (this.leftButtonDown || this.middleButtonDown || this.rightButtonDown);

        public bool IsPrimedForLeftMouseButton => IsPrimedForAnyButton && this.leftButtonDown;

        public bool IsHovered => this.hoverable.IsHovered;

        /// <summary>
        ///     Mouse was down, then up on the same object
        /// </summary>
        public event ClickAction OnClick;

        /// <summary>
        ///     Mouse was down on an object (not a complete Click)
        /// </summary>
        public event ClickAction ClickStarted;

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (this.hoverable.IsHovered)
            {
                if (buttonState == ButtonState.Pressed)
                {
                    ClickStarted?.Invoke(button);
                    if (button == MouseButton.Left)
                    {
                        this.leftButtonDown = true;
                    }

                    if (button == MouseButton.Right)
                    {
                        this.rightButtonDown = true;
                    }

                    if (button == MouseButton.Middle)
                    {
                        this.middleButtonDown = true;
                    }
                }
                else
                {
                    if (this.leftButtonDown && button == MouseButton.Left)
                    {
                        OnClick?.Invoke(MouseButton.Left);
                    }

                    if (this.rightButtonDown && button == MouseButton.Right)
                    {
                        OnClick?.Invoke(MouseButton.Right);
                    }

                    if (this.middleButtonDown && button == MouseButton.Middle)
                    {
                        OnClick?.Invoke(MouseButton.Middle);
                    }
                }
            }

            if (buttonState == ButtonState.Released)
            {
                this.leftButtonDown = false;
                this.rightButtonDown = false;
                this.middleButtonDown = false;
            }
        }
    }
}
