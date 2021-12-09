using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{

    public class BakedLayout
    {

        private readonly Dictionary<string, LayoutResultNode> content = new Dictionary<string, LayoutResultNode>();
        public LayoutResultNode RootNode { get; }

        public BakedLayout(LayoutResultNode rootNode)
        {
            RootNode = rootNode;
        }

        public LayoutResultNode Get(string name)
        {
            return content[name];
        }

        public LayoutResultNode[] GetAll()
        {
            var result = new LayoutResultNode[this.content.Values.Count];
            this.content.Values.CopyTo(result, 0);
            return result;
        }

        public IEnumerable<string> Keys()
        {
            return this.content.Keys;
        }

        public void Add(string key, LayoutResultNode value)
        {
            this.content[key] = value;
        }
    }
}
