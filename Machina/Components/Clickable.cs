using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Machina.Components
{
    class Clickable : BaseComponent
    {
        public Action<MouseButton> onClick;
        private bool leftButtonDown;
        private bool rightButtonDown;
        private bool middleButtonDown;

        private readonly Hoverable hoverable;

        public Clickable(Actor actor) : base(actor)
        {
            this.hoverable = RequireComponent<Hoverable>();
        }

        public bool IsHeldDown
        {
            get
            {
                return this.hoverable.IsHovered && (leftButtonDown || middleButtonDown || rightButtonDown);
            }
        }

        public bool IsHovered => this.hoverable.IsHovered;

        public override void OnMouseButton(MouseButton button, Point currentPosition, ButtonState buttonState)
        {
            if (this.hoverable.IsHovered)
            {
                if (buttonState == ButtonState.Pressed)
                {
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
                        onClick?.Invoke(MouseButton.Left);
                    }

                    if (rightButtonDown && button == MouseButton.Right)
                    {
                        onClick?.Invoke(MouseButton.Right);
                    }

                    if (middleButtonDown && button == MouseButton.Middle)
                    {
                        onClick?.Invoke(MouseButton.Middle);
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
