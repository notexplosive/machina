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
            var spacerActor = scene.AddActor("Spacer");
            new BoundingRect(spacerActor, size);
            var e = new LayoutElement(spacerActor);
            e.StretchHorizontally = stretchHorizontal;
            e.StretchVertically = stretchVertical;

            spacerActor.SetParent(group.actor);

            return spacerActor;
        }

        public void BuildCheckbox(LayoutGroup uiGroup, string labelText, bool startChecked = false)
        {
            BuildCheckboxOrRadioButton(uiGroup, labelText, startChecked, true);
        }

        public void BuildRadioButton(LayoutGroup uiGroup, string labelText, bool startFilled = false)
        {
            BuildCheckboxOrRadioButton(uiGroup, labelText, startFilled, false);
        }

        private void BuildCheckboxOrRadioButton(LayoutGroup uiGroup, string labelText, bool startChecked, bool isCheckbox)
        {
            var scene = uiGroup.actor.scene;

            var radioButtonGroup = uiGroup.actor.GetComponent<RadioButtonGroup>();

            if (!isCheckbox)
            {
                if (radioButtonGroup == null)
                {
                    radioButtonGroup = new RadioButtonGroup(uiGroup.actor);
                }
            }

            var checkboxContainer = scene.AddActor("Checkbox", new Vector2(40, 400));
            new BoundingRect(checkboxContainer, new Point(256, 24));
            new Hoverable(checkboxContainer);
            var checkboxClickable = new Clickable(checkboxContainer);
            ICheckboxStateProvider stateProvider;
            if (isCheckbox)
                stateProvider = new CheckboxState(checkboxContainer, startChecked);
            else
                stateProvider = new RadioButtonState(checkboxContainer, radioButtonGroup, startChecked);

            new LayoutElement(checkboxContainer).StretchHorizontally = true;
            new LayoutGroup(checkboxContainer, Orientation.Horizontal).PaddingBetweenElements = 5;

            var checkboxBox = scene.AddActor("Checkbox-Box");
            checkboxBox.SetParent(checkboxContainer);
            new BoundingRect(checkboxBox, new Point(24, 24));
            new LayoutElement(checkboxBox);
            if (isCheckbox)
                new CheckboxRenderer(checkboxBox, style.checkboxAndRadioBackground, style.checkboxImage, stateProvider, checkboxClickable, style.checkboxFrames);
            else
                new CheckboxRenderer(checkboxBox, style.checkboxAndRadioBackground, style.radioImage, stateProvider, checkboxClickable, style.radioFrames);

            var checkboxLabel = scene.AddActor("Checkbox-Label");
            checkboxLabel.SetParent(checkboxContainer);
            new BoundingRect(checkboxLabel, new Point(0, 24));
            new LayoutElement(checkboxLabel).StretchHorizontally = true;
            new BoundedTextRenderer(checkboxLabel, labelText, style.uiElementFont);

            checkboxContainer.SetParent(uiGroup.actor);
        }

        public Actor BuildLabel(LayoutGroup group, string textLabel)
        {
            var scene = group.actor.scene;
            var spacerActor = scene.AddActor("Label");
            new BoundingRect(spacerActor, new Point(32, 32));
            new BoundedTextRenderer(spacerActor, textLabel, style.uiElementFont);
            var e = new LayoutElement(spacerActor);
            e.StretchHorizontally = true;

            spacerActor.SetParent(group.actor);

            return spacerActor;
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
