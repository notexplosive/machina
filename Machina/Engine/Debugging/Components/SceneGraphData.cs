using System.Collections.Generic;
using Machina.Components;

namespace Machina.Engine.Debugging.Components
{
    public class SceneGraphData : BaseComponent
    {
        private readonly List<SceneGraphNode> nodes = new List<SceneGraphNode>();
        public readonly SceneLayers sceneLayers;

        public SceneGraphData(Actor actor, SceneLayers sceneLayers) : base(actor)
        {
            this.sceneLayers = sceneLayers;
        }

        public override void Update(float dt)
        {
            this.nodes.Clear();

            foreach (var scene in this.sceneLayers.AllScenesExceptDebug())
            {
                this.nodes.Add(new SceneGraphNode(0, scene));
                foreach (var targetActor in scene.GetRootLevelActors())
                {
                    this.nodes.Add(new SceneGraphNode(1, targetActor));
                    GetChildren(targetActor, this.nodes, 2);
                }
            }
        }

        private void GetChildren(Actor parentActor, List<SceneGraphNode> nodes, int indentLevel)
        {
            for (var i = 0; i < parentActor.transform.ChildCount; i++)
            {
                var child = parentActor.transform.ChildAt(i);
                this.nodes.Add(new SceneGraphNode(indentLevel, child));
                GetChildren(child, nodes, indentLevel + 1);
            }
        }

        public List<SceneGraphNode> GetAllNodes()
        {
            return this.nodes;
        }

        public SceneGraphNode? GetElementAt(int targetIndex)
        {
            if (targetIndex >= 0 && targetIndex < this.nodes.Count)
            {
                return this.nodes[targetIndex];
            }

            return null;
        }

        public struct SceneGraphNode
        {
            public readonly int depth;
            public readonly ICrane crane;
            public readonly string displayName;

            public SceneGraphNode(int depth, ICrane data)
            {
                this.depth = depth;
                this.crane = data;
                this.displayName = data.ToString().Split('/')[^1];
            }
        }
    }
}