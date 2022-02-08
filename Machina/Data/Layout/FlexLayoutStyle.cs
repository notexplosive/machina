namespace Machina.Data.Layout
{
    public struct FlexLayoutStyle
    {
        public static FlexLayoutStyle Empty => new FlexLayoutStyle();

        public LayoutStyle InnerStyle { get; }
        public int? MinAlongSize { get; }
        public int? MinPerpendicularSize { get; }

        public FlexLayoutStyle(LayoutStyle style = default, int? minAlongSize = null, int? minPerpendicularSize = null)
        {
            InnerStyle = style;
            MinAlongSize = minAlongSize;
            MinPerpendicularSize = minPerpendicularSize;
        }
    }
}
