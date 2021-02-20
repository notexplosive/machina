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
            new LayoutElement(buttonActor).StretchHorizontally();
            new LayoutGroup(buttonActor, Orientation.Vertical).SetMargin(5);
            var buttonLabelActor = scene.AddActor("Button Label");
            new BoundingRect(buttonLabelActor, Point.Zero);
            var buttonLabelElement = new LayoutElement(buttonLabelActor);
            buttonLabelElement.StretchHorizontally();
            buttonLabelElement.StretchVertically();
            new BoundedTextRenderer(buttonLabelActor, buttonLabelText, style.uiElementFont);

            buttonActor.transform.SetParent(group.actor);
            buttonLabelActor.transform.SetParent(buttonActor);

            buttonActor.transform.LocalDepth = new Depth(-1);
            buttonLabelActor.transform.LocalDepth = new Depth(-1);


            return buttonActor;
        }

        public Actor BuildSpacer(LayoutGroup group, Point size, bool stretchHorizontal, bool stretchVertical)
        {
            var scene = group.actor.scene;
            var spacerActor = scene.AddActor("Spacer");
            new BoundingRect(spacerActor, size);
            var e = new LayoutElement(spacerActor);
            if (stretchHorizontal)
            {
                e.StretchHorizontally();
            }
            if (stretchVertical)
            {
                e.StretchVertically();
            }

            spacerActor.transform.SetParent(group.actor);

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

            new LayoutElement(checkboxContainer).StretchHorizontally();
            new LayoutGroup(checkboxContainer, Orientation.Horizontal).PaddingBetweenElements = 5;

            var checkboxBox = scene.AddActor("Checkbox-Box");
            checkboxBox.transform.SetParent(checkboxContainer);
            new BoundingRect(checkboxBox, new Point(24, 24));
            new LayoutElement(checkboxBox);
            if (isCheckbox)
                new CheckboxRenderer(checkboxBox, style.uiSpriteSheet, style.checkboxImage, stateProvider, checkboxClickable, style.checkboxFrames);
            else
                new CheckboxRenderer(checkboxBox, style.uiSpriteSheet, style.radioImage, stateProvider, checkboxClickable, style.radioFrames);

            var checkboxLabel = scene.AddActor("Checkbox-Label");
            checkboxLabel.transform.SetParent(checkboxContainer);
            new BoundingRect(checkboxLabel, new Point(0, 24));
            new LayoutElement(checkboxLabel).StretchHorizontally();
            new BoundedTextRenderer(checkboxLabel, labelText, style.uiElementFont);

            checkboxContainer.transform.SetParent(uiGroup.actor);

            checkboxBox.transform.LocalDepth = new Depth(-1);
            checkboxContainer.transform.LocalDepth = new Depth(-1);
            checkboxLabel.transform.LocalDepth = new Depth(-1);
        }

        public Actor BuildSlider(LayoutGroup uiGroup)
        {
            var sliderActor = uiGroup.actor.transform.AddActorAsChild("Slider");
            new BoundingRect(sliderActor, new Point(0, 24));
            new LayoutElement(sliderActor).StretchHorizontally();
            new Hoverable(sliderActor);
            new Slider(sliderActor, style.sliderSheet, style.uiSpriteSheet, style.sliderThumbFrames);
            sliderActor.transform.LocalDepth = -1;

            return sliderActor;
        }

        public Actor BuildLabel(LayoutGroup group, string textLabel)
        {
            var scene = group.actor.scene;
            var labelActor = scene.AddActor("Label");
            new BoundingRect(labelActor, new Point(32, 32));
            new BoundedTextRenderer(labelActor, textLabel, style.uiElementFont);
            var e = new LayoutElement(labelActor);
            e.StretchHorizontally();

            labelActor.transform.SetParent(group.actor);
            labelActor.transform.LocalDepth = new Depth(-1);

            return labelActor;
        }

        public void BuildDropdownMenu(LayoutGroup group, params DropdownContent.DropdownItem[] items)
        {
            var actor = group.actor;
            var dropdown = actor.transform.AddActorAsChild("Dropdown");
            new BoundingRect(dropdown, new Point(32, 24));
            new LayoutElement(dropdown).StretchHorizontally();
            new Hoverable(dropdown);
            new Clickable(dropdown);
            var dropdownContent = dropdown.transform.AddActorAsChild("Dropdown-Content");
            new BoundingRect(dropdownContent, Point.Zero);
            new Hoverable(dropdownContent);
            var content = new DropdownContent(dropdownContent, style.uiElementFont, style.buttonPress, style.buttonHover);
            dropdownContent.transform.LocalDepth = new Depth(-10);
            dropdown.transform.LocalDepth = new Depth(-3);

            foreach (var item in items)
            {
                content.Add(item);
            }

            new BoundedTextRenderer(dropdown, "", style.uiElementFont);
            new DropdownTrigger(dropdown, content, style.uiSpriteSheet, style.dropdownFrames, style.buttonDefault);
        }

        public Actor BuildTextField(LayoutGroup uiGroup)
        {
            var textInput = uiGroup.actor.transform.AddActorAsChild("TextInput");
            textInput.transform.LocalDepth = new Depth(-1);
            new BoundingRect(textInput, new Point(32, 28));
            new LayoutElement(textInput).StretchHorizontally();
            new NinepatchRenderer(textInput, style.textboxSheet);
            new LayoutGroup(textInput, Orientation.Vertical).SetMargin(3);
            new Hoverable(textInput);
            var clickable = new Clickable(textInput);
            var text = textInput.transform.AddActorAsChild("TextInput - Text");
            text.transform.LocalDepth = new Depth(-1);
            new BoundingRect(text, new Point(32, 32));
            new LayoutElement(text).StretchHorizontally().StretchVertically();
            new BoundedTextRenderer(text, "", style.uiElementFont, Color.Black);
            new EditableText(text, clickable);

            return textInput;
        }
    }

    public class UIStyle
    {
        /// <summary>
        /// Should only be used in tests
        /// </summary>
        public static readonly UIStyle Empty = new UIStyle(null, null, null, null, null, null, null, null, null, null, new LinearFrameAnimation(0, 3), new LinearFrameAnimation(0, 3), new LinearFrameAnimation(0, 3), new LinearFrameAnimation(0, 3));

        public readonly IFrameAnimation checkboxFrames = new LinearFrameAnimation(0, 3);
        public readonly IFrameAnimation sliderThumbFrames;
        public readonly IFrameAnimation radioFrames = new LinearFrameAnimation(0, 3);
        public readonly IFrameAnimation dropdownFrames = new LinearFrameAnimation(0, 3);
        public readonly NinepatchSheet buttonDefault;
        public readonly NinepatchSheet buttonHover;
        public readonly NinepatchSheet buttonPress;
        public readonly NinepatchSheet textboxSheet;
        public readonly NinepatchSheet windowSheet;
        public readonly NinepatchSheet sliderSheet;
        public readonly SpriteFont uiElementFont;
        public readonly SpriteSheet uiSpriteSheet;
        public readonly Image checkboxImage;
        public readonly Image radioImage;

        public UIStyle(
            NinepatchSheet defaultButtonSheet,
            NinepatchSheet hoverButtonSheet,
            NinepatchSheet pressButtonSheet,
            NinepatchSheet textboxSheet,
            NinepatchSheet windowSheet,
            NinepatchSheet sliderSheet,
            SpriteFont buttonFont,
            SpriteSheet uiSpriteSheet,
            Image checkboxImage,
            Image radioImage,
            IFrameAnimation checkboxFrames,
            IFrameAnimation radioFrames,
            IFrameAnimation dropdownFrames,
            IFrameAnimation sliderFrames)
        {
            buttonDefault = defaultButtonSheet;
            buttonHover = hoverButtonSheet;
            buttonPress = pressButtonSheet;
            this.textboxSheet = textboxSheet;
            this.windowSheet = windowSheet;
            this.sliderSheet = sliderSheet;
            this.uiElementFont = buttonFont;
            this.uiSpriteSheet = uiSpriteSheet;
            this.checkboxImage = checkboxImage;
            this.radioImage = radioImage;
            this.radioFrames = radioFrames;
            this.dropdownFrames = dropdownFrames;
            this.checkboxFrames = checkboxFrames;
            this.sliderThumbFrames = sliderFrames;
        }
    }
}
