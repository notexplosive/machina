using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    class UIBuilder
    {
        private UIStyle style;

        public UIBuilder(UIStyle style)
        {
            this.style = style;
        }

        public Actor BuildButton(LayoutGroup group, string buttonLabelText, int height)
        {
            var scene = group.actor.scene;
            var buttonActor = scene.AddActor("Button");
            new BoundingRect(buttonActor, 0, height);
            new NinepatchRenderer(buttonActor, style.buttonDefault);
            new Hoverable(buttonActor);
            new Clickable(buttonActor);
            new ButtonNinepatchHandler(buttonActor, style.buttonHover, style.buttonPress);
            new LayoutElement(buttonActor).StretchHorizontally = true;
            new LayoutGroup(buttonActor).SetMargin(5);
            var buttonLabelActor = scene.AddActor("Button Label");
            buttonLabelActor.transform.LocalDepth = 0.000001f;
            new BoundingRect(buttonLabelActor, Point.Zero);
            var buttonLabelElement = new LayoutElement(buttonLabelActor);
            buttonLabelElement.StretchHorizontally = true;
            buttonLabelElement.StretchVertically = true;
            new BoundedTextRenderer(buttonLabelActor, buttonLabelText, style.buttonFont);

            buttonActor.SetParent(group.actor);
            buttonLabelActor.SetParent(buttonActor);

            return buttonActor;
        }

        public Actor BuildSpacer(LayoutGroup group, Point size, bool stretchHorizontal, bool stretchVertical)
        {
            var scene = group.actor.scene;
            var spacerActor = scene.AddActor("Button");
            new BoundingRect(spacerActor, size);
            var e = new LayoutElement(spacerActor);
            e.StretchHorizontally = stretchHorizontal;
            e.StretchVertically = stretchVertical;

            spacerActor.SetParent(group.actor);

            return spacerActor;
        }

    }

    class UIStyle
    {
        public readonly NinepatchSheet buttonDefault;
        public readonly NinepatchSheet buttonHover;
        public readonly NinepatchSheet buttonPress;
        public readonly SpriteFont buttonFont;

        public UIStyle(NinepatchSheet defaultButtonSheet, NinepatchSheet hoverButtonSheet, NinepatchSheet pressButtonSheet, SpriteFont buttonFont)
        {
            buttonDefault = defaultButtonSheet;
            buttonHover = hoverButtonSheet;
            buttonPress = pressButtonSheet;
            this.buttonFont = buttonFont;
        }

        /// <summary>
        /// Should only be used in tests
        /// </summary>
        public static readonly UIStyle Empty = new UIStyle(null, null, null, null);
    }
}
