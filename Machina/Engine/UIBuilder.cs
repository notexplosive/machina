﻿using Machina.Components;
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

        public Actor BuildButton(LayoutGroup group, string buttonLabelText, int height = 32)
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
            new BoundedTextRenderer(buttonLabelActor, buttonLabelText, style.uiElementFont);

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

        public void BuildCheckbox(LayoutGroup uiGroup, string labelText)
        {
            BuildCheckboxOrRadioButton(uiGroup, labelText, true);
        }

        public void BuildRadioButton(LayoutGroup uiGroup, string labelText)
        {
            BuildCheckboxOrRadioButton(uiGroup, labelText, false);
        }

        private void BuildCheckboxOrRadioButton(LayoutGroup uiGroup, string labelText, bool isCheckbox)
        {
            var scene = uiGroup.actor.scene;
            var checkboxContainer = scene.AddActor("Checkbox", new Vector2(40, 400));
            new BoundingRect(checkboxContainer, new Point(256, 32));
            new Hoverable(checkboxContainer);
            var checkboxClickable = new Clickable(checkboxContainer);
            var checkboxState = new CheckboxState(checkboxContainer);
            new LayoutElement(checkboxContainer).StretchHorizontally = true;
            new LayoutGroup(checkboxContainer, Orientation.Horizontal).PaddingBetweenElements = 5;

            var checkboxBox = scene.AddActor("Checkbox-Box");
            checkboxBox.SetParent(checkboxContainer);
            new BoundingRect(checkboxBox, new Point(32, 32));
            new LayoutElement(checkboxBox);
            if (isCheckbox)
            {
                new CheckboxRenderer(checkboxBox, style.checkboxAndRadioBackground, style.checkboxImage, checkboxState, checkboxClickable, style.checkboxFrames);
            }
            else
            {
                new CheckboxRenderer(checkboxBox, style.checkboxAndRadioBackground, style.radioImage, checkboxState, checkboxClickable, style.radioFrames);
            }

            var checkboxLabel = scene.AddActor("Checkbox-Label");
            checkboxLabel.SetParent(checkboxContainer);
            new BoundingRect(checkboxLabel, new Point(0, 32));
            new LayoutElement(checkboxLabel).StretchHorizontally = true;
            new BoundedTextRenderer(checkboxLabel, labelText, style.uiElementFont);

            checkboxContainer.SetParent(uiGroup.actor);
        }
    }

    class UIStyle
    {
        /// <summary>
        /// Should only be used in tests
        /// </summary>
        public static readonly UIStyle Empty = new UIStyle(null, null, null, null, null, null, null, new LinearFrameAnimation(0, 3), new LinearFrameAnimation(0, 3));

        public readonly IFrameAnimation checkboxFrames = new LinearFrameAnimation(0, 3);
        public readonly IFrameAnimation radioFrames = new LinearFrameAnimation(0, 3);
        public readonly NinepatchSheet buttonDefault;
        public readonly NinepatchSheet buttonHover;
        public readonly NinepatchSheet buttonPress;
        public readonly SpriteFont uiElementFont;
        public readonly SpriteSheet checkboxAndRadioBackground;
        public readonly Image checkboxImage;
        public readonly Image radioImage;

        public UIStyle(
            NinepatchSheet defaultButtonSheet,
            NinepatchSheet hoverButtonSheet,
            NinepatchSheet pressButtonSheet,
            SpriteFont buttonFont,
            SpriteSheet checkboxBackground,
            Image checkboxImage,
            Image radioImage,
            IFrameAnimation checkboxFrames,
            IFrameAnimation radioFrames)
        {
            buttonDefault = defaultButtonSheet;
            buttonHover = hoverButtonSheet;
            buttonPress = pressButtonSheet;
            this.uiElementFont = buttonFont;
            this.checkboxAndRadioBackground = checkboxBackground;
            this.checkboxImage = checkboxImage;
            this.radioImage = radioImage;
            this.radioFrames = radioFrames;
            this.checkboxFrames = checkboxFrames;
        }
    }
}
