using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Machina.Components
{
    /// <summary>
    /// ClickAction means an object was clicked (mouse down)
    /// </summary>
    /// <param name="button"></param>
    public delegate void ClickAction(MouseButton button);

    public class Clickable : BaseComponent
    {
        /// <summary>
        /// Mouse was down, then up on the same object
        /// </summary>
        public event ClickAction OnClick;
        /// <summary>
        /// Mouse was down on an object (not a complete Click)
        /// </summary>
        public event ClickAction ClickStarted;
        private bool leftButtonDown;
        private bool rightButtonDown;
        private bool middleButtonDown;

        private readonly Hoverable hoverable;

        public Clickable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
        }

        public bool IsPrimedForAnyButton
        {
            get
            {
                return this.hoverable.IsHovered && (leftButtonDown || middleButtonDown || rightButtonDown);
            }
        }

        public bool IsPrimedForLeftMouseButton => IsPrimedForAnyButton && leftButtonDown;

        public bool IsHovered => this.hoverable.IsHovered;

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (this.hoverable.IsHovered)
            {
                if (buttonState == ButtonState.Pressed)
                {
                    ClickStarted?.Invoke(button);
                    if (button == MouseButton.Left)
                    {
                        leftButtonDown = true;
                    }

                    if (button == MouseButton.Right)
                    {
                        rightButtonDown = true;
                    }

                    if (button == MouseButton.Middle)
                    {
                        middleButtonDown = true;
                    }
                }
                else
                {
                    if (leftButtonDown && button == MouseButton.Left)
                    {
                        OnClick?.Invoke(MouseButton.Left);
                    }

                    if (rightButtonDown && button == MouseButton.Right)
                    {
                        OnClick?.Invoke(MouseButton.Right);
                    }

                    if (middleButtonDown && button == MouseButton.Middle)
                    {
                        OnClick?.Invoke(MouseButton.Middle);
                    }
                }
            }

            if (buttonState == ButtonState.Released)
            {
                leftButtonDown = false;
                rightButtonDown = false;
                middleButtonDown = false;
            }
        }
    }
}
