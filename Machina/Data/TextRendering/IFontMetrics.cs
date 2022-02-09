using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public interface IFontMetrics
    {
        Vector2 MeasureString(string text);
        Point MeasureStringRounded(string text)
        {
            var characterBounds = MeasureString(text);
            characterBounds.Round();
            return characterBounds.ToPoint();
        }
        int LineSpacing { get; }
    }
}
