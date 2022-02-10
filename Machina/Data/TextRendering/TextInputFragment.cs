﻿using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct TextInputFragment : ITextInputFragment
    {
        public TextInputToken[] Tokens()
        {
            var result = new List<TextInputToken>();

            foreach (var tokenText in CreateTokens(Text))
            {
                result.Add(new TextInputToken(tokenText, this));
            }

            return result.ToArray();
        }

        public IFontMetrics FontMetrics { get; }
        public string Text { get; }

        public TextInputFragment(string text, IFontMetrics fontMetrics)
        {
            Text = text;
            FontMetrics = fontMetrics;
        }

        public static string[] CreateTokens(string text)
        {
            var words = new List<string>();
            var pendingWord = new StringBuilder();
            foreach (var character in text)
            {
                if (character == ' ' || character == '\n')
                {
                    words.Add(pendingWord.ToString());
                    pendingWord.Clear();
                    words.Add(character.ToString());
                }
                else
                {
                    pendingWord.Append(character);
                }
            }

            if (pendingWord.Length > 0)
            {
                words.Add(pendingWord.ToString());
            }


            return words.ToArray();
        }
    }
}
