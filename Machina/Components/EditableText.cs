using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class EditableText : BaseComponent
    {
        private readonly BoundedTextRenderer textRenderer;
        private readonly Clickable clickable;
        private bool isInFocus;
        private TextCursor cursor;
        public string Text
        {
            get => this.textRenderer.Text;
            set => this.textRenderer.Text = value;
        }

        public EditableText(Actor actor, Clickable clickable) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.clickable = clickable;
            this.clickable.onClick += OnClick;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isInFocus)
            {
                spriteBatch.FillRectangle(new Rectangle(transform.Position.ToPoint() +
                    new Point((int) textRenderer.font.MeasureString(Text.Substring(0, this.cursor.position.X)).X, 0),
                    new Point(1, (int) (textRenderer.font.LineSpacing * 0.9f))), Color.Black, this.transform.Depth);
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (this.isInFocus)
            {
                if (state == ButtonState.Pressed && !modifiers.control && !modifiers.alt)
                {
                    if (IsPrintable(key))
                    {
                        var str = modifiers.shift ? key.ToString() : key.ToString().ToLower();
                        Text = Text.Insert(this.cursor.position.X, str);
                        this.cursor.position.X++;
                    }
                    else
                    {
                        if (key == Keys.Left)
                        {
                            this.cursor.position.X--;
                            if (this.cursor.position.X < 0)
                            {
                                this.cursor.position.X = 0;
                            }
                        }
                        else if (key == Keys.Right)
                        {
                            this.cursor.position.X++;
                            if (this.cursor.position.X > Text.Length)
                            {
                                this.cursor.position.X = Text.Length;
                            }
                        }
                        else if (key == Keys.Back)
                        {
                            if (this.cursor.position.X > 0)
                            {
                                this.cursor.position.X--;
                                Text = Text.Remove(this.cursor.position.X, 1);
                            }
                        }
                        else if (key == Keys.Space)
                        {
                            Text = Text.Insert(this.cursor.position.X, " ");
                            this.cursor.position.X++;
                        }
                    }
                }
            }
        }

        private bool IsPrintable(Keys key)
        {
            return ((int) key >= 65 && (int) key <= 90);
        }

        public override void OnMouseUpdate(Vector2 currentPosition, Vector2 positionDelta, Vector2 rawDelta)
        {

        }

        public override void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState buttonState)
        {
            if (buttonState == ButtonState.Pressed)
            {
                if (!this.clickable.IsHovered)
                {
                    this.isInFocus = false;

                }
                else
                {
                    if (button == MouseButton.Left)
                    {
                        this.cursor = new TextCursor();
                        var totalLength = transform.Position.X;
                        int charIndex = 0;
                        foreach (var c in textRenderer.Text)
                        {
                            totalLength += textRenderer.font.MeasureString(c.ToString()).X;
                            charIndex++;

                            if (totalLength > currentPosition.X)
                            {
                                break;
                            }
                        }
                        this.cursor.position.X = charIndex;
                    }
                }
            }
        }

        private void OnClick(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                this.isInFocus = true;

            }
        }

        public override void OnDelete()
        {
            this.clickable.onClick -= OnClick;
        }

        struct TextCursor
        {
            public Point position;
            public Point anchorPosition;
        }
    }
}
