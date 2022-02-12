using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data.TextRendering
{
    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        int CharacterLength { get; }

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation);
        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, int substringLength);
        void Draw(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth);
        void DrawDropShadow(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth, Color dropShadowColor);

        /// <summary>
        /// BOOOO!!!! TEST ONLY! :(
        /// </summary>
        /// <returns></returns>
        Point MeasureString(string stringToMeasure);
    }
}
