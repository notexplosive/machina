﻿using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{

    public class BakedLayout
    {

        private readonly Dictionary<string, BakedLayoutNode> content = new Dictionary<string, BakedLayoutNode>();
        public LayoutNode OriginalRoot { get; }

        public BakedLayout(LayoutNode originalRoot)
        {
            OriginalRoot = originalRoot;
        }

        public BakedLayoutNode GetNode(string name)
        {
            return content[name];
        }

        public BakedLayoutNode[] GetAllResultNodes()
        {
            var result = new BakedLayoutNode[this.content.Values.Count];
            this.content.Values.CopyTo(result, 0);
            return result;
        }

        public List<BakedLayoutNode> GetAllResultNodesInHierarchyOrder()
        {
            var result = new List<BakedLayoutNode>();
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

        public void Add(string key, BakedLayoutNode value)
        {
            this.content[key] = value;
        }
    }
}
