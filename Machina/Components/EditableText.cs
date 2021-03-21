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
    public class EditableText : BaseComponent
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
                var topLeft = transform.Position.ToPoint() + this.textRenderer.TextLocalPos;

                if (Text.Length != 0)
                {
                    var cursorLocalPos = new Point((int) textRenderer.Font.MeasureString(Text.Substring(0, this.cursor.position.X)).X, 0);
                    var anchorLocalPos = new Point((int) textRenderer.Font.MeasureString(Text.Substring(0, this.cursor.anchorPos.X)).X, 0);
                    var lineHeight = (int) (textRenderer.Font.LineSpacing * 0.9f);

                    // Caret
                    spriteBatch.FillRectangle(new Rectangle(topLeft +
                        cursorLocalPos,
                        new Point(1, lineHeight)), Color.Black, transform.Depth - 1);

                    // Highlight
                    spriteBatch.FillRectangle(new Rectangle(topLeft + cursorLocalPos, new Point(anchorLocalPos.X - cursorLocalPos.X, lineHeight)), Color.CornflowerBlue, transform.Depth + 1);
                }
                else
                {
                    // fallback case, probably not needed
                    spriteBatch.FillRectangle(new Rectangle(topLeft +
                        new Point(0, 0),
                        new Point(1, (int) (textRenderer.Font.LineSpacing * 0.9f))), Color.Black, transform.Depth - 1);
                }
            }
        }

        public override void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            if (this.isInFocus)
            {
                if (IsPrintable(inputEventArgs.Character))
                {
                    Text = Text.Insert(this.cursor.position.X, inputEventArgs.Character.ToString());
                    this.cursor.position.X++;
                    this.cursor.ResetAnchor();
                }
                else
                {
                    var key = inputEventArgs.Key;
                    if (key == Keys.Back)
                    {
                        if (this.cursor.HasHighlight)
                        {
                            Text = Text.Remove(this.cursor.HighlightStart, this.cursor.HighlightLength);
                            this.cursor.position.X = this.cursor.HighlightStart;
                        }
                        else if (this.cursor.position.X > 0)
                        {
                            this.cursor.position.X--;
                            Text = Text.Remove(this.cursor.position.X, 1);
                        }
                        this.cursor.ResetAnchor();
                    }
                    if (key == Keys.Delete)
                    {
                        if (this.cursor.HasHighlight)
                        {
                            Text = Text.Remove(this.cursor.HighlightStart, this.cursor.HighlightLength);
                            this.cursor.position.X = this.cursor.HighlightStart;
                        }
                        else if (this.cursor.position.X != Text.Length)
                        {
                            Text = Text.Remove(this.cursor.position.X, 1);
                        }
                        this.cursor.ResetAnchor();
                    }
                }
            }
        }

        private bool IsPrintable(char character)
        {
            return !char.IsControl(character);
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (this.isInFocus && state == ButtonState.Pressed)
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
                else if (key == Keys.Home)
                {
                    this.cursor.position.X = 0;
                }
                else if (key == Keys.End)
                {
                    this.cursor.position.X = Text.Length;
                }

                if (!modifiers.shift)
                {
                    this.cursor.ResetAnchor();
                }
            }
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
                            var textWidth = textRenderer.Font.MeasureString(c.ToString()).X;
                            totalLength += textWidth / 2;

                            if (totalLength > currentPosition.X)
                            {
                                break;
                            }
                            totalLength += textWidth / 2;
                            charIndex++;
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
            public Point anchorPos;

            public void ResetAnchor()
            {
                this.anchorPos = this.position;
            }

            public bool HasHighlight => this.anchorPos != this.position;

            public int HighlightStart => Math.Min(this.position.X, this.anchorPos.X);

            public int HighlightEnd => Math.Max(this.position.X, this.anchorPos.X);

            public int HighlightLength => HighlightEnd - HighlightStart;
        }
    }
}
