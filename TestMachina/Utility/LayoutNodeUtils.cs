﻿using ApprovalTests.Core;
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
        public static string DrawResult(BoundedText textMeasurer)
        {
            var drawPanel = new AsciiDrawPanel(textMeasurer.TotalAvailableSize);

            drawPanel.DrawRectangle(new Rectangle(Point.Zero, textMeasurer.TotalAvailableSize), '#');

            return DrawRenderedText(drawPanel, textMeasurer.GetRenderedText());
        }

        public static string DrawRenderedText(AsciiDrawPanel drawPanel, List<RenderableText> renderedText)
        {
            foreach (var token in renderedText)
            {
                var totalWidth = 0;
                for (int characterIndex = 0; characterIndex < token.Drawable.CharacterLength; characterIndex++)
                {
                    var character = token.Drawable.GetCharacterAt(characterIndex);
                    var charPosition = token.Origin + token.Offset + new Point(totalWidth, 0);
                    var charSize = token.Drawable.SizeOfCharacter(characterIndex);

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
        public static string DrawResult(IBakedLayout layoutResult)
        {
            return DrawResultWithSpecificSize(layoutResult, layoutResult.GetNode(layoutResult.OriginalRoot.Name.Text).Size);
        }

        public static string DrawResultWithSpecificSize(IBakedLayout layoutResult, Point size)
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

        public static string DrawUsedRectangles(IBakedLayout layoutResult, IEnumerable<BakedFlowLayout.BakedRow> rows)
        {
            var drawPanel = new AsciiDrawPanel(layoutResult.GetNode(layoutResult.OriginalRoot.Name.Text).Size);

            foreach (var row in rows)
            {
                drawPanel.DrawRectangle(row.UsedRectangle, '#');
            }

            return drawPanel.GetImage();
        }

        public static string DrawItems(BakedFlowLayout layoutResult, IEnumerable<BakedFlowLayout.BakedRow> rows)
        {
            var drawPanel = new AsciiDrawPanel(layoutResult.GetNode(layoutResult.OriginalRoot.Name.Text).Size);

            foreach (var row in rows)
            {
                foreach (var item in row)
                {
                    drawPanel.DrawRectangle(item.Rectangle, '.');
                }
            }

            return drawPanel.GetImage();
        }
    }
}
