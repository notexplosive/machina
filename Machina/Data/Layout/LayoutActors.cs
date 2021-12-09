using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.Layout
{
    public class LayoutActors
    {
        private readonly Dictionary<string, Actor> actorTable = new Dictionary<string, Actor>();

        public Actor RootActor { get; }

        public LayoutActors(Scene scene, LayoutNode rootNode, Point position = default)
        {
            var layout = new LayoutBaker(rootNode).Bake();

            RootActor = scene.AddActor(rootNode.Name.Text);
            RootActor.transform.Position = position.ToVector2();

            // Create hierarchy
            CreateActorForChildren(rootNode, layout);
        }

        private void CreateActorForChildren(LayoutNode parent, BakedLayout layout)
        {
            if (!parent.HasChildren)
            {
                return;
            }

            foreach (var node in parent.Children)
            {
                var actorName = node.Name.Text;
                var bakedLayoutNode = layout.GetNode(actorName);
                var actor = GetActor(parent.Name.Text).transform.AddActorAsChild(actorName, bakedLayoutNode.PositionRelativeToRoot.ToVector2());
                actorTable[actorName] = actor;

                new BoundingRect(actor, bakedLayoutNode.Size);

                CreateActorForChildren(node, layout);
            }
        }

        private Actor GetActor(string actorName)
        {
            return this.actorTable[actorName];
        }
    }
}
