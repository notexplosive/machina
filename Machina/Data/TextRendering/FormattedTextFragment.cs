using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public delegate void BoundedDrawFunction(SpriteBatch spriteBatch, Rectangle bounds, Depth depth);

    public readonly struct ImageTextFragment : ITextInputFragment
    {
        public ImageTextFragment(Point size, BoundedDrawFunction drawFunction)
        {
            Size = size;
            this.drawFunction = drawFunction;
        }

        public Point Size { get; }

        private readonly BoundedDrawFunction drawFunction;

        public FormattedTextToken[] Tokens()
        {
            return new FormattedTextToken[] { new FormattedTextToken(new ImageToken(Size, this.drawFunction)) };
        }
    }

    public readonly struct FormattedTextFragment : ITextInputFragment
    {
        public FormattedTextToken[] Tokens()
        {
            var result = new List<FormattedTextToken>();

            foreach (var tokenText in SplitString())
            {
                result.Add(new FormattedTextToken(new DrawableToken(tokenText, FontMetrics, Color)));
            }

            return result.ToArray();
        }

        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public string RawText { get; }

        public FormattedTextFragment(string text, IFontMetrics fontMetrics, Color color)
        {
            RawText = text;
            FontMetrics = fontMetrics;
            Color = color;
        }

        public string[] SplitString()
        {
            var words = new List<string>();
            var pendingWord = new StringBuilder();
            foreach (var character in RawText)
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
