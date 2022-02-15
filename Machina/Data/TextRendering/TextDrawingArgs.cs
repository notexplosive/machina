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

        public Vector2 ResultTopLeft()
        {
            return TopLeft.ToVector2();
        }

        public Vector2 ResultOffset()
        {
            return -AdditionalOffset.ToVector2() - OffsetFromTopLeft.ToVector2();
        }

        public Vector2 FinalPosition()
        {
            var origin = Matrix.CreateTranslation(new Vector3(Origin.ToVector2(), 0));
            var rotation = Matrix.CreateRotationZ(Angle);
            var originToTopLeft = Matrix.CreateTranslation(new Vector3(OriginToTopLeftTranslation(), 0));
            var offset = Matrix.CreateTranslation(new Vector3(-ResultOffset(), 0));

            return Vector2.Transform(Vector2.Zero, originToTopLeft * offset * rotation * origin);
        }

        public Vector2 GetTextOffset()
        {
            return OriginToTopLeftTranslation() + ResultOffset();
        }
    }
}
