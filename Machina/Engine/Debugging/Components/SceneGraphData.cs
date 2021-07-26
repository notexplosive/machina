using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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

                    for (int i = 0; i < targetActor.transform.ChildCount; i++)
                    {
                        this.nodes.Add(new SceneGraphNode(2, targetActor.transform.ChildAt(i)));
                    }

                    // foreach (var component in targetActor.GetComponentsUnsafe<IComponent>())
                    {
                        // DrawString(component.GetType().Name.ToString(), 2);
                    }
                }
            }
        }

        public List<SceneGraphNode> GetAllNodes()
        {
            return this.nodes;
        }

        public SceneGraphNode? GetElementAt(int targetIndex)
        {
            if (targetIndex >= 0 && targetIndex < this.nodes.Count)
                return this.nodes[targetIndex];

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
