using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestMachina.Utility
{
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
