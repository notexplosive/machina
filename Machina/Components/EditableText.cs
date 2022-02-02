using System;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Machina.Components
{
    public class EditableText : BaseComponent
    {
        private readonly Clickable clickable;
        private readonly Color cursorColor = Color.White;
        private readonly Color highlightColor = Color.CornflowerBlue;
        private readonly BoundedTextRenderer textRenderer;
        private TextCursor cursor;
        private float flickerTimer;

        private bool isInFocus_impl;
        public Action<string> onSubmit;

        public EditableText(Actor actor, Clickable clickable, Color cursorColor, Color highlightColor) : base(actor)
        {
            this.textRenderer = RequireComponent<BoundedTextRenderer>();
            this.clickable = clickable;
            this.clickable.OnClick += OnClick;
            this.cursorColor = cursorColor;
            this.highlightColor = highlightColor;
        }

        public EditableText(Actor actor, Clickable clickable) : this(actor, clickable, Color.Black,
            Color.CornflowerBlue)
        {
            // forwards to main ctor
        }

        public bool IsInFocus
        {
            get => this.isInFocus_impl;
            set
            {
                this.isInFocus_impl = value;
                this.flickerTimer = 0;
            }
        }

        public string Text
        {
            get => this.textRenderer.Text;
            set => this.textRenderer.Text = value;
        }

        public string HighlightedSubstring => Text.Substring(this.cursor.HighlightStart, this.cursor.HighlightLength);

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsInFocus)
            {
                var topLeft = this.textRenderer.TextWorldPos;
                var lineHeight = (int)(this.textRenderer.FontMetrics.LineSpacing * 0.9f);

                if (MathF.Cos(this.flickerTimer * 5) > 0)
                {
                    if (Text.Length != 0)
                    {
                        var cursorLocalPos =
                            new Point(
                                (int)this.textRenderer.FontMetrics.MeasureString(Text.Substring(0, this.cursor.position.X)).X,
                                0);

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
                                new Point(1, (int)(this.textRenderer.FontMetrics.LineSpacing * 0.9f))), this.cursorColor,
                            transform.Depth - 1);
                    }
                }

                if (this.cursor.HighlightLength > 0)
                {
                    // Highlight
                    var highlightStartLocalPos = this.textRenderer.FontMetrics
                        .MeasureString(Text.Substring(0, this.cursor.HighlightStart)).X;
                    var highlightedWidth = this.textRenderer.FontMetrics.MeasureString(HighlightedSubstring).X;
                    spriteBatch.FillRectangle(
                        new RectangleF(topLeft.ToVector2() + new Vector2(highlightStartLocalPos, 0),
                            new Vector2(highlightedWidth, lineHeight)), this.highlightColor, transform.Depth + 2);

                    spriteBatch.DrawString(this.textRenderer.Font, HighlightedSubstring + ", " + highlightedWidth,
                        Vector2.Zero, Color.Red);
                }
            }
        }

        public override void Update(float dt)
        {
            this.flickerTimer += dt;
        }

        public override void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            if (IsInFocus && this.actor.Visible)
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
            if (IsInFocus && state == ButtonState.Pressed)
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
                    // this.IsInFocus = false;
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
                    IsInFocus = false;
                }
                else
                {
                    if (button == MouseButton.Left)
                    {
                        this.flickerTimer = 0;
                        this.cursor = new TextCursor();

                        var topLeft = this.textRenderer.TextWorldPos.ToVector2();
                        var substring = "";
                        var charIndex = 0;
                        foreach (var c in this.textRenderer.Text)
                        {
                            substring += c;
                            var substrWidth = this.textRenderer.FontMetrics.MeasureString(substring).X;
                            var rect = new Rectangle(topLeft.ToPoint(),
                                new Point((int)substrWidth, this.textRenderer.FontMetrics.LineSpacing));
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
            var charIndex = 0;
            foreach (var c in this.textRenderer.Text)
            {
                substring += c;
                var substrWidth = this.textRenderer.FontMetrics.MeasureString(substring).X;
                var rect = new Rectangle(topLeft.ToPoint(),
                    new Point((int)substrWidth, this.textRenderer.FontMetrics.LineSpacing));
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
                IsInFocus = true;
                this.cursor.ResetAnchor();
            }
        }

        public override void OnDeleteFinished()
        {
            this.clickable.OnClick -= OnClick;
        }

        private struct TextCursor
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