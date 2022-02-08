namespace Machina.Data.Layout
{
    public class RawLayout
    {
        public RawLayout(LayoutNode rootNode)
        {
            RootNode = rootNode;
            Baker = new LayoutBaker(RootNode);
        }

        public LayoutNode RootNode { get; }
        private LayoutBaker Baker { get; }

        public BakedLayout Bake()
        {
            return Baker.Bake();
        }

        public static implicit operator LayoutNode(RawLayout self)
        {
            return self.RootNode;
        }
    }

    public class RawFlowLayout : RawLayout
    {
        internal RawFlowLayout(string name, LayoutSize size, LayoutStyle workableAreaStyle, Orientation orientation, FlowLayoutStyle style, FlowLayoutRows rows) : base(
            LayoutNode.OneOffParent(name, size, workableAreaStyle,
                LayoutNode.OrientedParent(orientation.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            ))
        {
            Rows = rows;
        }

        private FlowLayoutRows Rows { get; }
    }
}
