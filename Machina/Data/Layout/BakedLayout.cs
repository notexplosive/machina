using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class BakedLayout : IBakedLayout
    {
        private readonly Dictionary<LayoutNode, BakedLayoutNode> rawToBakedLookup = new Dictionary<LayoutNode, BakedLayoutNode>();
        public LayoutNode OriginalRoot { get; }

        public BakedLayout(LayoutNode originalRoot)
        {
            OriginalRoot = originalRoot;
        }

        public BakedLayoutNode GetNode(string name, Point offset)
        {
            var node = OriginalRoot.FindChildNodeWithName(name);

            var result = this.rawToBakedLookup[node];

            result = new BakedLayoutNode(result.PositionRelativeToRoot + offset, result.Size, result.NestingLevel);
            
            return result;
        }

        public BakedLayoutNode GetNode(LayoutNode unbakedNode)
        {
            return this.rawToBakedLookup[unbakedNode];
        }

        public BakedLayoutNode[] GetAllResultNodes()
        {
            var result = new BakedLayoutNode[this.rawToBakedLookup.Values.Count];
            this.rawToBakedLookup.Values.CopyTo(result, 0);
            return result;
        }

        public List<BakedLayoutNode> GetAllResultNodesInHierarchyOrder()
        {
            var result = new List<BakedLayoutNode>();

            void AddAndRecurse(LayoutNode node)
            {
                if (node.IsBakable)
                {
                    result.Add(rawToBakedLookup[node]);
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

        public BakedLayoutNode GetNode(string name)
        {
            return GetNode(name, Point.Zero);
        }

        public IEnumerable<string> AllResultNodeNames()
        {
            foreach (var node in this.rawToBakedLookup.Keys)
            {
                if (node.Name.Exists)
                {
                    yield return node.Name.Text;
                }
            }
        }

        public void Add(LayoutNode key, BakedLayoutNode value)
        {
            this.rawToBakedLookup[key] = value;
        }

        public BakedLayoutNode[] GetDirectChildrenOfNode(string nodeName)
        {
            var node = OriginalRoot.FindChildNodeWithName(nodeName);
            return GetDirectChildrenOfNode(node);
        }

        public BakedLayoutNode[] GetDirectChildrenOfNode(LayoutNode node)
        {
            if (!this.rawToBakedLookup.ContainsKey(node))
            {
                throw new Exception($"No node matches {node}");
            }

            if (node.Name.IsNameless)
            {
                throw new Exception($"Found node {node} but its nameless so it can't have children");
            }

            var result = new List<BakedLayoutNode>();

            foreach (var child in node.Children)
            {
                if (child.IsBakable)
                {
                    result.Add(this.rawToBakedLookup[child]);
                }
            }


            return result.ToArray();
        }
    }
}
