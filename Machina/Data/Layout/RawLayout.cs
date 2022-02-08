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
        public RawFlowLayout(LayoutNode rootNode) : base(rootNode)
        {
        }
    }
}
