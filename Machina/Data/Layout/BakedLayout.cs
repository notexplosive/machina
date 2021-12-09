using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{

    public class BakedLayout
    {

        private readonly Dictionary<string, NodePositionAndSize> content = new Dictionary<string, NodePositionAndSize>();
        public LayoutNode OriginalRoot { get; }

        public BakedLayout(LayoutNode originalRoot)
        {
            OriginalRoot = originalRoot;
        }

        public NodePositionAndSize GetNode(string name)
        {
            return content[name];
        }

        public NodePositionAndSize[] GetAllResultNodes()
        {
            var result = new NodePositionAndSize[this.content.Values.Count];
            this.content.Values.CopyTo(result, 0);
            return result;
        }

        public IEnumerable<string> ResultNodeNames()
        {
            return this.content.Keys;
        }

        public void Add(string key, NodePositionAndSize value)
        {
            this.content[key] = value;
        }
    }
}
