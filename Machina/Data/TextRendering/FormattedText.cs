using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.TextRendering
{
    public readonly struct FormattedText
    {
        private readonly ITextInputFragment[] textFragments;

        public int TotalCharacterCount { get; }
        public string OutputString { get; }

        public FormattedText(params ITextInputFragment[] textFragments)
        {
            if (textFragments == null)
            {
                textFragments = Array.Empty<ITextInputFragment>();
            }
            this.textFragments = textFragments;

            OutputString = "";
            TotalCharacterCount = 0;

            foreach (var token in FormattedTokens())
            {
                OutputString += token.OutputFragment().Text;
            }

            TotalCharacterCount = OutputString.Length;
        }

        public static FormattedText FromString(string rawText, IFontMetrics fontMetrics, Color startingColor)
        {
            return new FormattedText(new FormattedTextFragment(rawText, fontMetrics, startingColor));
        }

        public IEnumerable<FormattedTextToken> FormattedTokens()
        {
            if (this.textFragments != null)
            {
                foreach (var textFragment in this.textFragments)
                {
                    foreach (var token in textFragment.Tokens())
                    {
                        yield return token;
                    }
                }
            }
        }

        public List<RenderableText> GetRenderedText(Point position)
        {
            var result = new List<RenderableText>();
            var characterIndex = 0;
            var offset = Point.Zero;
            foreach (var token in FormattedTokens())
            {
                result.Add(new RenderableText(token.ParentFragment.FontMetrics, token.Text, characterIndex, position, token.ParentFragment.Color, offset));
                offset += token.Size.WithJustAxisValue(Axis.X);
                characterIndex += token.Text.Length;
            }
            return result;
        }

        public char GetCharacterAt(int index)
        {
            return OutputString[index];
        }
    }
}
