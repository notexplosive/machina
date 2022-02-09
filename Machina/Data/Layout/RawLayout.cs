using System;

namespace Machina.Data.Layout
{
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
}
