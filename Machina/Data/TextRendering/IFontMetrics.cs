using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public interface IFontMetrics
    {
        Vector2 MeasureString(string text);
        int LineSpacing { get; }
    }
}
