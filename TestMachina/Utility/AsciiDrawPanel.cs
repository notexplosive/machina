using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMachina.Utility
{
    public class AsciiDrawPanel
    {
        private readonly Point size;
        private readonly char[,] content;

        public AsciiDrawPanel(Point size)
        {
            this.size = size;
            this.content = new char[this.size.X, this.size.Y];

            Clear(' ');
        }

        public void Clear(char pixel)
        {
            for (int y = 0; y < this.size.Y; y++)
            {
                for (int x = 0; x < this.size.X; x++)
                {
                    this.content[x, y] = pixel;
                }
            }
        }

        public void DrawPixelAt(Point position, char pixel)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= size.X || position.Y >= size.Y)
            {
                Console.Write($"Skipped pixel {position.X},{position.Y}");
                return;
            }

            this.content[position.X, position.Y] = pixel;
        }

        public void DrawStringAt(Point position, string text)
        {
            int index = 0;
            foreach (var ch in text)
            {
                DrawPixelAt(position + new Point(index, 0), ch);
                index++;
            }
        }

        public void DrawRectangle(Rectangle rectangle, char pixel)
        {
            for (var x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (var y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    var isAlongEdge = x == rectangle.X || (x == (rectangle.X + rectangle.Width - 1)) || y == rectangle.Y || (y == (rectangle.Y + rectangle.Height - 1));
                    if (isAlongEdge)
                    {
                        DrawPixelAt(new Point(x, y), pixel);
                    }
                }
            }
        }

        public string GetImage()
        {
            StringBuilder result = new StringBuilder();
            for (int y = 0; y < this.size.Y; y++)
            {
                for (int x = 0; x < this.size.X; x++)
                {
                    result.Append(this.content[x, y]);
                }
                result.Append('\n');
            }
            return result.ToString();
        }
    }
}
