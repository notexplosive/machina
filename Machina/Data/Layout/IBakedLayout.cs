using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public interface IBakedLayout
    {
        BakedLayoutNode GetNode(string name);
        IEnumerable<string> AllResultNodeNames();
        public LayoutNode OriginalRoot { get; }
    }
}
