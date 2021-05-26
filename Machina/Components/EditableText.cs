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
        private Color cursorColor = Color.White;
        private Color highlightColor = Color.CornflowerBlue;
        private float flickerTimer;
        public Action<string> onSubmit;

        public string Text
        {
            get => this.textRenderer.Text;
            set => this.textRenderer.Text = value;
        }

        public EditableText(Actor actor, Clickable clickable, Color cursorColor, Color highlightColor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.clickable = clickable;
            this.clickable.onClick += OnClick;
            this.cursorColor = cursorColor;
            this.highlightColor = highlightColor;
        }

        public EditableText(Actor actor, Clickable clickable) : this(actor, clickable, Color.Black, Color.CornflowerBlue)
        {
            // forwards to main ctor
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isInFocus)
            {
                var topLeft = this.textRenderer.TextWorldPos;
                var lineHeight = (int) (textRenderer.Font.LineSpacing * 0.9f);

                if (MathF.Cos(this.flickerTimer * 5) > 0)
                {
                    if (Text.Length != 0)
                    {
                        var cursorLocalPos = new Point((int) textRenderer.Font.MeasureString(Text.Substring(0, this.cursor.position.X)).X, 0);

                        // Caret
                        spriteBatch.FillRectangle(new Rectangle(topLeft +
                            cursorLocalPos,
                            new Point(1, lineHeight)), this.cursorColor, transform.Depth - 1);
                    }
                    else
                    {
                        // fallback case, probably not needed
                        spriteBatch.FillRectangle(new Rectangle(topLeft +
                            new Point(0, 0),
                            new Point(1, (int) (textRenderer.Font.LineSpacing * 0.9f))), this.cursorColor, transform.Depth - 1);
                    }
                }

                if (this.cursor.HighlightLength > 0)
                {
                    // Highlight
                    var highlightStartLocalPos = textRenderer.Font.MeasureString(Text.Substring(0, this.cursor.HighlightStart)).X;
                    var highlightedWidth = textRenderer.Font.MeasureString(HighlightedSubstring).X;
                    spriteBatch.FillRectangle(new RectangleF(topLeft.ToVector2() + new Vector2(highlightStartLocalPos, 0), new Vector2(highlightedWidth, lineHeight)), this.highlightColor, transform.Depth + 2);

                    spriteBatch.DrawString(textRenderer.Font, HighlightedSubstring + ", " + highlightedWidth, Vector2.Zero, Color.Red);
                }
            }
        }

        public string HighlightedSubstring => Text.Substring(this.cursor.HighlightStart, this.cursor.HighlightLength);

        public override void Update(float dt)
        {
            this.flickerTimer += dt;
        }

        public override void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            if (this.isInFocus && this.actor.Visible)
            {
                this.flickerTimer = 0;
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
                this.flickerTimer = 0;
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
                else if (key == Keys.Enter)
                {
                    this.isInFocus = false;
                    this.onSubmit?.Invoke(Text);
                }

                if (!modifiers.Shift)
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
                        this.flickerTimer = 0;
                        this.cursor = new TextCursor();

                        var topLeft = this.textRenderer.TextWorldPos.ToVector2();
                        var substring = "";
                        int charIndex = 0;
                        foreach (var c in textRenderer.Text)
                        {
                            substring += c;
                            var substrWidth = textRenderer.Font.MeasureString(substring).X;
                            var rect = new Rectangle(topLeft.ToPoint(), new Point((int) substrWidth, textRenderer.Font.LineSpacing));
                            if (rect.Contains(currentPosition))
                            {
                                break;
                            }
                            charIndex++;
                        }
                        this.cursor.position.X = charIndex;
                    }
                }
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            var topLeft = this.textRenderer.TextWorldPos.ToVector2();
            var substring = "";
            int charIndex = 0;
            foreach (var c in textRenderer.Text)
            {
                substring += c;
                var substrWidth = textRenderer.Font.MeasureString(substring).X;
                var rect = new Rectangle(topLeft.ToPoint(), new Point((int) substrWidth, textRenderer.Font.LineSpacing));
                spriteBatch.DrawRectangle(
                    rect,
                    Color.Orange, 1f, transform.Depth);
                charIndex++;

            }
            this.cursor.position.X = charIndex;
        }

        private void OnClick(MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                this.isInFocus = true;
                this.cursor.ResetAnchor();
            }
        }

        public override void OnDeleteFinished()
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
