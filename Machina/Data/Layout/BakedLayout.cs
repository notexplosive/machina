using System;
using System.Collections;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public interface IBakedLayout
    {
        BakedLayoutNode GetNode(string name);
        IEnumerable<string> AllResultNodeNames();
        public LayoutNode OriginalRoot { get; }
    }


    public class BakedFlowLayout : IBakedLayout
    {
        private readonly BakedLayout bakedLayout;
        private readonly RawFlowLayout rawFlowLayout;
        private readonly BakedRow[] rows;

        public LayoutNode OriginalRoot => this.bakedLayout.OriginalRoot;

        public BakedFlowLayout(BakedLayout bakedLayout, RawFlowLayout rawFlowLayout)
        {
            this.bakedLayout = bakedLayout;
            this.rawFlowLayout = rawFlowLayout;
            this.rows = new BakedRow[this.rawFlowLayout.RowCount];

            for (int rowIndex = 0; rowIndex < this.rawFlowLayout.RowCount; rowIndex++)
            {
                var rowItems = this.rawFlowLayout.GetItemNamesForRow(rowIndex);
                this.rows[rowIndex] = new BakedRow(bakedLayout, this.rawFlowLayout.GetRowName(rowIndex), rowItems);
            }
        }

        public BakedLayoutNode GetNode(string nodeName)
        {
            return this.bakedLayout.GetNode(nodeName);
        }

        public IEnumerable<string> AllResultNodeNames()
        {
            return this.bakedLayout.AllResultNodeNames();
        }

        public BakedRow GetRow(int rowIndex)
        {
            return this.rows[rowIndex];
        }

        public IEnumerable<BakedRow> Rows => this.rows;

        public class BakedRow : IEnumerable<BakedLayoutNode>
        {
            private readonly BakedLayoutNode rowNode;
            private readonly BakedLayoutNode[] itemNodes;

            public BakedRow(BakedLayout bakedLayout, string rowName, string[] rowItemNames)
            {
                this.rowNode = bakedLayout.GetNode(rowName);
                this.itemNodes = new BakedLayoutNode[rowItemNames.Length];

                for (int i = 0; i < this.itemNodes.Length; i++)
                {
                    this.itemNodes[i] = bakedLayout.GetNode(rowItemNames[i]);
                }
            }

            public BakedLayoutNode GetRowNode()
            {
                return this.rowNode;
            }

            public BakedLayoutNode GetItemNode(int itemIndex)
            {
                return this.itemNodes[itemIndex];
            }

            public IEnumerator<BakedLayoutNode> GetEnumerator()
            {
                foreach (var item in this.itemNodes)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.itemNodes.GetEnumerator();
            }

            public int ItemCount => this.itemNodes.Length;
        }
    }

    public class BakedLayout : IBakedLayout
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

        public List<BakedLayoutNode> GetDirectChildrenOfNode(string nodeName)
        {
            if (!this.content.ContainsKey(nodeName))
            {
                throw new Exception($"No node matches name {nodeName}");
            }

            var result = new List<BakedLayoutNode>();

            var parent = OriginalRoot.FindChildNodeWithName(nodeName);

            foreach (var child in parent.Children)
            {
                if (child.Name.Exists)
                {
                    result.Add(this.content[child.Name.Text]);
                }
            }


            return result;
        }
    }
}
