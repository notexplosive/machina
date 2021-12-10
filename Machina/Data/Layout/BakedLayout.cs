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

        public List<NodePositionAndSize> GetAllResultNodesInHierarchyOrder()
        {
            var result = new List<NodePositionAndSize>();
            result.Add(content[OriginalRoot.Name.Text]);

            void AddAndRecurse(LayoutNode node)
            {
                if (node.Name.Exists)
                {
                    result.Add(content[node.Name.Text]);
                    if (node.HasChildren)
                    {
                        foreach (var child in node.Children)
                        {
                            AddAndRecurse(child);
                        }
                    }
                }
            }

            AddAndRecurse(OriginalRoot);
            return result;
        }

        public IEnumerable<string> AllResultNodeNames()
        {
            return this.content.Keys;
        }

        public void Add(string key, NodePositionAndSize value)
        {
            this.content[key] = value;
        }
    }
}
