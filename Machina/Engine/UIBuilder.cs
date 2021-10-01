using System;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Engine
{
    public class UIBuilder
    {
        public readonly UIStyle style;

        public UIBuilder(UIStyle style)
        {
            this.style = style;
        }

        public Actor BuildButton(LayoutGroup group, string buttonLabelText, Action onPressCallback, int height = 48)
        {
            var scene = group.actor.scene;
            var buttonActor = scene.AddActor("Button");
            buttonActor.transform.SetParent(group.actor);
            new BoundingRect(buttonActor, 0, height);
            new NinepatchRenderer(buttonActor, this.style.buttonDefault);
            new Hoverable(buttonActor);
            new Clickable(buttonActor);
            new CallbackOnClick(buttonActor, onPressCallback);
            new ButtonNinepatchHandler(buttonActor, this.style.buttonHover, this.style.buttonPress);
            new LayoutElement(buttonActor).StretchHorizontally();
            new LayoutGroup(buttonActor, Orientation.Vertical).SetMarginSize(new Point(5, 5));
            var buttonLabelActor = scene.AddActor("Button Label");
            buttonLabelActor.transform.SetParent(buttonActor);
            new BoundingRect(buttonLabelActor, Point.Zero);
            var buttonLabelElement = new LayoutElement(buttonLabelActor);
            buttonLabelElement.StretchHorizontally();
            buttonLabelElement.StretchVertically();
            new BoundedTextRenderer(buttonLabelActor, buttonLabelText, this.style.uiElementFont, Color.White,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            buttonActor.transform.LocalDepth = new Depth(-1);
            buttonLabelActor.transform.LocalDepth = new Depth(-1);

            return buttonActor;
        }

        public CheckboxState BuildCheckbox(LayoutGroup uiGroup, string labelText, bool startChecked = false)
        {
            var actor = BuildCheckboxOrRadioButton(uiGroup, labelText, startChecked, true);
            return actor.GetComponent<CheckboxState>();
        }

        public void BuildRadioButton(LayoutGroup uiGroup, string labelText, bool startFilled = false)
        {
            BuildCheckboxOrRadioButton(uiGroup, labelText, startFilled, false);
        }

        private Actor BuildCheckboxOrRadioButton(LayoutGroup uiGroup, string labelText, bool startChecked,
            bool isCheckbox)
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
            checkboxContainer.transform.SetParent(uiGroup.actor);

            new BoundingRect(checkboxContainer, new Point(256, 30));
            new Hoverable(checkboxContainer);
            var checkboxClickable = new Clickable(checkboxContainer);
            ICheckboxStateProvider stateProvider;
            if (isCheckbox)
            {
                stateProvider = new CheckboxState(checkboxContainer, startChecked);
            }
            else
            {
                stateProvider = new RadioButtonState(checkboxContainer, radioButtonGroup, startChecked);
            }

            new LayoutElement(checkboxContainer).StretchHorizontally();
            new LayoutGroup(checkboxContainer, Orientation.Horizontal).SetPaddingBetweenElements(5);

            var checkboxBox = scene.AddActor("Checkbox-Box");
            checkboxBox.transform.SetParent(checkboxContainer);
            new BoundingRect(checkboxBox, new Point(24, 24));
            new LayoutElement(checkboxBox).StretchVertically();
            if (isCheckbox)
            {
                new CheckboxRenderer(checkboxBox, this.style.uiSpriteSheet, this.style.checkboxImage, stateProvider,
                    checkboxClickable, this.style.checkboxFrames);
            }
            else
            {
                new CheckboxRenderer(checkboxBox, this.style.uiSpriteSheet, this.style.radioImage, stateProvider,
                    checkboxClickable, this.style.radioFrames);
            }

            var checkboxLabel = scene.AddActor("Checkbox-Label");
            checkboxLabel.transform.SetParent(checkboxContainer);
            new BoundingRect(checkboxLabel, new Point(0, 24));
            new LayoutElement(checkboxLabel).StretchHorizontally().StretchVertically();
            new BoundedTextRenderer(checkboxLabel, labelText, this.style.uiElementFont, Color.White,
                HorizontalAlignment.Left, VerticalAlignment.Center);

            checkboxBox.transform.LocalDepth = new Depth(-1);
            checkboxContainer.transform.LocalDepth = new Depth(-1);
            checkboxLabel.transform.LocalDepth = new Depth(-1);

            return checkboxContainer;
        }

        public Slider BuildSlider(LayoutGroup uiGroup)
        {
            var sliderActor = uiGroup.actor.transform.AddActorAsChild("Slider");
            new BoundingRect(sliderActor, new Point(0, 24));
            new LayoutElement(sliderActor).StretchHorizontally();
            new Hoverable(sliderActor);
            var slider = new Slider(sliderActor, this.style.sliderSheet, this.style.uiSpriteSheet,
                this.style.sliderThumbFrames);
            sliderActor.transform.LocalDepth = -1;

            return slider;
        }

        public Actor BuildLabel(LayoutGroup group, string textLabel)
        {
            var scene = group.actor.scene;
            var labelActor = scene.AddActor("Label");
            labelActor.transform.SetParent(group.actor);
            new BoundingRect(labelActor, new Point(32, 32));
            new BoundedTextRenderer(labelActor, textLabel, this.style.uiElementFont, Color.White,
                HorizontalAlignment.Left, VerticalAlignment.Bottom);
            var e = new LayoutElement(labelActor);
            e.StretchHorizontally();

            labelActor.transform.LocalDepth = new Depth(-1);

            return labelActor;
        }

        public DropdownTrigger BuildDropdownMenu(LayoutGroup group, params DropdownContent.DropdownItem[] items)
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
            var content = new DropdownContent(dropdownContent, this.style.uiElementFont, this.style.buttonPress,
                this.style.buttonHover);
            dropdownContent.transform.LocalDepth = new Depth(-10);
            dropdown.transform.LocalDepth = new Depth(-3);

            foreach (var item in items)
            {
                content.Add(item);
            }

            new BoundedTextRenderer(dropdown, "", this.style.uiElementFont);
            return new DropdownTrigger(dropdown, content, this.style.uiSpriteSheet, this.style.dropdownFrames,
                this.style.buttonDefault);
        }

        public EditableText BuildTextField(LayoutGroup uiGroup)
        {
            var textInput = uiGroup.actor.transform.AddActorAsChild("TextInput");
            textInput.transform.LocalDepth = new Depth(-1);
            new BoundingRect(textInput, new Point(32, 28));
            new LayoutElement(textInput).StretchHorizontally();
            new NinepatchRenderer(textInput, this.style.textboxSheet);
            new LayoutGroup(textInput, Orientation.Vertical).SetMarginSize(new Point(3, 3));
            new Hoverable(textInput);
            var clickable = new Clickable(textInput);
            var text = textInput.transform.AddActorAsChild("TextInput - Text");
            text.transform.LocalDepth = new Depth(-1);
            new BoundingRect(text, new Point(32, 32));
            new LayoutElement(text).StretchHorizontally().StretchVertically();
            new BoundedTextRenderer(text, "", this.style.uiElementFont, Color.Black, overflow: Overflow.Ignore);
            return new EditableText(text, clickable);
        }
    }

    public class UIStyle
    {
        /// <summary>
        ///     Should only be used in tests
        /// </summary>
        public static readonly UIStyle Empty = new UIStyle(null, null, null, null, null, null, null, null, null, null);

        public readonly NinepatchSheet buttonDefault;
        public readonly NinepatchSheet buttonHover;
        public readonly NinepatchSheet buttonPress;

        public readonly IFrameAnimation checkboxFrames = new LinearFrameAnimation(0, 3);
        public readonly Image checkboxImage;
        public readonly IFrameAnimation closeButtonFrames = new LinearFrameAnimation(21, 3);
        public readonly IFrameAnimation dropdownFrames = new LinearFrameAnimation(0, 3);
        public readonly IFrameAnimation maximizeButtonFrames = new LinearFrameAnimation(24, 3);
        public readonly IFrameAnimation minimizeButtonFrames = new LinearFrameAnimation(27, 3);
        public readonly IFrameAnimation radioFrames = new LinearFrameAnimation(0, 3);
        public readonly Image radioImage;
        public readonly NinepatchSheet sliderSheet;
        public readonly IFrameAnimation sliderThumbFrames;
        public readonly NinepatchSheet textboxSheet;
        public readonly SpriteFont uiElementFont;
        public readonly SpriteSheet uiSpriteSheet;
        public readonly NinepatchSheet windowSheet;

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
            Image radioImage)
        {
            this.buttonDefault = defaultButtonSheet;
            this.buttonHover = hoverButtonSheet;
            this.buttonPress = pressButtonSheet;
            this.textboxSheet = textboxSheet;
            this.windowSheet = windowSheet;
            this.sliderSheet = sliderSheet;
            this.uiElementFont = buttonFont;
            this.uiSpriteSheet = uiSpriteSheet;
            this.checkboxImage = checkboxImage;
            this.radioImage = radioImage;
        }
    }
}
