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

            SetupRootActor(this.rootActor, actorName, layout, position.ToVector2());
            CreateActorsForChildren(rootNode, layout);
        }

        private void CreateActorsForChildren(LayoutNode parent, BakedLayout layout)
        {
            if (!parent.HasChildren)
            {
                return;
            }

            var parentActor = GetActor(parent.Name.Text);

            foreach (var childNode in parent.Children)
            {
                if (childNode.Name.Exists)
                {
                    var actorName = childNode.Name.Text;
                    var actor = parentActor.transform.AddActorAsChild(actorName);
                    new LayoutSiblingWithCachedOrientation(actor, parent.Orientation);

                    SetupChildActor(actor, actorName, layout);
                    CreateActorsForChildren(childNode, layout);
                }
            }
        }

        private void SetupChildActor(Actor actor, string actorName, BakedLayout layout)
        {
            var bakedLayoutNode = layout.GetNode(actorName);
            new BoundingRect(actor, bakedLayoutNode.Size);
            actor.transform.Position = this.rootActor.transform.Position + bakedLayoutNode.PositionRelativeToRoot.ToVector2();
            actor.transform.LocalDepth = -bakedLayoutNode.NestingLevel;
            AddActorToTable(actorName, actor);

        }

        private void SetupRootActor(Actor actor, string actorName, BakedLayout layout, Vector2 absolutePosition)
        {
            new BoundingRect(actor, layout.GetNode(actorName).Size);
            actor.transform.Position = absolutePosition;
            AddActorToTable(actorName, actor);
        }

        public void ReapplyLayout(LayoutNode rootNode)
        {

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
