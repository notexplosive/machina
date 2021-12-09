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
        private readonly Actor rootActor;

        public LayoutActors(Scene scene, LayoutNode rootNode, Point position = default)
        {
            var layout = new LayoutBaker(rootNode).Bake();

            var actorName = rootNode.Name.Text;
            this.rootActor = scene.AddActor(actorName);
            new BoundingRect(this.rootActor, layout.BakedRootNode.Size);
            rootActor.transform.Position = position.ToVector2();
            AddActorToTable(actorName, rootActor);

            // Create hierarchy
            CreateActorForChildren(rootNode, layout);
        }

        private void CreateActorForChildren(LayoutNode parent, BakedLayout layout)
        {
            if (!parent.HasChildren)
            {
                return;
            }

            var parentActor = GetActor(parent.Name.Text);

            foreach (var node in parent.Children)
            {
                if (node.Name.Exists)
                {
                    var actorName = node.Name.Text;
                    var bakedLayoutNode = layout.GetNode(actorName);
                    var actor = parentActor.transform.AddActorAsChild(actorName);
                    actor.transform.Position = this.rootActor.transform.Position + bakedLayoutNode.PositionRelativeToRoot.ToVector2();
                    AddActorToTable(actorName, actor);

                    new BoundingRect(actor, bakedLayoutNode.Size);

                    CreateActorForChildren(node, layout);
                }
            }
        }

        private void AddActorToTable(string text, Actor actor)
        {
            actorTable[text] = actor;
        }

        public Actor GetActor(string actorName)
        {
            return this.actorTable[actorName];
        }
    }
}
