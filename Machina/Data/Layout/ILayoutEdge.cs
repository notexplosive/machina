namespace Machina.Data.Layout
{
    public interface ILayoutEdge
    {
        public bool IsConstant { get; }
        public int ActualSize { get; }
        public int AspectSize { get; }
    }
}
