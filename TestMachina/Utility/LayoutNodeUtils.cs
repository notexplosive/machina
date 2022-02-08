using Machina.Data.Layout;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestMachina.Utility
{
    public class TextMeasureUtils
    {
        public static string DrawResult(TextMeasurer textMeasurer)
        {
            var drawPanel = new AsciiDrawPanel(textMeasurer.TotalAvailableRect.Location + textMeasurer.TotalAvailableRect.Size);

            drawPanel.DrawRectangle(textMeasurer.TotalAvailableRect, '#');

            foreach (var line in textMeasurer.GetRenderedLines(Vector2.Zero, Point.Zero, Color.White, 0f, 0))
            {
                var totalWidth = 0;
                foreach (var character in line.Content)
                {
                    var charPosition = line.PivotPosition.ToPoint() - line.OffsetFromPivot.ToPoint() + new Point(totalWidth, 0);
                    var charSize = textMeasurer.FontMetrics.MeasureString(character.ToString()).ToPoint();

                    drawPanel.DrawRectangle(new Rectangle(charPosition, charSize), '.');

                    for (int x = 0; x < charSize.X; x++)
                    {
                        for (int y = 0; y < charSize.Y; y++)
                        {
                            if ((int)(charSize.X / 2) == x && (int)(charSize.Y / 2) == y)
                            {
                                drawPanel.DrawPixelAt(charPosition + new Point(x, y), character);
                            }
                        }
                    }
                    totalWidth += (int)charSize.X;
                }
            }

            return drawPanel.GetImage();
        }
    }

    public class LayoutNodeUtils
    {
        public static string DrawResult(BakedLayout layoutResult)
        {
            return DrawResultWithSpecificSize(layoutResult, layoutResult.GetNode(layoutResult.OriginalRoot.Name.Text).Size);
        }

        public static string DrawResultWithSpecificSize(BakedLayout layoutResult, Point size)
        {
            var drawPanel = new AsciiDrawPanel(size);
            foreach (var key in layoutResult.AllResultNodeNames())
            {
                var node = layoutResult.GetNode(key);
                drawPanel.DrawRectangle(node.Rectangle, node.NestingLevel.ToString()[0]);
                drawPanel.DrawStringAt(node.Rectangle.Location + new Point(1, 1), key);
            }
            return drawPanel.GetImage();
        }
    }
}
