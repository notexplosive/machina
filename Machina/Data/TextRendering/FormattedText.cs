using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            foreach (var token in GetTokens())
            {
                OutputString += token.Drawable.TokenText;
            }

            TotalCharacterCount = OutputString.Length;
        }

        public static FormattedText FromString(string rawText, IFontMetrics fontMetrics, Color color)
        {
            return new FormattedText(new FormattedTextFragment(rawText, fontMetrics, color));
        }

        public static FormattedText FromString(string rawText, SpriteFont realFont, Color color)
        {
            return new FormattedText(new FormattedTextFragment(rawText, (SpriteFontMetrics)realFont, color));
        }

        public IEnumerable<TextOutputFragment> OutputFragments()
        {
            var characterIndex = 0;
            foreach (var token in GetTokens())
            {
                var output = token.CreateOutputFragment(characterIndex);
                yield return output;
                characterIndex += output.CharacterLength;
            }
        }

        private IEnumerable<FormattedTextToken> GetTokens()
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

        public List<RenderableText> GetRenderedText()
        {
            return new BoundedText(new Point(int.MaxValue, int.MaxValue), Alignment.TopLeft, Overflow.Ignore, this).GetRenderedText();
        }

        public static FormattedText FromParseString(string parsableString, IFontMetrics startingFont, Color startingColor)
        {
            var commands = FormattedTextParser.ParseCommands(parsableString);
            var result = FormattedTextParser.GetFragmentsFromCommands(commands, startingFont, startingColor);

            return new FormattedText(result);
        }

        public char GetCharacterAt(int index)
        {
            return OutputString[index];
        }
    }
}
