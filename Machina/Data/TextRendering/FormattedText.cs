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

            foreach (var token in GetAllTokens())
            {
                OutputString += token.OutputFragment().Text;
            }

            TotalCharacterCount = OutputString.Length;
        }

        public IEnumerable<TextInputToken> GetAllTokens()
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

        public char GetCharacterAt(int index)
        {
            return OutputString[index];
        }
    }
}
