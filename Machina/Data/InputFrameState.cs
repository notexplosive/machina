using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    [Serializable]
    public struct InputFrameState
    {
        public readonly KeyboardFrameState keyboardFrameState;
        public readonly MouseFrameState mouseFrameState;

        public InputFrameState(KeyboardFrameState keyboardFrameState, MouseFrameState mouseFrameState)
        {
            this.keyboardFrameState = keyboardFrameState;
            this.mouseFrameState = mouseFrameState;
        }

        public static InputFrameState Empty = new InputFrameState(KeyboardFrameState.Empty, MouseFrameState.Empty);
    }

    [Serializable]
    public struct MouseButtonList
    {
        [NonSerialized]
        public readonly bool left;
        [NonSerialized]
        public readonly bool middle;
        [NonSerialized]
        public readonly bool right;

        public MouseButtonList(bool left, bool middle, bool right)
        {
            this.left = left;
            this.middle = middle;
            this.right = right;
        }

        public MouseButtonList(int encoded)
        {
            this.left = (encoded & 1) == 1;
            this.middle = (encoded & (1 << 1)) == 1;
            this.right = (encoded & (1 << 2)) == 1;
        }

        public int EncodedInt
        {
            get => (left ? 1 : 0) | (middle ? 1 << 1 : 0) | (right ? 1 << 2 : 0);
        }

        public static MouseButtonList None => new MouseButtonList(false, false, false);
    }

    [Serializable]
    public struct MouseFrameState
    {
        public readonly MouseButtonList ButtonsReleasedThisFrame;
        public readonly MouseButtonList ButtonsPressedThisFrame;
        public readonly Point RawWindowPosition;
        public readonly Vector2 PositionDelta;
        public readonly int ScrollDelta;

        public MouseFrameState(MouseButtonList pressed, MouseButtonList released, Point windowPos, Vector2 positionDelta, int scrollDelta)
        {
            this.ButtonsPressedThisFrame = pressed;
            this.ButtonsReleasedThisFrame = released;
            this.RawWindowPosition = windowPos;
            this.PositionDelta = positionDelta;
            this.ScrollDelta = scrollDelta;
        }

        public static MouseFrameState Empty => new MouseFrameState(MouseButtonList.None, MouseButtonList.None, Point.Zero, Vector2.Zero, 0);
    }

    [Serializable]
    public struct SingleTouchFrameState
    {
        public readonly bool TouchDown;
        public readonly bool TouchUp;
        public readonly Point TouchPos;
        public readonly Vector2 TouchDelta;

        public SingleTouchFrameState(bool touchDown, bool touchUp, Point touchPos, Vector2 touchDelta)
        {
            this.TouchDown = touchDown;
            this.TouchUp = touchUp;
            this.TouchPos = touchPos;
            this.TouchDelta = touchDelta;
        }

        public static implicit operator MouseFrameState(SingleTouchFrameState touchFrameState)
        {
            return new MouseFrameState(
                new MouseButtonList(touchFrameState.TouchDown, false, false),
                new MouseButtonList(touchFrameState.TouchUp, false, false),
                touchFrameState.TouchPos, touchFrameState.TouchDelta, 0);

        }
    }

    [Serializable]
    public struct KeyboardFrameState
    {
        public readonly Keys[] Released;
        public readonly Keys[] Pressed;
        public readonly ModifierKeys Modifiers;

        public KeyboardFrameState(Keys[] pressed, Keys[] released, ModifierKeys modifiers)
        {
            this.Pressed = pressed;
            this.Released = released;
            this.Modifiers = modifiers;
        }

        public static KeyboardFrameState Empty = new KeyboardFrameState(Array.Empty<Keys>(), Array.Empty<Keys>(), ModifierKeys.NoModifiers);
    }
}
