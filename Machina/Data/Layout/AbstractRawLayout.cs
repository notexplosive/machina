namespace Machina.Data.Layout
{
    public abstract class AbstractRawLayout<BakedType> where BakedType : IBakedLayout
    {
        public LayoutNode RootNode { get; }
        private LayoutBaker Baker { get; }

        public AbstractRawLayout(LayoutNode rootNode)
        {
            RootNode = rootNode;
            Baker = new LayoutBaker(RootNode);
        }

        public BakedLayout DefaultBake()
        {
            return Baker.Bake();
        }

        public static implicit operator LayoutNode(AbstractRawLayout<BakedType> self)
        {
            return self.RootNode;
        }

        public abstract BakedType Bake();
    }
}
