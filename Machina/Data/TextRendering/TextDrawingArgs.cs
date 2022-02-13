using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public struct TextDrawingArgs
    {
        public Point Origin { get; set; }
        public Point Position { get; set; }
        public float Angle { get; set; }
        public Point AdditionalOffset { get; set; }
        public Depth Depth { get; set; }

        public Vector2 ResultOrigin()
        {
            return Origin.ToVector2();
        }

        public Vector2 ResultOffset()
        {
            return AdditionalOffset.ToVector2() - Position.ToVector2();
        }
    }
}
