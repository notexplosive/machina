using System;

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

    public class RawLayout : AbstractRawLayout<BakedLayout>
    {
        public RawLayout(LayoutNode rootNode) : base(rootNode)
        {
        }

        public override BakedLayout Bake()
        {
            return DefaultBake();
        }
    }

    public class RawFlowLayout : AbstractRawLayout<BakedFlowLayout>
    {
        internal RawFlowLayout(string name, LayoutSize size, LayoutStyle workableAreaStyle, Orientation orientation, FlowLayoutStyle style, FlowLayoutRows rows) : base(
            LayoutNode.OneOffParent(name, size, workableAreaStyle,
                LayoutNode.OrientedParent(orientation.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            ))
        {
            RowNodes = rows.GetLayoutNodesOfEachRow();
        }

        public string[] GetItemNamesForRow(int rowIndex)
        {
            var children = RowNodes[rowIndex].Children;
            var result = new string[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                result[i] = children[i].Name.Text;
            }

            return result;
        }

        public readonly LayoutNode[] RowNodes;

        public string GetRowName(int rowIndex)
        {
            return RowNodes[rowIndex].Name.Text;
        }

        public override BakedFlowLayout Bake()
        {
            return new BakedFlowLayout(DefaultBake(), this);
        }

        public int RowCount => RowNodes.Length;
    }
}
