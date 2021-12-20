using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestMachina.Utility
{
    public class AsciiDrawPanel
    {
        public Point Size { get; }
        private readonly char[,] content;
        private List<CharAndPostion> offscreenPixels = new List<CharAndPostion>();

        public AsciiDrawPanel(Point size)
        {
            this.Size = size;
            this.content = new char[this.Size.X, this.Size.Y];
            Clear(' ');
        }

        public void Clear(char pixel)
        {
            for (int y = 0; y < this.Size.Y; y++)
            {
                for (int x = 0; x < this.Size.X; x++)
                {
                    this.content[x, y] = pixel;
                }
            }
        }

        public void DrawPixelAt(Point position, char pixel)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= Size.X || position.Y >= Size.Y)
            {
                offscreenPixels.Add(new CharAndPostion(position, pixel));
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
            for (int y = 0; y < this.Size.Y; y++)
            {
                for (int x = 0; x < this.Size.X; x++)
                {
                    result.Append(this.content[x, y]);
                }
                result.Append('\n');
            }
            if (this.offscreenPixels.Count > 0)
            {
                result.Append('\n').Append($"{this.offscreenPixels.Count} chars were rendered offscreen and were skipped\n");
                foreach (var pixel in this.offscreenPixels)
                {
                    result.Append($"`{pixel.Char}` at {pixel.Position}").Append('\n');
                }
                result.Append('\n');
            }


            return result.ToString();
        }

        private struct CharAndPostion
        {
            public CharAndPostion(Point position, char text)
            {
                Char = text;
                Position = position;
            }

            public char Char { get; }
            public Point Position { get; }
        }

        public void DrawLine(Point start, Point end, char text)
        {
            var startAsVector = start.ToVector2();
            var relativeEndAsVector = end.ToVector2() - startAsVector;

            var distance = relativeEndAsVector.Length();

            for (int i = 0; i < distance; i++)
            {
                var lerp = Vector2.Lerp(Vector2.Zero, relativeEndAsVector, (float)i / distance);
                var location = (startAsVector + lerp).ToPoint();
                DrawPixelAt(location, text);
            }
        }

        public void DrawCanvasAt(Point pos, AsciiDrawPanel innerPanel)
        {
            for (int x = 0; x < innerPanel.Size.X; x++)
            {
                for (int y = 0; y < innerPanel.Size.Y; y++)
                {
                    var innerPanelPixel = innerPanel.content[x, y];
                    DrawPixelAt(new Point(pos.X + x, pos.Y + y), innerPanelPixel);
                }
            }

            foreach (var offscreenPixel in innerPanel.offscreenPixels)
            {
                this.offscreenPixels.Add(offscreenPixel);
            }
        }
    }
}
