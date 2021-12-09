namespace Machina.Data.Layout
{
    public interface ILayoutEdge
    {
        public bool IsStretched { get; }
        public int ActualSize { get; }
    }
}
