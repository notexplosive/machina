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

        public Actor BuildButton(LayoutGroup uiGroup, string buttonLabelText)
        {
            var scene = uiGroup.actor.scene;
            var buttonActor = scene.AddActor("Button");
            new BoundingRect(buttonActor, 32, 32);
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

            buttonActor.SetParent(uiGroup.actor);
            buttonLabelActor.SetParent(buttonActor);

            return buttonActor;
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
    }
}
