using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public struct TextDrawingArgs
    {
        public Point Origin { get; set; }
        public Point TopLeft { get; set; }
        public Point OffsetFromTopLeft { get; set; }
        public float Angle { get; set; }
        public Point AdditionalOffset { get; set; }
        public Depth Depth { get; set; }

        public Vector2 OriginToTopLeftTranslation()
        {
            return (TopLeft - Origin).ToVector2();
        }

        public Vector2 ResultOrigin()
        {
            return Origin.ToVector2();
        }

        public Vector2 ResultOffset()
        {
            return AdditionalOffset.ToVector2() - OffsetFromTopLeft.ToVector2();
        }

        public Vector2 FinalPosition()
        {
            var offset = ResultOffset();

            var offsetAngle = MathF.Atan2(offset.Y, offset.X);
            var angle = offsetAngle + Angle;
            var finalOffset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * offset.Length();
            return ResultOrigin() - finalOffset;
        }
    }
}
